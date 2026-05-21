using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using SeaHawkService.Application.Contract;
using Data;
using Microsoft.AspNetCore.Hosting;
using System.Net;
using Microsoft.Extensions.Configuration;

namespace SeaHawkService.Infrastructure.Emails
{
    //public class EmailService : IEmailService
    //{
    //    private readonly IWebHostEnvironment _env;
    //    private readonly IConfiguration _config;

    //    public EmailService(IWebHostEnvironment env, IConfiguration config)
    //    {
    //        _env = env;
    //        _config = config;
    //    }

    //    public async Task SendSampleCollectionEmailAsync(SampleCollection model)
    //    {
    //        string templatePath = Path.Combine(_env.WebRootPath, "MailTemplates", "SampleCollection.htm");
    //        string body = await File.ReadAllTextAsync(templatePath);

    //        body = body.Replace("<%VesselName%>", model.VesselName ?? string.Empty)
    //                   .Replace("<%IMONumber%>", model.IMONumber ?? string.Empty);

    //        var message = new MailMessage
    //        {
    //            From = new MailAddress("Tracking@SeahawkServices.com", "Seahawk Services"),
    //            Subject = "Pickup Request",
    //            Body = body,
    //            IsBodyHtml = true
    //        };

    //        if (!string.IsNullOrWhiteSpace(model.RequestorEmail))
    //            message.To.Add(model.RequestorEmail);

    //        using var smtp = new SmtpClient
    //        {
    //            Host = _config["Smtp:Host"],
    //            Port = int.Parse(_config["Smtp:Port"]),
    //            EnableSsl = bool.Parse(_config["Smtp:EnableSsl"]),
    //            Credentials = new NetworkCredential(_config["Smtp:Username"], _config["Smtp:Password"])
    //        };

    //        await smtp.SendMailAsync(message);
    //    }
    //}
}
