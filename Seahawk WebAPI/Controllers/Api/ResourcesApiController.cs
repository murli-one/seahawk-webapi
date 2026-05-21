using DinkToPdf;
using DinkToPdf.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Seahawk_WebAPI.Contracts.Resources;
using SeaHawkService.Application.Contract;
using SeaHawkServices.Web.ViewModels;
using System.Net;

namespace Seahawk_WebAPI.Controllers.Api;

[ApiController]
[Route("api/resources")]
public class ResourcesApiController : ControllerBase
{
    private readonly IConverter _converter;
    private readonly IEmailSender _emailSender;

    public ResourcesApiController(
        IConverter converter,
        IEmailSender emailSender)
    {
        _converter = converter;
        _emailSender = emailSender;
    }

    [HttpGet]
    [AllowAnonymous]
    public ActionResult<ResourceVM> GetDefaultResource()
    {
        var model = new ResourceVM
        {
            CurrentTab = "EnergyValue",
            EmailFrom = "Info@SeahawkServices.com"
        };

        return Ok(model);
    }

    [HttpGet("fuel-energy")]
    [AllowAnonymous]
    public ActionResult<ResourceVM> GetFuelEnergyCalculation()
    {
        var model = new ResourceVM
        {
            CurrentTab = "EnergyValue",
            EmailFrom = "Info@SeahawkServices.com"
        };

        return Ok(model);
    }

    [HttpPost("fuel-energy")]
    [AllowAnonymous]
    public ActionResult<ResourceVM> CalculateFuelEnergy([FromBody] ResourceVM viewModel)
    {
        if (viewModel == null)
            return BadRequest(new { message = "Request body is required." });

        viewModel.Calculate();
        viewModel.CurrentTab = "EnergyValue";
        viewModel.EmailFrom = "Info@SeahawkServices.com";

        return Ok(viewModel);
    }

    [HttpGet("ccai")]
    [AllowAnonymous]
    public ActionResult<ResourceVM> GetCcaiCalculation()
    {
        var model = new ResourceVM
        {
            CurrentTab = "CCAI",
            EmailFrom = "Info@SeahawkServices.com"
        };

        return Ok(model);
    }

    [HttpPost("ccai")]
    [AllowAnonymous]
    public ActionResult<ResourceVM> CalculateCcai([FromBody] ResourceVM viewModel)
    {
        if (viewModel == null)
            return BadRequest(new { message = "Request body is required." });

        viewModel.CalculateCCAI();
        viewModel.CurrentTab = "CCAI";
        viewModel.EmailFrom = "Info@SeahawkServices.com";

        return Ok(viewModel);
    }

    [HttpGet("temperature")]
    [AllowAnonymous]
    public ActionResult<ResourceVM> GetTemperatureCalculation()
    {
        var model = new ResourceVM
        {
            CurrentTab = "Temperature",
            EmailFrom = "Info@SeahawkServices.com"
        };

        return Ok(model);
    }

    [HttpPost("temperature")]
    [AllowAnonymous]
    public ActionResult<ResourceVM> ConvertTemperature([FromBody] ResourceVM viewModel)
    {
        if (viewModel == null)
            return BadRequest(new { message = "Request body is required." });

        viewModel.ConvertTemperature();
        viewModel.CurrentTab = "Temperature";
        viewModel.EmailFrom = "Info@SeahawkServices.com";

        return Ok(viewModel);
    }

    [HttpGet("sulfur-change-over")]
    [AllowAnonymous]
    public ActionResult<ResourceVM> GetSulfurChangeOverCalculation()
    {
        var model = new ResourceVM
        {
            CurrentTab = "SulfurChange",
            EmailFrom = "Info@SeahawkServices.com"
        };

        return Ok(model);
    }

    [HttpPost("sulfur-change-over")]
    [AllowAnonymous]
    public ActionResult<ResourceVM> CalculateSulfurChangeOver([FromBody] ResourceVM viewModel)
    {
        if (viewModel == null)
            return BadRequest(new { message = "Request body is required." });

        if (viewModel.LowSulfurContent >= 0.10m)
        {
            viewModel.SulphurErrorMessage = "Sulfur content of fuel to be mixed must be less than 0.10%";
            viewModel.CurrentTab = "SulfurChange";
            viewModel.EmailFrom = "Info@SeahawkServices.com";

            return BadRequest(viewModel);
        }

        viewModel.Compute();
        viewModel.CurrentTab = "SulfurChange";
        viewModel.EmailFrom = "Info@SeahawkServices.com";

        return Ok(viewModel);
    }

    [HttpPost("print-report")]
    [AllowAnonymous]
    public IActionResult PrintReport([FromBody] ResourceVM vm)
    {
        if (vm == null)
            return BadRequest(new { message = "Request body is required." });

        var (pdfBytes, fileName, _, _) = BuildEmailReport(vm);

        Response.Headers["Content-Disposition"] = $"inline; filename={fileName}";

        return File(pdfBytes, "application/pdf");
    }

