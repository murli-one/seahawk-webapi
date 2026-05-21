namespace Seahawk_WebAPI.Contracts.Companies;

public sealed class CompanyQueryRequest
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 13;

    public string? FilterCompanyName { get; set; }
    public string? FilterCity { get; set; }
    public string? FilterCountry { get; set; }
}

public sealed class CompanyPagedResponse
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }

    public string? CurrentUser { get; set; }
    public string? CurrentUserEmail { get; set; }
    public string? CurrentUserRole { get; set; }

    public string? FilterCompanyName { get; set; }
    public string? FilterCity { get; set; }
    public string? FilterCountry { get; set; }

    public List<CompanyDto> Items { get; set; } = new();
}

public sealed class CompanyDto
{
    public int Id { get; set; }
    public string? CompanyName { get; set; }
    public string? BillingAddress { get; set; }
    public string? City { get; set; }
    public string? StateOrProvince { get; set; }
    public string? PostalCode { get; set; }
    public string? Country { get; set; }
    public string? PhoneNumber { get; set; }
    public string? FaxNumber { get; set; }
    public string? EmailAddress { get; set; }
    public string? Notes { get; set; }
    public string? ShipOwner { get; set; }
    public string? FuelSupplier { get; set; }
    public string? ContractLab { get; set; }
    public string? Charterer { get; set; }
    public string? InvoiceType { get; set; }
    public string? CompanyKey { get; set; }
    public string? FaxCountry { get; set; }
    public string? FaxArea { get; set; }
    public string? ClientRef { get; set; }

    public List<CompanyVesselDto> AssignedVessels { get; set; } = new();
}

public sealed class CompanyUpsertRequest
{
    public string? CompanyName { get; set; }
    public string? BillingAddress { get; set; }
    public string? City { get; set; }
    public string? StateOrProvince { get; set; }
    public string? PostalCode { get; set; }
    public string? Country { get; set; }
    public string? PhoneNumber { get; set; }
    public string? FaxNumber { get; set; }
    public string? EmailAddress { get; set; }
    public string? Notes { get; set; }
    public string? ShipOwner { get; set; }
    public string? FuelSupplier { get; set; }
    public string? ContractLab { get; set; }
    public string? Charterer { get; set; }
    public string? InvoiceType { get; set; }
    public string? CompanyKey { get; set; }
    public string? FaxCountry { get; set; }
    public string? FaxArea { get; set; }
    public string? ClientRef { get; set; }
}

public sealed class CompanyDetailsResponse
{
    public CompanyDto Company { get; set; } = new();
    public List<CompanyVesselDto> AvailableVessels { get; set; } = new();
}

public sealed class CompanyVesselDto
{
    public int Id { get; set; }
    public string? VesselName { get; set; }
    public string? IMONumber { get; set; }
    public string? Owner { get; set; }
    public string? Email { get; set; }
    public int? CompanyId { get; set; }
}

public sealed class AssignVesselsRequest
{
    public List<int> SelectedVesselIds { get; set; } = new();
}

public sealed class CompanyMessageResponse
{
    public string Message { get; set; } = string.Empty;
    public int SuccessCount { get; set; }
    public int SkippedCount { get; set; }
}