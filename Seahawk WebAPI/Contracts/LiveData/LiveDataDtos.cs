namespace Seahawk_WebAPI.Contracts.LiveData;

public sealed class LiveDataQueryRequest
{
    public string? FilterSampleNumber { get; set; }
    public string? FilterVessel { get; set; }

    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 15;
}

public sealed class LiveDataPagedResponse
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }

    public string? CurrentUser { get; set; }
    public string? CurrentUserEmail { get; set; }
    public string? CurrentUserRole { get; set; }

    public string? FilterSampleNumber { get; set; }
    public string? FilterVessel { get; set; }

    public List<LiveDataColumnDto> Columns { get; set; } = new();
    public List<LiveDataRowDto> Samples { get; set; } = new();
}

public sealed class LiveDataColumnDto
{
    public string VesselName { get; set; } = "";
    public string SampleNumber { get; set; } = "";
    public string SampleDate { get; set; } = "";
}

public sealed class LiveDataRowDto
{
    public string? SampleNumber { get; set; }
    public string? Specification { get; set; }
    public string? VesselName { get; set; }
    public string? SampleLocation { get; set; }
    public string? SampleSentOn { get; set; }
    public string? Port { get; set; }
    public string? FuelType { get; set; }

    public decimal? Density { get; set; }
    public decimal? CstAt50 { get; set; }
    public decimal? Sulphur { get; set; }
    public decimal? PourPointSummStd { get; set; }
    public decimal? FlashPoint { get; set; }
    public decimal? Water { get; set; }
    public decimal? MCR { get; set; }
    public decimal? Aluminum { get; set; }
    public decimal? Silicon { get; set; }
    public decimal? AlSi { get; set; }
    public decimal? Ash { get; set; }
    public decimal? Vanadium { get; set; }
    public decimal? Sodium { get; set; }
    public decimal? TSP { get; set; }
    public decimal? CCAI { get; set; }
    public decimal? Ca { get; set; }
    public decimal? Zn { get; set; }
    public decimal? P { get; set; }
    public decimal? TotalAcid { get; set; }
    public decimal? CloudPoint { get; set; }
    public decimal? Cetane { get; set; }

    public string? Appearance { get; set; }
    public string? FTIR { get; set; }
    public decimal? NetCalVal { get; set; }
}