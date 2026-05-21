using Data;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Seahawk_WebAPI.Contracts.LiveData;
using SeaHawkServices.Application.Services.Interface;
using SeaHawkServices.Domain.StoredProcedures;
using System.Xml.Linq;
using static SeaHawkServices.Domain.Entities.Enums;

namespace Seahawk_WebAPI.Controllers.Api;

[ApiController]
[Route("api/live-data")]
[Authorize]
public class LiveDataApiController : ControllerBase
{
    private readonly ILiveDataService _liveDataService;
    private readonly IApplicationUserService _userService;

    public LiveDataApiController(
        ILiveDataService liveDataService,
        IApplicationUserService userService)
    {
        _liveDataService = liveDataService;
        _userService = userService;
    }

    [HttpGet]
    public async Task<ActionResult<LiveDataPagedResponse>> GetLiveData(
        [FromQuery] LiveDataQueryRequest request)
    {
        request = NormalizeQuery(request);

        var userName = User.Identity?.Name;
        var userRecord = await _userService.GetUserByUserNameAsync(userName);

        if (userRecord == null)
            return Unauthorized(new { message = "User not found." });

        var data = await _liveDataService.GetLiveDataAsync(
            userRecord.Id,
            userRecord.Role == Role.SystemAdmin);

        var list = ApplyFilters(data.ToList(), request);

        var totalCount = list.Count;

        var pageItems = list
            .OrderByDescending(x => x.SampleSentOn)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        return Ok(new LiveDataPagedResponse
        {
            PageNumber = request.Page,
            PageSize = request.PageSize,
            TotalCount = totalCount,
            TotalPages = request.PageSize <= 0
                ? 0
                : (int)Math.Ceiling((double)totalCount / request.PageSize),

            CurrentUser = userRecord.UserName,
            CurrentUserEmail = userRecord.Email,
            CurrentUserRole = userRecord.Role.ToString(),

            FilterSampleNumber = request.FilterSampleNumber,
            FilterVessel = request.FilterVessel,

            Columns = pageItems.Select(r => new LiveDataColumnDto
            {
                VesselName = r.VesselName ?? "",
                SampleNumber = r.SampleNumber ?? "",
                SampleDate = r.SampleSentOn ?? ""
            }).ToList(),

            Samples = pageItems.Select(ToDto).ToList()
        });
    }

    [HttpGet("export-xml")]
    public async Task<IActionResult> ExportXml(
        [FromQuery] LiveDataQueryRequest request)
    {
        request = NormalizeQuery(request);

        var userName = User.Identity?.Name;
        var userRecord = await _userService.GetUserByUserNameAsync(userName);

        if (userRecord == null)
            return Unauthorized(new { message = "User not found." });

        var data = await _liveDataService.GetLiveDataAsync(
            userRecord.Id,
            userRecord.Role == Role.SystemAdmin);

        var list = ApplyFilters(data.ToList(), request);

        var xml = new XDocument(
            new XElement("Samples",
                list.Select(s =>
                    new XElement("Sample",
                        new XElement("SampleNumber", s.SampleNumber),
                        new XElement("Specification", s.Specification),
                        new XElement("VesselName", s.VesselName),
                        new XElement("SampleLocation", s.SampleLocation),
                        new XElement("SampleSentOn", s.SampleSentOn),
                        new XElement("Port", s.Port),
                        new XElement("FuelType", s.FuelType),
                        new XElement("Density", s.Density),
                        new XElement("CstAt50", s.CstAt50),
                        new XElement("Sulphur", s.Sulphur),
                        new XElement("PourPointSummStd", s.PourPointSummStd),
                        new XElement("FlashPoint", s.FlashPoint),
                        new XElement("Water", s.Water),
                        new XElement("MCR", s.MCR),
                        new XElement("Aluminum", s.Aluminum),
                        new XElement("Silicon", s.Silicon),
                        new XElement("AlSi", s.AlSi),
                        new XElement("Ash", s.Ash),
                        new XElement("Vanadium", s.Vanadium),
                        new XElement("Sodium", s.Sodium),
                        new XElement("TSP", s.TSP),
                        new XElement("CCAI", s.CCAI),
                        new XElement("Ca", s.Ca),
                        new XElement("Zn", s.Zn),
                        new XElement("P", s.P),
                        new XElement("TotalAcid", s.TotalAcid),
                        new XElement("CloudPoint", s.CloudPoint),
                        new XElement("Cetane", s.Cetane),
                        new XElement("Appearance", s.Appearance),
                        new XElement("FTIR", s.FTIR),
                        new XElement("NetCalVal", s.NetCalVal)
                    )
                )
            )
        );

        var bytes = System.Text.Encoding.UTF8.GetBytes(xml.ToString());

        return File(
            bytes,
            "application/xml",
            $"LiveData_{DateTime.UtcNow:yyyyMMddHHmmss}.xml");
    }

