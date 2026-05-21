using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static SeaHawkServices.Domain.Entities.Enums;

namespace SeaHawkServices.Web.ViewModels
{
    [NotMapped]
    public class SamplePickupRequestVM
    {
        // Common fields for both DHL & FedEx
        [Required]
        [Display(Name = "Courier")]
        public CourierProvider Courier { get; set; }  // FedEx / Dhl

        [Required]
        [Display(Name = "Planned Pickup Date & Time")]
        public DateTime? PlannedPickupDateTime { get; set; }

        [Required]
        [Display(Name = "Close Time (HH:mm)")]
        [RegularExpression(@"^\d{2}:\d{2}$", ErrorMessage = "Time must be in HH:mm format")]
        public string CloseTime { get; set; } = "18:00";

        [Required]
        public string Location { get; set; } = "reception";

        [Required]
        [Display(Name = "Location Type")]
        public string LocationType { get; set; } = "business"; // DHL: business / residence

        // Minimal shipper contact info (you can expand)
        [Required]
        [Display(Name = "Shipper Name")]
        public string ShipperName { get; set; }

        [Required]
        [Display(Name = "Shipper Phone")]
        public string ShipperPhone { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Shipper Email")]
        public string ShipperEmail { get; set; }

        [Required]
        [Display(Name = "Shipper Company")]
        public string ShipperCompany { get; set; }

        [Required]
        [Display(Name = "Address Line 1")]
        public string AddressLine1 { get; set; }

        public string AddressLine2 { get; set; }
        public string AddressLine3 { get; set; }

        [Required]
        [Display(Name = "City")]
        public string CityName { get; set; }

        [Required]
        [Display(Name = "Postal Code")]
        public string PostalCode { get; set; }

        [Required]
        [Display(Name = "Country Code")]
        public string CountryCode { get; set; } // e.g. "US", "GB"

        [Required]
        [Display(Name = "Account Number")]
        public string AccountNumber { get; set; }

        // simple shipment info (can be expanded)
        [Required]
        [Display(Name = "Number of Packages")]
        public int PackageCount { get; set; } = 1;

        [Required]
        [Display(Name = "Total Weight (kg)")]
        public decimal TotalWeight { get; set; } = 1m;

        [Display(Name = "Declared Value")]
        public decimal? DeclaredValue { get; set; }

        [Display(Name = "Declared Value Currency")]
        public string DeclaredValueCurrency { get; set; } = "USD";

        [Display(Name = "Remark")]
        public string Remark { get; set; }
    }

    [NotMapped]
    public class SamplePickupResultVM
    {
        public bool Success { get; set; }
        public string Courier { get; set; }
        public string ConfirmationCode { get; set; }
        public string Message { get; set; }
        public string Location { get; set; }
        public string RawResponse { get; set; }
        public string Error { get; set; }
    }

    // Extend your existing SampleTrackingVM
   
}
