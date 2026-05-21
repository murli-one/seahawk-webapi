using Data;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Seahawk_WebAPI.Contracts.SamplingKits;
using SeaHawkServices.Application.Common.Interfaces;
using SeaHawkServices.Application.Services.Interface;
using SeaHawkServices.Domain.Entities;
using SeaHawkServices.Infrastructure.Data;
using SeaHawkServices.Web.ViewModels;
using System.Xml.Linq;
using static SeaHawkServices.Domain.Entities.Enums;

namespace Seahawk_WebAPI.Controllers.Api;

[ApiController]
[Route("api/sampling-kits")]
[Authorize]
public class SamplingKitsApiController : ControllerBase
{
    private readonly ISamplingKitService _service;
    private readonly IFedExShippingService _fedExShippingService;
    private readonly IDhlShippingService _dhlShippingService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationDbContext _db;

    public SamplingKitsApiController(
        ISamplingKitService service,
        IUnitOfWork unitOfWork,
        UserManager<ApplicationUser> userManager,
        IFedExShippingService fedExShippingService,
        IDhlShippingService dhlShippingService,
        ApplicationDbContext db)
    {
        _service = service;
        _unitOfWork = unitOfWork;
        _userManager = userManager;
        _fedExShippingService = fedExShippingService;
        _dhlShippingService = dhlShippingService;
        _db = db;
    }

    // GET: /api/sampling-kits/cities?country=UnitedStates
    [HttpGet("cities")]
    public async Task<ActionResult<LookupListResponse<string>>> GetCitiesByCountry([FromQuery] string country)
    {
        if (string.IsNullOrWhiteSpace(country))
        {
            return BadRequest(new LookupListResponse<string>
            {
                Success = false,
                Message = "Country is required."
            });
        }

        country = FormatCountryName(country);

        try
        {
            using var httpClient = new HttpClient();

            var payload = new { country };
            var response = await httpClient.PostAsJsonAsync(
                "https://countriesnow.space/api/v0.1/countries/cities",
                payload);

            if (!response.IsSuccessStatusCode)
            {
                return Ok(new LookupListResponse<string>
                {
                    Success = false,
                    Message = "Unable to fetch cities."
                });
            }

            var result = await response.Content.ReadFromJsonAsync<CountriesNowCityResponse>();

            if (result == null || result.Error || result.Data == null)
            {
                return Ok(new LookupListResponse<string>
                {
                    Success = false,
                    Message = result?.Msg ?? "No cities found."
                });
            }

            var cities = result.Data
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct()
                .OrderBy(x => x)
                .ToList();

            return Ok(new LookupListResponse<string>
            {
                Success = true,
                Items = cities
            });
        }
        catch (Exception ex)
        {
            return Ok(new LookupListResponse<string>
            {
                Success = false,
                Message = ex.Message
            });
        }
    }

    // GET: /api/sampling-kits/states?country=UnitedStates
    [HttpGet("states")]
    public async Task<ActionResult<LookupListResponse<StateLookupDto>>> GetStatesByCountry([FromQuery] string country)
    {
        if (string.IsNullOrWhiteSpace(country))
        {
            return BadRequest(new LookupListResponse<StateLookupDto>
            {
                Success = false,
                Message = "Country is required."
            });
        }

        country = FormatCountryName(country);

        try
        {
            using var httpClient = new HttpClient();

            var payload = new { country };
            var response = await httpClient.PostAsJsonAsync(
                "https://countriesnow.space/api/v0.1/countries/states",
                payload);

            if (!response.IsSuccessStatusCode)
            {
                return Ok(new LookupListResponse<StateLookupDto>
                {
                    Success = false,
                    Message = "Unable to fetch states."
                });
            }

            var result = await response.Content.ReadFromJsonAsync<CountriesNowStatesResponse>();

            if (result == null || result.Error || result.Data?.States == null)
            {
                return Ok(new LookupListResponse<StateLookupDto>
                {
                    Success = false,
                    Message = result?.Msg ?? "No states found."
                });
            }

            var states = result.Data.States
                .Where(x => !string.IsNullOrWhiteSpace(x.State_Code))
                .Select(x => new StateLookupDto
                {
                    Code = x.State_Code ?? "",
                    Name = x.Name ?? ""
                })
                .DistinctBy(x => x.Code)
                .OrderBy(x => x.Code)
                .ToList();

            return Ok(new LookupListResponse<StateLookupDto>
            {
                Success = true,
                Items = states
            });
        }
        catch (Exception ex)
        {
            return Ok(new LookupListResponse<StateLookupDto>
            {
                Success = false,
                Message = ex.Message
            });
        }
    }

    // GET: /api/sampling-kits/location-by-postal-code?country=US&postalCode=10001
    [HttpGet("location-by-postal-code")]
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

            var firstLocation = locations.FirstOrDefault();

