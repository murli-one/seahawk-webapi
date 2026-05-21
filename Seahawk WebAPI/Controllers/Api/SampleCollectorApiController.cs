using Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Seahawk_WebAPI.Contracts.SampleCollector;
using SeaHawkServices.Application.Common.Interfaces;
using SeaHawkServices.Application.Services.Interface;
using SeaHawkServices.Domain.Entities;
using SeaHawkServices.Infrastructure.Data;
using SeaHawkServices.Web.ViewModels;
using System.Net;
using System.Net.Mail;
using static SeaHawkServices.Domain.Entities.Enums;

namespace Seahawk_WebAPI.Controllers.Api;

[ApiController]
[Route("api/sample-collector")]
[Authorize]
public class SampleCollectorApiController : ControllerBase
{
    private readonly IFedExShippingService _fedExShippingService;
    private readonly FedExSettings _fedExSettings;
    private readonly IDhlShippingService _dhlShippingService;
    private readonly ApplicationDbContext _db;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IApplicationUserService _applicationUserService;
    private readonly ISampleCollectionsService _sampleCollectionsService;
    private readonly IWebHostEnvironment _env;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ISmtpService _smtpService;

    public SampleCollectorApiController(
        IUnitOfWork unitOfWork,
        IOptions<FedExSettings> fedExOptions,
        IFedExShippingService fedExShippingService,
        ISmtpService smtpService,
        ApplicationDbContext db,
        IApplicationUserService applicationUserService,
        ISampleCollectionsService sampleCollectionsService,
        IDhlShippingService dhlShippingService,
        IWebHostEnvironment env,
        UserManager<ApplicationUser> userManager)
    {
        _fedExShippingService = fedExShippingService;
        _fedExSettings = fedExOptions.Value;
        _db = db;
        _applicationUserService = applicationUserService;
        _sampleCollectionsService = sampleCollectionsService;
        _dhlShippingService = dhlShippingService;
        _env = env;
        _userManager = userManager;
        _smtpService = smtpService;
        _unitOfWork = unitOfWork;
    }

    [HttpGet]
    public ActionResult<SampleCollectionDefaultResponse> GetDefault()
    {
        return Ok(new SampleCollectionDefaultResponse
        {
            EarliestPickup = DateTime.Now.AddHours(2),
            LatestPickup = DateTime.Now.AddHours(4),
            Timezone = "",
            NoOfParcels = 1,
            Type = false,
            Country = "United States"
        });
    }

    [HttpGet("cities")]
    [AllowAnonymous]
    public async Task<ActionResult<CityLookupResponse>> GetCitiesByCountry([FromQuery] string country)
    {
        if (string.IsNullOrWhiteSpace(country))
        {
            return BadRequest(new CityLookupResponse
            {
                Success = false,
                Message = "Country is required."
            });
        }

        country = FormatCountryName(country);

        try
        {
            using var httpClient = new HttpClient();

            var response = await httpClient.PostAsJsonAsync(
                "https://countriesnow.space/api/v0.1/countries/cities",
                new { country });

            if (!response.IsSuccessStatusCode)
            {
                return Ok(new CityLookupResponse
                {
                    Success = false,
                    Message = "Unable to fetch cities."
                });
            }

            var result = await response.Content.ReadFromJsonAsync<CountriesNowCityResponse>();

            if (result == null || result.Error || result.Data == null)
            {
                return Ok(new CityLookupResponse
                {
                    Success = false,
                    Message = result?.Msg ?? "No cities found."
                });
            }

            return Ok(new CityLookupResponse
            {
                Success = true,
                Cities = result.Data
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .Distinct()
                    .OrderBy(x => x)
                    .ToList()
            });
        }
        catch (Exception ex)
        {
            return Ok(new CityLookupResponse
            {
                Success = false,
                Message = ex.Message
            });
        }
    }

    [HttpGet("states")]
    [AllowAnonymous]
    public async Task<ActionResult<StateLookupResponse>> GetStatesByCountry([FromQuery] string country)
    {
        if (string.IsNullOrWhiteSpace(country))
        {
            return BadRequest(new StateLookupResponse
            {
                Success = false,
                Message = "Country is required."
            });
        }

        country = FormatCountryName(country);

        try
        {
            using var httpClient = new HttpClient();

            var response = await httpClient.PostAsJsonAsync(
                "https://countriesnow.space/api/v0.1/countries/states",
                new { country });

            if (!response.IsSuccessStatusCode)
            {
                return Ok(new StateLookupResponse
                {
                    Success = false,
                    Message = "Unable to fetch states."
                });
            }

            var result = await response.Content.ReadFromJsonAsync<CountriesNowStatesResponse>();

            if (result == null || result.Error || result.Data?.States == null)
            {
                return Ok(new StateLookupResponse
                {
                    Success = false,
                    Message = result?.Msg ?? "No states found."
                });
            }

            return Ok(new StateLookupResponse
            {
                Success = true,
                States = result.Data.States
                    .Where(x => !string.IsNullOrWhiteSpace(x.State_Code))
                    .Select(x => new StateLookupDto
                    {
                        Code = x.State_Code,
                        Name = x.Name
                    })
                    .DistinctBy(x => x.Code)
                    .OrderBy(x => x.Code)
                    .ToList()
            });
        }
        catch (Exception ex)
        {
            return Ok(new StateLookupResponse
            {
                Success = false,
                Message = ex.Message
            });
        }
    }

