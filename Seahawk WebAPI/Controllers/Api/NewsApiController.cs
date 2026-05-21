using Data;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Seahawk_WebAPI.Contracts.News;
using SeaHawkServices.Application.Services.Interface;
using SeaHawkServices.Domain.Entities;
using System.Xml.Linq;
using static SeaHawkServices.Domain.Entities.Enums;

namespace Seahawk_WebAPI.Controllers.Api;

[ApiController]
[Route("api/news")]
public class NewsApiController : ControllerBase
{
    private readonly INewsService _service;
    private readonly UserManager<ApplicationUser> _userManager;

    public NewsApiController(
        INewsService service,
        UserManager<ApplicationUser> userManager)
    {
        _service = service;
        _userManager = userManager;
    }

    [HttpGet]
    
    public async Task<ActionResult<NewsPagedResponse>> GetNews([FromQuery] NewsQueryRequest request)
    {
        request = NormalizeQuery(request);

        var currentUser = await _userManager.GetUserAsync(User);

        var allNews = await _service.GetAllAsync();
        var query = (allNews ?? new List<NewsFeed>()).AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.Title))
        {
            var term = request.Title.Trim();

            query = query.Where(n =>
                !string.IsNullOrEmpty(n.Title) &&
                n.Title.Contains(term, StringComparison.OrdinalIgnoreCase));
        }

        query = query
            .OrderByDescending(x => x.PublishDate)
            .ThenByDescending(x => x.Id);

        var totalCount = query.Count();

        var items = query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(ToDto)
            .ToList();

        return Ok(new NewsPagedResponse
        {
            PageNumber = request.Page,
            PageSize = request.PageSize,
            TotalCount = totalCount,
            TotalPages = request.PageSize <= 0
                ? 0
                : (int)Math.Ceiling((double)totalCount / request.PageSize),

            UserName = currentUser?.UserName,
            UserEmail = currentUser?.Email,
            UserRole = currentUser?.Role.ToString(),

            FilterTitle = request.Title,

            Items = items
        });
    }

    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<ActionResult<NewsFeedDto>> GetNewsById(int id)
    {
        if (id <= 0)
            return BadRequest(new { message = "Invalid news id." });

        var news = await _service.GetByIdAsync(id);

        if (news == null)
            return NotFound(new { message = "News not found." });

        return Ok(ToDto(news));
    }

    [HttpGet("{id:int}/details")]
    [AllowAnonymous]
    public async Task<ActionResult<NewsDetailsResponse>> GetNewsDetails(int id)
    {
        if (id <= 0)
            return BadRequest(new { message = "Invalid news id." });

        var news = await _service.GetByIdAsync(id);

        if (news == null)
            return NotFound(new { message = "News not found." });

        var latestNews = (await _service.GetActiveFeedsAsync())
            .Where(n => n.Id != id)
            .OrderByDescending(n => n.PublishDate)
            .ThenByDescending(n => n.Id)
            .Select(ToDto)
            .ToList();

        return Ok(new NewsDetailsResponse
        {
            NewsFeed = ToDto(news),
            LatestNews = latestNews
        });
    }

    [HttpGet("feeds")]
    [AllowAnonymous]
    public async Task<ActionResult<List<NewsFeedDto>>> GetFeeds()
    {
        var list = await _service.GetActiveFeedsAsync();

        return Ok(list
            .OrderByDescending(x => x.PublishDate)
            .ThenByDescending(x => x.Id)
            .Select(ToDto)
            .ToList());
    }

    [HttpGet("form-options")]
    [Authorize]
    public ActionResult<NewsFormOptionsResponse> GetFormOptions()
    {
        return Ok(new NewsFormOptionsResponse
        {
            Statuses = Enum.GetValues<Status>()
                .Select(x => new NewsStatusOptionDto
                {
                    Text = x.ToString(),
                    Value = x.ToString(),
                    NumericValue = (int)x
                })
                .ToList()
        });
    }

    [HttpPost]
    [Authorize]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<NewsFeedDto>> CreateNews([FromForm] CreateNewsRequest request)
    {
        if (request == null)
            return BadRequest(new { message = "Request body is required." });

        if (string.IsNullOrWhiteSpace(request.Title))
            return BadRequest(new { message = "Title is required." });

        var newsFeed = new NewsFeed
        {
            Title = request.Title.Trim(),
            ShortDescription = request.ShortDescription?.Trim(),
            Content = request.Content?.Trim(),
            PublishDate = request.PublishDate ?? DateTime.UtcNow,
            Status = request.SelectedStatus != 0 && Enum.IsDefined(typeof(Status), request.SelectedStatus)
                ? (Status)request.SelectedStatus
                : Status.Active
        };

        await _service.AddNewsAsync(newsFeed, request.UploadFile);

        return CreatedAtAction(nameof(GetNewsById), new { id = newsFeed.Id }, ToDto(newsFeed));
    }
    [HttpPut("{id:int}")]
    [Authorize]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<NewsFeedDto>> UpdateNews(
        int id,
        [FromForm] UpdateNewsRequest request)
    {
        if (id <= 0)
            return BadRequest(new { message = "Invalid news id." });

        if (request == null)
            return BadRequest(new { message = "Request body is required." });

        if (string.IsNullOrWhiteSpace(request.Title))
            return BadRequest(new { message = "Title is required." });

        var existing = await _service.GetByIdAsync(id);

        if (existing == null)
            return NotFound(new { message = "News not found." });

        existing.Title = request.Title.Trim();
        existing.ShortDescription = request.ShortDescription?.Trim();
        existing.Content = request.Content?.Trim();
        existing.PublishDate = request.PublishDate ?? existing.PublishDate;

        if (request.SelectedStatus != 0 && Enum.IsDefined(typeof(Status), request.SelectedStatus))
        {
            existing.Status = (Status)request.SelectedStatus;
        }

        await _service.UpdateAsync(existing, request.UploadFile);

        return Ok(ToDto(existing));
    }

    [HttpDelete("{id:int}")]
    [Authorize]
    public async Task<IActionResult> DeleteNews(int id)
    {
        if (id <= 0)
            return BadRequest(new { message = "Invalid news id." });

        var newsFeed = await _service.GetByIdAsync(id);

        if (newsFeed == null)
            return NotFound(new { message = "News not found." });

        await _service.SoftDeleteAsync(newsFeed);

        return NoContent();
    }

    [Authorize]
    [HttpGet("export-xml")]
    [Authorize]
    public async Task<IActionResult> ExportXml()
    {
        var newsFeeds = await _service.GetAllAsync();

        var xml = new XDocument(
            new XElement("NewsFeeds",
                newsFeeds.Select(n =>
                    new XElement("NewsFeed",
                        new XElement("Id", n.Id),
                        new XElement("Title", n.Title),
                        new XElement("ShortDescription", n.ShortDescription),
                        new XElement("Content", n.Content),
                        new XElement("PublishDate", n.PublishDate),
                        new XElement("Status", n.Status)
                    )
                )
            )
        );

        var bytes = System.Text.Encoding.UTF8.GetBytes(xml.ToString());

        return File(
            bytes,
            "application/xml",
            $"NewsFeeds_{DateTime.UtcNow:yyyyMMddHHmmss}.xml");
    }

    [HttpPost("export-excel")]
    [Authorize]
    public async Task<IActionResult> ExportExcel()
    {
        var newsFeeds = await _service.GetAllAsync();

        using var stream = new MemoryStream();

        using (var spreadsheet = SpreadsheetDocument.Create(stream, SpreadsheetDocumentType.Workbook))
        {
            var workbookPart = spreadsheet.AddWorkbookPart();
            workbookPart.Workbook = new Workbook();

            var worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
            worksheetPart.Worksheet = new Worksheet();

            var sheetData = new SheetData();
            worksheetPart.Worksheet.AppendChild(sheetData);

            var sheets = spreadsheet.WorkbookPart!.Workbook.AppendChild(new Sheets());

            sheets.Append(new Sheet
            {
                Id = spreadsheet.WorkbookPart.GetIdOfPart(worksheetPart),
                SheetId = 1,
                Name = "NewsFeeds"
            });

            var headers = new[]
            {
                "Title",
                "Content",
                "Publish Date",
                "Status"
            };

            var maxLengths = new int[headers.Length];

            var headerRow = new Row();

            for (int i = 0; i < headers.Length; i++)
            {
                headerRow.AppendChild(CreateTextCell(headers[i]));
                maxLengths[i] = headers[i].Length;
            }

            sheetData.AppendChild(headerRow);

            foreach (var n in newsFeeds)
            {
                var values = new[]
                {
                    n.Title ?? "",
                    n.Content ?? "",
                    n.PublishDate.ToString("yyyy-MM-dd"),
                    n.Status.ToString()
                };

                var row = new Row();

                for (int i = 0; i < values.Length; i++)
                {
                    row.AppendChild(CreateTextCell(values[i]));
                    maxLengths[i] = Math.Max(maxLengths[i], values[i].Length);
                }

                sheetData.AppendChild(row);
            }

            var columns = new Columns();

            for (uint i = 1; i <= headers.Length; i++)
            {
                columns.Append(new Column
                {
                    Min = i,
                    Max = i,
                    Width = Math.Min(maxLengths[i - 1] + 2, 80),
                    CustomWidth = true
                });
            }

            worksheetPart.Worksheet.InsertAt(columns, 0);

            workbookPart.Workbook.Save();
        }

        stream.Position = 0;

        return File(
            stream.ToArray(),
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            $"NewsFeeds_{DateTime.UtcNow:yyyyMMddHHmmss}.xlsx");
    }

    private static NewsQueryRequest NormalizeQuery(NewsQueryRequest request)
    {
        if (request.Page < 1)
            request.Page = 1;

        if (request.PageSize < 1)
            request.PageSize = 15;

        request.Title = request.Title?.Trim();

        return request;
    }

    private static NewsFeedDto ToDto(NewsFeed news)
    {
        return new NewsFeedDto
        {
            Id = news.Id,
            Title = news.Title,
            ShortDescription = news.ShortDescription,
            Content = news.Content,
            PublishDate = news.PublishDate,
            Status = news.Status,
            StatusName = news.Status.ToString(),

            TinyTitle = news.TinyTitle,
            ShortPublishDate = news.ShortPublishDate,
            TinyDescription = news.TinyDescription,
            TinyContent = news.TinyContent
        };
    }

    private static Cell CreateTextCell(string value)
    {
        return new Cell
        {
            CellValue = new CellValue(value ?? string.Empty),
            DataType = CellValues.String
        };
    }
}