using System;
using System.ComponentModel.DataAnnotations;
using static SeaHawkServices.Domain.Entities.Enums;

namespace SeaHawkServices.Domain.Entities
{
    public enum SampleCollectionStatus
    {
        Pending = 0,          // Submitted by user, waiting for Seahawk
        UnderReview = 1,      // Admin opened / under review
        Approved = 2,         // Admin approved + AWB created (FedEx / DHL)
        Dispatched = 3,       // Picked up / dispatched
        Completed = 4,        // Final completed
        Rejected = 5,         // Rejected by admin
        CancelledByUser = 6,  // User cancelled before processing
        CancelledByAdmin = 7, // Admin cancelled with reason
        Failed = 8            // API error etc.
    }
        

    public class SampleCollections
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "This Field is required")]
        public int? NumberOfBoxes { get; set; }

        [Required(ErrorMessage = "This Field is required")]
        public SampleType SampleType { get; set; }

        [Required(ErrorMessage = "This Field is required")]
        public Country Country { get; set; }

        [Required(ErrorMessage = "This Field is required")]
        public DateTime? PickupDate { get; set; }

        [Required(ErrorMessage = "This Field is required")]
        public string? VesselName { get; set; }

        [Required(ErrorMessage = "This Field is required")]
        public string? CompanyName { get; set; }

        [Required(ErrorMessage = "This Field is required")]
        public string? PersonToBeContacted { get; set; }

        [Required(ErrorMessage = "This Field is required")]
        public string? PersonToBeContactTelephoneNumber { get; set; }

        [Required(ErrorMessage = "This Field is required")]
        public string? RequestorName { get; set; }

        [Required(ErrorMessage = "This Field is required")]
        public string? RequestorEmail { get; set; }

        public string? EmailCC { get; set; }

        [Required(ErrorMessage = "This Field is required")]
        public string? PickupAddressLine1 { get; set; }

        public string? PickupAddressLine2 { get; set; }

        [Required(ErrorMessage = "This Field is required")]
        public string? PickupCity { get; set; }

        // --- SHIPPING / TRACKING FIELDS ---

        public string? IMONumber { get; set; }

        // Common tracking number (FedEx or DHL)
        public string? TrackingNumber { get; set; }

        // FedEx document URLs (as you already had)
        public string? FedExLabelUrl { get; set; }
        public string? FedExInvoiceUrl { get; set; }

        // DHL document URLs (saved from base64 PDFs)
        public string? DhlLabelUrl { get; set; }
        public string? DhlInvoiceUrl { get; set; }

        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

        public ApplicationUser ApplicationUser { get; set; }
        public string? ApplicationUserId { get; set; }

        // Stored request JSON used when admin approves (originally for FedEx; reused for DHL too)
        public string? FedExRequestJson { get; set; }

        // Generic error for now (you can later split into FedEx/DHL if needed)
        public string? FedExErrorMessage { get; set; }

        public SampleCollectionStatus RequestStatus { get; set; } = SampleCollectionStatus.Pending;

        // Which courier was chosen for this request
        public CourierProvider CourierProvider { get; set; }   // FedEx or DHL
        public string? CourierServiceType { get; set; }        // e.g. "PRIORITY_OVERNIGHT", "INTERNATIONAL_PRIORITY"

        // API dispatch flags
        public bool IsApiDispatched { get; set; } = false;
        public string? ApiDispatchReference { get; set; }

        // Cancellation metadata
        public string? AdminCancellationReason { get; set; }
        public bool IsUserCancellationAllowed { get; set; } = true;
    }
}