    [HttpGet("location-by-postal-code")]
    [AllowAnonymous]
    public async Task<ActionResult<PostalLocationResponse>> GetLocationByPostalCode(
        [FromQuery] string country,
        [FromQuery] string postalCode)
    {
        if (string.IsNullOrWhiteSpace(country) || string.IsNullOrWhiteSpace(postalCode))
        {
            return BadRequest(new PostalLocationResponse
            {
                Success = false,
                Message = "Country and Postal Code are required."
            });
        }

        try
        {
            string countryIso2 = ResolveCountryIso2ForPostalLookup(country);

            using var httpClient = new HttpClient();

            var url = $"https://api.zippopotam.us/{countryIso2.ToLower()}/{Uri.EscapeDataString(postalCode.Trim())}";
            var response = await httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                return Ok(new PostalLocationResponse
                {
                    Success = false,
                    Message = "No city/state found for this postal code."
                });
            }

            using var jsonDoc = await System.Text.Json.JsonDocument.ParseAsync(
                await response.Content.ReadAsStreamAsync());

            var root = jsonDoc.RootElement;

            if (!root.TryGetProperty("places", out var places) || places.GetArrayLength() == 0)
            {
                return Ok(new PostalLocationResponse
                {
                    Success = false,
                    Message = "No city/state found for this postal code."
                });
            }

            var locations = places
                .EnumerateArray()
                .Select(place => new PostalLocationDto
                {
                    City = place.TryGetProperty("place name", out var cityElement)
                        ? cityElement.GetString() ?? ""
                        : "",
                    State = place.TryGetProperty("state", out var stateElement)
                        ? stateElement.GetString() ?? ""
                        : "",
                    StateCode = place.TryGetProperty("state abbreviation", out var stateCodeElement)
                        ? stateCodeElement.GetString() ?? ""
                        : ""
                })
                .Where(x => !string.IsNullOrWhiteSpace(x.City))
                .DistinctBy(x => $"{x.City}|{x.State}|{x.StateCode}")
                .ToList();

            var first = locations.FirstOrDefault();

            return Ok(new PostalLocationResponse
            {
                Success = true,
                City = first?.City ?? "",
                State = first?.State ?? "",
                StateCode = first?.StateCode ?? "",
                Locations = locations
            });
        }
        catch (Exception ex)
        {
            return Ok(new PostalLocationResponse
            {
                Success = false,
                Message = ex.Message
            });
        }
    }


    // GET: /api/sample-collector/cities-by-postal-code?country=UnitedStates&postalCode=10001
    // GET: /api/sample-collector/cities-by-postal-code?country=US&postalCode=10001
    [HttpGet("cities-by-postal-code")]
    [AllowAnonymous]
    public async Task<ActionResult<PostalCodeCityLookupResponse>> GetCitiesByPostalCode(
        [FromQuery] string country,
        [FromQuery] string postalCode)
    {
        if (string.IsNullOrWhiteSpace(country))
        {
            return BadRequest(new PostalCodeCityLookupResponse
            {
                Success = false,
                Message = "Country is required."
            });
        }

        if (string.IsNullOrWhiteSpace(postalCode))
        {
            return BadRequest(new PostalCodeCityLookupResponse
            {
                Success = false,
                Message = "Postal Code is required."
            });
        }

        try
        {
            string countryIso2 = ResolveCountryIso2ForPostalLookup(country);

            using var httpClient = new HttpClient();

            var url = $"https://api.zippopotam.us/{countryIso2.ToLower()}/{Uri.EscapeDataString(postalCode.Trim())}";
            var response = await httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                return Ok(new PostalCodeCityLookupResponse
                {
                    Success = false,
                    Message = "No cities found for this postal code.",
                    PostalCode = postalCode.Trim(),
                    CountryCode = countryIso2,
                    Cities = new List<string>(),
                    Locations = new List<PostalCodeCityLocationDto>()
                });
            }

            using var jsonDoc = await System.Text.Json.JsonDocument.ParseAsync(
                await response.Content.ReadAsStreamAsync());

            var root = jsonDoc.RootElement;

            if (!root.TryGetProperty("places", out var places) || places.GetArrayLength() == 0)
            {
                return Ok(new PostalCodeCityLookupResponse
                {
                    Success = false,
                    Message = "No cities found for this postal code.",
                    PostalCode = postalCode.Trim(),
                    CountryCode = countryIso2,
                    Cities = new List<string>(),
                    Locations = new List<PostalCodeCityLocationDto>()
                });
            }

            var locations = places
                .EnumerateArray()
                .Select(place => new PostalCodeCityLocationDto
                {
                    City = place.TryGetProperty("place name", out var cityElement)
                        ? cityElement.GetString() ?? ""
                        : "",

                    State = place.TryGetProperty("state", out var stateElement)
                        ? stateElement.GetString() ?? ""
                        : "",

                    StateCode = place.TryGetProperty("state abbreviation", out var stateCodeElement)
                        ? stateCodeElement.GetString() ?? ""
                        : ""
                })
                .Where(x => !string.IsNullOrWhiteSpace(x.City))
                .DistinctBy(x => $"{x.City}|{x.State}|{x.StateCode}")
                .OrderBy(x => x.City)
                .ToList();

            var cities = locations
                .Select(x => x.City)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(x => x)
                .ToList();

            var firstLocation = locations.FirstOrDefault();

            return Ok(new PostalCodeCityLookupResponse
            {
                Success = true,
                Message = "Cities fetched successfully.",
                PostalCode = postalCode.Trim(),
                CountryCode = countryIso2,

                // Useful if frontend wants to auto-fill first matching value
                City = firstLocation?.City ?? "",
                State = firstLocation?.State ?? "",
                StateCode = firstLocation?.StateCode ?? "",

                // Main cities dropdown/list
                Cities = cities,

                // Full city + state mapping if needed
                Locations = locations
            });
        }
        catch (Exception ex)
        {
            return Ok(new PostalCodeCityLookupResponse
            {
                Success = false,
                Message = ex.Message,
                PostalCode = postalCode.Trim(),
                Cities = new List<string>(),
                Locations = new List<PostalCodeCityLocationDto>()
            });
        }
    }




    // POST: /api/sample-collector/validate-address
    [HttpPost("validate-address")]
    public async Task<ActionResult<AddressValidationResponseDto>> ValidateAddress(
        [FromBody] AddressValidationRequestDto dto,
        CancellationToken ct)
    {
        if (dto == null)
        {
            return BadRequest(new AddressValidationResponseDto
            {
                IsValid = false,
                Message = "Invalid request."
            });
        }

        if (string.IsNullOrWhiteSpace(dto.AddressLine1))
        {
            return BadRequest(new AddressValidationResponseDto
            {
                IsValid = false,
                Message = "Address Line 1 is required."
            });
        }

        if (string.IsNullOrWhiteSpace(dto.City))
        {
            return BadRequest(new AddressValidationResponseDto
            {
                IsValid = false,
                Message = "City is required."
            });
        }

        if (string.IsNullOrWhiteSpace(dto.PostalCode))
        {
            return BadRequest(new AddressValidationResponseDto
            {
                IsValid = false,
                Message = "Postal Code is required."
            });
        }

        if (string.IsNullOrWhiteSpace(dto.CountryCode))
        {
            return BadRequest(new AddressValidationResponseDto
            {
                IsValid = false,
                Message = "Country Code is required."
            });
        }

        var courierProvider = string.IsNullOrWhiteSpace(dto.CourierProvider)
            ? "FedEx"
            : dto.CourierProvider.Trim();

        try
        {
            AddressValidationResponseDto result;

            if (courierProvider.Equals("Dhl", StringComparison.OrdinalIgnoreCase) ||
                courierProvider.Equals("DHL", StringComparison.OrdinalIgnoreCase))
            {
                result = await _dhlShippingService.ValidateAddressAsync(dto, ct);
            }
            else if (courierProvider.Equals("FedEx", StringComparison.OrdinalIgnoreCase))
            {
                result = await _fedExShippingService.ValidateAddressAsync(dto, ct);
            }
            else
            {
                return BadRequest(new AddressValidationResponseDto
                {
                    IsValid = false,
                    Message = "Invalid courier provider. Allowed values are FedEx or Dhl."
                });
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            return Ok(new AddressValidationResponseDto
            {
                IsValid = false,
                Message = $"Address validation failed. Please verify the address. Error: {ex.Message}"
            });
        }
    }





    [HttpGet("orders")]
    public async Task<ActionResult<SampleCollectionPagedResponse>> GetSampleCollectionsOrderDetails(
        [FromQuery] SampleCollectionQueryRequest request)
    {
        request = NormalizeQuery(request);

        var currentUserName = User.Identity?.Name;
        if (string.IsNullOrWhiteSpace(currentUserName))
            return Unauthorized(new { message = "User not found." });

        var userDetails = await _applicationUserService.GetUserByUserNameAsync(currentUserName.ToLower());

        if (userDetails == null)
            return Unauthorized(new { message = "User details not found." });

        IQueryable<SampleCollections> query = _db.SampleCollections.AsNoTracking();

        if (userDetails.Role != Role.SystemAdmin)
        {
            query = query.Where(x => x.ApplicationUserId == userDetails.Id);
        }

        query = ApplyOrderFilters(query, request);

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderByDescending(o => o.CreatedOn)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync();

        return Ok(new SampleCollectionPagedResponse
        {
            PageNumber = request.Page,
            PageSize = request.PageSize,
            TotalCount = totalCount,
            TotalPages = request.PageSize <= 0
                ? 0
                : (int)Math.Ceiling((double)totalCount / request.PageSize),
            UserRole = userDetails.Role.ToString(),
            UserEmail = userDetails.Email,
            Filters = request,
            Items = items.Select(ToDto).ToList()
        });
    }

    [HttpGet("getAllCountries")]
    public IActionResult GetAllCountries() {

        var countries = Enum.GetNames<Country>();
        return Ok(countries);
    }

    [HttpPost("orders")]
    public async Task<ActionResult<SampleCollectorMessageResponse>> CreateSampleCollectionRequest(
        [FromBody] SampleCollectionViewModel vm,
        CancellationToken ct)
    {
        if (vm == null)
            return BadRequest(new { message = "Request body is required." });

        if (!IsPreferredDateAllowed(vm.PreferredDate, out var preferredDateMessage))
        {
            return BadRequest(new SampleCollectorMessageResponse
            {
                Message = preferredDateMessage
            });
        }

        static string Safe(object? value, string fallback)
        {
            var s = value?.ToString();
            return string.IsNullOrWhiteSpace(s) ? fallback : s.Trim();
        }

        var selectedCountry = vm.Country ?? Country.UnitedStates;
        var originIso = vm.Country.HasValue ? CountryIsoHelper.ToIso2(vm.Country.Value) : "US";
        var isUsOrigin = selectedCountry == Country.UnitedStates;

        var courierProvider = isUsOrigin ? CourierProvider.FedEx : CourierProvider.DHL;
        var providerName = isUsOrigin ? "FedEx" : "Dhl";
        var courierServiceType = isUsOrigin ? "PRIORITY_OVERNIGHT" : null;

        var pickupValidation = await (isUsOrigin
            ? _fedExShippingService.ValidateAddressAsync(new AddressValidationRequestDto
            {
                AddressLine1 = vm.Address1,
                AddressLine2 = vm.Address1,
                AddressLine3 = vm.Address1,
                City = vm.City,
                PostalCode = vm.PostCode,
                CountryCode = originIso,
                CourierProvider = providerName
            }, ct)
            : _dhlShippingService.ValidateAddressAsync(new AddressValidationRequestDto
            {
                AddressLine1 = vm.Address1,
                AddressLine2 = vm.Address1,
                AddressLine3 = vm.Address1,
                City = vm.City,
                PostalCode = vm.PostCode,
                CountryCode = originIso,
                CourierProvider = providerName
            }, ct));

        if (!pickupValidation.IsValid)
        {
            return BadRequest(new SampleCollectorMessageResponse
            {
                Message = pickupValidation.Message ?? "Pickup address is invalid."
            });
        }

        var preferredDate = vm.PreferredDate!.Value.Date;
        var preferredTime = vm.PreferredTime!.Value;
        var earliest = preferredDate.Add(preferredTime);
        var latest = earliest.AddHours(2);

        var userName = User.Identity?.Name ?? "UnknownUser";
        var userDetails = await _applicationUserService.GetUserByUserNameAsync(userName.ToLower());

        if (userDetails == null)
            return Unauthorized(new { message = "User details not found." });

        var labRecipient = GetSeahawkLabRecipient();

        var req = new SampleCollectionRequestVM
        {
            EarliestPickupDateTime = earliest,
            LatestPickupDateTime = latest,
            Timezone = "UTC",
            Port = Safe(vm.Port, "N/A"),

            NoOfParcels = vm.NoOfSamples <= 0 ? 1 : vm.NoOfSamples,
            Fuel = vm.Type,

            IMONumber = Safe(vm.IMONumber, "0"),
            VesselName = Safe(vm.VesselName, "Vessel"),

            DeclaredValue = vm.DeclaredValue ?? 100m,
            PackageWeightLb = vm.PackageWeightLb ?? 1m,
            CommodityDescription = Safe(vm.CommodityDescription, "Sampling kit for fuel / oil condition monitoring"),

            SenderName = Safe(vm.RequestorName, "Requestor"),
            SenderEmail = Safe(vm.RequestorEmail, "no-reply@seahawkservices.com"),
            SenderPhone = Safe(vm.TelephoneNo, "0000000000"),
            CompanyName = Safe(vm.CompanyName, "Company"),

            AddressLine1 = Safe(vm.Address1, "Address"),
            AddressLine2 = vm.Address2 ?? "",
            AddressLine3 = vm.Address3 ?? "",
            City = Safe(vm.City, "City"),
            State = Safe(vm.State, "State"),
            PostCode = Safe(vm.PostCode, "00000"),
            Country = originIso,

            RecipientName = labRecipient.RecipientName,
            RecipientCompanyName = labRecipient.RecipientCompanyName,
            RecipientPhone = labRecipient.RecipientPhone,
            RecipientAddressLine1 = labRecipient.RecipientAddressLine1,
            RecipientAddressLine2 = labRecipient.RecipientAddressLine2 ?? "",
            RecipientAddressLine3 = labRecipient.RecipientAddressLine3 ?? "",
            RecipientCity = labRecipient.RecipientCity,
            RecipientState = labRecipient.RecipientState,
            RecipientPostalCode = labRecipient.RecipientPostalCode,
            RecipientCountry = labRecipient.RecipientCountryIso2
        };

        var pickup = new SampleCollections
        {
            NumberOfBoxes = req.NoOfParcels,
            SampleType = req.Fuel ? SampleType.BunkerFuel : SampleType.TransformingInsulatingFuel,
            Country = selectedCountry,
            PickupDate = req.EarliestPickupDateTime,

            IMONumber = req.IMONumber,
            VesselName = req.VesselName,
            CompanyName = req.CompanyName,

            PersonToBeContacted = req.SenderName,
            PersonToBeContactTelephoneNumber = req.SenderPhone,

            RequestorName = req.SenderName,
            RequestorEmail = req.SenderEmail,

            EmailCC = BuildCcString(vm),

            PickupAddressLine1 = req.AddressLine1,
            PickupAddressLine2 = string.IsNullOrWhiteSpace(req.AddressLine2) ? null : req.AddressLine2,
            PickupCity = req.City,

            ApplicationUserId = userDetails.Id,

            RequestStatus = SampleCollectionStatus.Pending,
            IsUserCancellationAllowed = true,
            IsApiDispatched = false,
            ApiDispatchReference = null,

            CourierProvider = courierProvider,
            CourierServiceType = courierServiceType,

            FedExRequestJson = JsonConvert.SerializeObject(req)
        };

        _db.SampleCollections.Add(pickup);
        await _db.SaveChangesAsync(ct);

        var vesselDetails = await _unitOfWork.VesselDetail.GetAsync(x => x.VesselName == pickup.VesselName);

        await AddHistoryAsync(
            requestId: pickup.Id,
            requestType: "SampleCollection",
            action: RequestHistoryAction.Created,
            performedBy: userName,
            notes: "Sample dispatch request created by client.",
            newStatus: pickup.RequestStatus.ToString(),
            vesselDetailId: vesselDetails?.Id);

        return Ok(new SampleCollectorMessageResponse
        {
            Message = "Sample Dispatch Request submitted for Seahawk Admin approval."
        });
    }

    [HttpGet("confirmation")]
    [AllowAnonymous]
    public ActionResult<SampleCollectionConfirmationResponse> Confirmation(
        [FromQuery] string? reference,
        [FromQuery] string? trackingNumber,
        [FromQuery] string? labelUrl,
        [FromQuery] string? invoiceUrl)
    {
        return Ok(new SampleCollectionConfirmationResponse
        {
            CollectionReference = string.IsNullOrWhiteSpace(reference)
                ? $"AME{DateTime.UtcNow:yyMMddHHmmssfff}"
                : reference,
            TrackingNumber = trackingNumber,
            LabelUrl = labelUrl,
            InvoiceUrl = invoiceUrl,
            MsdsUrl = "/msds/Seahawk_MSDS.pdf"
        });
    }

    [HttpPost("orders/{id:int}/approve")]
    public async Task<ActionResult<SampleCollectorMessageResponse>> ApproveSampleCollection(
        int id,
        [FromBody] ApproveSampleCollectionRequest? request,
        CancellationToken ct)
    {
        var currentUser = await _userManager.GetUserAsync(User);

        if (currentUser == null || currentUser.Role != Role.SystemAdmin)
            return Forbid();

        var order = await _sampleCollectionsService.GetByIdAsync(id);

        if (order == null)
            return NotFound(new { message = "Sample collection request not found." });

        var oldStatus = order.RequestStatus;
        var oldTracking = order.TrackingNumber;

        var vesselDetails = await _unitOfWork.VesselDetail
            .GetAsync(x => x.VesselName.ToLower() == (order.VesselName ?? "").ToLower());

        if (string.IsNullOrWhiteSpace(order.FedExRequestJson))
        {
            order.RequestStatus = SampleCollectionStatus.Pending;
            order.FedExErrorMessage = "Missing shipment payload.";
            order.IsApiDispatched = false;
            order.ApiDispatchReference = null;

            await _sampleCollectionsService.UpdateAsync(order);

            await AddHistoryAsync(
                order.Id,
                "SampleCollection",
                RequestHistoryAction.ErrorOccured,
                currentUser.UserName,
                "Missing FedEx/DHL request JSON.",
                oldStatus.ToString(),
                order.RequestStatus.ToString(),
                vesselDetailId: vesselDetails?.Id);

            return BadRequest(new SampleCollectorMessageResponse
            {
                Message = BuildShipmentErrorMessage(order.FedExErrorMessage, "Shipment")
            });
        }

        var req = JsonConvert.DeserializeObject<SampleCollectionRequestVM>(order.FedExRequestJson);

        if (req == null)
        {
            order.RequestStatus = SampleCollectionStatus.Pending;
            order.FedExErrorMessage = "Shipment payload could not be deserialized.";
            order.IsApiDispatched = false;
            order.ApiDispatchReference = null;

            await _sampleCollectionsService.UpdateAsync(order);

            await AddHistoryAsync(
                order.Id,
                "SampleCollection",
                RequestHistoryAction.ErrorOccured,
                currentUser.UserName,
                "Shipment payload JSON invalid.",
                oldStatus.ToString(),
                order.RequestStatus.ToString(),
                vesselDetailId: vesselDetails?.Id);

            return BadRequest(new SampleCollectorMessageResponse
            {
                Message = order.FedExErrorMessage
            });
        }

        if (string.IsNullOrWhiteSpace(req.Country))
            req.Country = CountryIsoHelper.ToIso2(order.Country);

        if (!string.IsNullOrWhiteSpace(request?.CourierProvider) &&
            Enum.TryParse<CourierProvider>(request.CourierProvider, out var postedCourier))
        {
            order.CourierProvider = postedCourier;
        }

        var courier = order.CourierProvider;

        if (courier == CourierProvider.FedEx)
        {
            if (!string.IsNullOrWhiteSpace(request?.ServiceType)) req.ServiceType = request.ServiceType;
            if (!string.IsNullOrWhiteSpace(request?.PickupType)) req.PickupType = request.PickupType;
            if (!string.IsNullOrWhiteSpace(request?.PackagingType)) req.PackagingType = request.PackagingType;
            if (!string.IsNullOrWhiteSpace(request?.ShippingPaymentType)) req.ShippingPaymentType = request.ShippingPaymentType;
            if (!string.IsNullOrWhiteSpace(request?.LabelImageType)) req.LabelImageType = request.LabelImageType;
            if (!string.IsNullOrWhiteSpace(request?.LabelStockType)) req.LabelStockType = request.LabelStockType;

            req.FedExAccountNumberOverride ??= _fedExSettings.AccountNumber;

            var originCountry = (req.Country ?? "").Trim().ToUpperInvariant();
            var destinationCountry = (req.RecipientCountry ?? "").Trim().ToUpperInvariant();

            var isDomesticUs = originCountry == "US" && destinationCountry == "US";

            req.PickupType ??= "DROPOFF_AT_FEDEX_LOCATION";
            req.PackagingType ??= "YOUR_PACKAGING";
            req.ShippingPaymentType ??= "SENDER";
            req.LabelImageType ??= "PDF";
            req.LabelStockType ??= "PAPER_85X11_TOP_HALF_LABEL";
            req.ServiceType ??= isDomesticUs ? "PRIORITY_OVERNIGHT" : "INTERNATIONAL_PRIORITY";
            req.RequestType ??= "FUTURE_DAY_SHIPMENT";

            var fedExResult = await _fedExShippingService.CreateShipmentSampleOrderAsync(req, ct);

            if (!fedExResult.Success)
            {
                order.RequestStatus = SampleCollectionStatus.Pending;
                order.FedExErrorMessage = fedExResult.Error;
                order.IsApiDispatched = false;
                order.ApiDispatchReference = null;

                await _sampleCollectionsService.UpdateAsync(order);

                await AddHistoryAsync(
                    order.Id,
                    "SampleCollection",
                    RequestHistoryAction.ErrorOccured,
                    currentUser.UserName,
                    $"FedEx error: {fedExResult.Error}",
                    oldStatus.ToString(),
                    order.RequestStatus.ToString(),
                    vesselDetailId: vesselDetails?.Id);

                return BadRequest(new SampleCollectorMessageResponse
                {
                    Message = BuildShipmentErrorMessage(fedExResult.Error, "FedEx")
                });
            }

            order.TrackingNumber = fedExResult.TrackingNumber;

            var localFedExLabelPath = "";
            var localFedExInvoicePath = "";

            if (!string.IsNullOrWhiteSpace(fedExResult.LabelBase64))
            {
                localFedExLabelPath = SaveBase64Pdf(fedExResult.LabelBase64, "labels", $"fedex_label_{order.Id}");
            }
            else if (!string.IsNullOrWhiteSpace(fedExResult.LabelUrl))
            {
                try
                {
                    var labelBytes = await _fedExShippingService.DownloadDocumentAsync(fedExResult.LabelUrl, ct);
                    if (labelBytes.Length > 0)
                        localFedExLabelPath = SavePdfBytes(labelBytes, "labels", $"fedex_label_{order.Id}");
                }
                catch (Exception ex)
                {
                    order.FedExErrorMessage = $"FedEx label download fallback failed: {ex.Message}";
                }
            }

            if (!string.IsNullOrWhiteSpace(fedExResult.InvoiceBase64))
            {
                localFedExInvoicePath = SaveBase64Pdf(fedExResult.InvoiceBase64, "invoices", $"fedex_invoice_{order.Id}");
            }
            else if (!string.IsNullOrWhiteSpace(fedExResult.CommercialInvoiceUrl))
            {
                try
                {
                    var invoiceBytes = await _fedExShippingService.DownloadDocumentAsync(fedExResult.CommercialInvoiceUrl, ct);
                    if (invoiceBytes.Length > 0)
                        localFedExInvoicePath = SavePdfBytes(invoiceBytes, "invoices", $"fedex_invoice_{order.Id}");
                }
                catch (Exception ex)
                {
                    order.FedExErrorMessage = string.IsNullOrWhiteSpace(order.FedExErrorMessage)
                        ? $"FedEx invoice download fallback failed: {ex.Message}"
                        : $"{order.FedExErrorMessage} | Invoice: {ex.Message}";
                }
            }

            order.FedExLabelUrl = !string.IsNullOrWhiteSpace(localFedExLabelPath)
                ? localFedExLabelPath
                : fedExResult.LabelUrl;

            order.FedExInvoiceUrl = !string.IsNullOrWhiteSpace(localFedExInvoicePath)
                ? localFedExInvoicePath
                : fedExResult.CommercialInvoiceUrl;

            order.IsApiDispatched = true;
            order.ApiDispatchReference = fedExResult.ApiReference ?? fedExResult.TrackingNumber;
        }
        else if (courier == CourierProvider.DHL)
        {
            var dhlResult = await _dhlShippingService.CreateShipmentSampleOrderAsync(req, ct);

            if (!dhlResult.Success)
            {
                order.RequestStatus = SampleCollectionStatus.Pending;
                order.FedExErrorMessage = dhlResult.Error;
                order.IsApiDispatched = false;
                order.ApiDispatchReference = null;

                await _sampleCollectionsService.UpdateAsync(order);

                await AddHistoryAsync(
                    order.Id,
                    "SampleCollection",
                    RequestHistoryAction.ErrorOccured,
                    currentUser.UserName,
                    $"DHL error: {dhlResult.Error}",
                    oldStatus.ToString(),
                    order.RequestStatus.ToString(),
                    vesselDetailId: vesselDetails?.Id);

                return BadRequest(new SampleCollectorMessageResponse
                {
                    Message = BuildShipmentErrorMessage(dhlResult.Error, "DHL")
                });
            }

            var labelPath = SaveBase64Pdf(dhlResult.LabelBase64, "labels", $"samplecollection_label_{order.Id}");
            var invoicePath = SaveBase64Pdf(dhlResult.InvoiceBase64, "invoices", $"samplecollection_invoice_{order.Id}");

            order.TrackingNumber = dhlResult.TrackingNumber;
            order.DhlLabelUrl = labelPath;
            order.DhlInvoiceUrl = invoicePath;
            order.IsApiDispatched = true;
            order.ApiDispatchReference = !string.IsNullOrWhiteSpace(dhlResult.ApiReference)
                ? dhlResult.ApiReference
                : dhlResult.TrackingNumber;
        }
        else
        {
            return BadRequest(new SampleCollectorMessageResponse
            {
                Message = "Invalid courier provider selected."
            });
        }

        order.RequestStatus = SampleCollectionStatus.Approved;
        order.FedExErrorMessage = null;

        await _sampleCollectionsService.UpdateAsync(order);

        await SendSampleOrderApprovedEmailAsync(order, ct);

        await AddHistoryAsync(
            order.Id,
            "SampleCollection",
            RequestHistoryAction.APIDispatch,
            currentUser.UserName,
            $"Request approved and shipment created via {courier} – Tracking: {order.TrackingNumber}",
            trackingNumber: order.TrackingNumber,
            courier: courier.ToString(),
            apiDispatchRef: order.ApiDispatchReference,
            vesselDetailId: vesselDetails?.Id);

        if (oldStatus != order.RequestStatus)
        {
            await AddHistoryAsync(
                order.Id,
                "SampleCollection",
                RequestHistoryAction.StatusChanged,
                currentUser.UserName,
                "Status updated to Approved after API dispatch.",
                oldStatus.ToString(),
                order.RequestStatus.ToString(),
                vesselDetailId: vesselDetails?.Id);
        }

        if (!string.Equals(oldTracking, order.TrackingNumber, StringComparison.OrdinalIgnoreCase))
        {
            await AddHistoryAsync(
                order.Id,
                "SampleCollection",
                RequestHistoryAction.TrackingUpdated,
                currentUser.UserName,
                "Tracking number set after API dispatch.",
                trackingNumber: order.TrackingNumber,
                vesselDetailId: vesselDetails?.Id);
        }

        return Ok(new SampleCollectorMessageResponse
        {
            Message = $"Sample Collection request approved and shipment created via {courier}."
        });
    }

    [HttpGet("orders/{id:int}/edit")]
    public async Task<ActionResult<SampleCollectionAdminEditVM>> GetEdit(int id, CancellationToken ct)
    {
        var currentUser = await _userManager.GetUserAsync(User);

        if (currentUser == null || currentUser.Role != Role.SystemAdmin)
            return Forbid();

        var order = await _db.SampleCollections
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, ct);

        if (order == null)
            return NotFound(new { message = "Sample collection request not found." });

        return Ok(MapToAdminEditVm(order));
    }

    [HttpPut("orders/{id:int}")]
    public async Task<ActionResult<SampleCollectorMessageResponse>> UpdateSampleCollection(
        int id,
        [FromBody] SampleCollectionAdminEditVM vm,
        CancellationToken ct)
    {
        var currentUser = await _userManager.GetUserAsync(User);

        if (currentUser == null || currentUser.Role != Role.SystemAdmin)
            return Forbid();

        if (vm == null)
            return BadRequest(new { message = "Request body is required." });

        if (id != vm.Id)
            return BadRequest(new { message = "Route id and body id do not match." });

        if (!IsPreferredDateAllowed(vm.PreferredDate, out var preferredDateMessage))
        {
            return BadRequest(new SampleCollectorMessageResponse
            {
                Message = preferredDateMessage
            });
        }

        var order = await _db.SampleCollections.FirstOrDefaultAsync(x => x.Id == vm.Id, ct);

        if (order == null)
            return NotFound(new { message = "Sample collection request not found." });

        var oldStatus = order.RequestStatus;
        var oldTracking = order.TrackingNumber;

        var vesselDetails = await _unitOfWork.VesselDetail
            .GetAsync(x => x.VesselName.ToLower() == (order.VesselName ?? "").ToLower());

        order.EmailCC = BuildCcString(vm.AgentEmail, vm.CC1, vm.CC2, vm.CC3);

        if (vm.PreferredDate.HasValue && vm.PreferredTime.HasValue)
            order.PickupDate = vm.PreferredDate.Value.Date.Add(vm.PreferredTime.Value);

        order.IMONumber = vm.IMONumber;
        order.VesselName = vm.VesselName;
        order.NumberOfBoxes = vm.NumberOfBoxes;
        order.RequestorName = vm.RequestorName;
        order.RequestorEmail = vm.RequestorEmail;
        order.CompanyName = vm.CompanyName;
        order.PersonToBeContacted = vm.PersonToContact;
        order.PersonToBeContactTelephoneNumber = vm.ContactPhoneNumber;
        order.Country = vm.Country ?? order.Country;
        order.PickupAddressLine1 = vm.Address1;
        order.PickupAddressLine2 = vm.Address2;
        order.PickupCity = vm.City;
        order.CourierProvider = vm.CourierProvider;
        order.CourierServiceType = vm.CourierProvider == CourierProvider.FedEx
            ? "PRIORITY_OVERNIGHT"
            : vm.CourierProvider == CourierProvider.DHL
                ? "DHL Express"
                : vm.CourierServiceType;
        order.TrackingNumber = vm.TrackingNumber;
        order.RequestStatus = vm.RequestStatus;
        order.IsUserCancellationAllowed = vm.IsUserCancellationAllowed;
        order.AdminCancellationReason = vm.AdminCancellationReason;

        var existingReq = !string.IsNullOrWhiteSpace(order.FedExRequestJson)
            ? JsonConvert.DeserializeObject<SampleCollectionRequestVM>(order.FedExRequestJson)
            : new SampleCollectionRequestVM();

        existingReq ??= new SampleCollectionRequestVM();

        var labRecipient = GetSeahawkLabRecipient();

        existingReq.IMONumber = vm.IMONumber;
        existingReq.VesselName = vm.VesselName;
        existingReq.NoOfParcels = vm.NumberOfBoxes <= 0 ? 1 : vm.NumberOfBoxes;
        existingReq.SenderName = vm.RequestorName;
        existingReq.SenderEmail = vm.RequestorEmail;
        existingReq.SenderPhone = !string.IsNullOrWhiteSpace(vm.TelephoneNo)
            ? vm.TelephoneNo.Trim()
            : vm.ContactPhoneNumber;
        existingReq.CompanyName = vm.CompanyName;
        existingReq.AddressLine1 = vm.Address1;
        existingReq.AddressLine2 = vm.Address2 ?? "";
        existingReq.City = vm.City;
        existingReq.State = !string.IsNullOrWhiteSpace(vm.Town) ? vm.Town : vm.State;
        existingReq.PostCode = vm.PostCode;
        existingReq.Country = CountryIsoHelper.ToIso2(vm.Country ?? order.Country);

        if (vm.CourierProvider == CourierProvider.FedEx)
            existingReq.ServiceType = "PRIORITY_OVERNIGHT";
        else if (vm.CourierProvider == CourierProvider.DHL)
            existingReq.ServiceType = "DHL Express";

        existingReq.RecipientName = !string.IsNullOrWhiteSpace(vm.RecipientName)
            ? vm.RecipientName.Trim()
            : labRecipient.RecipientName;

        existingReq.RecipientCompanyName = !string.IsNullOrWhiteSpace(vm.RecipientCompanyName)
            ? vm.RecipientCompanyName.Trim()
            : labRecipient.RecipientCompanyName;

        existingReq.RecipientPhone = !string.IsNullOrWhiteSpace(vm.RecipientPhone)
            ? vm.RecipientPhone.Trim()
            : labRecipient.RecipientPhone;

        existingReq.RecipientAddressLine1 = !string.IsNullOrWhiteSpace(vm.RecipientAddressLine1)
            ? vm.RecipientAddressLine1.Trim()
            : labRecipient.RecipientAddressLine1;

        existingReq.RecipientAddressLine2 = vm.RecipientAddressLine2?.Trim() ?? "";
        existingReq.RecipientAddressLine3 = "";

        existingReq.RecipientCity = !string.IsNullOrWhiteSpace(vm.RecipientCity)
            ? vm.RecipientCity.Trim()
            : labRecipient.RecipientCity;

        existingReq.RecipientState = !string.IsNullOrWhiteSpace(vm.RecipientState)
            ? vm.RecipientState.Trim()
            : labRecipient.RecipientState;

        existingReq.RecipientPostalCode = !string.IsNullOrWhiteSpace(vm.RecipientPostalCode)
            ? vm.RecipientPostalCode.Trim()
            : labRecipient.RecipientPostalCode;

        existingReq.RecipientCountry = NormalizeCountryForShipping(
            vm.RecipientCountry,
            labRecipient.RecipientCountryIso2);

        if (vm.PackageWeightLb.HasValue && vm.PackageWeightLb.Value > 0)
            existingReq.PackageWeightLb = vm.PackageWeightLb.Value;

        if (vm.PreferredDate.HasValue && vm.PreferredTime.HasValue)
        {
            var earliest = vm.PreferredDate.Value.Date.Add(vm.PreferredTime.Value);
            existingReq.EarliestPickupDateTime = earliest;
            existingReq.LatestPickupDateTime = earliest.AddHours(2);
        }

        existingReq.PickupType ??= "DROPOFF_AT_FEDEX_LOCATION";
        existingReq.PackagingType ??= "YOUR_PACKAGING";
        existingReq.ShippingPaymentType ??= "SENDER";
        existingReq.LabelImageType ??= "PDF";
        existingReq.LabelStockType ??= "PAPER_85X11_TOP_HALF_LABEL";

        order.FedExRequestJson = JsonConvert.SerializeObject(existingReq);

        await _db.SaveChangesAsync(ct);

        if (oldStatus != order.RequestStatus)
        {
            await AddHistoryAsync(
                order.Id,
                "SampleCollection",
                RequestHistoryAction.StatusChanged,
                currentUser.UserName,
                "Admin updated request details.",
                oldStatus.ToString(),
                order.RequestStatus.ToString(),
                vesselDetailId: vesselDetails?.Id);
        }

        if (!string.Equals(oldTracking, order.TrackingNumber, StringComparison.OrdinalIgnoreCase))
        {
            await AddHistoryAsync(
                order.Id,
                "SampleCollection",
                RequestHistoryAction.TrackingUpdated,
                currentUser.UserName,
                "Tracking number updated by admin.",
                trackingNumber: order.TrackingNumber,
                vesselDetailId: vesselDetails?.Id);
        }

        return Ok(new SampleCollectorMessageResponse
        {
            Message = "Request updated successfully."
        });
    }

    [HttpPost("orders/{id:int}/reject")]
    public async Task<ActionResult<SampleCollectorMessageResponse>> RejectSampleCollection(
        int id,
        [FromBody] RejectSampleCollectionRequest? request)
    {
        var currentUser = await _userManager.GetUserAsync(User);

        if (currentUser == null || currentUser.Role != Role.SystemAdmin)
            return Forbid();

        var order = await _sampleCollectionsService.GetByIdAsync(id);

        if (order == null)
            return NotFound(new { message = "Sample collection request not found." });

        order.RequestStatus = SampleCollectionStatus.Rejected;
        order.AdminCancellationReason = request?.Reason;

        await _sampleCollectionsService.UpdateAsync(order);

        await AddHistoryAsync(
            order.Id,
            "SampleCollection",
            RequestHistoryAction.CanceledByAdmin,
            currentUser.UserName,
            request?.Reason);

        return Ok(new SampleCollectorMessageResponse
        {
            Message = "Sample Collection request rejected."
        });
    }

    [HttpPost("orders/{id:int}/cancel-by-user")]
    public async Task<ActionResult<SampleCollectorMessageResponse>> CancelByUser(int id)
    {
        var currentUser = await _userManager.GetUserAsync(User);

        if (currentUser == null)
            return Unauthorized(new { message = "User not found." });

        var order = await _sampleCollectionsService.GetByIdAsync(id);

        if (order == null)
            return NotFound(new { message = "Sample collection request not found." });

        if (order.ApplicationUserId != currentUser.Id)
            return Forbid();

        if (!order.IsUserCancellationAllowed ||
            (order.RequestStatus != SampleCollectionStatus.Pending &&
             order.RequestStatus != SampleCollectionStatus.UnderReview))
        {
            return BadRequest(new SampleCollectorMessageResponse
            {
                Message = "This request can no longer be cancelled."
            });
        }

        var oldStatus = order.RequestStatus;

        var vesselDetails = await _unitOfWork.VesselDetail
            .GetAsync(x => x.VesselName.ToLower() == (order.VesselName ?? "").ToLower());

        order.RequestStatus = SampleCollectionStatus.CancelledByUser;
        order.IsUserCancellationAllowed = false;

        await _sampleCollectionsService.UpdateAsync(order);

        await AddHistoryAsync(
            order.Id,
            "SampleCollection",
            RequestHistoryAction.CanceledByClient,
            currentUser.UserName,
            "Request cancelled by user before processing.",
            oldStatus.ToString(),
            order.RequestStatus.ToString(),
            vesselDetailId: vesselDetails?.Id);

        return Ok(new SampleCollectorMessageResponse
        {
            Message = "Request has been cancelled successfully."
        });
    }

    [HttpPost("orders/{id:int}/cancel-by-admin")]
    public async Task<ActionResult<SampleCollectorMessageResponse>> CancelByAdmin(
        int id,
        [FromBody] CancelByAdminRequest? request)
    {
        var currentUser = await _userManager.GetUserAsync(User);

        if (currentUser == null || currentUser.Role != Role.SystemAdmin)
            return Forbid();

        var order = await _sampleCollectionsService.GetByIdAsync(id);

        if (order == null)
            return NotFound(new { message = "Sample collection request not found." });

        var oldStatus = order.RequestStatus;

        var vesselDetails = await _unitOfWork.VesselDetail
            .GetAsync(x => x.VesselName.ToLower() == (order.VesselName ?? "").ToLower());

        order.RequestStatus = SampleCollectionStatus.CancelledByAdmin;
        order.AdminCancellationReason = request?.Reason;
        order.IsUserCancellationAllowed = false;

        await _sampleCollectionsService.UpdateAsync(order);

        await AddHistoryAsync(
            order.Id,
            "SampleCollection",
            RequestHistoryAction.CanceledByAdmin,
            currentUser.UserName,
            request?.Reason,
            oldStatus.ToString(),
            order.RequestStatus.ToString(),
            vesselDetailId: vesselDetails?.Id);

        return Ok(new SampleCollectorMessageResponse
        {
            Message = "Request has been cancelled by admin."
        });
    }

    [HttpGet("shipment-document")]
    [AllowAnonymous]
    public async Task<IActionResult> DownloadShipmentDocument(
        [FromQuery] string type,
        [FromQuery] string trackingNumber,
        [FromQuery] string documentUrl,
        [FromQuery] string courier,
        CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(documentUrl))
            return BadRequest(new { message = "Missing document URL." });

        byte[] bytes;

        if (documentUrl.StartsWith("/labels/", StringComparison.OrdinalIgnoreCase) ||
            documentUrl.StartsWith("/invoices/", StringComparison.OrdinalIgnoreCase) ||
            documentUrl.StartsWith("/uploads/", StringComparison.OrdinalIgnoreCase))
        {
            var relativePath = documentUrl.TrimStart('/')
                .Replace("/", Path.DirectorySeparatorChar.ToString());

            var physicalPath = Path.Combine(_env.WebRootPath, relativePath);

            if (!System.IO.File.Exists(physicalPath))
                return NotFound(new { message = "Document file not found." });

            bytes = await System.IO.File.ReadAllBytesAsync(physicalPath, ct);
        }
        else if (courier.Equals("DHL", StringComparison.OrdinalIgnoreCase))
        {
            var relativePath = documentUrl.TrimStart('/')
                .Replace("/", Path.DirectorySeparatorChar.ToString());

            var physicalPath = Path.Combine(_env.WebRootPath, relativePath);

            if (!System.IO.File.Exists(physicalPath))
                return NotFound(new { message = "DHL document file not found." });

            bytes = await System.IO.File.ReadAllBytesAsync(physicalPath, ct);
        }
        else
        {
            bytes = await _fedExShippingService.DownloadDocumentAsync(documentUrl, ct);
        }

        if (bytes == null || bytes.Length == 0)
            return NotFound(new { message = "Document not found." });

        var fileName = courier.ToUpperInvariant() switch
        {
            "DHL" when type == "Label" => $"DHLLabel_{trackingNumber}.pdf",
            "DHL" when type == "Invoice" => $"DHLInvoice_{trackingNumber}.pdf",
            "FEDEX" when type == "Label" => $"FedExLabel_{trackingNumber}.pdf",
            "FEDEX" when type == "Invoice" => $"FedExInvoice_{trackingNumber}.pdf",
            _ => $"{courier}_{trackingNumber}.pdf"
        };

        return File(bytes, "application/pdf", fileName);
    }

    [HttpGet("vessel-name-by-imo")]
    public async Task<ActionResult<VesselNameLookupResponse>> GetVesselNameByImo([FromQuery] string imo)
    {
        if (string.IsNullOrWhiteSpace(imo))
            return BadRequest(new { message = "IMO is required." });

        var trimmedImo = imo.Trim();

        var vessels = await _db.VesselDetail
            .AsNoTracking()
            .Where(v => v.IMONumber == trimmedImo)
            .ToListAsync();

        if (!vessels.Any())
            return NotFound(new { message = "Vessel not found." });

        var currentUser = await _userManager.GetUserAsync(User);

        if (currentUser != null)
        {
            if (currentUser.Role == Role.ManagementUser)
            {
                var companyUser = _unitOfWork.CompanyUser.GetCompanyUserByUserId(currentUser.Id);

                if (companyUser == null)
                {
                    return StatusCode(StatusCodes.Status403Forbidden, new
                    {
                        error = true,
                        code = "VesselNotInCompany",
                        message = "This vessel does not belong to your company."
                    });
                }

                var allowedVessel = vessels.FirstOrDefault(v => v.CompanyId == companyUser.CompanyId);

                if (allowedVessel == null)
                {
                    return StatusCode(StatusCodes.Status403Forbidden, new
                    {
                        error = true,
                        code = "VesselNotInCompany",
                        message = "This vessel does not belong to your company."
                    });
                }

                return Ok(new VesselNameLookupResponse
                {
                    VesselName = allowedVessel.VesselName
                });
            }

            if (currentUser.Role == Role.VesselUser)
            {
                var vesselIds = vessels.Select(v => v.Id).ToList();

                var assignedVesselId = await _db.VesselUser
                    .AsNoTracking()
                    .Where(x => x.UserId == currentUser.Id && vesselIds.Contains(x.VesselDetailId))
                    .Select(x => x.VesselDetailId)
                    .FirstOrDefaultAsync();

                if (assignedVesselId == 0)
                {
                    return StatusCode(StatusCodes.Status403Forbidden, new
                    {
                        error = true,
                        code = "VesselNotAssigned",
                        message = "This vessel is not assigned to this user."
                    });
                }

                var allowedVessel = vessels.FirstOrDefault(v => v.Id == assignedVesselId);

                return Ok(new VesselNameLookupResponse
                {
                    VesselName = allowedVessel?.VesselName
                });
            }
        }

        return Ok(new VesselNameLookupResponse
        {
            VesselName = vessels.FirstOrDefault()?.VesselName
        });
    }

    private static SampleCollectionQueryRequest NormalizeQuery(SampleCollectionQueryRequest request)
    {
        if (request.Page < 1)
            request.Page = 1;

        if (request.PageSize < 1)
            request.PageSize = 15;

        request.Search = request.Search?.Trim();
        request.FilterTracking = request.FilterTracking?.Trim();
        request.FilterVesselName = request.FilterVesselName?.Trim();
        request.FilterIMO = request.FilterIMO?.Trim();
        request.FilterCompany = request.FilterCompany?.Trim();
        request.FilterStatus = request.FilterStatus?.Trim();

        return request;
    }

    private static IQueryable<SampleCollections> ApplyOrderFilters(
        IQueryable<SampleCollections> query,
        SampleCollectionQueryRequest request)
    {
        if (!string.IsNullOrWhiteSpace(request.FilterTracking))
        {
            var term = request.FilterTracking.Trim();
            query = query.Where(o => o.TrackingNumber != null && o.TrackingNumber.Contains(term));
        }

        if (!string.IsNullOrWhiteSpace(request.FilterVesselName))
        {
            var term = request.FilterVesselName.Trim();
            query = query.Where(o => o.VesselName != null && o.VesselName.Contains(term));
        }

        if (!string.IsNullOrWhiteSpace(request.FilterIMO))
        {
            var term = request.FilterIMO.Trim();
            query = query.Where(o => o.IMONumber != null && o.IMONumber.Contains(term));
        }

        if (!string.IsNullOrWhiteSpace(request.FilterCompany))
        {
            var term = request.FilterCompany.Trim();
            query = query.Where(o => o.CompanyName != null && o.CompanyName.Contains(term));
        }

        if (!string.IsNullOrWhiteSpace(request.FilterStatus))
        {
            query = query.Where(o => o.RequestStatus.ToString() == request.FilterStatus);
        }

        if (request.FilterFromDate.HasValue)
            query = query.Where(o => o.CreatedOn >= request.FilterFromDate.Value);

        if (request.FilterToDate.HasValue)
        {
            var toExclusive = request.FilterToDate.Value.Date.AddDays(1);
            query = query.Where(o => o.CreatedOn < toExclusive);
        }

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var term = request.Search.Trim();

            query = query.Where(o =>
                (o.VesselName != null && o.VesselName.Contains(term)) ||
                (o.IMONumber != null && o.IMONumber.Contains(term)) ||
                (o.CompanyName != null && o.CompanyName.Contains(term)) ||
                (o.TrackingNumber != null && o.TrackingNumber.Contains(term)));
        }

        return query;
    }

    private static SampleCollectionDto ToDto(SampleCollections item)
    {
        return new SampleCollectionDto
        {
            Id = item.Id,
            TrackingNumber = item.TrackingNumber,
            IMONumber = item.IMONumber,
            VesselName = item.VesselName,
            CompanyName = item.CompanyName,
            RequestorName = item.RequestorName,
            RequestorEmail = item.RequestorEmail,
            NumberOfBoxes = item.NumberOfBoxes,
            SampleType = item.SampleType,

            // Return readable country name instead of enum number
            Country = FormatCountryDisplayName(item.Country),

            PickupDate = item.PickupDate,
            CreatedOn = item.CreatedOn,
            RequestStatus = item.RequestStatus,
            RequestStatusName = item.RequestStatus.ToString(),
            CourierProvider = item.CourierProvider,
            CourierServiceType = item.CourierServiceType,
            IsUserCancellationAllowed = item.IsUserCancellationAllowed,
            IsApiDispatched = item.IsApiDispatched,
            FedExLabelUrl = item.FedExLabelUrl,
            FedExInvoiceUrl = item.FedExInvoiceUrl,
            DhlLabelUrl = item.DhlLabelUrl,
            DhlInvoiceUrl = item.DhlInvoiceUrl,
            FedExErrorMessage = item.FedExErrorMessage,
            AdminCancellationReason = item.AdminCancellationReason
        };
    }
    private static string FormatCountryDisplayName(Country country)
    {
        return System.Text.RegularExpressions.Regex
            .Replace(country.ToString(), "([a-z])([A-Z])", "$1 $2")
            .Trim();
    }
    private SampleCollectionAdminEditVM MapToAdminEditVm(SampleCollections order)
    {
        string? agentEmail = null;
        string? cc1 = null;
        string? cc2 = null;
        string? cc3 = null;

        if (!string.IsNullOrWhiteSpace(order.EmailCC))
        {
            var parts = order.EmailCC
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
                .ToList();

            if (parts.Count > 0) agentEmail = parts[0];
            if (parts.Count > 1) cc1 = parts[1];
            if (parts.Count > 2) cc2 = parts[2];
            if (parts.Count > 3) cc3 = parts[3];
        }

        var pickupDate = order.PickupDate ?? DateTime.UtcNow;
        var preferredDate = pickupDate.Date;
        var preferredTime = pickupDate.TimeOfDay;

        var req = !string.IsNullOrWhiteSpace(order.FedExRequestJson)
            ? JsonConvert.DeserializeObject<SampleCollectionRequestVM>(order.FedExRequestJson)
            : null;

        var labRecipient = GetSeahawkLabRecipient();

        return new SampleCollectionAdminEditVM
        {
            Id = order.Id,
            IMONumber = order.IMONumber,
            VesselName = order.VesselName,
            NumberOfBoxes = order.NumberOfBoxes ?? 1,
            RequestorName = order.RequestorName,
            RequestorEmail = order.RequestorEmail,
            TelephoneNo = req?.SenderPhone ?? order.PersonToBeContactTelephoneNumber,
            AgentEmail = agentEmail,
            CC1 = cc1,
            CC2 = cc2,
            CC3 = cc3,
            Country = order.Country,
            CompanyName = order.CompanyName,
            PersonToContact = order.PersonToBeContacted,
            ContactPhoneNumber = order.PersonToBeContactTelephoneNumber,
            Address1 = order.PickupAddressLine1,
            Address2 = order.PickupAddressLine2,
            City = req?.City ?? order.PickupCity,
            Town = req?.State,
            State = req?.State,
            PostCode = req?.PostCode,
            PreferredDate = preferredDate,
            PreferredTime = preferredTime,
            RecipientName = req?.RecipientName ?? labRecipient.RecipientName,
            RecipientCompanyName = req?.RecipientCompanyName ?? labRecipient.RecipientCompanyName,
            RecipientPhone = req?.RecipientPhone ?? labRecipient.RecipientPhone,
            RecipientAddressLine1 = req?.RecipientAddressLine1 ?? labRecipient.RecipientAddressLine1,
            RecipientAddressLine2 = req?.RecipientAddressLine2 ?? labRecipient.RecipientAddressLine2,
            RecipientCity = req?.RecipientCity ?? labRecipient.RecipientCity,
            RecipientState = req?.RecipientState ?? labRecipient.RecipientState,
            RecipientPostalCode = req?.RecipientPostalCode ?? labRecipient.RecipientPostalCode,
            RecipientCountry = req?.RecipientCountry ?? labRecipient.RecipientCountryIso2,
            PackageWeightLb = req?.PackageWeightLb,
            CourierProvider = order.CourierProvider,
            CourierServiceType = order.CourierServiceType,
            TrackingNumber = order.TrackingNumber,
            RequestStatus = order.RequestStatus,
            IsUserCancellationAllowed = order.IsUserCancellationAllowed,
            AdminCancellationReason = order.AdminCancellationReason
        };
    }

    private string SavePdfBytes(byte[] bytes, string subFolder, string filePrefix)
    {
        if (bytes == null || bytes.Length == 0)
            return string.Empty;

        var folder = Path.Combine(_env.WebRootPath, subFolder);
        Directory.CreateDirectory(folder);

        var fileName = $"{filePrefix}_{DateTime.UtcNow:yyyyMMddHHmmss}.pdf";
        var fullPath = Path.Combine(folder, fileName);

        System.IO.File.WriteAllBytes(fullPath, bytes);

        return $"/{subFolder}/{fileName}";
    }

    private string SaveBase64Pdf(string? base64, string subFolder, string filePrefix)
    {
        if (string.IsNullOrWhiteSpace(base64))
            return string.Empty;

        var folder = Path.Combine(_env.WebRootPath, subFolder);
        Directory.CreateDirectory(folder);

        var fileName = $"{filePrefix}_{DateTime.UtcNow:yyyyMMddHHmmss}.pdf";
        var fullPath = Path.Combine(folder, fileName);

        var bytes = Convert.FromBase64String(base64);
        System.IO.File.WriteAllBytes(fullPath, bytes);

        return $"/{subFolder}/{fileName}";
    }

    private bool IsPreferredDateAllowed(DateTime? preferredDate, out string message)
    {
        message = "";

        if (!preferredDate.HasValue)
        {
            message = "Preferred Date is required.";
            return false;
        }

        var today = DateTime.Today;
        var maxDate = today.AddDays(10);
        var selectedDate = preferredDate.Value.Date;

        if (selectedDate < today)
        {
            message = "Preferred Date cannot be earlier than today.";
            return false;
        }

        if (selectedDate > maxDate)
        {
            message = "Preferred Date cannot be more than 10 days from today.";
            return false;
        }

        return true;
    }

    private string BuildShipmentErrorMessage(string? apiError, string courierName)
    {
        var message = $"Something went wrong while creating the {courierName} shipment.";

        if (string.IsNullOrWhiteSpace(apiError))
            return message;

        return $"{message} Please verify the address, postal code, city, state, country, pickup date, package weight, and courier details. Technical details: {apiError}";
    }

    private string? BuildCcString(SampleCollectionViewModel vm)
    {
        return BuildCcString(vm.AgentEmail, vm.CC1, vm.CC2, vm.CC3);
    }

    private string? BuildCcString(string? agentEmail, string? cc1, string? cc2, string? cc3)
    {
        var cc = new List<string>();

        if (!string.IsNullOrWhiteSpace(agentEmail)) cc.Add(agentEmail.Trim());
        if (!string.IsNullOrWhiteSpace(cc1)) cc.Add(cc1.Trim());
        if (!string.IsNullOrWhiteSpace(cc2)) cc.Add(cc2.Trim());
        if (!string.IsNullOrWhiteSpace(cc3)) cc.Add(cc3.Trim());

        return cc.Count == 0 ? null : string.Join(",", cc);
    }

    private string ResolveCountryIso2ForPostalLookup(string country)
    {
        if (string.IsNullOrWhiteSpace(country))
            return "US";

        country = country.Trim();

        if (int.TryParse(country, out int countryValue))
        {
            var enumCountry = (Country)countryValue;
            return CountryIsoHelper.ToIso2(enumCountry);
        }

        var normalizedInput = country
            .Replace(" ", "")
            .Replace("-", "")
            .Trim();

        foreach (Country item in Enum.GetValues(typeof(Country)))
        {
            var enumName = item.ToString()
                .Replace(" ", "")
                .Replace("-", "")
                .Trim();

            if (enumName.Equals(normalizedInput, StringComparison.OrdinalIgnoreCase))
                return CountryIsoHelper.ToIso2(item);
        }

        if (country.Equals("USA", StringComparison.OrdinalIgnoreCase) ||
            country.Equals("US", StringComparison.OrdinalIgnoreCase) ||
            country.Equals("United States", StringComparison.OrdinalIgnoreCase) ||
            country.Equals("UnitedStates", StringComparison.OrdinalIgnoreCase))
        {
            return "US";
        }

        return "US";
    }

    private string NormalizeCountryForShipping(string? country, string fallbackIso2 = "US")
    {
        if (string.IsNullOrWhiteSpace(country))
            return fallbackIso2;

        country = country.Trim();

        if (country.Length == 2)
            return country.ToUpperInvariant();

        if (int.TryParse(country, out int countryValue))
        {
            var enumCountry = (Country)countryValue;
            return CountryIsoHelper.ToIso2(enumCountry);
        }

        var normalizedInput = country
            .Replace(" ", "")
            .Replace("-", "")
            .Trim();

        foreach (Country item in Enum.GetValues(typeof(Country)))
        {
            var enumName = item.ToString()
                .Replace(" ", "")
                .Replace("-", "")
                .Trim();

            if (enumName.Equals(normalizedInput, StringComparison.OrdinalIgnoreCase))
                return CountryIsoHelper.ToIso2(item);
        }

        if (country.Equals("USA", StringComparison.OrdinalIgnoreCase) ||
            country.Equals("US", StringComparison.OrdinalIgnoreCase) ||
            country.Equals("United States", StringComparison.OrdinalIgnoreCase) ||
            country.Equals("UnitedStates", StringComparison.OrdinalIgnoreCase))
        {
            return "US";
        }

        return fallbackIso2;
    }

    private string FormatCountryName(string country)
    {
        if (string.IsNullOrWhiteSpace(country))
            return country;

        return System.Text.RegularExpressions.Regex
            .Replace(country, "([a-z])([A-Z])", "$1 $2")
            .Trim();
    }

    private (string RecipientName,
        string RecipientCompanyName,
        string RecipientPhone,
        string RecipientAddressLine1,
        string? RecipientAddressLine2,
        string? RecipientAddressLine3,
        string RecipientCity,
        string? RecipientState,
        string RecipientPostalCode,
        string RecipientCountryIso2)
        GetSeahawkLabRecipient()
    {
        return (
            RecipientName: "Seahawk Lab",
            RecipientCompanyName: "Seahawk Services",
            RecipientPhone: "18568454142",
            RecipientAddressLine1: "123 Street",
            RecipientAddressLine2: null,
            RecipientAddressLine3: null,
            RecipientCity: "New York",
            RecipientState: "NY",
            RecipientPostalCode: "10001",
            RecipientCountryIso2: "US"
        );
    }

    private async Task AddHistoryAsync(
        int requestId,
        string requestType,
        RequestHistoryAction action,
        string? performedBy,
        string? notes = null,
        string? oldStatus = null,
        string? newStatus = null,
        string? trackingNumber = null,
        string? courier = null,
        string? apiDispatchRef = null,
        int? vesselDetailId = null)
    {
        var history = new RequestHistory
        {
            RequestId = requestId,
            RequestType = requestType,
            Action = action,
            PerformedBy = performedBy,
            Timestamp = DateTime.UtcNow,
            Notes = notes,
            OldStatus = oldStatus,
            NewStatus = newStatus,
            TrackingNumber = trackingNumber,
            Courier = courier,
            ApiDispatchReference = apiDispatchRef,
            VesselDetailId = vesselDetailId
        };

        _db.RequestHistory.Add(history);
        await _db.SaveChangesAsync();
    }

    private async Task SendSampleOrderApprovedEmailAsync(SampleCollections order, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(order.RequestorEmail))
            return;

        var smtpConfig = await _smtpService.GetAsync();

        if (smtpConfig == null)
            throw new Exception("SMTP configuration not found.");

        using var message = new MailMessage();

        var courierName = order.CourierProvider.ToString();
        var trackingNumber = string.IsNullOrWhiteSpace(order.TrackingNumber)
            ? "N/A"
            : order.TrackingNumber;

        message.From = new MailAddress(smtpConfig.Email, "SeaHawk Services");
        message.To.Add(new MailAddress(order.RequestorEmail));
        message.Subject = $"Sample Collection Order Confirmed - {order.VesselName} ({order.IMONumber})";

        // Same professional template for both FedEx and DHL
        message.Body = $@"
