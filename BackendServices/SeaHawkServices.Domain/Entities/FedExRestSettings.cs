using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeaHawkServices.Domain.Entities
{
    public class FedExRestSettings
    {
        public string BaseUrl { get; set; }
        public string ApiKey { get; set; }
        public string ApiSecret { get; set; }
        public string AccountNumber { get; set; }
    }
}
