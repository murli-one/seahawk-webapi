using ClosedXML.Excel;
using Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SeaHawkServices.Application.Services.Interface;
using SeaHawkServices.Domain.StoredProcedures;
using SeaHawkServices.Infrastructure.Data;
using Seahawk_WebAPI.Contracts.AccountHistory;
using System.Globalization;
using System.Reflection;
using System.Xml.Serialization;
using static SeaHawkServices.Domain.Entities.Enums;

namespace Seahawk_WebAPI.Controllers.Api;

[ApiController]
[Authorize]
[Route("api/account-history")]
public class AccountHistoryApiController : ControllerBase
{
    private readonly IAccountHistoryService _accountHistoryService;
    private readonly ILiveDataService _liveDataService;
    private readonly IApplicationUserService _userService;
    private readonly ISampleReportService _sampleReportService;
    private readonly ApplicationDbContext _db;

    public AccountHistoryApiController(
        IAccountHistoryService accountHistoryService,
        ILiveDataService liveDataService,
        IApplicationUserService userService,
        ISampleReportService sampleReportService,
        ApplicationDbContext db)
    {
        _accountHistoryService = accountHistoryService;
        _liveDataService = liveDataService;
        _userService = userService;
        _sampleReportService = sampleReportService;
        _db = db;
    }

    private static readonly HashSet<string> _clientRemovedFields = new(StringComparer.OrdinalIgnoreCase)
    {
        "CstAt80",
        "Comment1",
        "Comment2",
        "ASSESS"
    };

    private static readonly string[] _clientExportOrder =
    {
        "Vessel Name",
        "Port Bunkered",
        "Date Bunkered",
        "Fuel Type",
        "Grade",
        "Specification",
        "Sample Report Date",
        "ClientRefNumber",
        "Sample Number",
        "APIGravity",
        "Density",
        "CstAt40",
        "CstAt50",
        "FlashPoint",
        "PourPointSummStd",
        "CloudPoint",
        "MCR",
        "Ash",
        "Water",
        "Sulphur",
        "Vanadium",
        "Sodium",
        "Aluminum",
        "Silicon",
        "AlSi",
        "TSP",
        "TSE",
        "TSA",
        "Cetane",
        "Appearance",
        "NetCalVal",
        "CCAI",
        "Zn",
        "Ca",
        "P",
        "Mg",
        "Ni",
        "K",
        "Fe",
        "Pb",
        "SampleLocation",
        "SealNumber",
        "Supplier",
        "DateReceived",
        "BunkerReceipt",
        "SupplierDensity",
        "SupplierViscosity",
        "H2S",
        "Compatibility",
        "SampleSentOn",
        "AirwayNumber",
        "SealNumberSupplier",
        "SealNumberRetained",
        "SampleMethod",
        "FTIR",
        "Lubricity",
        "SupplierSulphur",
        "Confirmation",
        "Comment",
        "GrossHeatComb",
        "NetHeatComb",
        "InjTemp10cst",
        "InjTemp15cst",
        "InjTemp20cst",
        "InjTemp25cst"
    };

    private static readonly Dictionary<string, int> _clientDecimals = new(StringComparer.OrdinalIgnoreCase)
    {
        ["APIGravity"] = 1,
        ["Density"] = 1,
        ["CstAt40"] = 2,
        ["CstAt50"] = 1,
        ["FlashPoint"] = 1,
        ["PourPointSummStd"] = 0,
        ["CloudPoint"] = 0,
        ["MCR"] = 2,
        ["Ash"] = 2,
        ["Water"] = 2,
        ["Sulphur"] = 2,
        ["Vanadium"] = 0,
        ["Sodium"] = 0,
        ["Aluminum"] = 0,
        ["Silicon"] = 0,
        ["AlSi"] = 0,
        ["TSP"] = 2,
        ["TSE"] = 2,
        ["TSA"] = 2,
        ["Cetane"] = 1,
        ["NetCalVal"] = 1,
        ["CCAI"] = 0,
        ["Zn"] = 0,
        ["Ca"] = 0,
        ["P"] = 0,
        ["Mg"] = 0,
        ["Ni"] = 0,
        ["K"] = 0,
        ["Fe"] = 0,
        ["Pb"] = 0,
        ["BunkerReceipt"] = 3,
        ["SupplierDensity"] = 1,
        ["SupplierViscosity"] = 2,
        ["SupplierSulphur"] = 2,
        ["GrossHeatComb"] = 0,
        ["NetHeatComb"] = 0,
        ["InjTemp10cst"] = 1,
        ["InjTemp15cst"] = 1,
        ["InjTemp20cst"] = 1,
        ["InjTemp25cst"] = 1
    };