<!DOCTYPE html>
<html>
<head>
<meta charset='UTF-8'>
<title>Sample Collection Order Confirmed</title>
</head>
<body style='margin:0; padding:0; background-color:#f4f7fb; font-family:Arial, Helvetica, sans-serif; color:#1f2937;'>
<table width='100%' cellpadding='0' cellspacing='0' border='0' style='background-color:#f4f7fb; padding:24px 0;'>
<tr>
<td align='center'>
<table width='680' cellpadding='0' cellspacing='0' border='0' style='background-color:#ffffff; border-radius:12px; overflow:hidden; border:1px solid #e5e7eb;'>
<tr>
<td style='background-color:#0f3d5e; padding:24px 30px; color:#ffffff;'>
<h2 style='margin:0; font-size:22px; font-weight:700;'>SeaHawk Services</h2>
<p style='margin:6px 0 0; font-size:14px; color:#dbeafe;'>
                                Sample Collection Order Confirmation
</p>
</td>
</tr>
 
                    <tr>
<td style='padding:30px;'>
<p style='margin:0 0 14px; font-size:15px;'>Dear {order.RequestorName ?? "Customer"},</p>
 
                            <p style='margin:0 0 16px; font-size:15px; line-height:1.6;'>
                                Thank you for using SeaHawk Services. Your sample collection request has been approved and the shipment has been created successfully.
