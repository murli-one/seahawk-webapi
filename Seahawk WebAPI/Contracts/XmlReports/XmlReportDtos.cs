namespace Seahawk_WebAPI.Contracts.XmlReports;

public class GenerateXmlReportRequest
{
    public string? SampleNumber { get; set; }

    // 1 = Residual
    // Any other value = Distillate
    public int SelectedReportType { get; set; }
}

public class XmlReportErrorResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = "";
}