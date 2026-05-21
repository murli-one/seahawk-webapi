using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeaHawkServices.Domain.Entities
{
    [NotMapped]
    public class TrackingEvent
    {
        public DateTimeOffset When { get; set; }
        public string Code { get; set; } = "";
        public string Description { get; set; } = "";
        public string City { get; set; } = "";
        public string CountryCode { get; set; } = "";
        // Extra details (optional, but nice to show)
        public string StateOrProvince { get; set; } = "";
        public string PostalCode { get; set; } = "";
        public string ExceptionCode { get; set; } = "";
        public string ExceptionDescription { get; set; } = "";
        public string DerivedStatus { get; set; } = "";       // derivedStatus, e.g. "In transit"


    }
}
