using Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Seahawk_WebAPI.Contracts.Samples;
using SeaHawkServices.Application.Services.Interface;
using SeaHawkServices.Domain.Entities;
using System.Text.RegularExpressions;
using static SeaHawkServices.Domain.Entities.Enums;

namespace Seahawk_WebAPI.Controllers.Api;

[ApiController]
[Route("api/sample")]
[Authorize]
public class SampleApiController : ControllerBase
{
    private readonly IShipmentTrackingService _tracking;
    private readonly IApplicationUserService _applicationUserService;
    private readonly ISampleCollectionsService _sampleCollectionsService;
    private readonly ISamplingKitService _samplingKitService;

    public SampleApiController(
        IApplicationUserService applicationUserService,
        IShipmentTrackingService tracking,
        ISampleCollectionsService sampleCollectionsService,
        ISamplingKitService samplingKitService)
    {
        _applicationUserService = applicationUserService;
        _tracking = tracking;
        _sampleCollectionsService = sampleCollectionsService;
        _samplingKitService = samplingKitService;
    }

    // GET: /api/sample/tracking
    [HttpGet("tracking")]
    public async Task<ActionResult<SampleTrackingPageResponse>> GetTrackingPage()
    {
        var userDetails = await GetCurrentUserAsync();

        if (userDetails == null)
            return Unauthorized(new { message = "User not found." });

        var response = new SampleTrackingPageResponse
        {
            CourierType = CourierProvider.FedEx,
            UserEmail = userDetails.Email ?? "",
            UserRole = userDetails.Role.ToString(),
            IsAdmin = IsAdmin(userDetails),
            AllUsersShipments = await GetUsersShipmentsAsync(userDetails.Id)
        };

        return Ok(response);
    }

    // GET: /api/sample/shipments
    [HttpGet("shipments")]
    public async Task<ActionResult<List<ShipmentRowDto>>> GetCurrentUserShipments()
    {
        var userDetails = await GetCurrentUserAsync();

        if (userDetails == null)
            return Unauthorized(new { message = "User not found." });

        var rows = await GetUsersShipmentsAsync(userDetails.Id);

        return Ok(rows);
    }

    // POST: /api/sample/tracking
    [HttpPost("tracking")]
    public async Task<ActionResult<SampleTrackingResponse>> TrackShipment(
        [FromBody] SampleTrackingRequest request,
        CancellationToken ct)
    {
        if (request == null)
            return BadRequest(new { message = "Request body is required." });

        var awbNumber = NormalizeAwb(request.AWBNumber);

        if (!IsValidAwb(awbNumber))
        {
            return BadRequest(new SampleTrackingResponse
            {
                Success = false,
                Message = "Please enter a valid AWB/tracking number. Only digits are allowed and length must be between 8 and 20 digits.",
                AWBNumber = awbNumber,
                CourierType = request.CourierType
            });
        }

        var result = await _tracking.TrackAsync(request.CourierType, awbNumber, ct);

        if (!IsShipmentFound(result))
        {
            return NotFound(new SampleTrackingResponse
            {
                Success = false,
                Message = "Shipment not found. Please check the AWB/tracking number and try again.",
                AWBNumber = awbNumber,
                CourierType = request.CourierType,
                Result = result == null ? null : ToTrackingResultDto(result)
            });
        }

        return Ok(new SampleTrackingResponse
        {
            Success = true,
            Message = "Shipment found.",
            AWBNumber = awbNumber,
            CourierType = request.CourierType,
            TrackingDetailsUrl = $"/api/sample/tracking-details?trackingNo={Uri.EscapeDataString(awbNumber)}&courierType={request.CourierType}",
            Result = ToTrackingResultDto(result)
        });
    }

