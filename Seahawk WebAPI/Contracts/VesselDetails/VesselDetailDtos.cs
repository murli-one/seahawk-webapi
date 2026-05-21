namespace Seahawk_WebAPI.Contracts.VesselDetails;

public class VesselDetailListRequest
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 14;

    public string? FilterVesselName { get; set; }
    public string? FilterIMONumber { get; set; }
    public string? FilterOwner { get; set; }
    public string? FilterCallSign { get; set; }
    public string? FilterCompanyName { get; set; }
}

public class VesselDetailListResponse
{
    public string? CurrentUser { get; set; }
    public string? CurrentUserEmail { get; set; }
    public string? CurrentUserRole { get; set; }

    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }

    public int TotalPages =>
        PageSize == 0 ? 0 : (int)Math.Ceiling((double)TotalCount / PageSize);

    public string? FilterVesselName { get; set; }
    public string? FilterIMONumber { get; set; }
    public string? FilterOwner { get; set; }
    public string? FilterCallSign { get; set; }
    public string? FilterCompanyName { get; set; }

    public List<VesselDetailDto> Items { get; set; } = new();
}

public class VesselDetailDto
{
    public int Id { get; set; }

    public string? VesselName { get; set; }
    public string? IMONumber { get; set; }

    public string? BillTo { get; set; }
    public string? Built { get; set; }

    public string? ByEMail { get; set; }
    public string? ByFax { get; set; }
    public string? ByTelex { get; set; }

    public string? CallSign { get; set; }
    public string? Charterer { get; set; }
    public string? Class { get; set; }
    public string? Comments { get; set; }
    public string? CommissionType { get; set; }

    public string? DOGrade { get; set; }
    public string? DOReportType { get; set; }
    public string? Diesel { get; set; }

    public string? Draft { get; set; }
    public string? Dwt { get; set; }

    public string? Email { get; set; }
    public string? ExVesselName { get; set; }
    public string? FOReportType { get; set; }

    public string? FaxArea { get; set; }
    public string? FaxCountry { get; set; }
    public string? FaxNumber { get; set; }

    public string? Filter { get; set; }
    public string? FuelSystem { get; set; }

    public string? GO { get; set; }
    public string? GOGrade { get; set; }

    public string? GeneratorType { get; set; }

    public string? HFO { get; set; }
    public string? HFOGrade { get; set; }

    public string? IFO { get; set; }
    public string? IFOGrade { get; set; }

    public string? Owner { get; set; }
    public string? Propulsion { get; set; }
    public string? Purifier { get; set; }
    public string? Registry { get; set; }

    public string? TlxNumber { get; set; }
    public string? Type { get; set; }

    public int? CompanyId { get; set; }
    public string? CompanyName { get; set; }
}

public class VesselDetailCreateUpdateRequest
{
    public string? VesselName { get; set; }
    public string? IMONumber { get; set; }

    public string? BillTo { get; set; }
    public string? Built { get; set; }

    public string? ByEMail { get; set; }
    public string? ByFax { get; set; }
    public string? ByTelex { get; set; }

    public string? CallSign { get; set; }
    public string? Charterer { get; set; }
    public string? Class { get; set; }
    public string? Comments { get; set; }
    public string? CommissionType { get; set; }

    public string? DOGrade { get; set; }
    public string? DOReportType { get; set; }
    public string? Diesel { get; set; }

    public string? Draft { get; set; }
    public string? Dwt { get; set; }

    public string? Email { get; set; }
    public string? ExVesselName { get; set; }
    public string? FOReportType { get; set; }

    public string? FaxArea { get; set; }
    public string? FaxCountry { get; set; }
    public string? FaxNumber { get; set; }

    public string? Filter { get; set; }
    public string? FuelSystem { get; set; }

    public string? GO { get; set; }
    public string? GOGrade { get; set; }

    public string? GeneratorType { get; set; }

    public string? HFO { get; set; }
    public string? HFOGrade { get; set; }

    public string? IFO { get; set; }
    public string? IFOGrade { get; set; }

    public string? Owner { get; set; }
    public string? Propulsion { get; set; }
    public string? Purifier { get; set; }
    public string? Registry { get; set; }

    public string? TlxNumber { get; set; }
    public string? Type { get; set; }

    public int? CompanyId { get; set; }
}

public class VesselDetailOptionsResponse
{
    public string? CurrentUserRole { get; set; }
    public List<SelectOptionDto> Companies { get; set; } = new();
}

public class SelectOptionDto
{
    public string Value { get; set; } = "";
    public string Text { get; set; } = "";
    public bool Selected { get; set; }
}

public class VesselDetailApiResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = "";
}