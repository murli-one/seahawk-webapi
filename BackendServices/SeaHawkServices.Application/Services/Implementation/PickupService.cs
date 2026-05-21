using Microsoft.Extensions.Configuration;
using SeaHawkServices.Application.Services.Interface;
using SeaHawkServices.Domain.Entities;
using SeaHawkServices.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using static SeaHawkServices.Domain.Entities.Enums;

namespace SeaHawkServices.Application.Services.Implementation
{
    public class PickupService : IPickupService
    {
        private readonly HttpClient _http;
        private readonly IConfiguration _config;
        IFedExShippingService _fedExShippingService;

        public PickupService(HttpClient http, IConfiguration config, IFedExShippingService fedExShippingService)
        {
            _http = http;
            _config = config;
            _fedExShippingService = fedExShippingService;
        }

        public async Task<SamplePickupResultVM> CreateDhlPickupAsync(SamplePickupRequestVM vm, CancellationToken ct)
        {

            var url = "https://express.api.dhl.com/mydhlapi/pickups"; // move to config

            var body = new
            {
                plannedPickupDateAndTime = vm.PlannedPickupDateTime?.ToString("yyyy-MM-ddTHH:mm:ss 'GMT+01:00'"),
                closeTime = vm.CloseTime,
                location = vm.Location,
                locationType = vm.LocationType,
                accounts = new[]
                {
                    new { typeCode = "shipper", number = vm.AccountNumber }
                },
                specialInstructions = new[]
                {
                    new { value = "please ring front desk", typeCode = "TBD" }
                },
                remark = vm.Remark,
                customerDetails = new
                {
                    shipperDetails = new
                    {
                        postalAddress = new
                        {
                            postalCode = vm.PostalCode,
                            cityName = vm.CityName,
                            countryCode = vm.CountryCode,
                            addressLine1 = vm.AddressLine1,
                            addressLine2 = vm.AddressLine2,
                            addressLine3 = vm.AddressLine3
                        },
                        contactInformation = new
                        {
                            email = vm.ShipperEmail,
                            phone = vm.ShipperPhone,
                            mobilePhone = vm.ShipperPhone,
                            companyName = vm.ShipperCompany,
                            fullName = vm.ShipperName
                        }
                    },
                    // For now, mirror receiver & pickup details from shipper
                    receiverDetails = (object)null,
                    bookingRequestorDetails = (object)null,
                    pickupDetails = (object)null
                },
                shipmentDetails = new[]
                {
                    new
                    {
                        productCode = "P",
                        localProductCode = "P",
                        accounts = new[]
                        {
                            new { typeCode = "shipper", number = vm.AccountNumber }
                        },
                        isCustomsDeclarable = true,
                        declaredValue = vm.DeclaredValue ?? 50m,
                        declaredValueCurrency = vm.DeclaredValueCurrency ?? "USD",
                        unitOfMeasurement = "metric",
                        packages = new[]
                        {
                            new
                            {
                                typeCode = "3BX",
                                weight = (double)vm.TotalWeight,
                                dimensions = new { length = 25, width = 35, height = 15 }
                            }
                        }
                    }
                }
            };

            var json = JsonSerializer.Serialize(body);
            var req = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };

            // REMOVE THIS — not allowed
            // req.Headers.Add("content-type", "application/json");

            req.Headers.Add("Message-Reference", "1234567890");
            req.Headers.Add("Message-Reference-Date", DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"));
            req.Headers.Add("Plugin-Name", "SeaHawk");
            req.Headers.Add("Plugin-Version", "1.0.0");
            req.Headers.Add("Shipping-System-Platform-Name", "SeaHawkPlatform");
            req.Headers.Add("Shipping-System-Platform-Version", "1.0.0");
            req.Headers.Add("Webstore-Platform-Name", "SeaHawkPortal");
            req.Headers.Add("Webstore-Platform-Version", "1.0.0");
            req.Headers.Add("x-version", "1");

            req.Headers.Authorization = new AuthenticationHeaderValue(
                "Basic",
                _config["DHL:BasicAuth"] // MUST be Base64(user:pass)
            );

            var result = new SamplePickupResultVM { Courier = "DHL" };

            try
            {
                var resp = await _http.SendAsync(req, ct);
                var respContent = await resp.Content.ReadAsStringAsync(ct);
                result.RawResponse = respContent;

                if (!resp.IsSuccessStatusCode)
                {
                    result.Success = false;
                    result.Error = $"DHL pickup failed: {(int)resp.StatusCode} {resp.ReasonPhrase}";
                    return result;
                }

                result.Success = true;
                result.Message = "DHL pickup created successfully.";
                // Parse response JSON if you need confirmation code, etc.
                return result;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Error = ex.Message;
                return result;
            }
        }