    [HttpPost("download-report")]
    [AllowAnonymous]
    public IActionResult DownloadReport([FromBody] ResourceVM vm)
    {
        if (vm == null)
            return BadRequest(new { message = "Request body is required." });

        var (pdfBytes, fileName, _, _) = BuildEmailReport(vm);

        return File(pdfBytes, "application/pdf", fileName);
    }

    [HttpPost("send-email")]
    [AllowAnonymous]
    public async Task<ActionResult<ResourceMessageResponse>> SendEmail([FromBody] ResourceVM vm)
    {
        if (vm == null)
            return BadRequest(new { message = "Request body is required." });

        if (string.IsNullOrWhiteSpace(vm.EmailTo))
        {
            return BadRequest(new ResourceMessageResponse
            {
                Message = "Recipient email is required."
            });
        }

        var (pdf, fileName, subject, bodyHtml) = BuildEmailReport(vm);

        var attachments = new[]
        {
            (FileName: fileName, Content: pdf, ContentType: "application/pdf")
        };

        await _emailSender.SendEmailAsync(
            vm.EmailTo.Trim(),
            subject,
            bodyHtml,
            attachments);

        return Ok(new ResourceMessageResponse
        {
            Message = "Email with report sent successfully."
        });
    }

    [HttpPost("report-info")]
    [AllowAnonymous]
    public ActionResult<ResourceReportResponse> GetReportInfo([FromBody] ResourceVM vm)
    {
        if (vm == null)
            return BadRequest(new { message = "Request body is required." });

        var (_, fileName, subject, _) = BuildEmailReport(vm);

        return Ok(new ResourceReportResponse
        {
            FileName = fileName,
            CurrentTab = vm.CurrentTab ?? "",
            Subject = subject
        });
    }

    private (byte[] Pdf, string FileName, string Subject, string HtmlBody) BuildEmailReport(ResourceVM vm)
    {
        var tab = (vm.CurrentTab ?? "").Trim();

        string fileName;
        string subject;
        string reportTitle;

        switch (tab)
        {
            case "EnergyValue":
                vm.Calculate();
                fileName = "FuelEnergyCalculation.pdf";
                subject = "SeaHawk Services – Fuel Energy Report";
                reportTitle = "Fuel Energy Calculation Report";
                break;

            case "CCAI":
                vm.CalculateCCAI();
                fileName = "CCAI_Calculator.pdf";
                subject = "SeaHawk Services – CCAI Report";
                reportTitle = "CCAI Calculator Report";
                break;

            case "Temperature":
                vm.ConvertTemperature();
                fileName = "Temperature_Conversion.pdf";
                subject = "SeaHawk Services – Temperature Report";
                reportTitle = "Temperature Conversion Report";
                break;

            case "SulfurChange":
                if (vm.LowSulfurContent >= 0.10m)
                {
                    throw new InvalidOperationException("Sulfur content of fuel to be mixed must be less than 0.10%");
                }

                vm.Compute();
                fileName = "Sulfur_ChangeOver.pdf";
                subject = "SeaHawk Services – Sulfur ChangeOver Report";
                reportTitle = "Sulfur Change Over Report";
                break;

            default:
                vm.Calculate();
                vm.CurrentTab = "EnergyValue";
                fileName = "FuelEnergyCalculation.pdf";
                subject = "SeaHawk Services – Fuel Energy Report";
                reportTitle = "Fuel Energy Calculation Report";
                break;
        }

        var html = BuildReportHtml(reportTitle, vm);

        var doc = new HtmlToPdfDocument
        {
            GlobalSettings = new GlobalSettings
            {
                PaperSize = PaperKind.A4,
                Orientation = Orientation.Portrait,
                Margins = new MarginSettings
                {
                    Top = 25,
                    Bottom = 25,
                    Left = 25,
                    Right = 25
                }
            },
            Objects =
        {
            new ObjectSettings
            {
                HtmlContent = html,
                WebSettings =
                {
                    DefaultEncoding = "utf-8"
                }
            }
        }
        };

        var pdfBytes = _converter.Convert(doc);

        var emailHtml = $@"
<p>Hello,</p>
<p>Please find attached the requested SeaHawk Services report.</p>
<p>Report: <strong>{WebUtility.HtmlEncode(subject)}</strong></p>
<p>Regards,<br/>SeaHawk Services</p>";

        return (pdfBytes, fileName, subject, emailHtml);
    }

