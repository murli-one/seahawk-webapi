using Data;

namespace Seahawk_WebAPI.Contracts.Home;

public sealed class HomeIndexResponse
{
    public bool IsAuthenticated { get; set; }
    public string? RedirectTarget { get; set; }
    public List<string> PdfList { get; set; } = new();
}

public sealed class PdfListResponse
{
    public List<string> PdfList { get; set; } = new();
}

public sealed class CareerListResponse
{
    public List<CareerDto> Careers { get; set; } = new();
}

public sealed class CareerDto
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public string? JobDescription { get; set; }
    public int Experience { get; set; }
    public string Status { get; set; } = string.Empty;
}

public sealed class ContactUsRequest
{
    public string? FirstName { get; set; }
    public string? Email { get; set; }
    public string? Message { get; set; }
}

public sealed class ContactUsDto
{
    public int Id { get; set; }
    public string? FirstName { get; set; }
    public string? Email { get; set; }
    public string? Message { get; set; }
}

public sealed class ContactUsListResponse
{
    public List<ContactUsDto> ContactUsList { get; set; } = new();
}

public sealed class HomeMessageResponse
{
    public string Message { get; set; } = string.Empty;
}