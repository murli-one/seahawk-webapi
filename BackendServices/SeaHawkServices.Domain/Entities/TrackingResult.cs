using SeaHawkServices.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Data
{
    [NotMapped]
    public class TrackingResult
    {
        public bool Success { get; set; }
        public string Waybill { get; set; } = "";
        public string? StatusText { get; set; }
        public string? Error { get; set; }
        public List<TrackingEvent> Events { get; set; } = new();

        // ---- New summary fields ----
        public string ServiceDescription { get; set; } = "";        // e.g. "FedEx Ground"
        public string ShipperCity { get; set; } = "";
        public string ShipperCountryCode { get; set; } = "";
        public string RecipientCity { get; set; } = "";
        public string RecipientCountryCode { get; set; } = "";
        public DateTimeOffset? ShipDate { get; set; }
        public DateTimeOffset? ActualPickupDate { get; set; }
    }
}
