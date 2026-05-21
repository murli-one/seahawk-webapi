using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Seahawk_WebAPI.Contracts.RequestLogs;
using SeaHawkServices.Domain.Entities;
using SeaHawkServices.Infrastructure.Data;
using System.Xml.Serialization;
using static SeaHawkServices.Domain.Entities.Enums;

namespace Seahawk_WebAPI.Controllers.Api;

[ApiController]
[Route("api/request-logs")]
[Authorize]
public class RequestLogsApiController : ControllerBase
{
    private readonly ApplicationDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;

    public RequestLogsApiController(
        ApplicationDbContext db,
        UserManager<ApplicationUser> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<ActionResult<RequestLogPagedResponse>> GetRequestLogs(
        [FromQuery] RequestLogQueryRequest request)
    {
        request = NormalizeQuery(request);

        var currentUser = await _userManager.GetUserAsync(User);

        if (currentUser == null)
            return Unauthorized(new { message = "User not found." });

        var query = BuildBaseQueryByRole(currentUser);

        query = ApplyFilters(
            query,
            request.FilterRequestType,
            request.FilterRequestId,
            request.FilterAction,
            request.FilterPerformedBy,
            request.FilterNotes);

        var totalCount = await query.CountAsync();

        var pagedLogs = await query
            .OrderByDescending(h => h.Timestamp)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .AsNoTracking()
            .ToListAsync();

        return Ok(new RequestLogPagedResponse
        {
            PageNumber = request.Page,
            PageSize = request.PageSize,
            TotalCount = totalCount,
            TotalPages = request.PageSize <= 0
                ? 0
                : (int)Math.Ceiling((double)totalCount / request.PageSize),

            UserName = currentUser.Name,
            UserEmail = currentUser.Email,
            UserRole = currentUser.Role.ToString(),

            FilterRequestType = request.FilterRequestType,
            FilterRequestId = request.FilterRequestId,
            FilterAction = request.FilterAction,
            FilterPerformedBy = request.FilterPerformedBy,
            FilterNotes = request.FilterNotes,

            Items = pagedLogs.Select(ToDto).ToList()
        });
    }

    [HttpPost("export-excel")]
    public async Task<IActionResult> ExportExcel([FromBody] RequestLogQueryRequest? request)
    {
        request = NormalizeQuery(request ?? new RequestLogQueryRequest());

        var currentUser = await _userManager.GetUserAsync(User);

        if (currentUser == null)
            return Unauthorized(new { message = "User not found." });

        var query = BuildBaseQueryByRole(currentUser);

        query = ApplyFilters(
            query,
            request.FilterRequestType,
            request.FilterRequestId,
            request.FilterAction,
            request.FilterPerformedBy,
            request.FilterNotes);

        var logs = await query
            .OrderByDescending(h => h.Timestamp)
            .AsNoTracking()
            .ToListAsync();

        using var workbook = new XLWorkbook();
        var ws = workbook.Worksheets.Add("RequestLogs");

        ws.Cell(1, 1).Value = "Timestamp";
        ws.Cell(1, 2).Value = "Request Type";
        ws.Cell(1, 3).Value = "Request ID";
        ws.Cell(1, 4).Value = "Action";
        ws.Cell(1, 5).Value = "Performed By";
        ws.Cell(1, 6).Value = "Notes";

        ws.Row(1).Style.Font.Bold = true;

        int row = 2;

        foreach (var log in logs)
        {
            ws.Cell(row, 1).Value = log.Timestamp.ToString("yyyy-MM-dd HH:mm");
            ws.Cell(row, 2).Value = log.RequestType ?? "";
            ws.Cell(row, 3).Value = log.RequestId;
            ws.Cell(row, 4).Value = log.Action.ToString();
            ws.Cell(row, 5).Value = log.PerformedBy ?? "";
            ws.Cell(row, 6).Value = log.Notes ?? "";
            row++;
        }

        ws.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        stream.Position = 0;

        var fileName = $"RequestLogs_{DateTime.UtcNow:yyyyMMdd_HHmmss}.xlsx";

        return File(
            stream.ToArray(),
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            fileName);
    }

    [HttpPost("export-xml")]
    public async Task<IActionResult> ExportXml([FromBody] RequestLogQueryRequest? request)
    {
        request = NormalizeQuery(request ?? new RequestLogQueryRequest());

        var currentUser = await _userManager.GetUserAsync(User);

        if (currentUser == null)
            return Unauthorized(new { message = "User not found." });

        var query = BuildBaseQueryByRole(currentUser);

        query = ApplyFilters(
            query,
            request.FilterRequestType,
            request.FilterRequestId,
            request.FilterAction,
            request.FilterPerformedBy,
            request.FilterNotes);

        var logs = await query
            .OrderByDescending(h => h.Timestamp)
            .AsNoTracking()
            .ToListAsync();

        var exportDtos = logs.Select(x => new RequestLogExportDto
        {
            Timestamp = x.Timestamp.ToString("yyyy-MM-dd HH:mm"),
            RequestType = x.RequestType,
            RequestId = x.RequestId,
            Action = x.Action.ToString(),
            PerformedBy = x.PerformedBy,
            Notes = x.Notes
        }).ToList();

        var serializer = new XmlSerializer(typeof(List<RequestLogExportDto>));

        using var ms = new MemoryStream();
        serializer.Serialize(ms, exportDtos);
        ms.Position = 0;

        var fileName = $"RequestLogs_{DateTime.UtcNow:yyyyMMdd_HHmmss}.xml";

        return File(ms.ToArray(), "application/xml", fileName);
    }

    private IQueryable<RequestHistory> BuildBaseQueryByRole(ApplicationUser currentUser)
    {
        var query = _db.RequestHistory.AsQueryable();

        if (currentUser.Role == Role.SystemAdmin)
        {
            return query;
        }

        if (currentUser.Role == Role.ManagementUser)
        {
            var companyIds = _db.CompanyUser
                .Where(cu => cu.UserId == currentUser.Id)
                .Select(cu => cu.CompanyId)
                .Distinct();

            var vesselIds = _db.VesselDetail
                .Where(v => v.CompanyId != null && companyIds.Contains(v.CompanyId.Value))
                .Select(v => v.Id);

            return query.Where(h =>
                (h.VesselDetailId != null && vesselIds.Contains(h.VesselDetailId.Value)) ||
                h.PerformedBy == currentUser.UserName);
        }

        if (currentUser.Role == Role.VesselUser)
        {
            return query.Where(h => h.PerformedBy == currentUser.UserName);
        }

        return query.Where(h => h.PerformedBy == currentUser.UserName);
    }

    private static IQueryable<RequestHistory> ApplyFilters(
        IQueryable<RequestHistory> query,
        string? filterRequestType,
        string? filterRequestId,
        string? filterAction,
        string? filterPerformedBy,
        string? filterNotes)
    {
        static string Normalize(string? value) => (value ?? "").Trim();

        var requestType = Normalize(filterRequestType);
        var requestId = Normalize(filterRequestId);
        var action = Normalize(filterAction);
        var performedBy = Normalize(filterPerformedBy);
        var notes = Normalize(filterNotes);

        if (!string.IsNullOrWhiteSpace(requestType))
        {
            query = query.Where(x =>
                x.RequestType != null &&
                x.RequestType.Contains(requestType));
        }

        if (!string.IsNullOrWhiteSpace(requestId))
        {
            query = query.Where(x =>
                x.RequestId.ToString().Contains(requestId));
        }

        if (!string.IsNullOrWhiteSpace(action))
        {
            query = query.Where(x =>
                x.Action.ToString().Contains(action));
        }

        if (!string.IsNullOrWhiteSpace(performedBy))
        {
            query = query.Where(x =>
                x.PerformedBy != null &&
                x.PerformedBy.Contains(performedBy));
        }

        if (!string.IsNullOrWhiteSpace(notes))
        {
            query = query.Where(x =>
                x.Notes != null &&
                x.Notes.Contains(notes));
        }

        return query;
    }

    private static RequestLogQueryRequest NormalizeQuery(RequestLogQueryRequest request)
    {
        if (request.Page < 1)
            request.Page = 1;

        if (request.PageSize < 1)
            request.PageSize = 15;

        request.FilterRequestType = request.FilterRequestType?.Trim();
        request.FilterRequestId = request.FilterRequestId?.Trim();
        request.FilterAction = request.FilterAction?.Trim();
        request.FilterPerformedBy = request.FilterPerformedBy?.Trim();
        request.FilterNotes = request.FilterNotes?.Trim();

        return request;
    }

    private static RequestLogDto ToDto(RequestHistory log)
    {
        return new RequestLogDto
        {
            Id = log.Id,
            Timestamp = log.Timestamp,
            TimestampText = log.Timestamp.ToString("yyyy-MM-dd HH:mm"),

            RequestType = log.RequestType,
            RequestId = log.RequestId,

            Action = log.Action,
            ActionName = log.Action.ToString(),

            PerformedBy = log.PerformedBy,
            Notes = log.Notes,

            VesselDetailId = log.VesselDetailId
        };
    }
}