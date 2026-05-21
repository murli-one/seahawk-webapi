using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeaHawkServices.Domain.Entities
{
    [NotMapped]
    public class FedExShipmentResult
    {
        public bool Success { get; set; }
        public string? Error { get; set; }

        public string? TrackingNumber { get; set; }

        // Internal FedEx document URLs (from Ship API)
        public string? FedExLabelUrl { get; set; }
        public string? FedExInvoiceUrl { get; set; }
        public string? LabelBase64 { get; set; }
        public string? InvoiceBase64 { get; set; }

        // Public download URLs we show on the confirmation page
        public string? LabelUrl { get; set; }
        public string? CommercialInvoiceUrl { get; set; }

        public string? MsdsUrl { get; set; }  // if you want to keep this here

        // add this:
        public string ApiReference { get; set; }  // e.g. FedEx transaction ID / DHL shipment ID
    }


}
