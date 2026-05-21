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
using Microsoft.Extensions.Options;

namespace SeaHawkService.Infrastructure.Emails
{
    public class SmtpEmailSender : IEmailSender
    {
        private readonly SmtpSettings _smtpSettings;

        public SmtpEmailSender(IOptions<SmtpSettings> smtpSettings)
        {
            _smtpSettings = smtpSettings.Value;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            try
            {
                using (var client = new SmtpClient(_smtpSettings.Host, _smtpSettings.Port))
                {
                    client.Credentials = new NetworkCredential(_smtpSettings.Username, _smtpSettings.Password);
                    client.EnableSsl = _smtpSettings.EnableSsl;

                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress(_smtpSettings.FromEmail, _smtpSettings.DisplayName),
                        Subject = subject,
                        Body = htmlMessage,
                        IsBodyHtml = true
                    };

                    mailMessage.To.Add(email);

                    await client.SendMailAsync(mailMessage);
                }
            }
            catch (Exception ex)
            {

                throw;
            }

        }
   
        public async Task SendEmailAsync(string email, string subject, string htmlMessage,
           IEnumerable<(string FileName, byte[] Content, string ContentType)>? attachments)
        {
            using var client = new SmtpClient(_smtpSettings.Host, _smtpSettings.Port)
            {
                EnableSsl = _smtpSettings.EnableSsl,
                Credentials = new NetworkCredential(_smtpSettings.Username, _smtpSettings.Password)
            };

            using var mail = new MailMessage
            {
                From = new MailAddress(_smtpSettings.FromEmail, _smtpSettings.DisplayName),
                Subject = subject,
                Body = htmlMessage,
                IsBodyHtml = true
            };
            mail.To.Add(email);

            if (attachments != null)
            {
                foreach (var (name, bytes, type) in attachments)
                {
                    var ms = new MemoryStream(bytes, writable: false);
                    var a = new System.Net.Mail.Attachment(ms, name, type);
                    a.ContentDisposition.DispositionType = System.Net.Mime.DispositionTypeNames.Attachment;
                    mail.Attachments.Add(a);
                }
            }

            await client.SendMailAsync(mail);
        }
    }
}
