
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeaHawkService.Application.Contract
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string toEmail, string subject, string message);
        Task SendEmailAsync(string email, string subject, string htmlMessage,
     IEnumerable<(string FileName, byte[] Content, string ContentType)>? attachments);
    }
}