        public async Task<SamplePickupResultVM> CreateFedExPickupAsync(
      SamplePickupRequestVM vm,
      CancellationToken ct)
        {
            var result = new SamplePickupResultVM
            {
                Courier = "FedEx"
            };

            try
            {
                // 1) Get token & URL
                var accessToken = await _fedExShippingService.GetAccessTokenAsync(ct);
                var url = "https://apis-sandbox.fedex.com/pickup/v1/pickups";

                // ===== Country (enum → ISO2) =====
                Country selectedCountry;

                if (!string.IsNullOrWhiteSpace(vm.CountryCode) && vm.CountryCode!.Length == 2)
                {
                    selectedCountry = CountryIsoHelper.FromIso2(vm.CountryCode);
                }
                else
                {
                    // fallback – you can change this default if you want
                    selectedCountry = Country.UnitedStates;
                }

                var countryIso = CountryIsoHelper.ToIso2(selectedCountry); // e.g. "GB", "US"

                // ===== Close time normalization (HH:mm:ss) =====
                string closeTime = (vm.CloseTime ?? string.Empty).Trim();
                if (string.IsNullOrEmpty(closeTime))
                {
                    closeTime = "18:00:00";
                }
                else if (closeTime.Length == 5 && closeTime[2] == ':') // "HH:mm"
                {
                    closeTime += ":00";
                }
                // if it is already "HH:mm:ss", leave it

                // 2) Date/time handling
                var planned = vm.PlannedPickupDateTime ?? DateTime.UtcNow.AddDays(1);

                // FedEx expects full timestamp
                var readyDateTimestamp = planned.ToString("yyyy-MM-dd'T'HH:mm:ss");
                var dispatchDate = planned.ToString("yyyy-MM-dd");

                var pickupDateType =
                    planned.Date == DateTime.UtcNow.Date ? "SAME_DAY" : "FUTURE_DAY";

                // 3) Build street lines safely (no null/empty values)
                var streetLines = new List<string>();
                if (!string.IsNullOrWhiteSpace(vm.AddressLine1)) streetLines.Add(vm.AddressLine1);
                if (!string.IsNullOrWhiteSpace(vm.AddressLine2)) streetLines.Add(vm.AddressLine2);
                if (!string.IsNullOrWhiteSpace(vm.AddressLine3)) streetLines.Add(vm.AddressLine3);

                // Safe weight
                var totalWeightValue = vm.TotalWeight;

                // 4) Build request body using form values
                var body = new
                {
                    associatedAccountNumber = new
                    {
                        value = string.IsNullOrWhiteSpace(vm.AccountNumber)
                            ? "740561073" // fallback sandbox account (or move to config)
                            : vm.AccountNumber
                    },
                    associatedAccountNumberType = "FEDEX_GROUND",

                    originDetail = new
                    {
                        pickupAddressType = "ACCOUNT",
                        pickupLocation = new
                        {
                            contact = new
                            {
                                personName = vm.ShipperName,
                                companyName = vm.ShipperCompany,
                                phoneNumber = vm.ShipperPhone
                            },
                            address = new
                            {
                                streetLines = streetLines.ToArray(),
                                city = vm.CityName,
                                stateOrProvinceCode = "LND", // not on form – keep constant for now
                                postalCode = vm.PostalCode,
                                countryCode = countryIso,     // ✅ ISO2 from CountryIsoHelper
                                residential = false,
                                addressClassification = "BUSINESS"
                            }
                        },
                        readyDateTimestamp = readyDateTimestamp,
                        customerCloseTime = closeTime,          // ✅ normalized
                        pickupDateType = pickupDateType,        // SAME_DAY or FUTURE_DAY
                        packageLocation = "FRONT",
                        buildingPart = "SUITE",
                        buildingPartDescription = "Suite 10B",
                        earlyPickup = false
                    },

                    dispatchDate = dispatchDate,

                    totalWeight = new
                    {
                        units = "KG",
                        value = (double)totalWeightValue         // ✅ safe default
                    },
                    packageCount = vm.PackageCount > 0 ? vm.PackageCount : 1,
                    carrierCode = "FDXG",

                    accountAddressOfRecord = new
                    {
                        streetLines = streetLines.ToArray(),
                        city = vm.CityName,
                        stateOrProvinceCode = "LND",
                        postalCode = vm.PostalCode,
                        countryCode = countryIso,               // ✅ ISO2 here too
                        residential = false,
                        addressClassification = "BUSINESS"
                    },

                    remarks = string.IsNullOrWhiteSpace(vm.Remark)
                        ? "Pickup request from Sea Hawk portal."
                        : vm.Remark,
                    countryRelationships = "DOMESTIC",
                    pickupType = "ON_CALL",
                    commodityDescription = "Fuel test sampling kit",
                    oversizePackageCount = 0
                };

                var jsonOptions = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = null, // keep property names AS IS
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
                };

                var json = JsonSerializer.Serialize(body, jsonOptions);

                // 5) Build request message
                using var request = new HttpRequestMessage(HttpMethod.Post, url);
                request.Headers.Authorization =
                    new AuthenticationHeaderValue("Bearer", accessToken);
                request.Headers.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));
                request.Content = new StringContent(json, Encoding.UTF8, "application/json");

                // 6) Send & handle response
                var resp = await _http.SendAsync(request, ct);
                var respContent = await resp.Content.ReadAsStringAsync(ct);

                result.RawResponse = respContent;

                if (!resp.IsSuccessStatusCode)
                {
                    result.Success = false;
                    result.Error = $"FedEx pickup failed: {(int)resp.StatusCode} {resp.ReasonPhrase}";
                    return result;
                }

                // Parse the pickupConfirmationCode if present
                try
                {
                    using var doc = JsonDocument.Parse(respContent);
                    if (doc.RootElement.TryGetProperty("output", out var output) &&
                        output.TryGetProperty("pickupConfirmationCode", out var codeProp))
                    {
                        result.ConfirmationCode = codeProp.GetString();
                    }
                }
                catch
                {
                    // parsing is best-effort; don't fail pickup if schema changes slightly
                }

                result.Success = true;
                result.Message = string.IsNullOrEmpty(result.ConfirmationCode)
                    ? "FedEx pickup created successfully."
                    : $"FedEx pickup created successfully. Confirmation: {result.ConfirmationCode}";
                result.Location = vm.CityName;

                return result;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Error = ex.Message;
                return result;
            }
        }


    }
}
