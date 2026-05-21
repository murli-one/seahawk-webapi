using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static SeaHawkServices.Domain.Entities.Enums;

namespace SeaHawkServices.Domain.Entities
{
     public class SamplingKit
    {
        /*int*/
        public int Id { get; set; }
        public int? FuelQualityTestKit { get; set; }
        public int? PowerPlantKit { get; set; }
        public int? OilConditionMonitoringKit { get; set; }
        public int? FiveLitresCubitainers { get; set; }
        public int? TenLitresCubitainers { get; set; }

        /*string*/

        [Required(ErrorMessage = "This Field is required")]
        public string? VesselName { get; set; }
        public string? IMONumber { get; set; }
        [Required(ErrorMessage = "This Field is required")]
        public string? RequestorName { get; set; }
        [Required(ErrorMessage = "This Field is required")]
        public string? RequestorEmail { get; set; }
        public string? EmailCC { get; set; }
        [Required(ErrorMessage = "This Field is required")]
        public string? VPSCustomerName { get; set; }
        public string? PONumber { get; set; }
        [Required(ErrorMessage = "This Field is required")]
        public string? Street { get; set; }
        [Required(ErrorMessage = "This Field is required")]
        public string? City { get; set; }
        public string? HouseNo { get; set; }
        public string? PostalCode { get; set; }
        public string? AdditionalAddressInfo { get; set; }
        [Required(ErrorMessage = "This Field is required")]
        public string? CompanyName { get; set; }
        public string? PersonToContact { get; set; }
        [Required(ErrorMessage = "This Field is required")]
        public string? PersonToContactTelNo { get; set; }
        public string? DeliveryEmail { get; set; }
        [Required(ErrorMessage = "This Field is required")]
        public string? BillingCompanyName { get; set; }
        [Required(ErrorMessage = "This Field is required")]
        public string? BillingAddressLine1 { get; set; }
        public string? BillingAddressLine2 { get; set; }
        public string? BillingAddressLine3 { get; set; }
        [Required(ErrorMessage = "This Field is required")]
        public string? BillingPostalCode { get; set; }

        /*DateTime*/
        [Required(ErrorMessage = "This Field is required")]
        public DateTime? DeliveryDeadline { get; set; }

        /*Enum*/
        [Required(ErrorMessage = "This Field is required")]
        public Country Country { get; set; }
        [Required(ErrorMessage = "This Field is required")]
        public Country BillingCountry { get; set; }

        /* FedEx – Shipper details for shipment */
        public string? State { get; set; }
        public int? NumberOfParcels { get; set; }
        public decimal? DeclaredValue { get; set; }
        public decimal? PackageWeightLb { get; set; }
        public string? CommodityDescription { get; set; }

        /* Recipient (ship-to) details */
        public string? RecipientName { get; set; }
        public string? RecipientCompanyName { get; set; }
        public string? RecipientPhone { get; set; }
        public string? RecipientAddressLine1 { get; set; }
        public string? RecipientAddressLine2 { get; set; }
        public string? RecipientAddressLine3 { get; set; }
        public string? RecipientCity { get; set; }
        public string? RecipientState { get; set; }
        public string? RecipientPostalCode { get; set; }
        public string? RecipientCountry { get; set; }

        /* Shipment result storage – used for BOTH FedEx & DHL */
        public string? FedExTrackingNumber { get; set; }   // will also hold DHL tracking when DHL is used
        public string? FedExLabelUrl { get; set; }        // label (PDF path) for whichever provider
        public string? FedExInvoiceUrl { get; set; }      // invoice (PDF path) for whichever provider

        public ApplicationUser ApplicationUser { get; set; }
        public string? ApplicationUserId { get; set; }

        public SampleCollectionStatus RequestStatus { get; set; } = SampleCollectionStatus.Pending;
        public string? FedExRequestJson { get; set; }     // generic shipping request JSON (used for FedEx/DHL)
        public string? FedExErrorMessage { get; set; }

        public bool IsApiDispatched { get; set; }          // is_api_dispatched
        public string? ApiDispatchReference { get; set; }  // api_dispatch_reference

        // NEW: which courier admin selected for this order
        public CourierProvider CourierProvider { get; set; } = CourierProvider.FedEx;
    }
}   
