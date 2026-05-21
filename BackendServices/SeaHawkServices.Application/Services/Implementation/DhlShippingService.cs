using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SeaHawkServices.Application.Options;
using SeaHawkServices.Application.Services.Interface;
using SeaHawkServices.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SeaHawkServices.Application.Services.Implementation
{
    public sealed class DhlShippingService : IDhlShippingService
    {
        private readonly HttpClient _http;
        private readonly DhlShipmentOptions _opt;
        private readonly ILogger<DhlShippingService> _log;

        public DhlShippingService(
            HttpClient http,
            IOptions<DhlShipmentOptions> opt,
            ILogger<DhlShippingService> log)
        {
            _http = http;
            _opt = opt.Value;
            _log = log;
        }

        public async Task<DhlShipmentResult> CreateShipmentSampleOrderAsync(
            SampleCollectionRequestVM r,
            CancellationToken ct)
        {
            var result = new DhlShipmentResult();

            string Safe(string? value, string fallback) =>
                string.IsNullOrWhiteSpace(value) ? fallback : value.Trim();

            string SanitizePhone(string? phone, string fallback)
            {
                var digits = new string((phone ?? string.Empty).Where(char.IsDigit).ToArray());
                return string.IsNullOrEmpty(digits) ? fallback : digits;
            }

            string NormalizeCountry(string? value, string defaultIso2)
            {
                if (string.IsNullOrWhiteSpace(value))
                    return defaultIso2;

                var trimmed = value.Trim();

                if (trimmed.Length == 2)
                    return trimmed.ToUpperInvariant();

                if (trimmed.Equals("United States", StringComparison.OrdinalIgnoreCase) ||
                    trimmed.Equals("USA", StringComparison.OrdinalIgnoreCase))
                    return "US";

                if (trimmed.Equals("India", StringComparison.OrdinalIgnoreCase))
                    return "IN";

                if (trimmed.Equals("Canada", StringComparison.OrdinalIgnoreCase))
                    return "CA";

                return defaultIso2;
            }

            // ----------------- basic values -----------------

            var shipperCountryCode = NormalizeCountry(r.Country, "US");
            var recipientCountryCode = NormalizeCountry(r.RecipientCountry, "US");

            // DHL example format: 2022-10-19T19:19:40 GMT+00:00
            var shipDateTime = (r.EarliestPickupDateTime ?? DateTimeOffset.UtcNow)
                .ToString("yyyy-MM-dd'T'HH:mm:ss 'GMT'zzz", CultureInfo.InvariantCulture);

            var declaredValue = r.DeclaredValue > 0 ? r.DeclaredValue : 100m;
            var pkgWeightLb = r.PackageWeightLb > 0 ? r.PackageWeightLb : 70m;
            var pkgWeightKg = Math.Round(pkgWeightLb * 0.453592m, 2);
            var description = Safe(r.CommodityDescription, "Sample collection");

            var senderPhone = SanitizePhone(r.SenderPhone, "0000000000");
            var recipientPhone = SanitizePhone(r.RecipientPhone, "0000000000");

            // ----------------- request payload (MyDHL /shipments) -----------------



            var payload = new
            {
                plannedShippingDateAndTime = shipDateTime,
                pickup = new { isRequested = false },
                productCode = "P",
                localProductCode = "P",
                getRateEstimates = false,
                accounts = new[]
      {
        new { typeCode = "shipper", number = _opt.ShipperAccountNumber }
    },
                outputImageProperties = new
                {
                    printerDPI = 300,
                    encodingFormat = "pdf",
                    imageOptions = new object[]
          {
            new
            {
                typeCode = "invoice",
                templateName = "COMMERCIAL_INVOICE_P_10",
                isRequested = true,
                invoiceType = "commercial",
                languageCode = "eng",
                languageCountryCode = "US"
            },
            new
            {
                typeCode = "waybillDoc",
                templateName = "ARCH_8X4",
                isRequested = true,
                hideAccountNumber = false,
                numberOfCopies = 1
            },
            new
            {
                typeCode = "label",
                templateName = "ECOM26_84_001",
                isRequested = true,
                renderDHLLogo = true,
                fitLabelsToA4 = false
            }
          },
                    splitTransportAndWaybillDocLabels = true,
                    allDocumentsInOneImage = false,
                    splitDocumentsByPages = false,
                    splitInvoiceAndReceipt = true,
                    receiptAndLabelsInOneImage = false
                },
                customerDetails = new
                {
                    shipperDetails = new
                    {
                        postalAddress = new
                        {
                            cityName = Safe(r.City, "CITY"),
                            postalCode = Safe(r.PostCode, "00000"),
                            countryCode = shipperCountryCode,
                            addressLine1 = Safe(r.AddressLine1, "ADDRESS LINE 1")
                        },
                        contactInformation = new
                        {
                            companyName = Safe(r.CompanyName, "Shipper Company"),
                            fullName = Safe(r.SenderName, "Shipper Name"),
                            phone = senderPhone,
                            email = "no-reply@seahawkservices.com"
                        }
                    },
                    receiverDetails = new
                    {
                        postalAddress = new
                        {
                            cityName = Safe(r.RecipientCity, "CITY"),
                            postalCode = Safe(r.RecipientPostalCode, "00000"),
                            countryCode = recipientCountryCode,
                            addressLine1 = Safe(r.RecipientAddressLine1, "RECIPIENT ADDRESS 1")
                        },
                        contactInformation = new
                        {
                            companyName = Safe(r.RecipientCompanyName, "Recipient Company"),
                            fullName = Safe(r.RecipientName, "Recipient Name"),
                            phone = recipientPhone,
                            email = "no-reply@seahawkservices.com"
                        }
                    }
                },
                content = new
                {
                    packages = new[]
    {
        new
        {
            typeCode = "2BP",
            weight = pkgWeightKg,
            dimensions = new
            {
                length = 10,
                width = 10,
                height = 10
            },
            customerReferences = new[]
            {
                new
                {
                    value = "SampleCollection",
                    typeCode = "CU"
                }
            },
            description = description,
            labelDescription = description
        }
    },
                    isCustomsDeclarable = true,
                    declaredValue = declaredValue,
                    declaredValueCurrency = "USD",
                    exportDeclaration = new
                    {
                        lineItems = new[]
        {
            new
            {
                number = 1,
                description = description,
                price = declaredValue,
                quantity = new
                {
                    value = 1,
                    unitOfMeasurement = "PCS"
                },
                exportReasonType = "permanent",
                manufacturerCountry = shipperCountryCode,
                weight = new
                {
                    netValue = pkgWeightKg,
                    grossValue = pkgWeightKg
                }
            }
        },
                        invoice = new
                        {
                            number = $"INV-{DateTime.UtcNow:yyyyMMddHHmmss}",
                            date = DateTime.UtcNow.ToString("yyyy-MM-dd"),
                            totalNetWeight = pkgWeightKg,
                            totalGrossWeight = pkgWeightKg
                        }
                    },
                    description = description,
                    incoterm = "DAP",
                    unitOfMeasurement = "metric"
                }
        };

            var json = System.Text.Json.JsonSerializer.Serialize(
                payload,
                new JsonSerializerOptions
                {
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
                });

            var baseUrl = _opt.BaseUrl?.TrimEnd('/') ?? "https://express.api.dhl.com/mydhlapi/test";
            var url = $"{baseUrl}/shipments";

            using var request = new HttpRequestMessage(HttpMethod.Post, url);

            // Headers from Swagger (Message-Reference, x-version, etc.)
            var prefix = (_opt.MessageReferencePrefix ?? "DDTS").Trim();
            if (prefix.Length > 4) prefix = prefix.Substring(0, 4);
            if (prefix.Length < 4) prefix = prefix.PadRight(4, 'X');

            var msgRef = $"{prefix}{Guid.NewGuid():N}"; // exactly 36 chars
            request.Headers.Add("Message-Reference", msgRef);
            request.Headers.Add("Message-Reference-Date", DateTimeOffset.UtcNow.ToString("r"));
            request.Headers.Add("x-version", "3.1.0");

            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            // Basic auth: username:password -> base64
            var authBytes = Encoding.UTF8.GetBytes($"{_opt.Username}:{_opt.Password}");
            var authBase64 = Convert.ToBase64String(authBytes);
            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", authBase64);
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");

            _log.LogInformation("DHL Create Shipment JSON: {Json}", json);

            using var response = await _http.SendAsync(request, ct);
            var body = await response.Content.ReadAsStringAsync(ct);

            _log.LogInformation("DHL Create Shipment response ({Status}): {Body}",
                response.StatusCode, body);

            if (!response.IsSuccessStatusCode)
            {
                result.Success = false;
                result.Error =
                    $"DHL Create Shipment failed ({(int)response.StatusCode} {response.StatusCode}): {body}";
                return result;
            }

            using var doc = JsonDocument.Parse(body);
            var root = doc.RootElement;

            result.Success = true;
            if (root.TryGetProperty("shipmentTrackingNumber", out var trackingEl))
                result.TrackingNumber = trackingEl.GetString();

            if (root.TryGetProperty("dispatchConfirmationNumber", out var apiRefEl))
                result.ApiReference = apiRefEl.GetString() ?? result.TrackingNumber;
            else
                result.ApiReference = result.TrackingNumber;

            // Root-level documents array: label / invoice etc.
            if (root.TryGetProperty("documents", out var docsElement) &&
                docsElement.ValueKind == JsonValueKind.Array)
            {
                foreach (var d in docsElement.EnumerateArray())
                {
                    var typeCode = d.TryGetProperty("typeCode", out var typeEl) && typeEl.ValueKind == JsonValueKind.String
    ? typeEl.GetString()
    : null;

                    var content = d.TryGetProperty("content", out var contentEl) && contentEl.ValueKind == JsonValueKind.String
                        ? contentEl.GetString()
                        : null;

                    if (string.IsNullOrWhiteSpace(typeCode) || string.IsNullOrWhiteSpace(content))
                        continue;
                    var t = typeCode.ToLowerInvariant();

                    if ((t.Contains("label") || t.Contains("waybill")) && result.LabelBase64 == null)
                    {
                        result.LabelBase64 = content;
                    }
                    else if (t.Contains("invoice") && result.InvoiceBase64 == null)
                    {
                        result.InvoiceBase64 = content;
                    }
                }
            }

            return result;
        }

        public async Task<AddressValidationResponseDto> ValidateAddressAsync(AddressValidationRequestDto dto, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(dto.CountryCode))
                return new AddressValidationResponseDto { IsValid = false, Message = "Country code is required." };


            var type = "pickup"; // or "delivery"

            var qs = new List<string>
    {
        $"type={Uri.EscapeDataString(type)}",
        $"countryCode={Uri.EscapeDataString(dto.CountryCode)}",
        "strictValidation=true"
    };

            if (!string.IsNullOrWhiteSpace(dto.PostalCode))
                qs.Add($"postalCode={Uri.EscapeDataString(dto.PostalCode)}");

            if (!string.IsNullOrWhiteSpace(dto.City))
                qs.Add($"cityName={Uri.EscapeDataString(dto.City)}");

            if (!string.IsNullOrWhiteSpace(dto.StateOrProvinceCode))
                qs.Add($"countyName={Uri.EscapeDataString(dto.StateOrProvinceCode)}");

            var url = $"{_opt.BaseUrl.TrimEnd('/')}/address-validate?{string.Join("&", qs)}";

            var req = new HttpRequestMessage(HttpMethod.Get, url);

            // ✅ Message-Reference max 36 chars
            // Ensure prefix is 4 chars (DDTS) => 4 + 32 = 36
            var prefix = (_opt.MessageReferencePrefix ?? "DDTS").Trim();
            if (prefix.Length > 4) prefix = prefix.Substring(0, 4);
            if (prefix.Length < 4) prefix = prefix.PadRight(4, 'X');

            var msgRef = $"{prefix}{Guid.NewGuid():N}";
            req.Headers.Add("Message-Reference", msgRef);

            req.Headers.Add("Message-Reference-Date", DateTimeOffset.UtcNow.ToString("r"));
            req.Headers.Add("x-version", "3.2.0");

            // Basic Auth (API key : API secret)
            var basic = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_opt.Username}:{_opt.Password}"));
            req.Headers.Authorization = new AuthenticationHeaderValue("Basic", basic);

            var res = await _http.SendAsync(req, ct);
            var json = await res.Content.ReadAsStringAsync(ct);

            if (!res.IsSuccessStatusCode)
            {
                return new AddressValidationResponseDto
                {
                    IsValid = false,
                    Message = "Unable to validate address with DHL."
                };
            }

            dynamic obj = JsonConvert.DeserializeObject(json);
            var addrArray = obj?.address;

            bool hasMatch = addrArray != null && addrArray.HasValues;
            if (!hasMatch)
                return new AddressValidationResponseDto { IsValid = false, Message = "No pickup/delivery capability found for this location." };

            var first = addrArray[0];

            return new AddressValidationResponseDto
            {
                IsValid = true,
                Message = "Address is valid.",
                SuggestedAddress = new AddressValidationRequestDto
                {
                    CountryCode = (string?)first.countryCode ?? dto.CountryCode,
                    PostalCode = (string?)first.postalCode ?? dto.PostalCode,
                    City = (string?)first.cityName ?? dto.City
                }
            };
        }



        public async Task<string> GetAccessTokenAsync(System.Threading.CancellationToken ct)
        {
            var tokenUrl = $"{_opt.BaseUrl.TrimEnd('/')}/oauth/token";

            var form = new Dictionary<string, string>
            {
                ["grant_type"] = "client_credentials",
                ["client_id"] = _opt.Username,
                ["client_secret"] = _opt.Password
            };

            var resp = await _http.PostAsync(tokenUrl, new FormUrlEncodedContent(form), ct);
            var body = await resp.Content.ReadAsStringAsync(ct);

            resp.EnsureSuccessStatusCode();

            using var doc = JsonDocument.Parse(body);
            return doc.RootElement.GetProperty("access_token").GetString();
        }
    }
}

