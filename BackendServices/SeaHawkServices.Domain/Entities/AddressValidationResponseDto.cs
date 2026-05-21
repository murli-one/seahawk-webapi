using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeaHawkServices.Domain.Entities
{
    [NotMapped]
    public class AddressValidationResponseDto
    {
        public bool IsValid { get; set; }
        public string? Message { get; set; }

        // optional suggestion (if API returns a corrected address)
        public AddressValidationRequestDto? SuggestedAddress { get; set; }
    }
}
