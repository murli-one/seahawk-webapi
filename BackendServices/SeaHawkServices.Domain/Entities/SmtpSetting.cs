using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeaHawkServices.Domain.Entities
{
    public class SmtpSetting
    {
        public int Id { get; set; }

        public string SmtpHost { get; set; } = "";
        public int SmtpPort { get; set; }

        public string Email { get; set; } = "";
        public string Password { get; set; } = "";

        public bool EnableSSL { get; set; }

        public DateTime UpdatedOn { get; set; } = DateTime.UtcNow;
    }
}