    // GET: /api/sample/tracking-details?trackingNo=12345678&courierType=FedEx
    [HttpGet("tracking-details")]
    public async Task<ActionResult<SampleTrackingResponse>> GetTrackingDetails(
        [FromQuery] string trackingNo,
        [FromQuery] CourierProvider courierType,
        CancellationToken ct)
    {
        trackingNo = NormalizeAwb(trackingNo);

        if (!IsValidAwb(trackingNo))
        {
            return BadRequest(new SampleTrackingResponse
            {
                Success = false,
                Message = "Please enter a valid AWB/tracking number. Only digits are allowed and length must be between 8 and 20 digits.",
                AWBNumber = trackingNo,
                CourierType = courierType
            });
        }

        var result = await _tracking.TrackAsync(courierType, trackingNo, ct);

        if (!IsShipmentFound(result))
        {
            return NotFound(new SampleTrackingResponse
            {
                Success = false,
                Message = "Shipment not found. Please check the AWB/tracking number and try again.",
                AWBNumber = trackingNo,
                CourierType = courierType,
                Result = result == null ? null : ToTrackingResultDto(result)
            });
        }

        return Ok(new SampleTrackingResponse
        {
            Success = true,
            Message = "Shipment found.",
            AWBNumber = trackingNo,
            CourierType = courierType,
            TrackingDetailsUrl = $"/api/sample/tracking-details?trackingNo={Uri.EscapeDataString(trackingNo)}&courierType={courierType}",
            Result = ToTrackingResultDto(result)
        });
    }

    // GET: /api/sample/sample-collection-tracking-details/{id}
    // This replaces MVC SampleTrackingDetails(int id), admin-only.
    [HttpGet("sample-collection-tracking-details/{id:int}")]
    public async Task<ActionResult<SampleCollectionTrackingDetailsDto>> GetSampleCollectionTrackingDetails(int id)
    {
        var userDetails = await GetCurrentUserAsync();

        if (userDetails == null)
            return Unauthorized(new { message = "User not found." });

        if (userDetails.Role != Role.SystemAdmin)
            return Forbid();

        var order = await _sampleCollectionsService.GetByIdAsync(id);

        if (order == null)
            return NotFound(new { message = "Sample collection order not found." });

        return Ok(ToSampleCollectionTrackingDetailsDto(order));
    }

    private async Task<ApplicationUser?> GetCurrentUserAsync()
    {
        var userName = User.Identity?.Name;

        if (string.IsNullOrWhiteSpace(userName))
            return null;

        return await _applicationUserService.GetUserByUserNameAsync(userName.ToLower());
    }

    private async Task<List<ShipmentRowDto>> GetUsersShipmentsAsync(string userId)
    {
        var rows = new List<ShipmentRowDto>();

        var kitOrders = await _samplingKitService.GetAllAsync();

        if (kitOrders != null)
        {
            var kitRows = kitOrders
                .Where(x =>
                    !string.IsNullOrWhiteSpace(x.FedExTrackingNumber) &&
                    x.ApplicationUserId == userId &&
                    x.RequestStatus == SampleCollectionStatus.Approved)
                .Select(x => new ShipmentRowDto
                {
                    Source = "Sample Kit",
                    VesselName = x.VesselName ?? "-",
                    TrackingNumber = x.FedExTrackingNumber!,
                    Status = x.RequestStatus.ToString(),
                    DetailsUrl = $"/api/sample/tracking-details?trackingNo={Uri.EscapeDataString(x.FedExTrackingNumber!)}&courierType={x.CourierProvider}",
                    CourierProvider = x.CourierProvider
                });

            rows.AddRange(kitRows);
        }

        var collections = await _sampleCollectionsService.GetAllAsync();

        if (collections != null)
        {
            var collectionRows = collections
                .Where(x =>
                    !string.IsNullOrWhiteSpace(x.TrackingNumber) &&
                    x.ApplicationUserId == userId &&
                    x.RequestStatus == SampleCollectionStatus.Approved)
                .Select(x => new ShipmentRowDto
                {
                    Source = "Sample Collection",
                    VesselName = x.VesselName ?? "-",
                    TrackingNumber = x.TrackingNumber!,
                    Status = x.RequestStatus.ToString(),
                    DetailsUrl = $"/api/sample/tracking-details?trackingNo={Uri.EscapeDataString(x.TrackingNumber!)}&courierType={x.CourierProvider}",
                    CourierProvider = x.CourierProvider
                });

            rows.AddRange(collectionRows);
        }

        return rows
            .OrderByDescending(x => x.TrackingNumber)
            .ToList();
    }