    private static string BuildReportHtml(string reportTitle, ResourceVM vm)
    {
        var currentTab = (vm.CurrentTab ?? "").Trim();

        var rows = currentTab switch
        {
            "EnergyValue" => BuildFuelEnergyRows(vm),
            "CCAI" => BuildCcaiRows(vm),
            "Temperature" => BuildTemperatureRows(vm),
            "SulfurChange" => BuildSulfurChangeRows(vm),
            _ => BuildFuelEnergyRows(vm)
        };

        return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8' />
    <title>{WebUtility.HtmlEncode(reportTitle)}</title>
    <style>
        body {{
            font-family: Arial, Helvetica, sans-serif;
            color: #1f2937;
            font-size: 13px;
            line-height: 1.5;
        }}

        .header {{
            border-bottom: 2px solid #0f766e;
            padding-bottom: 12px;
            margin-bottom: 20px;
        }}

        .brand {{
            color: #0f766e;
            font-size: 22px;
            font-weight: bold;
        }}

        .title {{
            font-size: 18px;
            font-weight: bold;
            margin-top: 8px;
        }}

        .meta {{
            color: #6b7280;
            margin-top: 4px;
        }}

        table {{
            width: 100%;
            border-collapse: collapse;
            margin-top: 20px;
        }}

        th {{
            background: #0f766e;
            color: #ffffff;
            text-align: left;
            padding: 10px;
            border: 1px solid #0f766e;
        }}

        td {{
            padding: 10px;
            border: 1px solid #d1d5db;
        }}

        tr:nth-child(even) {{
            background: #f9fafb;
        }}

        .footer {{
            margin-top: 28px;
            color: #6b7280;
            font-size: 12px;
            border-top: 1px solid #e5e7eb;
            padding-top: 10px;
        }}
    </style>
</head>
<body>
    <div class='header'>
        <div class='brand'>SeaHawk Services</div>
        <div class='title'>{WebUtility.HtmlEncode(reportTitle)}</div>
        <div class='meta'>Generated on {DateTime.UtcNow:MM/dd/yyyy HH:mm} UTC</div>
    </div>

    <table>
        <thead>
            <tr>
                <th>Field</th>
                <th>Value</th>
            </tr>
        </thead>
        <tbody>
            {rows}
        </tbody>
    </table>

    <div class='footer'>
        This report was generated by SeaHawk Services.
    </div>
</body>
</html>";
    }

    private static string BuildFuelEnergyRows(ResourceVM vm)
    {
        return string.Join("", new[]
        {
        Row("Current Tab", vm.CurrentTab),
        Row("Fuel Type", vm.FuelType),
        Row("Density", vm.Density),
        Row("Water Content", vm.WaterContent),
        Row("Ash Content", vm.AshContent),
        Row("Sulfur Content", vm.SulfurContent),
        Row("Price Per MT", vm.PricePerMT),
        Row("Gross Specific Energy", vm.GrossSpecificEnergy),
        Row("Net Specific Energy", vm.NetSpecificEnergy),
        Row("Gross Btu/lb", vm.GrossBtulb),
        Row("Net Btu/lb", vm.NetBtulb),
        Row("Gross Btu/gal", vm.GrossBtugal),
        Row("Net Btu/gal", vm.NetBtugal)
    });
    }

    private static string BuildCcaiRows(ResourceVM vm)
    {
        return string.Join("", new[]
        {
        Row("Current Tab", vm.CurrentTab),
        Row("Density", vm.Density),
        Row("Kinematic Viscosity", vm.KinematicViscosity),
        Row("CCAI", vm.CCAI),
        Row("CII", vm.CII)
    });
    }

    private static string BuildTemperatureRows(ResourceVM vm)
    {
        return string.Join("", new[]
        {
        Row("Current Tab", vm.CurrentTab),
        Row("Selected Temperature Unit", vm.SelectedTempratureUnit),
        Row("Value To Convert", vm.ValueToConvert),
        Row("Celsius", vm.Celsius),
        Row("Fahrenheit", vm.Fahrenheit)
    });
    }

    private static string BuildSulfurChangeRows(ResourceVM vm)
    {
        return string.Join("", new[]
        {
        Row("Current Tab", vm.CurrentTab),
        Row("Low Sulfur Content", vm.LowSulfurContent),
        Row("High Sulfur Content", vm.HighSulfurContent),
        Row("Fuel Oil Quantity", vm.FuelOilQty),
        Row("Total Fuel Consumption Rate", vm.TotalFuelConsumptionRate),
        Row("Required Sulfur Content", vm.SelectedValueForRequiredSulfurContent),
        Row("Calculation Step Hours", vm.CalculationStepHours),
        Row("Time Required", vm.TimeRequired),
        Row("Error Message", vm.ErrorMessage),
        Row("Sulphur Error Message", vm.SulphurErrorMessage)
    });
    }

    private static string Row(string label, object? value)
    {
        var safeLabel = WebUtility.HtmlEncode(label);
        var safeValue = WebUtility.HtmlEncode(value?.ToString() ?? "");

        return $@"
<tr>
    <td>{safeLabel}</td>
    <td>{safeValue}</td>
</tr>";
    }
}