    [HttpPost("export-excel")]
    public async Task<IActionResult> ExportExcel(
        [FromBody] LiveDataQueryRequest? request)
    {
        request = NormalizeQuery(request ?? new LiveDataQueryRequest());

        var userName = User.Identity?.Name;
        var userRecord = await _userService.GetUserByUserNameAsync(userName);

        if (userRecord == null)
            return Unauthorized(new { message = "User not found." });

        var data = await _liveDataService.GetLiveDataAsync(
            userRecord.Id,
            userRecord.Role == Role.SystemAdmin);

        var list = ApplyFilters(data.ToList(), request);

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
                Name = "Samples"
            });

            var headers = new[]
            {
                "Sample Number",
                "Specification",
                "Vessel Name",
                "Sample Location",
                "Sample Sent On",
                "Port",
                "Fuel Type",
                "Density",
                "Cst @ 50",
                "Sulphur",
                "Pour Point Summ Std",
                "Flash Point",
                "Water",
                "MCR",
                "Aluminum",
                "Silicon",
                "Al+Si",
                "Ash",
                "Vanadium",
                "Sodium",
                "TSP",
                "CCAI",
                "Ca",
                "Zn",
                "P",
                "Total Acid",
                "Cloud Point",
                "Cetane",
                "Appearance",
                "FTIR",
                "Net Cal Val"
            };

            var maxLengths = new int[headers.Length];

            var headerRow = new Row();

            for (int i = 0; i < headers.Length; i++)
            {
                headerRow.AppendChild(CreateTextCell(headers[i]));
                maxLengths[i] = headers[i].Length;
            }

            sheetData.AppendChild(headerRow);

            foreach (var s in list)
            {
                var values = new[]
                {
                    s.SampleNumber ?? "",
                    s.Specification ?? "",
                    s.VesselName ?? "",
                    s.SampleLocation ?? "",
                    s.SampleSentOn ?? "",
                    s.Port ?? "",
                    s.FuelType ?? "",
                    s.Density?.ToString() ?? "",
                    s.CstAt50?.ToString() ?? "",
                    s.Sulphur?.ToString() ?? "",
                    s.PourPointSummStd?.ToString() ?? "",
                    s.FlashPoint?.ToString() ?? "",
                    s.Water?.ToString() ?? "",
                    s.MCR?.ToString() ?? "",
                    s.Aluminum?.ToString() ?? "",
                    s.Silicon?.ToString() ?? "",
                    s.AlSi?.ToString() ?? "",
                    s.Ash?.ToString() ?? "",
                    s.Vanadium?.ToString() ?? "",
                    s.Sodium?.ToString() ?? "",
                    s.TSP?.ToString() ?? "",
                    s.CCAI?.ToString() ?? "",
                    s.Ca?.ToString() ?? "",
                    s.Zn?.ToString() ?? "",
                    s.P?.ToString() ?? "",
                    s.TotalAcid?.ToString() ?? "",
                    s.CloudPoint?.ToString() ?? "",
                    s.Cetane?.ToString() ?? "",
                    s.Appearance ?? "",
                    s.FTIR?.ToString() ?? "",
                    s.NetCalVal?.ToString() ?? ""
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
            $"LiveData_{DateTime.UtcNow:yyyyMMddHHmmss}.xlsx");
    }

    private static List<LiveDataRow> ApplyFilters(
      List<LiveDataRow> list,
      LiveDataQueryRequest request)
    {
        if (!string.IsNullOrWhiteSpace(request.FilterSampleNumber))
        {
            var sample = request.FilterSampleNumber.Trim();

            list = list.Where(x =>
                !string.IsNullOrEmpty(x.SampleNumber) &&
                x.SampleNumber.Contains(sample, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        if (!string.IsNullOrWhiteSpace(request.FilterVessel))
        {
            var vessel = request.FilterVessel.Trim();

            list = list.Where(x =>
                !string.IsNullOrEmpty(x.VesselName) &&
                x.VesselName.Contains(vessel, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        return list;
    }
    private static LiveDataQueryRequest NormalizeQuery(LiveDataQueryRequest request)
    {
        if (request.Page < 1)
            request.Page = 1;

        if (request.PageSize < 1)
            request.PageSize = 15;

        request.FilterSampleNumber = request.FilterSampleNumber?.Trim();
        request.FilterVessel = request.FilterVessel?.Trim();

        return request;
    }

    private static LiveDataRowDto ToDto(LiveDataRow s)
    {
        return new LiveDataRowDto
        {
            SampleNumber = s.SampleNumber,
            Specification = s.Specification,
            VesselName = s.VesselName,
            SampleLocation = s.SampleLocation,
            SampleSentOn = s.SampleSentOn,
            Port = s.Port,
            FuelType = s.FuelType,

            Density = s.Density,
            CstAt50 = s.CstAt50,
            Sulphur = s.Sulphur,
            PourPointSummStd = s.PourPointSummStd,
            FlashPoint = s.FlashPoint,
            Water = s.Water,
            MCR = s.MCR,
            Aluminum = s.Aluminum,
            Silicon = s.Silicon,
            AlSi = s.AlSi,
            Ash = s.Ash,
            Vanadium = s.Vanadium,
            Sodium = s.Sodium,
            TSP = s.TSP,
            CCAI = s.CCAI,
            Ca = s.Ca,
            Zn = s.Zn,
            P = s.P,
            TotalAcid = s.TotalAcid,
            CloudPoint = s.CloudPoint,
            Cetane = s.Cetane,

            Appearance = s.Appearance,
            FTIR = s.FTIR?.ToString(),
            NetCalVal = s.NetCalVal
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