using Data;

namespace Seahawk_WebAPI.Contracts.AnalysisResults;

public sealed class AnalysisResultQueryRequest
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 15;

    public string? FilterSampleNumber { get; set; }
    public string? FilterVessel { get; set; }
    public string? FilterPort { get; set; }
    public DateTime? FilterBunkerDateFrom { get; set; }
    public DateTime? FilterBunkerDateTo { get; set; }
    public string? FilterFuelType { get; set; }
    public string? FilterComment { get; set; }
    public string? FilterAiQuery { get; set; }
}

public sealed class AnalysisResultPagedResponse
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }

    public string? CurrentUser { get; set; }
    public string? CurrentUserRole { get; set; }

    public List<AnalysisResultListItemDto> Items { get; set; } = new();
}

public sealed class AnalysisResultListItemDto
{
    public int Id { get; set; }
    public string? SampleNumber { get; set; }
    public string? PortBunkered { get; set; }
    public DateTime? DateBunkered { get; set; }
    public string? Specification { get; set; }
    public string? FuelType { get; set; }
    public DateTime? DateReceived { get; set; }
    public string? Comment { get; set; }
    public int? VesselDetailId { get; set; }
    public string? VesselName { get; set; }
    public string? Grade { get; set; }
    public decimal? H2S { get; set; }
    public decimal? AlSi { get; set; }
}

public sealed class AnalysisResultDetailResponse
{
    public int Id { get; set; }

    public Dictionary<string, object?> Fields { get; set; } = new();

    public string? VesselName { get; set; }
    public string? CompanyName { get; set; }
}

public sealed class AnalysisResultFormOptionsResponse
{
    public List<SelectOptionDto> Vessels { get; set; } = new();
    public List<SelectOptionDto> Companies { get; set; } = new();
    public List<SelectOptionDto> SampleCollections { get; set; } = new();
}

public sealed class SelectOptionDto
{
    public string Text { get; set; } = "";
    public string Value { get; set; } = "";
}

public sealed class AnalysisResultUpsertRequest
{
    public AnalysisResult AnalysisResult { get; set; } = new();
}

public sealed class AiSuggestResponse
{
    public List<string> Suggestions { get; set; } = new();
}

public sealed class ReindexTypesenseResponse
{
    public int IndexedCount { get; set; }
    public string Message { get; set; } = "";
}