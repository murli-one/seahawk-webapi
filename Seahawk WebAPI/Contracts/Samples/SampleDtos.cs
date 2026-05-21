using Data;
using static SeaHawkServices.Domain.Entities.Enums;

namespace Seahawk_WebAPI.Contracts.Samples;

public class SampleTrackingPageResponse
{
    public CourierProvider CourierType { get; set; } = CourierProvider.FedEx;

    public string UserEmail { get; set; } = "";
    public string UserRole { get; set; } = "";
    public bool IsAdmin { get; set; }

    public List<ShipmentRowDto> AllUsersShipments { get; set; } = new();
}

public class ShipmentRowDto
{
    public string Source { get; set; } = "";
    public string VesselName { get; set; } = "";
    public string TrackingNumber { get; set; } = "";
    public string Status { get; set; } = "";
    public string DetailsUrl { get; set; } = "";
    public CourierProvider CourierProvider { get; set; }
}

public class SampleTrackingRequest
{
    public CourierProvider CourierType { get; set; } = CourierProvider.FedEx;
    public string? AWBNumber { get; set; }
}

public class SampleTrackingResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = "";

    public string AWBNumber { get; set; } = "";
    public CourierProvider CourierType { get; set; }

    public string TrackingDetailsUrl { get; set; } = "";
    public TrackingResultDto? Result { get; set; }
}

public class TrackingResultDto
{
    public bool Success { get; set; }
    public string Waybill { get; set; } = "";
    public string? StatusText { get; set; }
    public string? Error { get; set; }

    public string ServiceDescription { get; set; } = "";
    public string ShipperCity { get; set; } = "";
    public string ShipperCountryCode { get; set; } = "";
    public string RecipientCity { get; set; } = "";
    public string RecipientCountryCode { get; set; } = "";

    public DateTimeOffset? ShipDate { get; set; }
    public DateTimeOffset? ActualPickupDate { get; set; }

    public List<TrackingEventDto> Events { get; set; } = new();
}

public class TrackingEventDto
{
    public DateTimeOffset When { get; set; }
    public string Code { get; set; } = "";
    public string Description { get; set; } = "";
    public string City { get; set; } = "";
    public string CountryCode { get; set; } = "";

    public string StateOrProvince { get; set; } = "";
    public string PostalCode { get; set; } = "";
    public string ExceptionCode { get; set; } = "";
    public string ExceptionDescription { get; set; } = "";
    public string DerivedStatus { get; set; } = "";
}

public class SampleCollectionTrackingDetailsDto
{
    public int Id { get; set; }

    public int? NumberOfBoxes { get; set; }
    public string? SampleType { get; set; }
    public string? Country { get; set; }
    public DateTime? PickupDate { get; set; }

    public string? VesselName { get; set; }
    public string? CompanyName { get; set; }
    public string? IMONumber { get; set; }

    public string? PersonToBeContacted { get; set; }
    public string? PersonToBeContactTelephoneNumber { get; set; }

    public string? RequestorName { get; set; }
    public string? RequestorEmail { get; set; }
    public string? EmailCC { get; set; }

    public string? PickupAddressLine1 { get; set; }
    public string? PickupAddressLine2 { get; set; }
    public string? PickupCity { get; set; }

    public string? TrackingNumber { get; set; }
    public string? FedExLabelUrl { get; set; }
    public string? FedExInvoiceUrl { get; set; }
    public string? DhlLabelUrl { get; set; }
    public string? DhlInvoiceUrl { get; set; }

    public DateTime CreatedOn { get; set; }

    public string? ApplicationUserId { get; set; }
    public string? FedExRequestJson { get; set; }
    public string? FedExErrorMessage { get; set; }

    public string? RequestStatus { get; set; }
    public string? CourierProvider { get; set; }
    public string? CourierServiceType { get; set; }

    public bool IsApiDispatched { get; set; }
    public string? ApiDispatchReference { get; set; }

    public string? AdminCancellationReason { get; set; }
    public bool IsUserCancellationAllowed { get; set; }
}