using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static SeaHawkServices.Domain.Entities.Enums;

namespace SeaHawkServices.Domain.Entities
{
    [NotMapped]
    public class SampleCollectionRequestVM
    {
        // ========= Core pickup times used by courier APIs =========
        [Required] public DateTime? EarliestPickupDateTime { get; set; }
        [Required] public DateTime? LatestPickupDateTime { get; set; }

        public bool Type { get; set; }
        [Required] public string Timezone { get; set; } = "UTC";
        [Required] public string Port { get; set; } = string.Empty;

        // ========= Parcel =========
        [Required, Range(1, int.MaxValue)]
        public int NoOfParcels { get; set; } = 1;

        [Required, Range(0.01, double.MaxValue)]
        public decimal DeclaredValue { get; set; } = 100m;

        [Required, Range(0.01, double.MaxValue)]
        public decimal PackageWeightLb { get; set; } = 1m;

        [Required]
        public string CommodityDescription { get; set; } = "Sampling kit for fuel / oil condition monitoring";

        [Required] public string IMONumber { get; set; } = string.Empty;
        [Required] public string VesselName { get; set; } = string.Empty;
        [Required] public bool Fuel { get; set; }

        // ========= Sender (Shipper) =========
        [Required] public string SenderName { get; set; } = string.Empty;
        [Required] public string CompanyName { get; set; } = string.Empty;
        [Required] public string SenderPhone { get; set; } = string.Empty;
        [Required, EmailAddress] public string SenderEmail { get; set; } = string.Empty;

        // ========= Sender Address =========
        [Required] public string AddressLine1 { get; set; } = string.Empty;
        public string AddressLine2 { get; set; } = string.Empty;
        public string AddressLine3 { get; set; } = string.Empty;

        [Required] public string State { get; set; } = string.Empty;
        [Required] public string City { get; set; } = string.Empty;

        // IMPORTANT: keep as ISO2 for FedEx/DHL calls ("US", "GB", etc.)
        [Required] public string Country { get; set; } = "US";

        [Required] public string PostCode { get; set; } = string.Empty;

        // ========= Recipient (Lab) =========
        [Required] public string RecipientName { get; set; } = string.Empty;
        [Required] public string RecipientCompanyName { get; set; } = string.Empty;
        [Required] public string RecipientPhone { get; set; } = string.Empty;

        [Required] public string RecipientAddressLine1 { get; set; } = string.Empty;
        public string RecipientAddressLine2 { get; set; } = string.Empty;
        public string RecipientAddressLine3 { get; set; } = string.Empty;

        [Required] public string RecipientCity { get; set; } = string.Empty;
        [Required] public string RecipientState { get; set; } = string.Empty;
        [Required] public string RecipientPostalCode { get; set; } = string.Empty;

        // ISO2
        [Required] public string RecipientCountry { get; set; } = "CA";

        // ========= FedEx Required Inputs (Config/Admin-driven) =========

        // Account Number – FedEx shipping account number
        public string? FedExAccountNumberOverride { get; set; } // optional; if null use appsettings

        // Pickup Type
        public string? PickupType { get; set; } = "USE_SCHEDULED_PICKUP";

        // Service Type
        public string? ServiceType { get; set; } // e.g. PRIORITY_OVERNIGHT / INTERNATIONAL_PRIORITY

        // Packaging Type
        public string? PackagingType { get; set; } = "YOUR_PACKAGING";

        // Shipping Payment Type
        public string? ShippingPaymentType { get; set; } = "SENDER"; // SENDER/RECIPIENT/THIRD_PARTY

        // Payer Information (optional when payment type is SENDER)
        public string? PayerAccountNumber { get; set; } // required if THIRD_PARTY/RECIPIENT depending on FedEx rules

        // Label Specification
        public string? LabelImageType { get; set; } = "PDF";
        public string? LabelStockType { get; set; } = "PAPER_85X11_TOP_HALF_LABEL";

        // Optional fields you mentioned
        public string? ProcessingOptions { get; set; }
        public string? RequestType { get; set; }
    }
}
