namespace Seahawk_WebAPI.Contracts.Resources;

public sealed class ResourceMessageResponse
{
    public string Message { get; set; } = string.Empty;
}

public sealed class ResourceReportResponse
{
    public string FileName { get; set; } = string.Empty;
    public string CurrentTab { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
}