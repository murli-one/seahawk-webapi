using Data;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Seahawk_WebAPI.Contracts.AnalysisResults;
using Seahawk_WebAPI.Helpers;
using SeaHawkServices.Application.Common.Interfaces;
using SeaHawkServices.Application.Services.Interface;
using SeaHawkServices.Domain.Entities;
using SeaHawkServices.Web.ViewModels;
using System.Reflection;
using System.Xml.Linq;
using static SeaHawkServices.Domain.Entities.Enums;

namespace Seahawk_WebAPI.Controllers.Api;

[ApiController]
[Authorize]
[Route("api/analysis-results")]
public class AnalysisResultApiController : ControllerBase
{
    private readonly IAnalysisResultService _service;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IUnitOfWork _unitOfWork;

    public AnalysisResultApiController(
        IAnalysisResultService service,
        UserManager<ApplicationUser> userManager,
        IUnitOfWork unitOfWork)
    {
        _service = service;
        _userManager = userManager;
        _unitOfWork = unitOfWork;
    }

    [HttpGet]
    public async Task<ActionResult<AnalysisResultPagedResponse>> GetAnalysisResults(
        [FromQuery] AnalysisResultQueryRequest request)
    {
        request = NormalizeRequest(request);

        var currentUser = await _userManager.GetUserAsync(User);

        if (currentUser == null)
            return Unauthorized("User not found.");

        var results = await GetFilteredResultsAsync(currentUser, request);

        var filteredList = results
            .OrderByDescending(x => x.Id)
            .ToList();

        var totalCount = filteredList.Count;

        var pageItems = filteredList
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(ToListItemDto)
            .ToList();

        var response = new AnalysisResultPagedResponse
        {
            PageNumber = request.Page,
            PageSize = request.PageSize,
            TotalCount = totalCount,
            TotalPages = request.PageSize <= 0
                ? 0
                : (int)Math.Ceiling((double)totalCount / request.PageSize),
            CurrentUser = currentUser.UserName,
            CurrentUserRole = currentUser.Role.ToString(),
            Items = pageItems
        };

        return Ok(response);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<AnalysisResultDetailResponse>> GetById(int id)
    {
        var result = await _service.GetByIdAsync(id);

        if (result == null)
            return NotFound("Analysis result not found.");

        return Ok(ToDetailResponse(result));
    }

    [HttpGet("form-options")]
    public async Task<ActionResult<AnalysisResultFormOptionsResponse>> GetFormOptions()
    {
        var vessels = await _unitOfWork.VesselDetail.GetAllAsync();
        var companies = await _unitOfWork.Companies.GetAllAsync();
        var samples = await _unitOfWork.SampleCollections.GetAllAsync();

        var response = new AnalysisResultFormOptionsResponse
        {
            Vessels = vessels
                .OrderBy(v => v.VesselName)
                .Select(v => new SelectOptionDto
                {
                    Value = v.Id.ToString(),
                    Text = v.VesselName ?? ""
                })
                .ToList(),

            Companies = companies
                .OrderBy(c => c.CompanyName)
                .Select(c => new SelectOptionDto
                {
                    Value = c.Id.ToString(),
                    Text = c.CompanyName ?? ""
                })
                .ToList(),

            SampleCollections = samples
                .OrderBy(s => s.IMONumber)
                .Select(s => new SelectOptionDto
                {
                    Value = s.Id.ToString(),
                    Text = s.IMONumber ?? ""
                })
                .ToList()
        };

        return Ok(response);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] AnalysisResultUpsertRequest request)
    {
        if (request?.AnalysisResult == null)
            return BadRequest("Analysis result data is required.");

        await _service.AddAsync(request.AnalysisResult);

        return Ok(new
        {
            message = "Analysis result created successfully.",
            id = request.AnalysisResult.Id
        });
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(
        [FromRoute] int id,
        [FromBody] AnalysisResultUpsertRequest request)
    {
        if (request?.AnalysisResult == null)
            return BadRequest("Analysis result data is required.");

        var existing = await _service.GetByIdAsync(id);

        if (existing == null)
            return NotFound("Analysis result not found.");

        CopyEditableScalarProperties(request.AnalysisResult, existing);

        await _service.UpdateAsync(existing);

        return Ok(new
        {
            message = "Analysis result updated successfully.",
            id = existing.Id
        });
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete([FromRoute] int id)
    {
        var existing = await _service.GetByIdAsync(id);

        if (existing == null)
            return NotFound("Analysis result not found.");

        await _service.DeleteAsync(id);

        return NoContent();
    }

    [HttpGet("export-xml")]
    public async Task<IActionResult> ExportXml([FromQuery] AnalysisResultQueryRequest request)
    {
        request = NormalizeRequest(request);

        var currentUser = await _userManager.GetUserAsync(User);

        if (currentUser == null)
            return Unauthorized("User not found.");

        var results = await GetFilteredResultsAsync(currentUser, request);

        var orderedResults = results
            .OrderByDescending(x => x.Id)
            .ToList();

        var columns = BuildExportColumns();

        var xml = new XDocument(
            new XElement("AnalysisResults",
                orderedResults.Select(r =>
                    new XElement("AnalysisResult",
                        columns.Select(col =>
                            new XElement(col.Header, col.Selector(r))
                        )
                    )
                )
            )
        );

        var bytes = System.Text.Encoding.UTF8.GetBytes(xml.ToString());

        return File(bytes, "application/xml", "AnalysisResults.xml");
    }

    [HttpPost("export-excel")]
    public async Task<IActionResult> ExportExcel([FromBody] AnalysisResultQueryRequest request)
    {
        request = NormalizeRequest(request);

        var currentUser = await _userManager.GetUserAsync(User);

        if (currentUser == null)
            return Unauthorized("User not found.");

        var results = await GetFilteredResultsAsync(currentUser, request);

        var orderedResults = results
            .OrderByDescending(x => x.Id)
            .ToList();

        var columns = BuildExportColumns();

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
                Name = "AnalysisResults"
            });

            var headerRow = new Row();

            foreach (var col in columns)
            {
                headerRow.AppendChild(new Cell
                {
                    CellValue = new CellValue(col.Header),
                    DataType = CellValues.String
                });
            }

            sheetData.AppendChild(headerRow);

            var maxLengths = new int[columns.Count];

            for (int i = 0; i < columns.Count; i++)
                maxLengths[i] = columns[i].Header.Length;

            foreach (var item in orderedResults)
            {
                var row = new Row();

                for (int i = 0; i < columns.Count; i++)
                {
                    var value = columns[i].Selector(item) ?? string.Empty;

                    row.AppendChild(new Cell
                    {
                        CellValue = new CellValue(value),
                        DataType = CellValues.String
                    });

                    maxLengths[i] = Math.Max(maxLengths[i], value.Length);
                }

                sheetData.AppendChild(row);
            }

            var cols = new Columns();

            for (uint i = 1; i <= columns.Count; i++)
            {
                cols.Append(new Column
                {
                    Min = i,
                    Max = i,
                    Width = maxLengths[i - 1] + 2,
                    CustomWidth = true
                });
            }

            worksheetPart.Worksheet.InsertAt(cols, 0);

            workbookPart.Workbook.Save();
        }

