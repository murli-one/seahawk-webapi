using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeaHawkServices.Domain.Entities
{
    public class SMTPSettings
    {
        public string Host { get; set; }   // e.g. https://apis.fedex.com
        public string Port { get; set; }         // your FedEx API Key
        public string EnableSsl { get; set; }      // your FedEx API Secret

        // Optional but good to keep configurable
        public string Username { get; set; }       // e.g. https://apis.fedex.com/track/v1/trackingnumbers

        // Keep these if you need them later (labels, shipping, etc.)
        public string Password { get; set; }
        public string FromEmail { get; set; }
        public string DisplayName { get; set; } = "SeaHawk Services";

    }
}
