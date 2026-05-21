using SeaHawkServices.Domain.Entities;
using static SeaHawkServices.Domain.Entities.Enums;

namespace Seahawk_WebAPI.Contracts.SamplingKits;

public class SamplingKitListRequest
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 15;

    public string? FilterTracking { get; set; }
    public string? FilterVesselName { get; set; }
    public string? FilterIMO { get; set; }
    public string? FilterStatus { get; set; }

    public DateTime? FilterFromDate { get; set; }
    public DateTime? FilterToDate { get; set; }
}

public class SamplingKitListResponse
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }

    public int TotalPages =>
        PageSize == 0 ? 0 : (int)Math.Ceiling((double)TotalCount / PageSize);

    public string UserEmail { get; set; } = "";
    public string UserRole { get; set; } = "";

    public List<SamplingKitDto> Items { get; set; } = new();
}

public class SamplingKitDto
{
    public int Id { get; set; }

    public int? FuelQualityTestKit { get; set; }
    public int? PowerPlantKit { get; set; }
    public int? OilConditionMonitoringKit { get; set; }
    public int? FiveLitresCubitainers { get; set; }
    public int? TenLitresCubitainers { get; set; }

    public string? VesselName { get; set; }
    public string? IMONumber { get; set; }

    public string? RequestorName { get; set; }
    public string? RequestorEmail { get; set; }
    public string? EmailCC { get; set; }

    public string? VPSCustomerName { get; set; }
    public string? PONumber { get; set; }

    public string? Street { get; set; }
    public string? City { get; set; }
    public string? HouseNo { get; set; }
    public string? PostalCode { get; set; }
    public string? AdditionalAddressInfo { get; set; }

    public string? CompanyName { get; set; }
    public string? PersonToContact { get; set; }
    public string? PersonToContactTelNo { get; set; }
    public string? DeliveryEmail { get; set; }

    public string? BillingCompanyName { get; set; }
    public string? BillingAddressLine1 { get; set; }
    public string? BillingAddressLine2 { get; set; }
    public string? BillingAddressLine3 { get; set; }
    public string? BillingPostalCode { get; set; }

    public DateTime? DeliveryDeadline { get; set; }

    public Country Country { get; set; }
    public Country BillingCountry { get; set; }

    public string? State { get; set; }

    public int? NumberOfParcels { get; set; }
    public decimal? DeclaredValue { get; set; }
    public decimal? PackageWeightLb { get; set; }
    public string? CommodityDescription { get; set; }

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

    public string? ApplicationUserId { get; set; }

    public SampleCollectionStatus RequestStatus { get; set; }
    public string? RequestStatusText { get; set; }

    public CourierProvider CourierProvider { get; set; }
    public string? CourierProviderText { get; set; }
    public string? FedExTrackingNumber { get; set; }
    public string? FedExLabelUrl { get; set; }
    public string? FedExInvoiceUrl { get; set; }

    public string? FedExRequestJson { get; set; }
    public string? FedExErrorMessage { get; set; }

    public bool IsApiDispatched { get; set; }
    public string? ApiDispatchReference { get; set; }

}

public class SamplingKitCreateUpdateRequest
{
    public int? FuelQualityTestKit { get; set; }
    public int? PowerPlantKit { get; set; }
    public int? OilConditionMonitoringKit { get; set; }
    public int? FiveLitresCubitainers { get; set; }
    public int? TenLitresCubitainers { get; set; }

    public string? VesselName { get; set; }
    public string? IMONumber { get; set; }

    public string? RequestorName { get; set; }
    public string? RequestorEmail { get; set; }
    public string? EmailCC { get; set; }

    public string? VPSCustomerName { get; set; }
    public string? PONumber { get; set; }

    public string? Street { get; set; }
    public string? City { get; set; }
    public string? HouseNo { get; set; }
    public string? PostalCode { get; set; }
    public string? AdditionalAddressInfo { get; set; }

    public string? CompanyName { get; set; }
    public string? PersonToContact { get; set; }
    public string? PersonToContactTelNo { get; set; }
    public string? DeliveryEmail { get; set; }

    public string? BillingCompanyName { get; set; }
    public string? BillingAddressLine1 { get; set; }
    public string? BillingAddressLine2 { get; set; }
    public string? BillingAddressLine3 { get; set; }
    public string? BillingPostalCode { get; set; }

    public DateTime? DeliveryDeadline { get; set; }

    public Country Country { get; set; }
    public Country BillingCountry { get; set; }

    public string? State { get; set; }

    public int? NumberOfParcels { get; set; }
    public decimal? DeclaredValue { get; set; }
    public decimal? PackageWeightLb { get; set; }
    public string? CommodityDescription { get; set; }

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

    public CourierProvider CourierProvider { get; set; } = CourierProvider.FedEx;
}

public class SamplingKitAdminEditRequest
{
    public int? NumberOfParcels { get; set; }
    public decimal? DeclaredValue { get; set; }
    public decimal? PackageWeightLb { get; set; }
    public string? CommodityDescription { get; set; }

    public CourierProvider CourierProvider { get; set; }
    public string? TrackingNumber { get; set; }
    public SampleCollectionStatus RequestStatus { get; set; }
}

public class SamplingKitRejectRequest
{
    public string? Reason { get; set; }
}

public class SamplingKitCancelRequest
{
    public string? Reason { get; set; }
}

public class SamplingKitCommentRequest
{
    public string? Comment { get; set; }
}

public class SamplingKitApproveRequest
{
    public CourierProvider? CourierProvider { get; set; }
}

public class SamplingKitConfirmationResponse
{
    public string CollectionReference { get; set; } = "";
    public string? TrackingNumber { get; set; }
    public string? LabelUrl { get; set; }
    public string? InvoiceUrl { get; set; }
    public string MsdsUrl { get; set; } = "/msds/Seahawk_MSDS.pdf";
}

public class LookupListResponse<T>
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public List<T> Items { get; set; } = new();
}

public class StateLookupDto
{
    public string Code { get; set; } = "";
    public string Name { get; set; } = "";
}

public class PostalLocationDto
{
    public string City { get; set; } = "";
    public string State { get; set; } = "";
    public string StateCode { get; set; } = "";
}

public class PostalLocationResponse
{
    public bool Success { get; set; }
    public string? Message { get; set; }

    public string City { get; set; } = "";
    public string State { get; set; } = "";
    public string StateCode { get; set; } = "";

    public List<PostalLocationDto> Locations { get; set; } = new();
}

public class VesselNameLookupResponse
{
    public string VesselName { get; set; } = "";
}

public class ApiMessageResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = "";
}