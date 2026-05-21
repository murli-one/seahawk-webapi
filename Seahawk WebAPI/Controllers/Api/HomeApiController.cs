using Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Seahawk_WebAPI.Contracts.Home;
using SeaHawkServices.Application.Services.Interface;
using SeaHawkServices.Domain.Entities;
using System.Net;
using System.Net.Mail;
using System.Web;

namespace Seahawk_WebAPI.Controllers.Api;

[ApiController]
[Route("api/home")]
public class HomeApiController : ControllerBase
{
    private readonly string _uploadsPath;
    private readonly string _thumbsPath;
    private readonly IContactUsService _contactUsService;
    private readonly ICareerService _careerService;
    private readonly IOptions<SMTPSettings> _smtpSettings;

    public HomeApiController(
        IWebHostEnvironment env,
        IContactUsService contactUsService,
        ICareerService careerService,
        IOptions<SMTPSettings> smtpSettings)
    {
        _uploadsPath = Path.Combine(env.WebRootPath, "uploads");
        _thumbsPath = Path.Combine(_uploadsPath, "thumbnails");

        Directory.CreateDirectory(_uploadsPath);
        Directory.CreateDirectory(_thumbsPath);

        _contactUsService = contactUsService;
        _careerService = careerService;
        _smtpSettings = smtpSettings;
    }

    [HttpGet]
    [AllowAnonymous]
    public ActionResult<HomeIndexResponse> GetHome()
    {
        var pdfList = GetPdfFiles();

        var isAuthenticated =
            User.Identity != null &&
            User.Identity.IsAuthenticated &&
            !string.IsNullOrWhiteSpace(User.Identity.Name);

        return Ok(new HomeIndexResponse
        {
            IsAuthenticated = isAuthenticated,
            RedirectTarget = isAuthenticated ? "/api/companies" : null,
            PdfList = pdfList
        });
    }

    [HttpGet("pdfs")]
    [AllowAnonymous]
    public ActionResult<PdfListResponse> GetPdfs()
    {
        return Ok(new PdfListResponse
        {
            PdfList = GetPdfFiles()
        });
    }

