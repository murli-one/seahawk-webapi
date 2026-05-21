using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeaHawkServices.Domain.Entities
{
    [NotMapped]
    public class AddressValidationRequestDto
    {
        // which address you want to validate (use Recipient for pickup/delivery location)
        public string? AddressLine1 { get; set; }
        public string? AddressLine2 { get; set; }
        public string? AddressLine3 { get; set; }
        public string? City { get; set; }
        public string? StateOrProvinceCode { get; set; }
        public string? PostalCode { get; set; }
        public string? CountryCode { get; set; } // ISO2 e.g. "US"

        // optional: if user can choose courier on create page
        public string? CourierProvider { get; set; } // "FedEx" | "Dhl"
    }
}