    private static string NormalizeAwb(string? awb)
    {
        return Regex.Replace(awb ?? string.Empty, @"\s+", string.Empty).Trim();
    }

    private static bool IsValidAwb(string? awb)
    {
        return !string.IsNullOrWhiteSpace(awb) &&
               Regex.IsMatch(awb, @"^\d{8,20}$");
    }

    private static bool IsShipmentFound(TrackingResult? result)
    {
        if (result == null || !result.Success)
            return false;

        return (result.Events != null && result.Events.Any())
               || !string.IsNullOrWhiteSpace(result.StatusText)
               || !string.IsNullOrWhiteSpace(result.Waybill);
    }

    private static bool IsAdmin(ApplicationUser user)
    {
        return user.Role.ToString().Equals("SystemAdmin", StringComparison.OrdinalIgnoreCase)
               || user.Role.ToString().Equals("Admin", StringComparison.OrdinalIgnoreCase);
    }

    private static TrackingResultDto ToTrackingResultDto(TrackingResult result)
    {
        return new TrackingResultDto
        {
            Success = result.Success,
            Waybill = result.Waybill,
            StatusText = result.StatusText,
            Error = result.Error,

            ServiceDescription = result.ServiceDescription,
            ShipperCity = result.ShipperCity,
            ShipperCountryCode = result.ShipperCountryCode,
            RecipientCity = result.RecipientCity,
            RecipientCountryCode = result.RecipientCountryCode,

            ShipDate = result.ShipDate,
            ActualPickupDate = result.ActualPickupDate,

            Events = result.Events?.Select(e => new TrackingEventDto
            {
                When = e.When,
                Code = e.Code,
                Description = e.Description,
                City = e.City,
                CountryCode = e.CountryCode,
                StateOrProvince = e.StateOrProvince,
                PostalCode = e.PostalCode,
                ExceptionCode = e.ExceptionCode,
                ExceptionDescription = e.ExceptionDescription,
                DerivedStatus = e.DerivedStatus
            }).ToList() ?? new List<TrackingEventDto>()
        };
    }

    private static SampleCollectionTrackingDetailsDto ToSampleCollectionTrackingDetailsDto(SampleCollections order)
    {
        return new SampleCollectionTrackingDetailsDto
        {
            Id = order.Id,

            NumberOfBoxes = order.NumberOfBoxes,
            SampleType = order.SampleType.ToString(),
            Country = order.Country.ToString(),
            PickupDate = order.PickupDate,

            VesselName = order.VesselName,
            CompanyName = order.CompanyName,
            IMONumber = order.IMONumber,

            PersonToBeContacted = order.PersonToBeContacted,
            PersonToBeContactTelephoneNumber = order.PersonToBeContactTelephoneNumber,

            RequestorName = order.RequestorName,
            RequestorEmail = order.RequestorEmail,
            EmailCC = order.EmailCC,

            PickupAddressLine1 = order.PickupAddressLine1,
            PickupAddressLine2 = order.PickupAddressLine2,
            PickupCity = order.PickupCity,

            TrackingNumber = order.TrackingNumber,
            FedExLabelUrl = order.FedExLabelUrl,
            FedExInvoiceUrl = order.FedExInvoiceUrl,
            DhlLabelUrl = order.DhlLabelUrl,
            DhlInvoiceUrl = order.DhlInvoiceUrl,

            CreatedOn = order.CreatedOn,

            ApplicationUserId = order.ApplicationUserId,
            FedExRequestJson = order.FedExRequestJson,
            FedExErrorMessage = order.FedExErrorMessage,

            RequestStatus = order.RequestStatus.ToString(),
            CourierProvider = order.CourierProvider.ToString(),
            CourierServiceType = order.CourierServiceType,

            IsApiDispatched = order.IsApiDispatched,
            ApiDispatchReference = order.ApiDispatchReference,

            AdminCancellationReason = order.AdminCancellationReason,
            IsUserCancellationAllowed = order.IsUserCancellationAllowed
        };
    }
}