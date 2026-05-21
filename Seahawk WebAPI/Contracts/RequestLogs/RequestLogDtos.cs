using SeaHawkServices.Domain.Entities;
using static SeaHawkServices.Domain.Entities.Enums;

namespace Seahawk_WebAPI.Contracts.RequestLogs;

public sealed class RequestLogQueryRequest
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 15;

    public string? FilterRequestType { get; set; }
    public string? FilterRequestId { get; set; }
    public string? FilterAction { get; set; }
    public string? FilterPerformedBy { get; set; }
    public string? FilterNotes { get; set; }
}

public sealed class RequestLogPagedResponse
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }

    public string? UserName { get; set; }
    public string? UserEmail { get; set; }
    public string? UserRole { get; set; }

    public string? FilterRequestType { get; set; }
    public string? FilterRequestId { get; set; }
    public string? FilterAction { get; set; }
    public string? FilterPerformedBy { get; set; }
    public string? FilterNotes { get; set; }

    public List<RequestLogDto> Items { get; set; } = new();
}

public sealed class RequestLogDto
{
    public int Id { get; set; }
    public DateTime Timestamp { get; set; }
    public string TimestampText { get; set; } = "";

    public string? RequestType { get; set; }
    public int RequestId { get; set; }

    public RequestHistoryAction Action { get; set; }
    public string ActionName { get; set; } = "";

    public string? PerformedBy { get; set; }
    public string? Notes { get; set; }

    public int? VesselDetailId { get; set; }
}

[Serializable]
public sealed class RequestLogExportDto
{
    public string Timestamp { get; set; } = "";
    public string? RequestType { get; set; }
    public int RequestId { get; set; }
    public string Action { get; set; } = "";
    public string? PerformedBy { get; set; }
    public string? Notes { get; set; }
}