    [HttpGet]
    public async Task<ActionResult<AccountHistoryPagedResponse>> GetAccountHistory(
        [FromQuery] AccountHistoryQueryRequest request)
    {
        var userResult = await GetCurrentUserContextAsync();
        if (userResult.User == null)
            return Unauthorized("User not found.");

        request = NormalizeRequest(request);

        var accountRows = await GetFilteredAccountRowsAsync(
            request,
            userResult.User.Id,
            userResult.IsSystemAdmin);

        accountRows = ApplySorting(
            accountRows,
            request.SortColumn ?? "DateBunkered",
            request.SortDirection ?? "desc").ToList();

        var totalCount = accountRows.Count;
        var totalPages = request.PageSize <= 0
            ? 0
            : (int)Math.Ceiling((double)totalCount / request.PageSize);

        var items = accountRows
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(ToRowDto)
            .ToList();

        var response = new AccountHistoryPagedResponse
        {
            PageNumber = request.Page,
            PageSize = request.PageSize,
            TotalCount = totalCount,
            TotalPages = totalPages,
            CurrentUser = userResult.User.UserName,
            CurrentUserEmail = userResult.User.Email,
            CurrentUserRole = userResult.User.Role.ToString(),
            Items = items
        };

        return Ok(response);
    }

    [HttpGet("filter-options")]
    [AllowAnonymous]
    public ActionResult<AccountHistoryFilterOptionsResponse> GetFilterOptions()
    {
        return Ok(new AccountHistoryFilterOptionsResponse
        {
            ShowingPeriods =
            {
                new OptionDto { Text = "Last 15 Days", Value = "0" },
                new OptionDto { Text = "Last 30 Days", Value = "1" },
                new OptionDto { Text = "Last 60 Days", Value = "2" },
                new OptionDto { Text = "Last 120 Days", Value = "3" },
                new OptionDto { Text = "Current Quarter", Value = "4" },
                new OptionDto { Text = "Last Quarter", Value = "5" },
                new OptionDto { Text = "All", Value = "6" }
            },
            FuelTypes =
            {
                new OptionDto { Text = "All", Value = "0" },
                new OptionDto { Text = "IFO", Value = "1" },
                new OptionDto { Text = "DO", Value = "2" }
            },
            Specifications =
            {
                new OptionDto { Text = "All", Value = "0" },
                new OptionDto { Text = "Normal", Value = "1" },
                new OptionDto { Text = "Caution", Value = "2" },
                new OptionDto { Text = "Critical", Value = "3" },
                new OptionDto { Text = "In Process", Value = "4" },
                new OptionDto { Text = "Completed", Value = "5" },
                new OptionDto { Text = "OK", Value = "6" }
            }
        });
    }

