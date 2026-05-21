using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeaHawkServices.Application.Options
{
    public sealed class DhlShipmentOptions
    {
        // e.g. "https://express.api.dhl.com/mydhlapi/test" for sandbox
        public string BaseUrl { get; set; } = "https://express.api.dhl.com/mydhlapi/test";

        // MyDHL API basic auth credentials
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;

        // DHL Express shipper account number
        public string ShipperAccountNumber { get; set; } = string.Empty;

        // Optional – for Message-Reference header
        public string MessageReferencePrefix { get; set; } = "DDTS";
    }
}
