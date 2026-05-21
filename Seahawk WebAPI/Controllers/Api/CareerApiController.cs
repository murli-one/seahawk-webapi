using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Seahawk_WebAPI.Contracts.Careers;
using SeaHawkServices.Application.Services.Interface;
using SeaHawkServices.Domain.Entities;
using System.Xml.Linq;
using static SeaHawkServices.Domain.Entities.Enums;

namespace Seahawk_WebAPI.Controllers.Api;

[ApiController]
[Route("api/careers")]
public class CareerApiController : ControllerBase
{
    private readonly ICareerService _careerService;
    private readonly UserManager<ApplicationUser> _userManager;

    public CareerApiController(
        ICareerService careerService,
        UserManager<ApplicationUser> userManager)
    {
        _careerService = careerService;
        _userManager = userManager;
    }


    [AllowAnonymous]
    [HttpGet]
    public async Task<ActionResult<CareerPagedResponse>> GetCareers([FromQuery] CareerQueryRequest request)
    {
        request = NormalizeRequest(request);

        var currentUser = await _userManager.GetUserAsync(User);

        var query = await GetFilteredCareersAsync(request);

        if (currentUser == null)
        {
            query = query.Where(x => x.Status == 0);
        }

        var list = query
            .OrderByDescending(x => x.Id)
            .ToList();

        var totalCount = list.Count;

        var items = list
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(ToDto)
            .ToList();

        return Ok(new CareerPagedResponse
        {
            PageNumber = request.Page,
            PageSize = request.PageSize,
            TotalCount = totalCount,
            TotalPages = request.PageSize <= 0
                ? 0
                : (int)Math.Ceiling((double)totalCount / request.PageSize),

            CurrentUser = currentUser?.UserName ?? "Public",
            CurrentUserEmail = currentUser?.Email ?? "",
            CurrentUserRole = currentUser?.Role.ToString() ?? "Anonymous",

            Items = items
        });
    }

    //[AllowAnonymous]
    //[HttpGet]
    //public async Task<ActionResult<CareerPagedResponse>> GetCareers([FromQuery] CareerQueryRequest request)
    //{
    //    request = NormalizeRequest(request);

    //    var currentUser = await _userManager.GetUserAsync(User);
    //    if (currentUser == null)
    //        return Unauthorized("User not found.");

    //    var query = await GetFilteredCareersAsync(request);
    //    var list = query.OrderByDescending(x => x.Id).ToList();

    //    var totalCount = list.Count;

    //    var items = list
    //        .Skip((request.Page - 1) * request.PageSize)
    //        .Take(request.PageSize)
    //        .Select(ToDto)
    //        .ToList();

    //    return Ok(new CareerPagedResponse
    //    {
    //        PageNumber = request.Page,
    //        PageSize = request.PageSize,
    //        TotalCount = totalCount,
    //        TotalPages = request.PageSize <= 0
    //            ? 0
    //            : (int)Math.Ceiling((double)totalCount / request.PageSize),

    //        CurrentUser = currentUser.UserName,
    //        CurrentUserEmail = currentUser.Email,
    //        CurrentUserRole = currentUser.Role.ToString(),

    //        Items = items
    //    });
    //}

    [HttpGet("{id:int}")]
    [Authorize]
    public async Task<ActionResult<CareerDto>> GetCareer(int id)
    {
        if (id <= 0)
            return BadRequest("Invalid career id.");

        var career = await _careerService.GetByIdAsync(id);
        if (career == null)
            return NotFound("Career not found.");

        return Ok(ToDto(career));
    }

    [HttpGet("{id:int}/details")]
    [AllowAnonymous]
    public async Task<ActionResult<CareerDetailsResponse>> GetCareerDetails(int id)
    {
        if (id <= 0)
            return BadRequest("Invalid career id.");

        var career = await _careerService.GetByIdAsync(id);
        if (career == null)
            return NotFound("Career not found.");

        var latestCareers = (await _careerService.GetAllAsync())
            .Where(x => x.Id != id)
            .OrderByDescending(x => x.Id)
            .Select(ToDto)
            .ToList();

        return Ok(new CareerDetailsResponse
        {
            Career = ToDto(career),
            LatestCareers = latestCareers
        });
    }

    [HttpGet("feeds")]
    [AllowAnonymous]
    public async Task<ActionResult<List<CareerDto>>> GetCareerFeeds()
    {
        var careers = (await _careerService.GetAllAsync())
            .OrderByDescending(x => x.Id)
            .Select(ToDto)
            .ToList();

        return Ok(careers);
    }