    [HttpPost("export-excel")]
    public async Task<IActionResult> ExportExcel([FromBody] AccountHistoryQueryRequest request)
    {
        var userResult = await GetCurrentUserContextAsync();
        if (userResult.User == null)
            return Unauthorized("User not found.");

        request = NormalizeRequest(request);

        var accountRows = await GetFilteredAccountRowsAsync(
            request,
            userResult.User.Id,
            userResult.IsSystemAdmin);

        accountRows = ApplySorting(
            accountRows,
            request.SortColumn ?? "DateBunkered",
            request.SortDirection ?? "desc").ToList();

        var analysisResults = await GetAnalysisResultsForAccountRowsAsync(accountRows);

        var accountMap = accountRows
            .Where(x => !string.IsNullOrWhiteSpace(x.SampleNumber))
            .GroupBy(x => x.SampleNumber!)
            .ToDictionary(g => g.Key, g => g.First());

        var orderMap = accountRows
            .Select((x, idx) => new { x.SampleNumber, idx })
            .Where(x => !string.IsNullOrWhiteSpace(x.SampleNumber))
            .GroupBy(x => x.SampleNumber!)
            .ToDictionary(g => g.Key, g => g.Min(z => z.idx));

        analysisResults = analysisResults
            .OrderBy(x => x.SampleNumber != null && orderMap.ContainsKey(x.SampleNumber)
                ? orderMap[x.SampleNumber]
                : int.MaxValue)
            .ThenBy(x => x.SampleNumber)
            .ToList();

        var exportColumns = _clientExportOrder
            .Where(c => !_clientRemovedFields.Contains(c))
            .ToList();

        using var workbook = new XLWorkbook();
        var ws = workbook.Worksheets.Add("AccountHistory");

        for (int i = 0; i < exportColumns.Count; i++)
            ws.Cell(1, i + 1).Value = exportColumns[i];

        int row = 2;

        foreach (var analysis in analysisResults)
        {
            var sampleNumber = analysis.SampleNumber ?? "";
            accountMap.TryGetValue(sampleNumber, out var accountRow);

            for (int i = 0; i < exportColumns.Count; i++)
            {
                var columnName = exportColumns[i];
                var columnIndex = i + 1;

                var value = GetExportValue(columnName, accountRow, analysis);

                if (value == null)
                {
                    ws.Cell(row, columnIndex).Value = "";
                    continue;
                }

                if (value is DateTime dateValue)
                {
                    ws.Cell(row, columnIndex).Value = dateValue;

                    if (columnName.Equals("DateReceived", StringComparison.OrdinalIgnoreCase) ||
                        columnName.Equals("SampleSentOn", StringComparison.OrdinalIgnoreCase))
                    {
                        ws.Cell(row, columnIndex).Style.DateFormat.Format = "yyyy-MM-dd HH:mm:ss";
                    }
                    else
                    {
                        ws.Cell(row, columnIndex).Style.DateFormat.Format = "yyyy-MM-dd";
                    }

                    continue;
                }

                if (_clientDecimals.TryGetValue(columnName, out var decimals))
                {
                    if (TryToDecimal(value, out var decimalValue))
                    {
                        ws.Cell(row, columnIndex).Value = decimalValue;
                        ws.Cell(row, columnIndex).Style.NumberFormat.Format = BuildDecimalFormat(decimals);
                    }
                    else
                    {
                        ws.Cell(row, columnIndex).Value = value.ToString();
                    }

                    continue;
                }

                ws.Cell(row, columnIndex).Value = value.ToString();
            }

            row++;
        }

        ws.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);

        var fileName = $"AccountHistory_Export_{DateTime.UtcNow:yyyyMMddHHmmss}.xlsx";