</p>
 
                            <table width='100%' cellpadding='0' cellspacing='0' border='0' style='margin:22px 0; border-collapse:collapse; border:1px solid #e5e7eb;'>
<tr>
<td colspan='2' style='background-color:#eef6fb; padding:12px 16px; font-size:16px; font-weight:700; color:#0f3d5e; border-bottom:1px solid #e5e7eb;'>
                                        Order Details
</td>
</tr>
 
                                <tr>
<td style='padding:12px 16px; font-weight:700; width:38%; border-bottom:1px solid #e5e7eb;'>Order Number</td>
<td style='padding:12px 16px; border-bottom:1px solid #e5e7eb;'>{order.Id}</td>
</tr>
 
                                <tr>
<td style='padding:12px 16px; font-weight:700; border-bottom:1px solid #e5e7eb;'>Vessel Name</td>
<td style='padding:12px 16px; border-bottom:1px solid #e5e7eb;'>{order.VesselName ?? "N/A"}</td>
</tr>
 
                                <tr>
<td style='padding:12px 16px; font-weight:700; border-bottom:1px solid #e5e7eb;'>IMO Number</td>
<td style='padding:12px 16px; border-bottom:1px solid #e5e7eb;'>{order.IMONumber ?? "N/A"}</td>
</tr>
 
                                <tr>
