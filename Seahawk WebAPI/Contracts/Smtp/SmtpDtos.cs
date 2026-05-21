namespace Seahawk_WebAPI.Contracts.Smtp;

public class SmtpSettingDto
{
    public int Id { get; set; }

    public string? Email { get; set; }
    public string? SmtpServer { get; set; }
    public int SmtpPort { get; set; }

    public string? SmtpUsername { get; set; }
    public string? SmtpPassword { get; set; }

    public bool EnableSSL { get; set; }
}

public class UpdateSmtpSettingRequest
{
    public int Id { get; set; }

    public string? Email { get; set; }
    public string? SmtpServer { get; set; }
    public int SmtpPort { get; set; }

    public string? SmtpUsername { get; set; }
    public string? SmtpPassword { get; set; }

    public bool EnableSSL { get; set; }
}

public class SmtpApiResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = "";
}