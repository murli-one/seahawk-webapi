using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeaHawkServices.Domain.Entities
{
    public sealed class DhlShipmentResult
    {
        public bool Success { get; set; }
        public string? Error { get; set; }

        public string? TrackingNumber { get; set; }
        public string? TrackingUrl { get; set; }

        // Base64 encoded PDF documents returned by DHL
        public string? LabelBase64 { get; set; }
        public string? InvoiceBase64 { get; set; }

        // add this:
        public string ApiReference { get; set; }  // e.g. FedEx transaction ID / DHL shipment ID
    }
}