<td style='padding:12px 16px; font-weight:700; border-bottom:1px solid #e5e7eb;'>Company Name</td>
<td style='padding:12px 16px; border-bottom:1px solid #e5e7eb;'>{order.CompanyName ?? "N/A"}</td>
</tr>
 
                                <tr>
<td style='padding:12px 16px; font-weight:700; border-bottom:1px solid #e5e7eb;'>Courier</td>
<td style='padding:12px 16px; border-bottom:1px solid #e5e7eb;'>{courierName}</td>
</tr>
 
                                <tr>
<td style='padding:12px 16px; font-weight:700; border-bottom:1px solid #e5e7eb;'>Tracking Number</td>
<td style='padding:12px 16px; border-bottom:1px solid #e5e7eb;'>{trackingNumber}</td>
</tr>
 
                                <tr>
<td style='padding:12px 16px; font-weight:700;'>Pickup Date</td>
<td style='padding:12px 16px;'>{order.PickupDate?.ToString("dd MMM yyyy HH:mm") ?? "N/A"}</td>
</tr>
</table>
 
                            <p style='margin:0 0 12px; font-size:15px; line-height:1.6;'>
                                The shipping documents are attached with this email. Please use the attached documents for shipment processing.
</p>
 
                            <p style='margin:0 0 6px; font-size:15px; line-height:1.6;'>
                                Regards,<br/>