    [HttpGet("pdf-preview/{fileName}")]
    [AllowAnonymous]
    public IActionResult PreviewPdf(string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
            return BadRequest(new { message = "File name is required." });

        // Security: prevent path traversal.
        var safeFileName = Path.GetFileName(fileName);

        if (!safeFileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
            return BadRequest(new { message = "Only PDF files are allowed." });

        var filePath = Path.Combine(_uploadsPath, safeFileName);

        if (!System.IO.File.Exists(filePath))
            return NotFound(new { message = "PDF file not found." });

        var fileStream = System.IO.File.OpenRead(filePath);

        Response.Headers["Content-Disposition"] = $"inline; filename=\"{safeFileName}\"";

        return File(fileStream, "application/pdf");
    }

    [HttpGet("careers")]
    [AllowAnonymous]
    public async Task<ActionResult<CareerListResponse>> GetCareers()
    {
        var careers = await _careerService.GetAllAsync();

        return Ok(new CareerListResponse
        {
            Careers = careers
                .OrderByDescending(x => x.Id)
                .Select(ToCareerDto)
                .ToList()
        });
    }

    [HttpGet("contact-us")]
    [Authorize]
    public async Task<ActionResult<ContactUsListResponse>> GetContactUsInfo()
    {
        var contactUsList = await _contactUsService.GetAllAsync();

        return Ok(new ContactUsListResponse
        {
            ContactUsList = contactUsList
                .OrderByDescending(x => x.Id)
                .Select(ToContactUsDto)
                .ToList()
        });
    }

    [HttpPost("contact-us")]
    [AllowAnonymous]
    public async Task<ActionResult<HomeMessageResponse>> SubmitContactUs(
        [FromBody] ContactUsRequest request)
    {
        if (request == null)
            return BadRequest(new { message = "Request body is required." });

        var validationError = ValidateContactUsRequest(request);
        if (validationError != null)
            return BadRequest(new { message = validationError });

        var contactUs = new ContactUs
        {
            FirstName = request.FirstName?.Trim(),
            Email = request.Email?.Trim(),
            Message = request.Message?.Trim()
        };

        await _contactUsService.AddAsync(contactUs);

        await SendContactUsEmailAsync(contactUs);

        return Ok(new HomeMessageResponse
        {
            Message = "Thank you for contacting us. We will get back to you shortly."
        });
    }

    [HttpGet("about-us")]
    [AllowAnonymous]
    public ActionResult<HomeMessageResponse> AboutUs()
    {
        return Ok(new HomeMessageResponse
        {
            Message = "About Us page content should be handled by the React frontend."
        });
    }

    [HttpGet("why-testing")]
    [AllowAnonymous]
    public ActionResult<HomeMessageResponse> WhyTesting()
    {
        return Ok(new HomeMessageResponse
        {
            Message = "Why Testing page content should be handled by the React frontend."
        });
    }

    [HttpGet("privacy")]
    [AllowAnonymous]
    public ActionResult<HomeMessageResponse> Privacy()
    {
        return Ok(new HomeMessageResponse
        {
            Message = "Privacy page content should be handled by the React frontend."
        });
    }

    [HttpGet("sample-collection")]
    [AllowAnonymous]
    public ActionResult<HomeMessageResponse> SampleCollection()
    {
        return Ok(new HomeMessageResponse
        {
            Message = "Use the Sample API endpoint for sample collection functionality."
        });
    }

    private List<string> GetPdfFiles()
    {
        if (!Directory.Exists(_uploadsPath))
            return new List<string>();

        return Directory
            .EnumerateFiles(_uploadsPath, "*.pdf")
            .Select(Path.GetFileName)
            .Where(fileName =>
                !string.IsNullOrWhiteSpace(fileName) &&
                !fileName.StartsWith("Temp_", StringComparison.OrdinalIgnoreCase))
            .OrderBy(fileName => fileName)
            .ToList()!;
    }

    private async Task SendContactUsEmailAsync(ContactUs contactUs)
    {
        var smtpHost = _smtpSettings.Value.Host;
        var smtpPort = Convert.ToInt32(_smtpSettings.Value.Port);
        var smtpUser = _smtpSettings.Value.Username;
        var smtpPass = _smtpSettings.Value.Password;
        var fromEmail = _smtpSettings.Value.FromEmail;
        var displayName = _smtpSettings.Value.DisplayName;

        // Existing MVC logic used fixed recipient.
        var toAddress = "shubham.jos@cisinlabs.com";

        var subject = $"New Contact Enquiry from {contactUs.FirstName}";

        var name = HttpUtility.HtmlEncode(contactUs.FirstName);
        var email = HttpUtility.HtmlEncode(contactUs.Email);
        var message = HttpUtility.HtmlEncode(contactUs.Message);

        var body = $@"
<!DOCTYPE html>
<html>
  <body style=""margin:0;padding:24px;font-family:Segoe UI,Arial,sans-serif;
               font-size:14px;color:#111827;background-color:#f3f4f6;"">

    <table role=""presentation"" cellpadding=""0"" cellspacing=""0"" width=""100%""
           style=""max-width:640px;margin:0 auto;background-color:#ffffff;
                  border-radius:8px;border:1px solid #e5e7eb;"">

      <tr>
        <td style=""padding:20px 24px 12px 24px;border-bottom:1px solid #e5e7eb;"">
          <h2 style=""margin:0;font-size:18px;color:#15447a;"">
            New Contact Request - SeaHawk Services
          </h2>
          <p style=""margin:4px 0 0 0;color:#6b7280;"">
            A new message has been submitted from the website Contact Us form.
          </p>
        </td>
      </tr>

      <tr>
        <td style=""padding:16px 24px 20px 24px;"">

          <h3 style=""margin:0 0 8px 0;font-size:15px;color:#111827;"">
            Sender details
          </h3>

          <table cellpadding=""0"" cellspacing=""0"" width=""100%""
                 style=""border-collapse:collapse;margin-bottom:16px;"">
            <tbody>
              <tr>
                <td style=""padding:4px 8px;width:160px;color:#6b7280;"">Name</td>
                <td style=""padding:4px 8px;""><strong>{name}</strong></td>
              </tr>
              <tr>
                <td style=""padding:4px 8px;color:#6b7280;"">Email</td>
                <td style=""padding:4px 8px;""><a href=""mailto:{email}"">{email}</a></td>
              </tr>
            </tbody>
          </table>

          <h3 style=""margin:0 0 8px 0;font-size:15px;color:#111827;"">
            Message
          </h3>

          <div style=""padding:12px 12px;border-radius:6px;
                      background-color:#f9fafb;border:1px solid #e5e7eb;
                      white-space:pre-wrap;"">
            {message}
          </div>

          <p style=""margin:16px 0 0 0;color:#6b7280;font-size:12px;"">
            This email was generated automatically from the SeaHawk Services website.
          </p>

        </td>
      </tr>
    </table>
  </body>
</html>";

        using var mail = new MailMessage();

        mail.From = new MailAddress(fromEmail, displayName);
        mail.To.Add(new MailAddress(toAddress));

        if (!string.IsNullOrWhiteSpace(contactUs.Email))
        {
            mail.ReplyToList.Add(new MailAddress(contactUs.Email));
        }

        mail.Subject = subject;
        mail.Body = body;
        mail.IsBodyHtml = true;

        using var client = new SmtpClient(smtpHost, smtpPort)
        {
            EnableSsl = true,
            Credentials = new NetworkCredential(smtpUser, smtpPass)
        };

        await client.SendMailAsync(mail);
    }

    private static string? ValidateContactUsRequest(ContactUsRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.FirstName))
            return "First name is required.";

        if (string.IsNullOrWhiteSpace(request.Email))
            return "Email is required.";

        if (string.IsNullOrWhiteSpace(request.Message))
            return "Message is required.";

        return null;
    }

    private static CareerDto ToCareerDto(Career career)
    {
        return new CareerDto
        {
            Id = career.Id,
            Title = career.Title,
            JobDescription = career.JobDescription,
            Experience = career.Experience,
            Status = career.Status.ToString()
        };
    }

    private static ContactUsDto ToContactUsDto(ContactUs contactUs)
    {
        return new ContactUsDto
        {
            Id = contactUs.Id,
            FirstName = contactUs.FirstName,
            Email = contactUs.Email,
            Message = contactUs.Message
        };
    }
}