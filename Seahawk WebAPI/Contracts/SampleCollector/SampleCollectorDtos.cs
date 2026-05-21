using Data;
using SeaHawkServices.Domain.Entities;
using static SeaHawkServices.Domain.Entities.Enums;

namespace Seahawk_WebAPI.Contracts.SampleCollector;

public sealed class SampleCollectionQueryRequest
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 15;

    public string? Search { get; set; }
    public string? FilterTracking { get; set; }
    public string? FilterVesselName { get; set; }
    public string? FilterIMO { get; set; }
    public string? FilterCompany { get; set; }
    public string? FilterStatus { get; set; }
    public DateTime? FilterFromDate { get; set; }
    public DateTime? FilterToDate { get; set; }
}

public sealed class SampleCollectionPagedResponse
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }

    public string? UserRole { get; set; }
    public string? UserEmail { get; set; }

    public SampleCollectionQueryRequest Filters { get; set; } = new();
    public List<SampleCollectionDto> Items { get; set; } = new();
}

public sealed class SampleCollectionDto
{
    public int Id { get; set; }

    public string? TrackingNumber { get; set; }
    public string? IMONumber { get; set; }
    public string? VesselName { get; set; }
    public string? CompanyName { get; set; }

    public string? RequestorName { get; set; }
    public string? RequestorEmail { get; set; }

    public int? NumberOfBoxes { get; set; }
    public SampleType? SampleType { get; set; }
    public string Country { get; set; }

    public DateTime? PickupDate { get; set; }
    public DateTime? CreatedOn { get; set; }

    public SampleCollectionStatus RequestStatus { get; set; }
    public string RequestStatusName { get; set; } = "";

    public CourierProvider CourierProvider { get; set; }
    public string? CourierServiceType { get; set; }

    public bool IsUserCancellationAllowed { get; set; }
    public bool IsApiDispatched { get; set; }

    public string? FedExLabelUrl { get; set; }
    public string? FedExInvoiceUrl { get; set; }
    public string? DhlLabelUrl { get; set; }
    public string? DhlInvoiceUrl { get; set; }
    public string? FedExErrorMessage { get; set; }

    public string? AdminCancellationReason { get; set; }
}


//New Classes : 

public sealed class PostalCodeCityLookupResponse
{
    public bool Success { get; set; }
    public string? Message { get; set; }

    public string? PostalCode { get; set; }
    public string? CountryCode { get; set; }

    public string? City { get; set; }
    public string? State { get; set; }
    public string? StateCode { get; set; }

    public List<string> Cities { get; set; } = new();
    public List<PostalCodeCityLocationDto> Locations { get; set; } = new();
}

public sealed class PostalCodeCityLocationDto
{
    public string City { get; set; } = "";
    public string State { get; set; } = "";
    public string StateCode { get; set; } = "";
}
//--------------------------------------------------

public sealed class SampleCollectionDefaultResponse
{
    public DateTime EarliestPickup { get; set; }
    public DateTime LatestPickup { get; set; }
    public string Timezone { get; set; } = "";
    public int NoOfParcels { get; set; }
    public bool Type { get; set; }
    public string? Country { get; set; }
}

public sealed class CityLookupResponse
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public List<string> Cities { get; set; } = new();
}

public sealed class StateLookupResponse
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public List<StateLookupDto> States { get; set; } = new();
}

public sealed class StateLookupDto
{
    public string Code { get; set; } = "";
    public string? Name { get; set; }
}

public sealed class PostalLocationResponse
{
    public bool Success { get; set; }
    public string? Message { get; set; }

    public string City { get; set; } = "";
    public string State { get; set; } = "";
    public string StateCode { get; set; } = "";

    public List<PostalLocationDto> Locations { get; set; } = new();
}

public sealed class PostalLocationDto
{
    public string City { get; set; } = "";
    public string State { get; set; } = "";
    public string StateCode { get; set; } = "";
}

public sealed class SampleCollectionConfirmationResponse
{
    public string CollectionReference { get; set; } = "";
    public string? TrackingNumber { get; set; }
    public string? LabelUrl { get; set; }
    public string? InvoiceUrl { get; set; }
    public string MsdsUrl { get; set; } = "/msds/Seahawk_MSDS.pdf";
}

public sealed class ApproveSampleCollectionRequest
{
    public string? CourierProvider { get; set; }
    public string? ServiceType { get; set; }
    public string? PickupType { get; set; }
    public string? PackagingType { get; set; }
    public string? ShippingPaymentType { get; set; }
    public string? LabelImageType { get; set; }
    public string? LabelStockType { get; set; }
}

public sealed class RejectSampleCollectionRequest
{
    public string? Reason { get; set; }
}

public sealed class CancelByAdminRequest
{
    public string? Reason { get; set; }
}

public sealed class ShipmentDocumentRequest
{
    public string Type { get; set; } = "";
    public string TrackingNumber { get; set; } = "";
    public string DocumentUrl { get; set; } = "";
    public string Courier { get; set; } = "";
}

public sealed class VesselNameLookupResponse
{
    public string? VesselName { get; set; }
}

public sealed class SampleCollectorMessageResponse
{
    public string Message { get; set; } = "";
}