        return File(
            stream.ToArray(),
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            fileName);
    }

    [HttpPost("export-xml")]
    public async Task<IActionResult> ExportXml([FromBody] AccountHistoryQueryRequest request)
    {
        var userResult = await GetCurrentUserContextAsync();
        if (userResult.User == null)
            return Unauthorized("User not found.");

        request = NormalizeRequest(request);

        var accountRows = await GetFilteredAccountRowsAsync(
            request,
            userResult.User.Id,
            userResult.IsSystemAdmin);

        accountRows = ApplySorting(
            accountRows,
            request.SortColumn ?? "DateBunkered",
            request.SortDirection ?? "desc").ToList();

        var analysisResults = await GetAnalysisResultsForAccountRowsAsync(accountRows);

        var accountMap = accountRows
            .Where(x => !string.IsNullOrWhiteSpace(x.SampleNumber))
            .GroupBy(x => x.SampleNumber!)
            .ToDictionary(g => g.Key, g => g.First());

        var orderMap = accountRows
            .Select((x, idx) => new { x.SampleNumber, idx })
            .Where(x => !string.IsNullOrWhiteSpace(x.SampleNumber))
            .GroupBy(x => x.SampleNumber!)
            .ToDictionary(g => g.Key, g => g.Min(z => z.idx));

        analysisResults = analysisResults
            .OrderBy(x => x.SampleNumber != null && orderMap.ContainsKey(x.SampleNumber)
                ? orderMap[x.SampleNumber]
                : int.MaxValue)
            .ThenBy(x => x.SampleNumber)
            .ToList();

        var excluded = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "Id",
            "VesselName",
            "PortBunkered",
            "DateBunkered",
            "FuelType",
            "Specification",
            "SampleReportDate",
            "SampleNumber"
        };

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

                if (!isScalar || isId)
                    return false;

                if (excluded.Contains(p.Name))
                    return false;

                return true;
            })
            .ToList();

        var xmlRows = new List<XmlRowDto>();

        foreach (var analysis in analysisResults)
        {
            var sampleNumber = analysis.SampleNumber ?? "";
            accountMap.TryGetValue(sampleNumber, out var accountRow);

            var row = new XmlRowDto();

            row.Fields.Add(new XmlFieldDto { Name = "Vessel Name", Value = accountRow?.VesselName ?? analysis.VesselDetail?.VesselName ?? "" });
            row.Fields.Add(new XmlFieldDto { Name = "Port Bunkered", Value = accountRow?.PortBunkered ?? analysis.PortBunkered ?? "" });
            row.Fields.Add(new XmlFieldDto { Name = "Date Bunkered", Value = FormatDate(accountRow?.DateBunkered ?? analysis.DateBunkered) });
            row.Fields.Add(new XmlFieldDto { Name = "Fuel Type", Value = accountRow?.FuelType ?? analysis.FuelType ?? "" });
            row.Fields.Add(new XmlFieldDto { Name = "Specification", Value = accountRow?.Specification ?? analysis.Specification ?? "" });
            row.Fields.Add(new XmlFieldDto { Name = "Sample Report Date", Value = FormatDate(accountRow?.DateReceived ?? analysis.SampleReportDate) });
            row.Fields.Add(new XmlFieldDto { Name = "Sample Number", Value = accountRow?.SampleNumber ?? analysis.SampleNumber ?? "" });

            foreach (var prop in props)
            {
                var type = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
                var rawValue = prop.GetValue(analysis);

                string value;

                if (rawValue == null)
                    value = "";
                else if (type == typeof(DateTime))
                    value = ((DateTime)rawValue).ToString("yyyy-MM-dd HH:mm:ss");
                else
                    value = rawValue.ToString() ?? "";

                row.Fields.Add(new XmlFieldDto
                {
                    Name = prop.Name,
                    Value = value
                });
            }

            row.Fields.Add(new XmlFieldDto
            {
                Name = "CompanyName",
                Value = analysis.Company?.CompanyName ?? ""
            });

            xmlRows.Add(row);
        }

        var serializer = new XmlSerializer(
            typeof(List<XmlRowDto>),
            new XmlRootAttribute("AccountHistory"));

        using var ms = new MemoryStream();
        serializer.Serialize(ms, xmlRows);
        ms.Position = 0;

        var fileName = $"AccountHistory_AllColumns_{DateTime.UtcNow:yyyyMMddHHmmss}.xml";

        return File(ms.ToArray(), "application/xml", fileName);
    }

    [HttpGet("live")]
    public async Task<ActionResult<LiveDataResponse>> GetLiveData()
    {
        var userResult = await GetCurrentUserContextAsync();
        if (userResult.User == null)
            return Unauthorized("User not found.");

        var data = await _liveDataService.GetLiveDataAsync(
            userResult.User.Id,
            userResult.IsSystemAdmin);

        var response = new LiveDataResponse
        {
            Columns = data.Select(r => new LiveColumnDto
            {
                VesselName = r.VesselName ?? "",
                SampleNumber = r.SampleNumber ?? "",
                SampleDate = r.SampleSentOn ?? ""
            }).ToList(),

            Samples = data.Cast<object>().ToList()
        };

        return Ok(response);
    }

    [HttpGet("sample/{sampleNumber}")]
    public async Task<IActionResult> GetSamplePdf(
        [FromRoute] string sampleNumber,
        [FromQuery] string disposition = "inline")
    {
        if (string.IsNullOrWhiteSpace(sampleNumber))
            return BadRequest("Sample number is required.");

        var file = await _sampleReportService.GetPdfAsync(sampleNumber);

        if (file is null)
            return NotFound("Report not found.");

        var (fileName, stream) = file.Value;

        if (disposition.Equals("attachment", StringComparison.OrdinalIgnoreCase))
            return File(stream, "application/pdf", fileName);

        Response.Headers["Content-Disposition"] = $"inline; filename=\"{fileName}\"";
        return File(stream, "application/pdf");
    }

    private async Task<(SeaHawkServices.Domain.Entities.ApplicationUser? User, bool IsSystemAdmin)> GetCurrentUserContextAsync()
    {
        var userName = User.Identity?.Name;

        if (string.IsNullOrWhiteSpace(userName))
            return (null, false);

        var user = await _userService.GetUserByUserNameAsync(userName);

        if (user == null)
            return (null, false);

        return (user, user.Role == Role.SystemAdmin);
    }

    private async Task<List<AccountHistoryWithVessel>> GetFilteredAccountRowsAsync(
        AccountHistoryQueryRequest request,
        string userId,
        bool isSystemAdmin)
    {
        var (fromDate, toDate) = ResolveDateRange(request.SelectedShowRange, null, null);

        var fuelType = (FuelType)request.SelectedFuelType;
        var specification = (Specification)request.SelectedSpecification;

        var accountRows = await _accountHistoryService.GetAccountHistoryWithVesselAsync(
            userId,
            isSystemAdmin,
            fuelType,
            specification,
            fromDate,
            toDate,
            request.FilterVesselName ?? "",
            request.FilterPortBunkered ?? "");

        if (request.FilterByFromDate.HasValue)
        {
            var bunkerDate = request.FilterByFromDate.Value.Date;
            accountRows = accountRows
                .Where(x => x.DateBunkered.Date >= bunkerDate)
                .ToList();
        }

        if (request.FilterByToDate.HasValue)
        {
            var sampleReportDate = request.FilterByToDate.Value.Date;
            accountRows = accountRows
                .Where(x => x.DateReceived.Date <= sampleReportDate)
                .ToList();
        }

        if (!string.IsNullOrWhiteSpace(request.FilterVesselName))
        {
            var vesselName = request.FilterVesselName.Trim();

            accountRows = accountRows
                .Where(x =>
                    !string.IsNullOrEmpty(x.VesselName) &&
                    x.VesselName.Contains(vesselName, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        if (!string.IsNullOrWhiteSpace(request.FilterPortBunkered))
        {
            var portBunkered = request.FilterPortBunkered.Trim();

            accountRows = accountRows
                .Where(x =>
                    !string.IsNullOrEmpty(x.PortBunkered) &&
                    x.PortBunkered.Contains(portBunkered, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        return accountRows;
    }

    private async Task<List<AnalysisResult>> GetAnalysisResultsForAccountRowsAsync(
        List<AccountHistoryWithVessel> accountRows)
    {
        var sampleNumbers = accountRows
            .Select(x => x.SampleNumber)
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .Distinct()
            .ToList();

        if (sampleNumbers.Count == 0)
            return new List<AnalysisResult>();

        return await _db.AnalysisResult
            .AsNoTracking()
            .Include(x => x.VesselDetail)
            .Include(x => x.Company)
            .Where(x => x.SampleNumber != null && sampleNumbers.Contains(x.SampleNumber))
            .ToListAsync();
    }

    private static AccountHistoryQueryRequest NormalizeRequest(AccountHistoryQueryRequest? request)
    {
        request ??= new AccountHistoryQueryRequest();

        if (request.Page < 1)
            request.Page = 1;

        if (request.PageSize < 1)
            request.PageSize = 22;

        if (string.IsNullOrWhiteSpace(request.SortColumn))
            request.SortColumn = "DateBunkered";

        request.SortDirection = request.SortDirection?.Equals("asc", StringComparison.OrdinalIgnoreCase) == true
            ? "asc"
            : "desc";

        request.FilterVesselName = request.FilterVesselName?.Trim();
        request.FilterPortBunkered = request.FilterPortBunkered?.Trim();

        return request;
    }

    private static IEnumerable<AccountHistoryWithVessel> ApplySorting(
        IEnumerable<AccountHistoryWithVessel> data,
        string sortColumn,
        string sortDirection)
    {
        var asc = sortDirection.Equals("asc", StringComparison.OrdinalIgnoreCase);

        return sortColumn switch
        {
            "VesselName" => asc
                ? data.OrderBy(x => x.VesselName)
                : data.OrderByDescending(x => x.VesselName),

            "PortBunkered" => asc
                ? data.OrderBy(x => x.PortBunkered)
                : data.OrderByDescending(x => x.PortBunkered),

            "DateBunkered" => asc
                ? data.OrderBy(x => x.DateBunkered)
                : data.OrderByDescending(x => x.DateBunkered),

            "FuelType" => asc
                ? data.OrderBy(x => x.FuelType)
                : data.OrderByDescending(x => x.FuelType),

            "Specification" => asc
                ? data.OrderBy(x => x.Specification)
                : data.OrderByDescending(x => x.Specification),

            "DateReceived" => asc
                ? data.OrderBy(x => x.DateReceived)
                : data.OrderByDescending(x => x.DateReceived),

            "SampleNumber" => asc
                ? data.OrderBy(x => x.SampleNumber)
                : data.OrderByDescending(x => x.SampleNumber),

            "Comment" => asc
                ? data.OrderBy(x => x.Comment)
                : data.OrderByDescending(x => x.Comment),

            _ => data.OrderByDescending(x => x.DateBunkered)
        };
    }

    private static AccountHistoryRowDto ToRowDto(AccountHistoryWithVessel row)
    {
        return new AccountHistoryRowDto
        {
            IMONumber = row.IMONumber,
            VesselName = row.VesselName,
            PortBunkered = row.PortBunkered,
            DateBunkered = row.DateBunkered,
            Specification = row.Specification,
            FuelType = row.FuelType,
            DateReceived = row.DateReceived,
            SampleNumber = row.SampleNumber,
            Comment = row.Comment
        };
    }

    private static (DateTime fromDate, DateTime toDate) ResolveDateRange(
        int selectedShowRange,
        DateTime? from,
        DateTime? to)
    {
        if (from.HasValue && to.HasValue)
            return (from.Value, to.Value);

        var fromDate = DateTime.UtcNow;
        var toDate = DateTime.UtcNow;

        switch (selectedShowRange)
        {
            case 0:
                fromDate = DateTime.UtcNow.AddDays(-15);
                break;

            case 1:
                fromDate = DateTime.UtcNow.AddDays(-30);
                break;

            case 2:
                fromDate = DateTime.UtcNow.AddDays(-60);
                break;

            case 3:
                fromDate = DateTime.UtcNow.AddDays(-120);
                break;

            case 4:
                var currentQuarter = (DateTime.UtcNow.Month - 1) / 3 + 1;
                fromDate = new DateTime(DateTime.UtcNow.Year, (currentQuarter - 1) * 3 + 1, 1);
                toDate = fromDate.AddMonths(3).AddDays(-1);
                break;

            case 5:
                var lastQuarter = (DateTime.UtcNow.Month - 1) / 3;

                if (lastQuarter == 0)
                {
                    fromDate = new DateTime(DateTime.UtcNow.Year - 1, 10, 1);
                    toDate = new DateTime(DateTime.UtcNow.Year - 1, 12, 31);
                }
                else
                {
                    fromDate = new DateTime(DateTime.UtcNow.Year, (lastQuarter - 1) * 3 + 1, 1);
                    toDate = fromDate.AddMonths(3).AddDays(-1);
                }

                break;

            case 6:
            default:
                fromDate = DateTime.MinValue;
                toDate = DateTime.UtcNow;
                break;
        }

        return (fromDate, toDate);
    }

    private static object? GetExportValue(
        string column,
        AccountHistoryWithVessel? accountRow,
        AnalysisResult analysis)
    {
        switch (column)
        {
            case "Vessel Name":
                return accountRow?.VesselName ?? analysis.VesselDetail?.VesselName ?? "";

            case "Port Bunkered":
                return accountRow?.PortBunkered ?? analysis.PortBunkered ?? "";

            case "Date Bunkered":
                return accountRow?.DateBunkered ?? analysis.DateBunkered;

            case "Fuel Type":
                return accountRow?.FuelType ?? analysis.FuelType ?? "";

            case "Specification":
                return accountRow?.Specification ?? analysis.Specification ?? "";

            case "Sample Report Date":
                return accountRow?.DateReceived ?? analysis.SampleReportDate;

            case "Sample Number":
                return accountRow?.SampleNumber ?? analysis.SampleNumber ?? "";

            case "Grade":
                return analysis.Grade ?? accountRow?.Specification ?? analysis.Specification ?? "";
        }

        var analysisProperty = analysis
            .GetType()
            .GetProperty(column, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);

        if (analysisProperty != null)
            return analysisProperty.GetValue(analysis);

        if (accountRow != null)
        {
            var accountProperty = accountRow
                .GetType()
                .GetProperty(column, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);

            if (accountProperty != null)
                return accountProperty.GetValue(accountRow);
        }

        return null;
    }

    private static bool TryToDecimal(object value, out decimal decimalValue)
    {
        decimalValue = 0m;

        if (value == null)
            return false;

        if (value is decimal d)
        {
            decimalValue = d;
            return true;
        }

        if (value is double db)
        {
            decimalValue = (decimal)db;
            return true;
        }

        if (value is float f)
        {
            decimalValue = (decimal)f;
            return true;
        }

        if (value is int i)
        {
            decimalValue = i;
            return true;
        }

        if (value is long l)
        {
            decimalValue = l;
            return true;
        }

        var text = value.ToString()?.Trim();

        if (string.IsNullOrWhiteSpace(text))
            return false;

        return decimal.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out decimalValue)
            || decimal.TryParse(text, NumberStyles.Any, CultureInfo.CurrentCulture, out decimalValue);
    }

    private static string BuildDecimalFormat(int decimals)
    {
        if (decimals <= 0)
            return "0;-0;;@";

        var zeros = new string('0', decimals);
        return $"0.{zeros};-0.{zeros};;@";
    }

    private static string FormatDate(DateTime? date)
    {
        return date.HasValue
            ? date.Value.ToString("yyyy-MM-dd")
            : "";
    }
}