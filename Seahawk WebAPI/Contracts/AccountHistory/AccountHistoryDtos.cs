using System.Xml.Serialization;

namespace Seahawk_WebAPI.Contracts.AccountHistory;

public sealed class AccountHistoryQueryRequest
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 22;

    public int SelectedShowRange { get; set; } = 6;
    public int SelectedFuelType { get; set; } = 0;
    public int SelectedSpecification { get; set; } = 0;

    public DateTime? FilterByFromDate { get; set; }
    public DateTime? FilterByToDate { get; set; }

    public string? FilterVesselName { get; set; }
    public string? FilterPortBunkered { get; set; }

    public string? SortColumn { get; set; } = "DateBunkered";
    public string? SortDirection { get; set; } = "desc";
}

public sealed class AccountHistoryRowDto
{
    public string? IMONumber { get; set; }
    public string? VesselName { get; set; }
    public string? PortBunkered { get; set; }
    public DateTime DateBunkered { get; set; }
    public string? Specification { get; set; }
    public string? FuelType { get; set; }
    public DateTime DateReceived { get; set; }
    public string? SampleNumber { get; set; }
    public string? Comment { get; set; }
}

public sealed class AccountHistoryPagedResponse
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }

    public string? CurrentUser { get; set; }
    public string? CurrentUserEmail { get; set; }
    public string? CurrentUserRole { get; set; }

    public List<AccountHistoryRowDto> Items { get; set; } = new();
}

public sealed class AccountHistoryFilterOptionsResponse
{
    public List<OptionDto> ShowingPeriods { get; set; } = new();
    public List<OptionDto> FuelTypes { get; set; } = new();
    public List<OptionDto> Specifications { get; set; } = new();
}

public sealed class OptionDto
{
    public string Text { get; set; } = "";
    public string Value { get; set; } = "";
}

public sealed class LiveDataResponse
{
    public List<LiveColumnDto> Columns { get; set; } = new();
    public List<object> Samples { get; set; } = new();
}

public sealed class LiveColumnDto
{
    public string VesselName { get; set; } = "";
    public string SampleNumber { get; set; } = "";
    public string SampleDate { get; set; } = "";
}

[XmlRoot("Row")]
public sealed class XmlRowDto
{
    [XmlElement("Field")]
    public List<XmlFieldDto> Fields { get; set; } = new();
}

public sealed class XmlFieldDto
{
    [XmlAttribute("name")]
    public string Name { get; set; } = "";

    [XmlText]
    public string Value { get; set; } = "";
}