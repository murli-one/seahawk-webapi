using System.ComponentModel.DataAnnotations;
using static SeaHawkServices.Domain.Entities.Enums;

namespace SeaHawkServices.Web.ViewModels
{
    public class SampleCollectionViewModel
    {
        // ========= Vessel =========
        [Required]
        [Display(Name = "IMO Number")]
        public string IMONumber { get; set; } = string.Empty;

        [Display(Name = "Vessel Name")]
        public string? VesselName { get; set; }

        // ========= BV-style =========
        [Required]
        [Range(1, 999)]
        [Display(Name = "No. Of Samples")]
        public int NoOfSamples { get; set; } = 1;

        [Required]
        [Display(Name = "Requestor Name")]
        public string RequestorName { get; set; } = string.Empty;

        [Required, EmailAddress]
        [Display(Name = "Email")]
        public string RequestorEmail { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Telephone No.")]
        public string TelephoneNo { get; set; } = string.Empty;

        [EmailAddress]
        [Display(Name = "Agent's Email")]
        public string? AgentEmail { get; set; }

        [EmailAddress]
        [Display(Name = "CC1")]
        public string? CC1 { get; set; }

        [EmailAddress]
        [Display(Name = "CC2")]
        public string? CC2 { get; set; }

        [EmailAddress]
        [Display(Name = "CC3")]
        public string? CC3 { get; set; }

        // ========= Pickup Information =========
        [Required]
        [Display(Name = "Country Name")]
        public Country? Country { get; set; }

        [Required]
        [Display(Name = "Company Name")]
        public string CompanyName { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Person to Contact")]
        public string PersonToContact { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Contact Phone Number")]
        public string ContactPhoneNumber { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Pick Up Address")]
        public string Address1 { get; set; } = string.Empty;

        [Display(Name = "Address Line 2")]
        public string? Address2 { get; set; }

        [Display(Name = "Address Line 3")]
        public string? Address3 { get; set; }

        [Required]
        [Display(Name = "City")]
        public string City { get; set; } = string.Empty;

        [Display(Name = "Town")]
        public string? Town { get; set; }

        [Display(Name = "State")]
        public string? State { get; set; }

        [Display(Name = "Postal Code")]
        public string? PostCode { get; set; }

        [Required]
        [Display(Name = "Preferred Date")]
        public DateTime? PreferredDate { get; set; }

        public string? FedExError { get; set; }

        /// <summary>
        /// Use TimeSpan for `<input type="time">`
        /// </summary>
        [Required]
        [Display(Name = "Preferred Time")]
        public TimeSpan? PreferredTime { get; set; }

        // ========= Courier/Admin Defaults (NOT user input) =========
        // These are required by FedEx but should be set via config/admin during approval.

        [Display(Name = "Pickup Type (FedEx)")]
        public string? PickupType { get; set; } // ex: USE_SCHEDULED_PICKUP / CONTACT_FEDEX_TO_SCHEDULE

        [Display(Name = "Service Type (FedEx)")]
        public string? ServiceType { get; set; } // ex: FEDEX_INTERNATIONAL_PRIORITY

        [Display(Name = "Packaging Type (FedEx)")]
        public string? PackagingType { get; set; } // ex: YOUR_PACKAGING

        [Display(Name = "Shipping Payment Type")]
        public string? ShippingPaymentType { get; set; } // SENDER / RECIPIENT / THIRD_PARTY

        [Display(Name = "Label Stock Type")]
        public string? LabelStockType { get; set; } // ex: PAPER_4X6

        [Display(Name = "Image Type")]
        public string? LabelImageType { get; set; } // ex: PDF / PNG / ZPLII

        [Display(Name = "Request Type")]
        public string? RequestType { get; set; } // ex: FUTURE_DAY_SHIPMENT / SAME_DAY

        [Display(Name = "Processing Options")]
        public string? ProcessingOptions { get; set; } // optional enum

        public decimal? DeclaredValue { get; set; }
        public decimal? PackageWeightLb { get; set; }
        public bool Type { get; set; }
        public string? CommodityDescription { get; set; }
        public string? Port { get; set; }

        public DateTime? EarliestPickup { get; set; }
        public DateTime? LatestPickup { get; set; }
        public string? Timezone { get; set; }

        public int NoOfParcels { get; set; }
    }
}
