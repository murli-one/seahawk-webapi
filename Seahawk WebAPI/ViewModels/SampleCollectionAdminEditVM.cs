using SeaHawkServices.Domain.Entities;
using System.ComponentModel.DataAnnotations;
using static SeaHawkServices.Domain.Entities.Enums;

namespace SeaHawkServices.Web.ViewModels
{
    //public class SampleCollectionAdminEditVM
    //{
    //    public int Id { get; set; }

    //    // For context (read-only on the page)
    //    public string VesselName { get; set; }
    //    public string IMONumber { get; set; }
    //    public string CompanyName { get; set; }

    //    public int NumberOfBoxes { get; set; }
    //    public DateTime? PickupDate { get; set; }

    //    // Admin-editable logistics fields
    //    public CourierProvider CourierProvider { get; set; }
    //    public string CourierServiceType { get; set; }
    //    public string TrackingNumber { get; set; }
    //    public SampleCollectionStatus RequestStatus { get; set; }
    //    public bool IsUserCancellationAllowed { get; set; }
    //}

    public class SampleCollectionAdminEditVM
    {
        public int Id { get; set; }

        // ==== Sample Dispatch (top) ====
        public string? IMONumber { get; set; }

        [Required]
        public string? VesselName { get; set; }

        [Required]
        [Range(1, 999)]
        public int NumberOfBoxes { get; set; }

        // ==== Requestor Details ====
        [Required]
        public string? RequestorName { get; set; }

        [Required, EmailAddress]
        public string? RequestorEmail { get; set; }

        [Required]
        public string? TelephoneNo { get; set; }   // maps to PersonToBeContactTelephoneNumber in entity

        [EmailAddress]
        public string? AgentEmail { get; set; }    // stored inside EmailCC (optional)

        [EmailAddress]
        public string? CC1 { get; set; }

        [EmailAddress]
        public string? CC2 { get; set; }

        [EmailAddress]
        public string? CC3 { get; set; }

        // ==== Pickup Information ====
        [Required]
        public Country? Country { get; set; }

        [Required]
        public string? CompanyName { get; set; }

        [Required]
        public string? PersonToContact { get; set; } // maps to PersonToBeContacted

        [Required]
        public string? ContactPhoneNumber { get; set; } // maps to PersonToBeContactTelephoneNumber (if you use it as contact phone)

        [Required]
        public string? Address1 { get; set; } // PickupAddressLine1

        public string? Address2 { get; set; } // PickupAddressLine2

        [Required]
        public string? City { get; set; } // PickupCity

        public string? Town { get; set; }
        public string? State { get; set; }
        public string? PostCode { get; set; }

        [Required]
        public DateTime? PreferredDate { get; set; }

        [Required]
        public TimeSpan? PreferredTime { get; set; }

        // ==== Admin / Tracking ====
        public CourierProvider CourierProvider { get; set; }
        public string? CourierServiceType { get; set; }
        public string? TrackingNumber { get; set; }

        public SampleCollectionStatus RequestStatus { get; set; }
        public bool IsUserCancellationAllowed { get; set; }
        public string? AdminCancellationReason { get; set; }

        // ==== Recipient / Lab Shipment Details ====

        public string? RecipientName { get; set; }

        public string? RecipientCompanyName { get; set; }

        public string? RecipientPhone { get; set; }

        public string? RecipientAddressLine1 { get; set; }

        public string? RecipientAddressLine2 { get; set; }

        public string? RecipientCity { get; set; }

        public string? RecipientState { get; set; }

        public string? RecipientPostalCode { get; set; }

        public string? RecipientCountry { get; set; }

        public decimal? PackageWeightLb { get; set; }
    }
}