        stream.Position = 0;

        return File(
            stream.ToArray(),
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "AnalysisResults.xlsx");
    }

    [HttpGet("ai-suggest")]
    public async Task<ActionResult<AiSuggestResponse>> AiSuggest(
        [FromServices] ITypesenseSearchService searchService,
        [FromQuery] string field,
        [FromQuery] string q)
    {
        if (string.IsNullOrWhiteSpace(q) || q.Length < 2)
            return Ok(new AiSuggestResponse());

        var allowedFields = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "PortBunkered",
            "FuelType",
            "Comment",
            "VesselName",
            "SampleNumber"
        };

        if (!allowedFields.Contains(field))
            return BadRequest("Invalid field.");

        var suggestions = await searchService.GetSuggestionsAsync(field, q);

        return Ok(new AiSuggestResponse
        {
            Suggestions = suggestions?.ToList() ?? new List<string>()
        });
    }

    [HttpPost("reindex-typesense")]
    public async Task<ActionResult<ReindexTypesenseResponse>> ReindexTypesense(
        [FromServices] ITypesenseSearchService typesense)
    {
        var currentUser = await _userManager.GetUserAsync(User);

        if (currentUser == null)
            return Unauthorized("User not found.");

        if (currentUser.Role != Role.SystemAdmin)
            return Forbid();

        var all = await _service.GetAllAsync();

        var docs = all
            .Where(x =>
                x.PortBunkered != null &&
                x.FuelType != null &&
                x.VesselDetail != null)
            .Select(r => new
            {
                id = r.Id.ToString(),
                PortBunkered = r.PortBunkered ?? "",
                FuelType = r.FuelType ?? "",
                Comment = r.Comment ?? "",
                VesselName = r.VesselDetail?.VesselName ?? "",
                SampleNumber = r.SampleNumber ?? ""
            })
            .ToList();

        await typesense.BulkUpsertAsync(docs);

        return Ok(new ReindexTypesenseResponse
        {
            IndexedCount = docs.Count,
            Message = $"Indexed {docs.Count} documents into Typesense."
        });
    }

    private async Task<IEnumerable<AnalysisResult>> GetFilteredResultsAsync(
        ApplicationUser currentUser,
        AnalysisResultQueryRequest request)
    {
        IEnumerable<AnalysisResult> results = await GetRoleBasedResultsAsync(currentUser);

        if (!string.IsNullOrWhiteSpace(request.FilterAiQuery))
            AnalysisResultSmartSearchParser.Apply(request);

        if (!string.IsNullOrWhiteSpace(request.FilterSampleNumber))
        {
            var sampleFilter = request.FilterSampleNumber.Trim();

            results = results.Where(r =>
                !string.IsNullOrEmpty(r.SampleNumber) &&
                r.SampleNumber.Contains(sampleFilter, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(request.FilterVessel))
        {
            var vesselFilter = request.FilterVessel.Trim();

            results = results.Where(r =>
                r.VesselDetail != null &&
                !string.IsNullOrEmpty(r.VesselDetail.VesselName) &&
                r.VesselDetail.VesselName.Contains(vesselFilter, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(request.FilterPort))
        {
            var portFilter = request.FilterPort.Trim();

            results = results.Where(r =>
                !string.IsNullOrEmpty(r.PortBunkered) &&
                r.PortBunkered.Contains(portFilter, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(request.FilterFuelType))
        {
            var fuelFilter = request.FilterFuelType.Trim();

            results = results.Where(r =>
                !string.IsNullOrEmpty(r.FuelType) &&
                r.FuelType.Contains(fuelFilter, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(request.FilterComment))
        {
            var commentFilter = request.FilterComment.Trim();

            results = results.Where(r =>
                !string.IsNullOrEmpty(r.Comment) &&
                r.Comment.Contains(commentFilter, StringComparison.OrdinalIgnoreCase));
        }

        if (request.FilterBunkerDateFrom.HasValue)
        {
            var from = request.FilterBunkerDateFrom.Value.Date;

            results = results.Where(r =>
                r.DateBunkered.HasValue &&
                r.DateBunkered.Value.Date >= from);
        }

        if (request.FilterBunkerDateTo.HasValue)
        {
            var to = request.FilterBunkerDateTo.Value.Date;

            results = results.Where(r =>
                r.DateBunkered.HasValue &&
                r.DateBunkered.Value.Date <= to);
        }

        return results;
    }

    private async Task<IEnumerable<AnalysisResult>> GetRoleBasedResultsAsync(
        ApplicationUser currentUser)
    {
        IEnumerable<AnalysisResult> results = Enumerable.Empty<AnalysisResult>();

        if (currentUser.Role == Role.SystemAdmin)
        {
            results = await _service.GetAllAsync();
        }
        else if (currentUser.Role == Role.ManagementUser)
        {
            var companyUser = _unitOfWork.CompanyUser.GetCompanyUserByUserId(currentUser.Id);

            if (companyUser != null)
            {
                var companyDetails = await _unitOfWork.Companies.GetByIdAsync(companyUser.CompanyId);

                var vesselIds = companyDetails?.VesselDetailList?
                    .Select(v => v.Id)
                    .ToList() ?? new List<int>();

                if (vesselIds.Any())
                {
                    var allResults = await _service.GetAllAsync();

                    results = allResults.Where(r =>
                        r.VesselDetailId.HasValue &&
                        vesselIds.Contains(r.VesselDetailId.Value));
                }
            }
        }
        else
        {
            var vesselUser = await _unitOfWork.VesselUser.GetAsync(x => x.UserId == currentUser.Id);

            if (vesselUser?.VesselDetailId != null)
                results = await _service.GetAllByIdAsync(vesselUser.VesselDetailId);
        }

        return results;
    }

    private static AnalysisResultQueryRequest NormalizeRequest(AnalysisResultQueryRequest? request)
    {
        request ??= new AnalysisResultQueryRequest();

        if (request.Page < 1)
            request.Page = 1;

        if (request.PageSize < 1)
            request.PageSize = 15;

        request.FilterSampleNumber = request.FilterSampleNumber?.Trim();
        request.FilterVessel = request.FilterVessel?.Trim();
        request.FilterPort = request.FilterPort?.Trim();
        request.FilterFuelType = request.FilterFuelType?.Trim();
        request.FilterComment = request.FilterComment?.Trim();
        request.FilterAiQuery = request.FilterAiQuery?.Trim();

        return request;
    }

    private static AnalysisResultListItemDto ToListItemDto(AnalysisResult x)
    {
        return new AnalysisResultListItemDto
        {
            Id = x.Id,
            SampleNumber = x.SampleNumber,
            PortBunkered = x.PortBunkered,
            DateBunkered = x.DateBunkered,
            Specification = x.Specification,
            FuelType = x.FuelType,
            DateReceived = x.DateReceived,
            Comment = x.Comment,
            VesselDetailId = x.VesselDetailId,
            VesselName = x.VesselDetail?.VesselName,
            Grade = x.Grade,
            H2S = x.H2S,
            AlSi = x.AlSi
        };
    }

    private static AnalysisResultDetailResponse ToDetailResponse(AnalysisResult result)
    {
        var fields = new Dictionary<string, object?>();

        foreach (var prop in GetScalarProperties())
        {
            fields[prop.Name] = prop.GetValue(result);
        }

        return new AnalysisResultDetailResponse
        {
            Id = result.Id,
            Fields = fields,
            VesselName = result.VesselDetail?.VesselName,
            CompanyName = result.Company?.CompanyName
        };
    }

    private static void CopyEditableScalarProperties(
        AnalysisResult source,
        AnalysisResult target)
    {
        foreach (var prop in GetScalarProperties())
        {
            if (string.Equals(prop.Name, nameof(AnalysisResult.Id), StringComparison.OrdinalIgnoreCase))
                continue;

            if (!prop.CanWrite)
                continue;

            var sourceValue = prop.GetValue(source);
            prop.SetValue(target, sourceValue);
        }
    }

    private static List<PropertyInfo> GetScalarProperties()
    {
        return typeof(AnalysisResult)
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p =>
            {
                var type = Nullable.GetUnderlyingType(p.PropertyType) ?? p.PropertyType;

                return type.IsPrimitive ||
                       type == typeof(string) ||
                       type == typeof(decimal) ||
                       type == typeof(DateTime) ||
                       type == typeof(Guid) ||
                       type == typeof(bool);
            })
            .ToList();
    }

    private static List<(string Header, Func<AnalysisResult, string> Selector)> BuildExportColumns()
    {
        var props = typeof(AnalysisResult)
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p =>
            {
                var type = Nullable.GetUnderlyingType(p.PropertyType) ?? p.PropertyType;

                var isScalar =
                    type.IsPrimitive ||
                    type == typeof(string) ||
                    type == typeof(decimal) ||
                    type == typeof(DateTime) ||
                    type == typeof(Guid) ||
                    type == typeof(bool);

                var isId =
                    string.Equals(p.Name, "Id", StringComparison.OrdinalIgnoreCase) ||
                    p.Name.EndsWith("Id", StringComparison.OrdinalIgnoreCase);

                return isScalar && !isId;
            })
            .ToList();

        var columns = new List<(string Header, Func<AnalysisResult, string> Selector)>();

        foreach (var prop in props)
        {
            var type = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;

            columns.Add((
                prop.Name,
                r =>
                {
                    var raw = prop.GetValue(r);

                    if (raw == null)
                        return string.Empty;

                    if (type == typeof(DateTime))
                        return ((DateTime)raw).ToString("yyyy-MM-dd HH:mm:ss");

                    return raw.ToString() ?? string.Empty;
                }
            ));
        }

        columns.Add(("VesselName", r => r.VesselDetail?.VesselName ?? string.Empty));
        columns.Add(("CompanyName", r => r.Company?.CompanyName ?? string.Empty));

        return columns;
    }
}