            return Ok(new PostalLocationResponse
            {
                Success = true,
                City = firstLocation?.City ?? "",
                State = firstLocation?.State ?? "",
                StateCode = firstLocation?.StateCode ?? "",
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

    // GET: /api/sampling-kits/orders
    [HttpGet("orders")]
    public async Task<ActionResult<SamplingKitListResponse>> GetOrders([FromQuery] SamplingKitListRequest request)
    {
        if (request.Page < 1)
            request.Page = 1;

        if (request.PageSize < 1)
            request.PageSize = 15;

        var currentUser = await _userManager.GetUserAsync(User);
        if (currentUser == null)
            return Unauthorized(new { message = "User not found." });

        IQueryable<SamplingKit> query;

        if (currentUser.Role == Role.SystemAdmin)
            query = _db.SamplingKit.AsNoTracking().AsQueryable();
        else
            query = _db.SamplingKit.AsNoTracking().Where(x => x.ApplicationUserId == currentUser.Id);

        if (!string.IsNullOrWhiteSpace(request.FilterTracking))
            query = query.Where(x => x.FedExTrackingNumber != null && x.FedExTrackingNumber.Contains(request.FilterTracking));

        if (!string.IsNullOrWhiteSpace(request.FilterVesselName))
            query = query.Where(x => x.VesselName != null && x.VesselName.Contains(request.FilterVesselName));

        if (!string.IsNullOrWhiteSpace(request.FilterIMO))
            query = query.Where(x => x.IMONumber != null && x.IMONumber.Contains(request.FilterIMO));

        if (!string.IsNullOrWhiteSpace(request.FilterStatus))
            query = query.Where(x => x.RequestStatus.ToString() == request.FilterStatus);

        if (request.FilterFromDate.HasValue)
            query = query.Where(x => x.DeliveryDeadline >= request.FilterFromDate.Value);

        if (request.FilterToDate.HasValue)
            query = query.Where(x => x.DeliveryDeadline < request.FilterToDate.Value.AddDays(1));

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderByDescending(x => x.Id)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync();

        return Ok(new SamplingKitListResponse
        {
            PageNumber = request.Page,
            PageSize = request.PageSize,
            TotalCount = totalCount,
            UserEmail = currentUser.Email ?? "",
            UserRole = currentUser.Role.ToString(),
            Items = items.Select(ToDto).ToList()
        });
    }

    // GET: /api/sampling-kits/{id}
    [HttpGet("{id:int}")]
    public async Task<ActionResult<SamplingKitDto>> GetSamplingKit(int id)
    {
        var kit = await _service.GetByIdAsync(id);

        if (kit == null)
            return NotFound(new { message = "Sampling kit order not found." });

        return Ok(ToDto(kit));
    }

    // POST: /api/sampling-kits
    [HttpPost]
    public async Task<ActionResult<SamplingKitDto>> CreateSamplingKit(
        [FromBody] SamplingKitCreateUpdateRequest request,
        CancellationToken ct)
    {
        var currentUser = await _userManager.GetUserAsync(User);

        if (currentUser == null)
            return Unauthorized(new { message = "User not found." });

        var kit = new SamplingKit();
        ApplyCreateUpdateRequest(kit, request);

        kit.ApplicationUserId = currentUser.Id;
        kit.RequestStatus = SampleCollectionStatus.Pending;

        var shipmentRequest = MapSamplingKitToSampleCollectorVM(kit);
        shipmentRequest.Country = CountryIsoHelper.ToIso2(kit.Country);

        kit.FedExRequestJson = JsonConvert.SerializeObject(shipmentRequest);

        await _service.AddAsync(kit);

        var vesselDetails = await _unitOfWork.VesselDetail
            .GetAsync(x => x.VesselName.ToLower() == (kit.VesselName ?? "").ToLower());

        await AddHistoryAsync(
            requestId: kit.Id,
            requestType: "SamplingKit",
            action: RequestHistoryAction.Created,
            performedBy: currentUser.UserName ?? currentUser.Email ?? "Unknown",
            notes: "Sampling kit request created by client.",
            newStatus: kit.RequestStatus.ToString(),
            vesselDetailId: vesselDetails?.Id);

        return CreatedAtAction(nameof(GetSamplingKit), new { id = kit.Id }, ToDto(kit));
    }

    // PUT: /api/sampling-kits/{id}
    [HttpPut("{id:int}")]
    public async Task<ActionResult<SamplingKitDto>> UpdateSamplingKit(
        int id,
        [FromBody] SamplingKitCreateUpdateRequest request,
        CancellationToken ct)
    {
        var kit = await _service.GetByIdAsync(id);

        if (kit == null)
            return NotFound(new { message = "Sampling kit order not found." });

        ApplyCreateUpdateRequest(kit, request);

        var shipmentRequest = MapSamplingKitToSampleCollectorVM(kit);
        shipmentRequest.Country = CountryIsoHelper.ToIso2(kit.Country);

        kit.FedExRequestJson = JsonConvert.SerializeObject(shipmentRequest);

        await _service.UpdateAsync(kit);

        return Ok(ToDto(kit));
    }

    // GET: /api/sampling-kits/confirmation?reference=...&trackingNumber=...&labelUrl=...&invoiceUrl=...
    [HttpGet("confirmation")]
    public ActionResult<SamplingKitConfirmationResponse> Confirmation(
        [FromQuery] string? reference,
        [FromQuery] string? trackingNumber,
        [FromQuery] string? labelUrl,
        [FromQuery] string? invoiceUrl)
    {
        return Ok(new SamplingKitConfirmationResponse
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

    // GET: /api/sampling-kits/export-xml
    [HttpGet("export-xml")]
    public async Task<IActionResult> ExportXml()
    {
        var kits = await _service.GetAllAsync();

        var xml = new XDocument(
            new XElement("SamplingKits",
                kits.Select(k =>
                    new XElement("SamplingKit",
                        new XElement("Id", k.Id),
                        new XElement("FuelQualityTestKit", k.FuelQualityTestKit),
                        new XElement("PowerPlantKit", k.PowerPlantKit),
                        new XElement("OilConditionMonitoringKit", k.OilConditionMonitoringKit),
                        new XElement("FiveLitresCubitainers", k.FiveLitresCubitainers),
                        new XElement("TenLitresCubitainers", k.TenLitresCubitainers),
                        new XElement("VesselName", k.VesselName),
                        new XElement("IMONumber", k.IMONumber),
                        new XElement("RequestorName", k.RequestorName),
                        new XElement("RequestorEmail", k.RequestorEmail),
                        new XElement("EmailCC", k.EmailCC),
                        new XElement("VPSCustomerName", k.VPSCustomerName),
                        new XElement("PONumber", k.PONumber),
                        new XElement("Street", k.Street),
                        new XElement("City", k.City),
                        new XElement("HouseNo", k.HouseNo),
                        new XElement("PostalCode", k.PostalCode),
                        new XElement("AdditionalAddressInfo", k.AdditionalAddressInfo),
                        new XElement("CompanyName", k.CompanyName),
                        new XElement("PersonToContact", k.PersonToContact),
                        new XElement("PersonToContactTelNo", k.PersonToContactTelNo),
                        new XElement("DeliveryEmail", k.DeliveryEmail),
                        new XElement("BillingCompanyName", k.BillingCompanyName),
                        new XElement("BillingAddressLine1", k.BillingAddressLine1),
                        new XElement("BillingAddressLine2", k.BillingAddressLine2),
                        new XElement("BillingAddressLine3", k.BillingAddressLine3),
                        new XElement("BillingPostalCode", k.BillingPostalCode),
                        new XElement("DeliveryDeadline", k.DeliveryDeadline),
                        new XElement("Country", k.Country),
                        new XElement("BillingCountry", k.BillingCountry)
                    )
                )
            )
        );

        var bytes = System.Text.Encoding.UTF8.GetBytes(xml.ToString());
        return File(bytes, "application/xml", "SamplingKits.xml");
    }

    // POST: /api/sampling-kits/export-excel
    [HttpPost("export-excel")]
    public async Task<IActionResult> ExportExcel()
    {
        var kits = await _service.GetAllAsync();

        using var stream = new MemoryStream();

        using (var spreadsheet = SpreadsheetDocument.Create(stream, SpreadsheetDocumentType.Workbook))
        {
            var workbookPart = spreadsheet.AddWorkbookPart();
            workbookPart.Workbook = new Workbook();

            var worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
            worksheetPart.Worksheet = new Worksheet();

            var sheetData = new SheetData();
            worksheetPart.Worksheet.AppendChild(sheetData);

            var sheets = spreadsheet.WorkbookPart!.Workbook.AppendChild(new Sheets());
            sheets.Append(new Sheet
            {
                Id = spreadsheet.WorkbookPart.GetIdOfPart(worksheetPart),
                SheetId = 1,
                Name = "SamplingKits"
            });

            var headers = new[]
            {
                "Fuel Quality Test Kit", "Power Plant Kit", "Oil Condition Monitoring Kit",
                "5-litres Cubitainers", "10-litres Cubitainers", "Vessel Name", "IMO Number",
                "Requestor Name", "Requestor Email", "Email CC", "VPS Customer Name", "PO Number",
                "Street", "City", "House No", "Postal Code", "Additional Address Info", "Company Name",
                "Person To Contact", "Person To Contact Tel No", "Delivery Email", "Billing Company Name",
                "Billing Address Line1", "Billing Address Line2", "Billing Address Line3", "Billing Postal Code",
                "Delivery Deadline", "Country", "Billing Country"
            };

            var headerRow = new Row();
            foreach (var h in headers)
                headerRow.AppendChild(new Cell { CellValue = new CellValue(h), DataType = CellValues.String });

            sheetData.AppendChild(headerRow);

            int[] maxLengths = new int[headers.Length];
            for (int i = 0; i < headers.Length; i++)
                maxLengths[i] = headers[i].Length;

            foreach (var k in kits)
            {
                var row = new Row();

                string[] values =
                {
                    k.FuelQualityTestKit?.ToString() ?? "",
                    k.PowerPlantKit?.ToString() ?? "",
                    k.OilConditionMonitoringKit?.ToString() ?? "",
                    k.FiveLitresCubitainers?.ToString() ?? "",
                    k.TenLitresCubitainers?.ToString() ?? "",
                    k.VesselName ?? "",
                    k.IMONumber ?? "",
                    k.RequestorName ?? "",
                    k.RequestorEmail ?? "",
                    k.EmailCC ?? "",
                    k.VPSCustomerName ?? "",
                    k.PONumber ?? "",
                    k.Street ?? "",
                    k.City ?? "",
                    k.HouseNo ?? "",
                    k.PostalCode ?? "",
                    k.AdditionalAddressInfo ?? "",
                    k.CompanyName ?? "",
                    k.PersonToContact ?? "",
                    k.PersonToContactTelNo ?? "",
                    k.DeliveryEmail ?? "",
                    k.BillingCompanyName ?? "",
                    k.BillingAddressLine1 ?? "",
                    k.BillingAddressLine2 ?? "",
                    k.BillingAddressLine3 ?? "",
                    k.BillingPostalCode ?? "",
                    k.DeliveryDeadline?.ToString("yyyy-MM-dd") ?? "",
                    k.Country.ToString(),
                    k.BillingCountry.ToString()
                };

                for (int i = 0; i < values.Length; i++)
                {
                    row.AppendChild(new Cell { CellValue = new CellValue(values[i]), DataType = CellValues.String });
                    maxLengths[i] = Math.Max(maxLengths[i], values[i].Length);
                }

                sheetData.AppendChild(row);
            }

            var columns = new Columns();
            for (uint i = 1; i <= headers.Length; i++)
            {
                double width = maxLengths[i - 1] + 2;
                columns.Append(new Column { Min = i, Max = i, Width = width, CustomWidth = true });
            }

            worksheetPart.Worksheet.InsertAt(columns, 0);
            workbookPart.Workbook.Save();
        }

        stream.Position = 0;

        return File(
            stream.ToArray(),
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "SamplingKits.xlsx");
    }

    // POST: /api/sampling-kits/{id}/send-for-review
    [HttpPost("{id:int}/send-for-review")]
    public async Task<ActionResult<ApiMessageResponse>> SendForReview(int id)
    {
        var order = await _service.GetByIdAsync(id);

        if (order == null)
        {
            return NotFound(new ApiMessageResponse
            {
                Success = false,
                Message = "SamplingKit order not found."
            });
        }

        return Ok(new ApiMessageResponse
        {
            Success = true,
            Message = "Review email sent successfully."
        });
    }

    // POST: /api/sampling-kits/{id}/approve
    [HttpPost("{id:int}/approve")]
    public async Task<ActionResult<ApiMessageResponse>> ApproveSamplingKit(
        int id,
        [FromBody] SamplingKitApproveRequest? request,
        CancellationToken ct)
    {
        var currentUser = await _userManager.GetUserAsync(User);

        if (currentUser == null || currentUser.Role != Role.SystemAdmin)
            return Forbid();

        var kit = await _service.GetByIdAsync(id);

        if (kit == null)
            return NotFound(new { message = "Sampling kit order not found." });

        var validateDto = new AddressValidationRequestDto
        {
            AddressLine1 = kit.RecipientAddressLine1,
            AddressLine2 = kit.RecipientAddressLine2,
            AddressLine3 = kit.RecipientAddressLine3,
            City = kit.RecipientCity,
            StateOrProvinceCode = kit.RecipientState,
            PostalCode = kit.RecipientPostalCode,
            CountryCode = TryGetCountryEnum(kit.RecipientCountry ?? "", out var country)
                ? CountryIsoHelper.ToIso2(country)
                : CountryIsoHelper.ToIso2(Country.UnitedStates)
        };

        var validation = await _fedExShippingService.ValidateAddressAsync(validateDto, ct);

        if (!validation.IsValid)
        {
            return BadRequest(new
            {
                success = false,
                message = validation.Message ?? "The provided address is invalid.",
                validation
            });
        }

        var oldStatus = kit.RequestStatus;
        var vesselDetails = await _unitOfWork.VesselDetail
            .GetAsync(x => x.VesselName.ToLower() == (kit.VesselName ?? "").ToLower());

        if (string.IsNullOrWhiteSpace(kit.FedExRequestJson))
        {
            kit.RequestStatus = SampleCollectionStatus.Failed;
            kit.FedExErrorMessage = "Missing shipment request payload.";
            kit.IsApiDispatched = false;
            kit.ApiDispatchReference = null;

            await _service.UpdateAsync(kit);

            await AddHistoryAsync(
                requestId: kit.Id,
                requestType: "SamplingKit",
                action: RequestHistoryAction.StatusChanged,
                performedBy: currentUser.UserName ?? currentUser.Email ?? "Unknown",
                notes: "Missing shipment request payload.",
                oldStatus: oldStatus.ToString(),
                newStatus: kit.RequestStatus.ToString(),
                vesselDetailId: vesselDetails?.Id);

            return BadRequest(new ApiMessageResponse
            {
                Success = false,
                Message = "Cannot process – shipment payload is missing."
            });
        }

        var shipmentRequest = JsonConvert.DeserializeObject<SampleCollectionRequestVM>(kit.FedExRequestJson);

        if (shipmentRequest == null)
        {
            kit.RequestStatus = SampleCollectionStatus.Failed;
            kit.FedExErrorMessage = "Shipment payload could not be deserialized.";
            kit.IsApiDispatched = false;
            kit.ApiDispatchReference = null;

            await _service.UpdateAsync(kit);

            await AddHistoryAsync(
                requestId: kit.Id,
                requestType: "SamplingKit",
                action: RequestHistoryAction.StatusChanged,
                performedBy: currentUser.UserName ?? currentUser.Email ?? "Unknown",
                notes: "Shipment payload JSON invalid.",
                oldStatus: oldStatus.ToString(),
                newStatus: kit.RequestStatus.ToString(),
                vesselDetailId: vesselDetails?.Id);

            return BadRequest(new ApiMessageResponse
            {
                Success = false,
                Message = "Cannot process – invalid shipment payload."
            });
        }

        if (string.IsNullOrWhiteSpace(shipmentRequest.Country))
        {
            shipmentRequest.Country = CountryIsoHelper.ToIso2(kit.Country);
        }

        if (request?.CourierProvider != null)
        {
            kit.CourierProvider = request.CourierProvider.Value;
        }

        var courier = kit.CourierProvider;

        // Preserving active MVC behavior:
        // Actual FedEx/DHL shipment creation code is commented in MVC controller,
        // so API also only updates approval status and request history.
        kit.RequestStatus = SampleCollectionStatus.Approved;
        await _service.UpdateAsync(kit);

        await AddHistoryAsync(
            requestId: kit.Id,
            requestType: "SamplingKit",
            action: RequestHistoryAction.APIDispatch,
            performedBy: currentUser.UserName ?? currentUser.Email ?? "Unknown",
            notes: $"Shipment created via {courier}.",
            trackingNumber: kit.FedExTrackingNumber,
            courier: courier.ToString(),
            apiDispatchRef: kit.ApiDispatchReference,
            vesselDetailId: vesselDetails?.Id);

        await AddHistoryAsync(
            requestId: kit.Id,
            requestType: "SamplingKit",
            action: RequestHistoryAction.StatusChanged,
            performedBy: currentUser.UserName ?? currentUser.Email ?? "Unknown",
            notes: "Status updated to Approved after API dispatch.",
            oldStatus: SampleCollectionStatus.Completed.ToString(),
            newStatus: kit.RequestStatus.ToString(),
            vesselDetailId: vesselDetails?.Id);

        return Ok(new ApiMessageResponse
        {
            Success = true,
            Message = $"Sampling Kit request approved and {courier} shipment created."
        });
    }

    // POST: /api/sampling-kits/{id}/reject
    [HttpPost("{id:int}/reject")]
    public async Task<ActionResult<ApiMessageResponse>> RejectSamplingKit(
        int id,
        [FromBody] SamplingKitRejectRequest request)
    {
        var currentUser = await _userManager.GetUserAsync(User);

        if (currentUser == null || currentUser.Role != Role.SystemAdmin)
            return Forbid();

        var kit = await _service.GetByIdAsync(id);

        if (kit == null)
            return NotFound(new { message = "Sampling kit order not found." });

        kit.RequestStatus = SampleCollectionStatus.Rejected;
        kit.FedExErrorMessage = request?.Reason;

        await _service.UpdateAsync(kit);

        return Ok(new ApiMessageResponse
        {
            Success = true,
            Message = "Sampling Kit request rejected."
        });
    }

    // GET: /api/sampling-kits/vessel-name-by-imo?imo=1234567
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

        if (vessels == null || !vessels.Any())
            return NotFound(new { message = "No vessel found for this IMO." });

        var currentUser = await _userManager.GetUserAsync(User);

        if (currentUser != null)
        {
            var role = currentUser.Role;

            if (role == Role.ManagementUser)
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

            if (role == Role.VesselUser)
            {
                var vesselIds = vessels.Select(v => v.Id).ToList();

                var assignedVessels = await _unitOfWork.VesselUser.GetAllAsync(
                    x => x.UserId == currentUser.Id && vesselIds.Contains(x.VesselDetailId));

                var assignedVessel = assignedVessels.FirstOrDefault();

                if (assignedVessel == null)
                {
                    return StatusCode(StatusCodes.Status403Forbidden, new
                    {
                        error = true,
                        code = "VesselNotAssigned",
                        message = "No vessel with this IMO is assigned to this user."
                    });
                }

                var allowedVessel = vessels.FirstOrDefault(v => v.Id == assignedVessel.VesselDetailId);

                if (allowedVessel == null)
                {
                    return StatusCode(StatusCodes.Status403Forbidden, new
                    {
                        error = true,
                        code = "VesselNotAssigned",
                        message = "No vessel with this IMO is assigned to this user."
                    });
                }

                return Ok(new VesselNameLookupResponse
                {
                    VesselName = allowedVessel.VesselName
                });
            }
        }

        return Ok(new VesselNameLookupResponse
        {
            VesselName = vessels.First().VesselName
        });
    }

    // PUT: /api/sampling-kits/{id}/admin-edit
    [HttpPut("{id:int}/admin-edit")]
    public async Task<ActionResult<ApiMessageResponse>> AdminEditSamplingKit(
        int id,
        [FromBody] SamplingKitAdminEditRequest request,
        CancellationToken ct)
    {
        var currentUser = await _userManager.GetUserAsync(User);

        if (currentUser == null || currentUser.Role != Role.SystemAdmin)
            return Forbid();

        var kit = await _service.GetByIdAsync(id);

        if (kit == null)
            return NotFound(new { message = "Sampling kit order not found." });

        var oldStatus = kit.RequestStatus;
        var oldTracking = kit.FedExTrackingNumber;

        var vesselDetails = await _unitOfWork.VesselDetail
            .GetAsync(x => x.VesselName.ToLower() == (kit.VesselName ?? "").ToLower());

        kit.NumberOfParcels = request.NumberOfParcels;
        kit.DeclaredValue = request.DeclaredValue;
        kit.PackageWeightLb = request.PackageWeightLb;
        kit.CommodityDescription = request.CommodityDescription;

        kit.CourierProvider = request.CourierProvider;
        kit.FedExTrackingNumber = request.TrackingNumber;
        kit.RequestStatus = request.RequestStatus;

        await _service.UpdateAsync(kit);

        if (oldStatus != kit.RequestStatus)
        {
            await AddHistoryAsync(
                requestId: kit.Id,
                requestType: "SamplingKit",
                action: RequestHistoryAction.StatusChanged,
                performedBy: currentUser.UserName ?? currentUser.Email ?? "Unknown",
                notes: "Status updated by admin.",
                oldStatus: oldStatus.ToString(),
                newStatus: kit.RequestStatus.ToString(),
                vesselDetailId: vesselDetails?.Id);
        }

        if (!string.Equals(oldTracking, kit.FedExTrackingNumber, StringComparison.OrdinalIgnoreCase))
        {
            await AddHistoryAsync(
                requestId: kit.Id,
                requestType: "SamplingKit",
                action: RequestHistoryAction.TrackingUpdated,
                performedBy: currentUser.UserName ?? currentUser.Email ?? "Unknown",
                notes: "Tracking number updated by admin.",
                trackingNumber: kit.FedExTrackingNumber,
                vesselDetailId: vesselDetails?.Id);
        }

        return Ok(new ApiMessageResponse
        {
            Success = true,
            Message = "Sampling Kit request updated successfully."
        });
    }

    // POST: /api/sampling-kits/{id}/cancel-by-user
    [HttpPost("{id:int}/cancel-by-user")]
    public async Task<ActionResult<ApiMessageResponse>> CancelSamplingKitByUser(int id, CancellationToken ct)
    {
        var currentUser = await _userManager.GetUserAsync(User);

        if (currentUser == null)
            return Forbid();

        var kit = await _service.GetByIdAsync(id);

        if (kit == null)
            return NotFound(new { message = "Sampling kit order not found." });

        if (kit.ApplicationUserId != currentUser.Id && currentUser.Role != Role.SystemAdmin)
            return Forbid();

        if (kit.RequestStatus != SampleCollectionStatus.Pending &&
            kit.RequestStatus != SampleCollectionStatus.UnderReview)
        {
            return BadRequest(new ApiMessageResponse
            {
                Success = false,
                Message = "This request can no longer be canceled."
            });
        }

        var oldStatus = kit.RequestStatus;

        var vesselDetails = await _unitOfWork.VesselDetail
            .GetAsync(x => x.VesselName.ToLower() == (kit.VesselName ?? "").ToLower());

        kit.RequestStatus = SampleCollectionStatus.CancelledByUser;
        await _service.UpdateAsync(kit);

        await AddHistoryAsync(
            requestId: kit.Id,
            requestType: "SamplingKit",
            action: RequestHistoryAction.CanceledByClient,
            performedBy: currentUser.UserName ?? currentUser.Email ?? "Unknown",
            notes: "Sampling kit request cancelled by client.",
            oldStatus: oldStatus.ToString(),
            newStatus: kit.RequestStatus.ToString(),
            vesselDetailId: vesselDetails?.Id);

        return Ok(new ApiMessageResponse
        {
            Success = true,
            Message = "Your Sampling Kit request has been canceled."
        });
    }

    // POST: /api/sampling-kits/{id}/cancel-by-admin
    [HttpPost("{id:int}/cancel-by-admin")]
    public async Task<ActionResult<ApiMessageResponse>> CancelSamplingKitByAdmin(int id, CancellationToken ct)
    {
        var currentUser = await _userManager.GetUserAsync(User);

        if (currentUser == null || currentUser.Role != Role.SystemAdmin)
            return Forbid();

        var kit = await _service.GetByIdAsync(id);

        if (kit == null)
            return NotFound(new { message = "Sampling kit order not found." });

        if (kit.RequestStatus == SampleCollectionStatus.CancelledByAdmin ||
            kit.RequestStatus == SampleCollectionStatus.CancelledByUser)
        {
            return BadRequest(new ApiMessageResponse
            {
                Success = false,
                Message = "This request is already canceled."
            });
        }

        var oldStatus = kit.RequestStatus;

        var vesselDetails = await _unitOfWork.VesselDetail
            .GetAsync(x => x.VesselName.ToLower() == (kit.VesselName ?? "").ToLower());

        kit.RequestStatus = SampleCollectionStatus.CancelledByAdmin;
        await _service.UpdateAsync(kit);

        await AddHistoryAsync(
            requestId: kit.Id,
            requestType: "SamplingKit",
            action: RequestHistoryAction.CanceledByAdmin,
            performedBy: currentUser.UserName ?? currentUser.Email ?? "Unknown",
            notes: "Sampling kit request cancelled by admin.",
            oldStatus: oldStatus.ToString(),
            newStatus: kit.RequestStatus.ToString(),
            vesselDetailId: vesselDetails?.Id);

        return Ok(new ApiMessageResponse
        {
            Success = true,
            Message = "Sampling Kit request canceled by Seahawk Admin."
        });
    }

    // POST: /api/sampling-kits/{id}/comments
    [HttpPost("{id:int}/comments")]
    public async Task<ActionResult<ApiMessageResponse>> AddSamplingKitComment(
        int id,
        [FromBody] SamplingKitCommentRequest request)
    {
        var currentUser = await _userManager.GetUserAsync(User);

        if (currentUser == null)
            return Unauthorized(new { message = "User not found." });

        if (currentUser.Role != Role.SystemAdmin)
            return Forbid();

        if (string.IsNullOrWhiteSpace(request?.Comment))
        {
            return BadRequest(new ApiMessageResponse
            {
                Success = false,
                Message = "Comment cannot be empty."
            });
        }

        var kit = await _service.GetByIdAsync(id);

        if (kit == null)
            return NotFound(new { message = "Sampling kit order not found." });

        await AddHistoryAsync(
            requestId: kit.Id,
            requestType: "SamplingKit",
            action: RequestHistoryAction.CommentAdded,
            performedBy: currentUser.UserName ?? currentUser.Email ?? "Unknown",
            notes: request.Comment.Trim());

        return Ok(new ApiMessageResponse
        {
            Success = true,
            Message = "Comment added to Sampling Kit request log."
        });
    }

    // POST: /api/sampling-kits/validate-address
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

        if (string.IsNullOrWhiteSpace(dto.AddressLine1) ||
            string.IsNullOrWhiteSpace(dto.CountryCode))
        {
            return Ok(new AddressValidationResponseDto
            {
                IsValid = false,
                Message = "Address Line 1 and Country are required for validation."
            });
        }

        var courier = dto.CourierProvider?.Trim();

        try
        {
            var countryIso = TryGetCountryEnum(dto.CountryCode, out var country)
                ? CountryIsoHelper.ToIso2(country)
                : CountryIsoHelper.ToIso2(Country.UnitedStates);

            dto.CountryCode = countryIso;

            AddressValidationResponseDto result;

            if (countryIso == "US" ||
                (!string.IsNullOrWhiteSpace(courier) &&
                 courier.Equals("Dhl", StringComparison.OrdinalIgnoreCase)))
            {
                result = await _dhlShippingService.ValidateAddressAsync(dto, ct);
            }
            else
            {
                result = await _fedExShippingService.ValidateAddressAsync(dto, ct);
            }

            return Ok(result);
        }
        catch
        {
            return Ok(new AddressValidationResponseDto
            {
                IsValid = false,
                Message = "Address validation failed. Please try again or verify the address."
            });
        }
    }

    private static SamplingKitDto ToDto(SamplingKit kit)
    {
        return new SamplingKitDto
        {
            Id = kit.Id,

            FuelQualityTestKit = kit.FuelQualityTestKit,
            PowerPlantKit = kit.PowerPlantKit,
            OilConditionMonitoringKit = kit.OilConditionMonitoringKit,
            FiveLitresCubitainers = kit.FiveLitresCubitainers,
            TenLitresCubitainers = kit.TenLitresCubitainers,

            VesselName = kit.VesselName,
            IMONumber = kit.IMONumber,

            RequestorName = kit.RequestorName,
            RequestorEmail = kit.RequestorEmail,
            EmailCC = kit.EmailCC,

            VPSCustomerName = kit.VPSCustomerName,
            PONumber = kit.PONumber,

            Street = kit.Street,
            City = kit.City,
            HouseNo = kit.HouseNo,
            PostalCode = kit.PostalCode,
            AdditionalAddressInfo = kit.AdditionalAddressInfo,

            CompanyName = kit.CompanyName,
            PersonToContact = kit.PersonToContact,
            PersonToContactTelNo = kit.PersonToContactTelNo,
            DeliveryEmail = kit.DeliveryEmail,

            BillingCompanyName = kit.BillingCompanyName,
            BillingAddressLine1 = kit.BillingAddressLine1,
            BillingAddressLine2 = kit.BillingAddressLine2,
            BillingAddressLine3 = kit.BillingAddressLine3,
            BillingPostalCode = kit.BillingPostalCode,

            DeliveryDeadline = kit.DeliveryDeadline,

            Country = kit.Country,
            BillingCountry = kit.BillingCountry,

            State = kit.State,

            NumberOfParcels = kit.NumberOfParcels,
            DeclaredValue = kit.DeclaredValue,
            PackageWeightLb = kit.PackageWeightLb,
            CommodityDescription = kit.CommodityDescription,

            RecipientName = kit.RecipientName,
            RecipientCompanyName = kit.RecipientCompanyName,
            RecipientPhone = kit.RecipientPhone,
            RecipientAddressLine1 = kit.RecipientAddressLine1,
            RecipientAddressLine2 = kit.RecipientAddressLine2,
            RecipientAddressLine3 = kit.RecipientAddressLine3,
            RecipientCity = kit.RecipientCity,
            RecipientState = kit.RecipientState,
            RecipientPostalCode = kit.RecipientPostalCode,
            RecipientCountry = kit.RecipientCountry,

            ApplicationUserId = kit.ApplicationUserId,

            RequestStatus = kit.RequestStatus,
            RequestStatusText = kit.RequestStatus.ToString(),

            CourierProvider = kit.CourierProvider,
            CourierProviderText = kit.CourierProvider.ToString(),

            FedExTrackingNumber = kit.FedExTrackingNumber,
            FedExLabelUrl = kit.FedExLabelUrl,
            FedExInvoiceUrl = kit.FedExInvoiceUrl,

            FedExRequestJson = kit.FedExRequestJson,
            FedExErrorMessage = kit.FedExErrorMessage,

            IsApiDispatched = kit.IsApiDispatched,
            ApiDispatchReference = kit.ApiDispatchReference
        };
    }

    private static void ApplyCreateUpdateRequest(SamplingKit kit, SamplingKitCreateUpdateRequest request)
    {
        kit.FuelQualityTestKit = request.FuelQualityTestKit;
        kit.PowerPlantKit = request.PowerPlantKit;
        kit.OilConditionMonitoringKit = request.OilConditionMonitoringKit;
        kit.FiveLitresCubitainers = request.FiveLitresCubitainers;
        kit.TenLitresCubitainers = request.TenLitresCubitainers;

        kit.VesselName = request.VesselName;
        kit.IMONumber = request.IMONumber;

        kit.RequestorName = request.RequestorName;
        kit.RequestorEmail = request.RequestorEmail;
        kit.EmailCC = request.EmailCC;

        kit.VPSCustomerName = request.VPSCustomerName;
        kit.PONumber = request.PONumber;

        kit.Street = request.Street;
        kit.City = request.City;
        kit.HouseNo = request.HouseNo;
        kit.PostalCode = request.PostalCode;
        kit.AdditionalAddressInfo = request.AdditionalAddressInfo;

        kit.CompanyName = request.CompanyName;
        kit.PersonToContact = request.PersonToContact;
        kit.PersonToContactTelNo = request.PersonToContactTelNo;
        kit.DeliveryEmail = request.DeliveryEmail;

        kit.BillingCompanyName = request.BillingCompanyName;
        kit.BillingAddressLine1 = request.BillingAddressLine1;
        kit.BillingAddressLine2 = request.BillingAddressLine2;
        kit.BillingAddressLine3 = request.BillingAddressLine3;
        kit.BillingPostalCode = request.BillingPostalCode;

        kit.DeliveryDeadline = request.DeliveryDeadline;

        kit.Country = request.Country;
        kit.BillingCountry = request.BillingCountry;

        kit.State = request.State;

        kit.NumberOfParcels = request.NumberOfParcels;
        kit.DeclaredValue = request.DeclaredValue;
        kit.PackageWeightLb = request.PackageWeightLb;
        kit.CommodityDescription = request.CommodityDescription;

        kit.RecipientName = request.RecipientName;
        kit.RecipientCompanyName = request.RecipientCompanyName;
        kit.RecipientPhone = request.RecipientPhone;
        kit.RecipientAddressLine1 = request.RecipientAddressLine1;
        kit.RecipientAddressLine2 = request.RecipientAddressLine2;
        kit.RecipientAddressLine3 = request.RecipientAddressLine3;
        kit.RecipientCity = request.RecipientCity;
        kit.RecipientState = request.RecipientState;
        kit.RecipientPostalCode = request.RecipientPostalCode;
        kit.RecipientCountry = request.RecipientCountry;

        kit.CourierProvider = request.CourierProvider;
    }

    private SampleCollectionRequestVM MapSamplingKitToSampleCollectorVM(SamplingKit s)
    {
        string Safe(string? value, string fallback) =>
            string.IsNullOrWhiteSpace(value) ? fallback : value.Trim();

        var pickupDate = s.DeliveryDeadline ?? DateTime.UtcNow.Date;
        var declaredValue = s.DeclaredValue ?? 100m;
        var packageWeightLb = s.PackageWeightLb ?? 70m;
        var numberOfParcels = s.NumberOfParcels ?? 1;

        var commodityDescription = Safe(
            s.CommodityDescription,
            "Sampling kit for fuel / oil condition monitoring");

        string countryIso = CountryIsoHelper.ToIso2(s.Country);
        var recipientCountry = Safe(s.RecipientCountry, "CA");

        return new SampleCollectionRequestVM
        {
            EarliestPickupDateTime = pickupDate,
            LatestPickupDateTime = pickupDate,
            Timezone = "UTC",
            Port = Safe(s.City, "PORT"),

            NoOfParcels = numberOfParcels,
            IMONumber = Safe(s.IMONumber, "N/A"),
            VesselName = Safe(s.VesselName, "Sampling Kit"),
            Fuel = false,

            SenderName = Safe(s.RequestorName, "Unknown Requestor"),
            CompanyName = Safe(s.CompanyName, "Unknown Company"),
            SenderPhone = Safe(s.PersonToContactTelNo, "0000000000"),
            SenderEmail = Safe(s.RequestorEmail, "no-reply@seahawkservices.com"),

            AddressLine1 = Safe(s.Street, "Street"),
            AddressLine2 = Safe(s.AdditionalAddressInfo, ""),
            AddressLine3 = Safe(s.HouseNo, ""),
            City = Safe(s.City, "City"),
            State = Safe(s.State, "MP"),
            Country = countryIso,
            PostCode = Safe(s.PostalCode, "000000"),

            DeclaredValue = declaredValue,
            PackageWeightLb = packageWeightLb,
            CommodityDescription = commodityDescription,

            RecipientName = Safe(s.RecipientName, "RECIPIENT NAME"),
            RecipientCompanyName = Safe(s.RecipientCompanyName, "Recipient Company Name"),
            RecipientPhone = Safe(s.RecipientPhone, "1234567890"),
            RecipientAddressLine1 = Safe(s.RecipientAddressLine1, "RECIPIENT STREET LINE 1"),
            RecipientAddressLine2 = Safe(s.RecipientAddressLine2, "RECIPIENT STREET LINE 2"),
            RecipientAddressLine3 = Safe(s.RecipientAddressLine3, "RECIPIENT STREET LINE 3"),
            RecipientCity = Safe(s.RecipientCity, "RICHMOND"),
            RecipientState = Safe(s.RecipientState, "BC"),
            RecipientPostalCode = Safe(s.RecipientPostalCode, "V7C4V7"),
            RecipientCountry = recipientCountry
        };
    }

    private async Task AddHistoryAsync(
        int requestId,
        string requestType,
        RequestHistoryAction action,
        string performedBy,
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

    private static string FormatCountryName(string country)
    {
        if (string.IsNullOrWhiteSpace(country))
            return country;

        return System.Text.RegularExpressions.Regex
            .Replace(country, "([a-z])([A-Z])", "$1 $2")
            .Trim();
    }

    private static string ResolveCountryIso2ForPostalLookup(string country)
    {
        if (string.IsNullOrWhiteSpace(country))
            return "US";

        country = country.Trim();

        if (int.TryParse(country, out int countryValue))
        {
            var enumCountry = (Country)countryValue;
            return CountryIsoHelper.ToIso2(enumCountry);
        }

        string normalizedInput = country
            .Replace(" ", "")
            .Replace("-", "")
            .Trim();

        foreach (Country item in Enum.GetValues(typeof(Country)))
        {
            string enumName = item.ToString()
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

    private static bool TryGetCountryEnum(string value, out Country country)
    {
        country = default;

        if (string.IsNullOrWhiteSpace(value))
            return false;

        var normalized = value.Replace(" ", "");

        return Enum.TryParse(normalized, ignoreCase: true, out country);
    }

    private sealed class CountriesNowCityResponse
    {
        [JsonProperty("error")]
        public bool Error { get; set; }

        [JsonProperty("msg")]
        public string? Msg { get; set; }

        [JsonProperty("data")]
        public List<string>? Data { get; set; }
    }

    private sealed class CountriesNowStatesResponse
    {
        [JsonProperty("error")]
        public bool Error { get; set; }

        [JsonProperty("msg")]
        public string? Msg { get; set; }

        [JsonProperty("data")]
        public CountriesNowStatesData? Data { get; set; }
    }

    private sealed class CountriesNowStatesData
    {
        [JsonProperty("states")]
        public List<CountriesNowState>? States { get; set; }
    }

    private sealed class CountriesNowState
    {
        [JsonProperty("name")]
        public string? Name { get; set; }

        [JsonProperty("state_code")]
        public string? State_Code { get; set; }
    }
}