<strong>SeaHawk Services Team</strong>
</p>
</td>
</tr>
 
                    <tr>
<td style='background-color:#f9fafb; padding:16px 30px; border-top:1px solid #e5e7eb;'>
<p style='margin:0; font-size:12px; color:#6b7280; line-height:1.5;'>
                                This is an automated email. Please do not reply directly to this message.
</p>
</td>
</tr>
 
                </table>
</td>
</tr>
</table>
</body>
</html>";

        message.IsBodyHtml = true;

        AddCcRecipients(message, order.EmailCC, order.RequestorEmail);

        var msdsPath = Path.Combine(_env.WebRootPath, "msds", "Seahawk_MSDS.pdf");

        // Keep existing document selection logic.
        // DHL will use DHL documents, FedEx will use FedEx documents.
        var labelSource = order.CourierProvider == CourierProvider.DHL
            ? order.DhlLabelUrl
            : order.FedExLabelUrl;

        var invoiceSource = order.CourierProvider == CourierProvider.DHL
            ? order.DhlInvoiceUrl
            : order.FedExInvoiceUrl;

        var attachments = new List<(string? Source, string FileName, bool IsPhysicalFile)>
    {
        (labelSource, $"ShippingDocuments_{trackingNumber}.pdf", false),
        (invoiceSource, $"AirWayBill_{trackingNumber}.pdf", false),
        (msdsPath, $"MSDS_{trackingNumber}.pdf", true)
    };

        foreach (var item in attachments)
        {
            var bytes = await ReadAttachmentBytesAsync(order, item.Source, item.IsPhysicalFile, ct);

            if (bytes == null || bytes.Length == 0)
                continue;

            var stream = new MemoryStream(bytes);
            message.Attachments.Add(new Attachment(stream, item.FileName, "application/pdf"));
        }

        using var client = new SmtpClient(smtpConfig.SmtpHost, smtpConfig.SmtpPort)
        {
            EnableSsl = smtpConfig.EnableSSL,
            Credentials = new NetworkCredential(smtpConfig.Email, smtpConfig.Password)
        };

        await client.SendMailAsync(message);
    }

    private async Task<byte[]?> ReadAttachmentBytesAsync(
        SampleCollections order,
        string? sourcePathOrUrl,
        bool forcePhysicalFile,
        CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(sourcePathOrUrl))
            return null;

        if (forcePhysicalFile)
        {
            if (!System.IO.File.Exists(sourcePathOrUrl))
                return null;

            return await System.IO.File.ReadAllBytesAsync(sourcePathOrUrl, ct);
        }

        if (sourcePathOrUrl.StartsWith("/labels/", StringComparison.OrdinalIgnoreCase) ||
            sourcePathOrUrl.StartsWith("/invoices/", StringComparison.OrdinalIgnoreCase) ||
            sourcePathOrUrl.StartsWith("/uploads/", StringComparison.OrdinalIgnoreCase))
        {
            var relativePath = sourcePathOrUrl.TrimStart('/')
                .Replace("/", Path.DirectorySeparatorChar.ToString());

            var physicalPath = Path.Combine(_env.WebRootPath, relativePath);

            if (!System.IO.File.Exists(physicalPath))
                return null;

            return await System.IO.File.ReadAllBytesAsync(physicalPath, ct);
        }

        if (order.CourierProvider == CourierProvider.DHL)
            return null;

        var bytes = await _fedExShippingService.DownloadDocumentAsync(sourcePathOrUrl, ct);
        return bytes != null && bytes.Length > 0 ? bytes : null;
    }

    private void AddCcRecipients(MailMessage message, string? emailCc, string? toAddress)
    {
        if (string.IsNullOrWhiteSpace(emailCc))
            return;

        var toEmail = (toAddress ?? "").Trim().ToLowerInvariant();

        var ccEmails = emailCc
            .Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries)
            .Select(x => x.Trim())
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        foreach (var email in ccEmails)
        {
            try
            {
                if (email.Trim().ToLowerInvariant() == toEmail)
                    continue;

                var alreadyExists = message.CC
                    .Cast<MailAddress>()
                    .Any(x => x.Address.Equals(email, StringComparison.OrdinalIgnoreCase));

                if (alreadyExists)
                    continue;

                message.CC.Add(new MailAddress(email));
            }
            catch
            {
                continue;
            }
        }
    }
}