    [HttpGet("filter-options")]
    [Authorize]
    public ActionResult<CareerFilterOptionsResponse> GetFilterOptions()
    {
        return Ok(new CareerFilterOptionsResponse
        {
            Statuses = Enum.GetValues<Status>()
                .Select(x => new CareerStatusOptionDto
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
    public async Task<ActionResult<CareerDto>> CreateCareer([FromBody] CareerUpsertRequest request)
    {
        var validationError = ValidateRequest(request);
        if (validationError != null)
            return BadRequest(validationError);

        var career = new Career
        {
            Title = request.Title.Trim(),
            JobDescription = request.JobDescription.Trim(),
            Experience = request.Experience,
            Status = request.Status
        };

        await _careerService.AddAsync(career);

        return CreatedAtAction(nameof(GetCareer), new { id = career.Id }, ToDto(career));
    }

    [HttpPut("{id:int}")]
    [Authorize]
    public async Task<ActionResult<CareerDto>> UpdateCareer(int id, [FromBody] CareerUpsertRequest request)
    {
        if (id <= 0)
            return BadRequest("Invalid career id.");

        var validationError = ValidateRequest(request);
        if (validationError != null)
            return BadRequest(validationError);

        var existingCareer = await _careerService.GetByIdAsync(id);
        if (existingCareer == null)
            return NotFound("Career not found.");

        existingCareer.Title = request.Title.Trim();
        existingCareer.JobDescription = request.JobDescription.Trim();
        existingCareer.Experience = request.Experience;
        existingCareer.Status = request.Status;

        await _careerService.UpdateAsync(existingCareer);

        return Ok(ToDto(existingCareer));
    }

    [HttpDelete("{id:int}")]
    [Authorize]
    public async Task<IActionResult> DeleteCareer(int id)
    {
        if (id <= 0)
            return BadRequest("Invalid career id.");

        var career = await _careerService.GetByIdAsync(id);
        if (career == null)
            return NotFound("Career not found.");

        await _careerService.DeleteAsync(id);

        return NoContent();
    }

    [HttpGet("export-xml")]
    [Authorize]
    public async Task<IActionResult> ExportXml([FromQuery] CareerQueryRequest request)
    {
        request = NormalizeRequest(request, applyPagingDefaults: false);

        var careers = (await GetFilteredCareersAsync(request))
            .OrderByDescending(x => x.Id)
            .ToList();

        var xml = new XDocument(
            new XElement("Careers",
                careers.Select(c =>
                    new XElement("Career",
                        new XElement("Id", c.Id),
                        new XElement("Title", c.Title),
                        new XElement("JobDescription", c.JobDescription),
                        new XElement("Experience", c.Experience),
                        new XElement("Status", c.Status.ToString())
                    )
                )
            )
        );

        var bytes = System.Text.Encoding.UTF8.GetBytes(xml.ToString());

        return File(
            bytes,
            "application/xml",
            $"Careers_{DateTime.UtcNow:yyyyMMddHHmmss}.xml");
    }

    [HttpPost("export-excel")]
    [Authorize]
    public async Task<IActionResult> ExportExcel([FromBody] CareerQueryRequest? request)
    {
        request = NormalizeRequest(request ?? new CareerQueryRequest(), applyPagingDefaults: false);

        var careers = (await GetFilteredCareersAsync(request))
            .OrderByDescending(x => x.Id)
            .ToList();

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
                Name = "Careers"
            });

            var headers = new[]
            {
                "Id",
                "Title",
                "Job Description",
                "Experience",
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

            foreach (var c in careers)
            {
                var values = new[]
                {
                    c.Id.ToString(),
                    c.Title ?? "",
                    c.JobDescription ?? "",
                    c.Experience.ToString(),
                    c.Status.ToString()
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
            $"Careers_{DateTime.UtcNow:yyyyMMddHHmmss}.xlsx");
    }

    private async Task<IEnumerable<Career>> GetFilteredCareersAsync(CareerQueryRequest request)
    {
        var careers = await _careerService.GetAllAsync();

        var query = (careers ?? Enumerable.Empty<Career>()).AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.FilterTitle))
        {
            var term = request.FilterTitle.Trim();

            query = query.Where(c =>
                !string.IsNullOrEmpty(c.Title) &&
                c.Title.Contains(term, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(request.FilterStatus))
        {
            var term = request.FilterStatus.Trim();

            query = query.Where(c =>
                c.Status.ToString().Equals(term, StringComparison.OrdinalIgnoreCase) ||
                ((int)c.Status).ToString().Equals(term, StringComparison.OrdinalIgnoreCase));
        }

        return query.ToList();
    }

    private static CareerQueryRequest NormalizeRequest(
        CareerQueryRequest request,
        bool applyPagingDefaults = true)
    {
        if (applyPagingDefaults)
        {
            if (request.Page < 1)
                request.Page = 1;

            if (request.PageSize < 1)
                request.PageSize = 15;
        }

        request.FilterTitle = request.FilterTitle?.Trim();
        request.FilterStatus = request.FilterStatus?.Trim();

        return request;
    }

    private static string? ValidateRequest(CareerUpsertRequest request)
    {
        if (request == null)
            return "Request body is required.";

        if (string.IsNullOrWhiteSpace(request.Title))
            return "Title is required.";

        if (string.IsNullOrWhiteSpace(request.JobDescription))
            return "Job description is required.";

        if (request.Experience < 0)
            return "Experience cannot be negative.";

        if (!Enum.IsDefined(typeof(Status), request.Status))
            return "Invalid status.";

        return null;
    }

    private static CareerDto ToDto(Career career)
    {
        return new CareerDto
        {
            Id = career.Id,
            Title = career.Title,
            JobDescription = career.JobDescription,
            Experience = career.Experience,
            Status = career.Status,
            StatusName = career.Status.ToString()
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