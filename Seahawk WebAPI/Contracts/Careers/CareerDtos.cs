using static SeaHawkServices.Domain.Entities.Enums;

namespace Seahawk_WebAPI.Contracts.Careers;

public sealed class CareerQueryRequest
{
    public string? FilterTitle { get; set; }
    public string? FilterStatus { get; set; }

    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 15;
}

public sealed class CareerPagedResponse
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }

    public string? CurrentUser { get; set; }
    public string? CurrentUserEmail { get; set; }
    public string? CurrentUserRole { get; set; }

    public List<CareerDto> Items { get; set; } = new();
}

public sealed class CareerDto
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public string? JobDescription { get; set; }
    public int Experience { get; set; }
    public Status Status { get; set; }
    public string StatusName { get; set; } = "";
}

public sealed class CareerDetailsResponse
{
    public CareerDto Career { get; set; } = new();
    public List<CareerDto> LatestCareers { get; set; } = new();
}

public sealed class CareerUpsertRequest
{
    public string Title { get; set; } = "";
    public string JobDescription { get; set; } = "";
    public int Experience { get; set; }
    public Status Status { get; set; } = Status.Active;
}

public sealed class CareerFilterOptionsResponse
{
    public List<CareerStatusOptionDto> Statuses { get; set; } = new();
}

public sealed class CareerStatusOptionDto
{
    public string Text { get; set; } = "";
    public string Value { get; set; } = "";
    public int NumericValue { get; set; }
}