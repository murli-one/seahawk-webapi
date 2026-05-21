using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SeaHawkServices.Application.Services.Interface;
using SeaHawkServices.Domain.Entities;
using SeaHawkServices.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SeaHawkServices.Application.Services.Implementation
{
    public class FedExShippingService : IFedExShippingService
    {
        private readonly HttpClient _http;
        private readonly FedExRestSettings _settings;
        private readonly ILogger<FedExShippingService> _log;

        public FedExShippingService(
            HttpClient http,
            IOptions<FedExRestSettings> options,
            ILogger<FedExShippingService> log)
        {
            _http = http;
            _settings = options.Value;
            _log = log;
        }

        //public async Task<FedExShipmentResult> CreateShipmentAsync(
        //    SampleCollectionRequestVM r,
        //    System.Threading.CancellationToken ct)
        //{
        //    var result = new FedExShipmentResult();

        //    // 1) OAuth
        //    var token = await GetAccessTokenAsync(ct);

        //    // 2) Endpoint
        //    var shipUrl = $"{_settings.BaseUrl.TrimEnd('/')}/ship/v1/shipments";

        //    // Helpers
        //    string Safe(string? value, string fallback) =>
        //        string.IsNullOrWhiteSpace(value) ? fallback : value;

        //    string NormalizeCountry(string? value, string defaultIso2)
        //    {
        //        if (string.IsNullOrWhiteSpace(value))
        //            return defaultIso2;

        //        var trimmed = value.Trim();

        //        // if already 2 letters, assume valid ISO code
        //        if (trimmed.Length == 2)
        //            return trimmed.ToUpperInvariant();

        //        if (trimmed.Equals("India", StringComparison.OrdinalIgnoreCase))
        //            return "IN";

        //        if (trimmed.Equals("United States", StringComparison.OrdinalIgnoreCase) ||
        //            trimmed.Equals("USA", StringComparison.OrdinalIgnoreCase))
        //            return "US";

        //        if (trimmed.Equals("Canada", StringComparison.OrdinalIgnoreCase))
        //            return "CA";

        //        // fallback to default
        //        return defaultIso2;
        //    }

        //    string NormalizeState(string? value, string defaultCode)
        //    {
        //        if (string.IsNullOrWhiteSpace(value))
        //            return defaultCode;

        //        var trimmed = value.Trim().ToUpperInvariant();
        //        // FedEx expects short codes (TN, MH, etc.)
        //        if (trimmed.Length <= 3)
        //            return trimmed;

        //        return defaultCode;
        //    }

        //    // Ship date (use EarliestPickup or today)
        //    var shipDate = (r.EarliestPickupDateTime ?? DateTime.UtcNow)
        //        .ToString("yyyy-MM-dd");

        //    // Money / weight / description from VM with defaults
        //    var declaredValue = r.DeclaredValue > 0 ? r.DeclaredValue : 100m;
        //    var pkgWeightLb = r.PackageWeightLb > 0 ? r.PackageWeightLb : 70m;
        //    var description = Safe(r.CommodityDescription, "Commodity description");

        //    // Phone: numeric only, fallback if blank
        //    var senderPhone = Safe(r.SenderPhone, "1234567890");
        //    senderPhone = new string(senderPhone.Where(char.IsDigit).ToArray());
        //    if (string.IsNullOrEmpty(senderPhone))
        //        senderPhone = "1234567890";

        //    // Normalize country/state for FedEx
        //    var countryCode = NormalizeCountry(r.Country, "US");
        //    var stateCode = NormalizeState(r.State, "TN");

        //    var payload = new
        //    {
        //        labelResponseOptions = "URL_ONLY",

        //        accountNumber = new
        //        {
        //            value = _settings.AccountNumber // "740561073"
        //        },

        //        requestedShipment = new
        //        {
        //            shipDatestamp = shipDate,
        //            serviceType = "INTERNATIONAL_PRIORITY",
        //            packagingType = "YOUR_PACKAGING",
        //            pickupType = "USE_SCHEDULED_PICKUP",
        //            blockInsightVisibility = false,

        //            // ===== SHIPPER =====
        //            shipper = new
        //            {
        //                contact = new
        //                {
        //                    personName = Safe(r.SenderName, "SHIPPER NAME"),
        //                    phoneNumber = senderPhone,
        //                    companyName = Safe(r.CompanyName, "Shipper Company Name")
        //                },
        //                address = new
        //                {
        //                    streetLines = new[]
        //                    {
        //                        Safe(r.AddressLine1, "10 FedEx Parkway"),
        //                        Safe(r.AddressLine2, "Suite 302")
        //                    }.Where(x => !string.IsNullOrWhiteSpace(x)).ToArray(),
        //                    city = Safe(r.City, "Memphis"),
        //                    stateOrProvinceCode = stateCode,
        //                    postalCode = Safe(r.PostCode, "38116"),
        //                    countryCode = countryCode
        //                }
        //            },

        //            // ===== RECIPIENT (Lab – CA) =====
        //            recipients = new[]
        //            {
        //                new
        //                {
        //                    contact = new
        //                    {
        //                        personName  = "RECIPIENT NAME",
        //                        phoneNumber = "1234567890",
        //                        companyName = "Recipient Company Name"
        //                    },
        //                    address = new
        //                    {
        //                        streetLines = new[]
        //                        {
        //                            "RECIPIENT STREET LINE 1",
        //                            "RECIPIENT STREET LINE 2",
        //                            "RECIPIENT STREET LINE 3"
        //                        },
        //                        city                = "RICHMOND",
        //                        stateOrProvinceCode = "BC",
        //                        postalCode          = "V7C4V7",
        //                        countryCode         = "CA"
        //                    }
        //                }
        //            },

        //            shippingChargesPayment = new
        //            {
        //                paymentType = "SENDER"
        //            },

        //            labelSpecification = new
        //            {
        //                imageType = "PDF",
        //                labelStockType = "PAPER_85X11_TOP_HALF_LABEL"
        //            },

        //            customsClearanceDetail = new
        //            {
        //                dutiesPayment = new
        //                {
        //                    paymentType = "SENDER"
        //                },
        //                isDocumentOnly = true,
        //                commodities = new[]
        //                {
        //                    new
        //                    {
        //                        description          = description,
        //                        countryOfManufacture = countryCode, // normalized
        //                        quantity             = 1,
        //                        quantityUnits        = "PCS",
        //                        unitPrice = new
        //                        {
        //                            amount   = declaredValue,
        //                            currency = "USD"
        //                        },
        //                        customsValue = new
        //                        {
        //                            amount   = declaredValue,
        //                            currency = "USD"
        //                        },
        //                        weight = new
        //                        {
        //                            units = "LB",
        //                            value = pkgWeightLb
        //                        }
        //                    }
        //                }
        //            },

        //            shippingDocumentSpecification = new
        //            {
        //                shippingDocumentTypes = new[] { "COMMERCIAL_INVOICE" },
        //                commercialInvoiceDetail = new
        //                {
        //                    documentFormat = new
        //                    {
        //                        stockType = "PAPER_LETTER",
        //                        docType = "PDF"
        //                    }
        //                }
        //            },

        //            requestedPackageLineItems = new[]
        //            {
        //                new
        //                {
        //                    weight = new
        //                    {
        //                        units = "LB",
        //                        value = pkgWeightLb
        //                    }
        //                }
        //            }
        //        }
        //    };

        //    var json = JsonSerializer.Serialize(
        //        payload,
        //        new JsonSerializerOptions
        //        {
        //            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        //        });

        //    var req = new HttpRequestMessage(HttpMethod.Post, shipUrl);
        //    req.Headers.Authorization =
        //        new AuthenticationHeaderValue("Bearer", token);
        //    req.Headers.Accept.Add(
        //        new MediaTypeWithQualityHeaderValue("application/json"));
        //    req.Content = new StringContent(json, Encoding.UTF8, "application/json");

        //    _log.LogInformation("FedEx Ship JSON: {Json}", json);

        //    var resp = await _http.SendAsync(req, ct);
        //    var body = await resp.Content.ReadAsStringAsync(ct);

        //    _log.LogInformation("FedEx Ship response ({Status}): {Body}",
        //        resp.StatusCode, body);

        //    if (!resp.IsSuccessStatusCode)
        //    {
        //        result.Success = false;
        //        result.Error =
        //            $"FedEx Ship failed ({(int)resp.StatusCode} {resp.StatusCode}): {body}";
        //        return result;
        //    }

        //    // 4) Parse response (matches sample you pasted)
        //    using var doc = JsonDocument.Parse(body);
        //    var root = doc.RootElement;
        //    var output = root.GetProperty("output");
        //    var shipment = output.GetProperty("transactionShipments")[0];

        //    var masterTracking = shipment
        //        .GetProperty("masterTrackingNumber")
        //        .GetString();

        //    var docsArray = shipment
        //        .GetProperty("shipmentDocuments")
        //        .EnumerateArray();

        //    string? mergedLabelUrl = null;
        //    string? ciUrl = null;

        //    foreach (var d in docsArray)
        //    {
        //        var contentType = d.GetProperty("contentType").GetString();
        //        var url = d.GetProperty("url").GetString();

        //        if (contentType == "MERGED_LABEL_DOCUMENTS")
        //            mergedLabelUrl = url;
        //        else if (contentType == "COMMERCIAL_INVOICE")
        //            ciUrl = url;
        //    }

        //    // store the raw FedEx URLs here:
        //    result.FedExLabelUrl = mergedLabelUrl ?? ciUrl;
        //    result.FedExInvoiceUrl = ciUrl;

        //    result.Success = true;
        //    result.TrackingNumber = masterTracking;
        //    result.LabelUrl = mergedLabelUrl ?? ciUrl;
        //    result.CommercialInvoiceUrl = ciUrl;

        //    return result;
        //}



        // Sample Order
        public async Task<FedExShipmentResult> CreateShipmentSampleOrderAsync(
     SampleCollectionRequestVM r,
     CancellationToken ct)
        {
            var result = new FedExShipmentResult();

            var token = await GetAccessTokenAsync(ct);
            var shipUrl = $"{_settings.BaseUrl.TrimEnd('/')}/ship/v1/shipments";

            string Safe(string? value, string fallback) =>
                string.IsNullOrWhiteSpace(value) ? fallback : value.Trim();

            string NormalizeCountry(string? value, string defaultIso2)
            {
                if (string.IsNullOrWhiteSpace(value)) return defaultIso2;
                var trimmed = value.Trim();

                if (trimmed.Length == 2) return trimmed.ToUpperInvariant();

                if (trimmed.Equals("India", StringComparison.OrdinalIgnoreCase)) return "IN";

                if (trimmed.Equals("United States", StringComparison.OrdinalIgnoreCase) ||
                    trimmed.Equals("USA", StringComparison.OrdinalIgnoreCase) ||
                    trimmed.Equals("UnitedStates", StringComparison.OrdinalIgnoreCase))
                    return "US";

                if (trimmed.Equals("Canada", StringComparison.OrdinalIgnoreCase)) return "CA";

                if (trimmed.Equals("United Kingdom", StringComparison.OrdinalIgnoreCase) ||
                    trimmed.Equals("UK", StringComparison.OrdinalIgnoreCase) ||
                    trimmed.Equals("Great Britain", StringComparison.OrdinalIgnoreCase) ||
                    trimmed.Equals("UnitedKingdom", StringComparison.OrdinalIgnoreCase))
                    return "GB";

                return defaultIso2;
            }

            string NormalizeState(string? value, string defaultCode)
            {
                if (string.IsNullOrWhiteSpace(value)) return defaultCode;
                var trimmed = value.Trim().ToUpperInvariant();
                return trimmed.Length <= 3 ? trimmed : defaultCode;
            }

            var shipDate = (r.EarliestPickupDateTime ?? DateTime.UtcNow).ToString("yyyy-MM-dd");

            var declaredValue = r.DeclaredValue > 0 ? r.DeclaredValue : 100m;
            var pkgWeightLb = r.PackageWeightLb > 0 ? r.PackageWeightLb : 1m;
            var description = Safe(r.CommodityDescription, "Sampling kit for fuel / oil condition monitoring");

            var senderPhone = new string(Safe(r.SenderPhone, "1234567890").Where(char.IsDigit).ToArray());
            if (string.IsNullOrEmpty(senderPhone)) senderPhone = "1234567890";

            var recipientPhone = new string(Safe(r.RecipientPhone, "1234567890").Where(char.IsDigit).ToArray());
            if (string.IsNullOrEmpty(recipientPhone)) recipientPhone = "1234567890";

            var shipperCountryCode = NormalizeCountry(r.Country, "US");
            var shipperStateCode = NormalizeState(r.State, "TN");

            if (string.Equals(shipperCountryCode, "GB", StringComparison.OrdinalIgnoreCase))
                shipperStateCode = "";

            var recipientCountryCode = NormalizeCountry(r.RecipientCountry, "CA");
            var recipientStateCode = NormalizeState(r.RecipientState, "BC");

            var isInternational =
                !string.Equals(shipperCountryCode, recipientCountryCode, StringComparison.OrdinalIgnoreCase);

            var serviceType = ResolveFedExServiceType(r.ServiceType, shipperCountryCode, recipientCountryCode);

            var pickupType = Safe(r.PickupType, "DROPOFF_AT_FEDEX_LOCATION");
            var packagingType = Safe(r.PackagingType, "YOUR_PACKAGING");
            var paymentType = Safe(r.ShippingPaymentType, "SENDER");
            var labelStockType = Safe(r.LabelStockType, "PAPER_85X11_TOP_HALF_LABEL");

            var accountNumber = Safe(r.FedExAccountNumberOverride, _settings.AccountNumber);

            object shippingChargesPayment =
                string.Equals(paymentType, "SENDER", StringComparison.OrdinalIgnoreCase)
                    ? new { paymentType = "SENDER" }
                    : new
                    {
                        paymentType = paymentType,
                        payor = new
                        {
                            responsibleParty = new
                            {
                                accountNumber = new { value = Safe(r.PayerAccountNumber, accountNumber) }
                            }
                        }
                    };

            var payload = new
            {
                labelResponseOptions = "LABEL",
                requestedShipment = new
                {
                    shipDatestamp = shipDate,
                    serviceType = serviceType,
                    packagingType = packagingType,
                    pickupType = pickupType,
                    blockInsightVisibility = false,

                    shipper = new
                    {
                        contact = new
                        {
                            personName = Safe(r.SenderName, "SHIPPER NAME"),
                            phoneNumber = senderPhone,
                            companyName = Safe(r.CompanyName, "Shipper Company Name")
                        },
                        address = new
                        {
                            streetLines = new[]
                            {
                        Safe(r.AddressLine1, "10 FedEx Parkway"),
                        r.AddressLine2,
                        r.AddressLine3
                    }.Where(x => !string.IsNullOrWhiteSpace(x)).ToArray(),
                            city = Safe(r.City, "Memphis"),
                            stateOrProvinceCode = string.IsNullOrWhiteSpace(shipperStateCode) ? null : shipperStateCode,
                            postalCode = Safe(r.PostCode, "38116"),
                            countryCode = shipperCountryCode
                        }
                    },

                    recipients = new[]
                    {
                new
                {
                    contact = new
                    {
                        personName = Safe(r.RecipientName, "RECIPIENT NAME"),
                        phoneNumber = recipientPhone,
                        companyName = Safe(r.RecipientCompanyName, "Recipient Company Name")
                    },
                    address = new
                    {
                        streetLines = new[]
                        {
                            Safe(r.RecipientAddressLine1, "RECIPIENT STREET LINE 1"),
                            r.RecipientAddressLine2,
                            r.RecipientAddressLine3
                        }.Where(x => !string.IsNullOrWhiteSpace(x)).ToArray(),
                        city = Safe(r.RecipientCity, "RICHMOND"),
                        stateOrProvinceCode = recipientStateCode,
                        postalCode = Safe(r.RecipientPostalCode, "V7C4V7"),
                        countryCode = recipientCountryCode
                    }
                }
            },

                    shippingChargesPayment = shippingChargesPayment,

                    labelSpecification = new
                    {
                        labelFormatType = "COMMON2D",
                        imageType = "PDF",
                        labelStockType = labelStockType
                    },

                    shipmentSpecialServices = isInternational
                        ? new
                        {
                            specialServiceTypes = new[] { "ELECTRONIC_TRADE_DOCUMENTS" },
                            etdDetail = new
                            {
                                requestedDocumentTypes = new[] { "COMMERCIAL_INVOICE" }
                            }
                        }
                        : null,

                    shippingDocumentSpecification = isInternational
                        ? new
                        {
                            shippingDocumentTypes = new[] { "COMMERCIAL_INVOICE" },
                            commercialInvoiceDetail = new
                            {
                                documentFormat = new
                                {
                                    stockType = "PAPER_LETTER",
                                    docType = "PDF"
                                }
                            }
                        }
                        : null,

                    customsClearanceDetail = isInternational
                        ? new
                        {
                            dutiesPayment = new { paymentType = "SENDER" },
                            isDocumentOnly = false,
                            commercialInvoice = new
                            {
                                shipmentPurpose = "SOLD",
                                declarationStatement = "Samples for laboratory testing",
                                termsOfSale = "FCA"
                            },
                            totalCustomsValue = new
                            {
                                amount = declaredValue,
                                currency = "USD"
                            },
                            commodities = new[]
                            {
                        new
                        {
                            description = description,
                            countryOfManufacture = shipperCountryCode,
                            quantity = 1,
                            quantityUnits = "PCS",
                            unitPrice = new { amount = declaredValue, currency = "USD" },
                            customsValue = new { amount = declaredValue, currency = "USD" },
                            weight = new { units = "LB", value = pkgWeightLb }
                        }
                            }
                        }
                        : null,

                    requestedPackageLineItems = new[]
                    {
                new
                {
                    weight = new
                    {
                        units = "LB",
                        value = pkgWeightLb
                    }
                }
            }
                },
                accountNumber = new { value = accountNumber }
            };

            var json = System.Text.Json.JsonSerializer.Serialize(
                payload,
                new JsonSerializerOptions
                {
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
                });

            var httpReq = new HttpRequestMessage(HttpMethod.Post, shipUrl);
            httpReq.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            httpReq.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpReq.Headers.Add("X-locale", "en_US");
            httpReq.Content = new StringContent(json, Encoding.UTF8, "application/json");

            _log.LogInformation("FedEx Ship JSON: {Json}", json);

            var resp = await _http.SendAsync(httpReq, ct);
            var body = await resp.Content.ReadAsStringAsync(ct);

            _log.LogInformation("FedEx Ship response ({Status}): {Body}", resp.StatusCode, body);

            if (!resp.IsSuccessStatusCode)
            {
                result.Success = false;
                result.Error = $"FedEx Ship failed ({(int)resp.StatusCode} {resp.StatusCode}): {body}";
                return result;
            }

            using var doc = JsonDocument.Parse(body);
            var root = doc.RootElement;

            if (!root.TryGetProperty("output", out var output))
            {
                result.Success = false;
                result.Error = $"FedEx success response but missing 'output'. Raw: {body}";
                return result;
            }

            if (!output.TryGetProperty("transactionShipments", out var txShipments) ||
                txShipments.ValueKind != JsonValueKind.Array ||
                txShipments.GetArrayLength() == 0)
            {
                result.Success = false;
                result.Error = $"FedEx success response but missing 'transactionShipments'. Raw: {body}";
                return result;
            }

            var shipment = txShipments[0];

            string? trackingNumber = null;

            if (shipment.TryGetProperty("masterTrackingNumber", out var masterTrk) &&
                masterTrk.ValueKind == JsonValueKind.String)
            {
                trackingNumber = masterTrk.GetString();
            }
            else if (shipment.TryGetProperty("pieceResponses", out var pieceResponses) &&
                     pieceResponses.ValueKind == JsonValueKind.Array &&
                     pieceResponses.GetArrayLength() > 0 &&
                     pieceResponses[0].TryGetProperty("trackingNumber", out var pieceTrk) &&
                     pieceTrk.ValueKind == JsonValueKind.String)
            {
                trackingNumber = pieceTrk.GetString();
            }

            result.Success = true;
            result.TrackingNumber = trackingNumber;

            string? labelUrl = null;
            string? invoiceUrl = null;
            string? labelBase64 = null;
            string? invoiceBase64 = null;

            static string? GetStr(JsonElement obj, string name) =>
                obj.TryGetProperty(name, out var v) && v.ValueKind == JsonValueKind.String
                    ? v.GetString()
                    : null;

            static string? GetEncoded(JsonElement obj)
            {
                if (obj.TryGetProperty("encodedLabel", out var a) && a.ValueKind == JsonValueKind.String)
                    return a.GetString();

                if (obj.TryGetProperty("contents", out var b) && b.ValueKind == JsonValueKind.String)
                    return b.GetString();

                if (obj.TryGetProperty("content", out var c) && c.ValueKind == JsonValueKind.String)
                    return c.GetString();

                return null;
            }

            if (shipment.TryGetProperty("shipmentDocuments", out var shipmentDocs) &&
                shipmentDocs.ValueKind == JsonValueKind.Array)
            {
                foreach (var d in shipmentDocs.EnumerateArray())
                {
                    var contentType = GetStr(d, "contentType") ?? "";
                    var url = GetStr(d, "url");
                    var encoded = GetEncoded(d);

                    if (contentType.Equals("COMMERCIAL_INVOICE", StringComparison.OrdinalIgnoreCase))
                    {
                        invoiceUrl ??= url;
                        invoiceBase64 ??= encoded;
                    }

                    if (contentType.Equals("MERGED_LABEL_DOCUMENTS", StringComparison.OrdinalIgnoreCase) ||
                        contentType.Equals("LABEL", StringComparison.OrdinalIgnoreCase))
                    {
                        labelUrl ??= url;
                        labelBase64 ??= encoded;
                    }
                }
            }

            if (shipment.TryGetProperty("pieceResponses", out var pieces) &&
                pieces.ValueKind == JsonValueKind.Array)
            {
                foreach (var piece in pieces.EnumerateArray())
                {
                    if (!piece.TryGetProperty("packageDocuments", out var pkgDocs) ||
                        pkgDocs.ValueKind != JsonValueKind.Array)
                        continue;

                    foreach (var d in pkgDocs.EnumerateArray())
                    {
                        var contentType = GetStr(d, "contentType") ?? "";
                        var url = GetStr(d, "url");
                        var encoded = GetEncoded(d);

                        if (contentType.Equals("LABEL", StringComparison.OrdinalIgnoreCase) ||
                            contentType.Equals("MERGED_LABEL_DOCUMENTS", StringComparison.OrdinalIgnoreCase))
                        {
                            labelUrl ??= url;
                            labelBase64 ??= encoded;
                        }

                        if (contentType.Equals("COMMERCIAL_INVOICE", StringComparison.OrdinalIgnoreCase))
                        {
                            invoiceUrl ??= url;
                            invoiceBase64 ??= encoded;
                        }
                    }
                }
            }

            result.FedExLabelUrl = labelUrl;
            result.FedExInvoiceUrl = invoiceUrl;
            result.LabelUrl = labelUrl;
            result.CommercialInvoiceUrl = invoiceUrl;
            result.LabelBase64 = labelBase64;
            result.InvoiceBase64 = invoiceBase64;

            return result;
        }


        // Sample Order Ended 

        // Sample OrderKit 
        public async Task<FedExShipmentResult> CreateShipmentSampleKitOrderAsync(
       SampleCollectionRequestVM r,
       System.Threading.CancellationToken ct)
        {
            var result = new FedExShipmentResult();

            // 1) OAuth
            var token = await GetAccessTokenAsync(ct);

            // 2) Endpoint
            var shipUrl = $"{_settings.BaseUrl.TrimEnd('/')}/ship/v1/shipments";

            // Helpers
            string Safe(string? value, string fallback) =>
                string.IsNullOrWhiteSpace(value) ? fallback : value;

            string NormalizeCountry(string? value, string defaultIso2)
            {
                if (string.IsNullOrWhiteSpace(value))
                    return defaultIso2;

                var trimmed = value.Trim();

                // if already 2 letters, assume valid ISO code
                if (trimmed.Length == 2)
                    return trimmed.ToUpperInvariant();

                if (trimmed.Equals("India", StringComparison.OrdinalIgnoreCase))
                    return "IN";

                if (trimmed.Equals("United States", StringComparison.OrdinalIgnoreCase) ||
                    trimmed.Equals("USA", StringComparison.OrdinalIgnoreCase))
                    return "US";

                if (trimmed.Equals("Canada", StringComparison.OrdinalIgnoreCase))
                    return "CA";

                // fallback to default
                return defaultIso2;
            }

            string NormalizeState(string? value, string defaultCode)
            {
                if (string.IsNullOrWhiteSpace(value))
                    return defaultCode;

                var trimmed = value.Trim().ToUpperInvariant();
                // FedEx expects short codes (TN, MH, etc.)
                if (trimmed.Length <= 3)
                    return trimmed;

                return defaultCode;
            }

            // Ship date (use now)
            var shipDate = DateTime.UtcNow.ToString("yyyy-MM-dd");

            // Money / weight / description from VM with defaults
            var declaredValue = r.DeclaredValue > 0 ? r.DeclaredValue : 100m;
            var pkgWeightLb = r.PackageWeightLb > 0 ? r.PackageWeightLb : 70m;
            var description = Safe(r.CommodityDescription, "Commodity description");

            // Phone: numeric only, fallback if blank
            var senderPhone = Safe(r.SenderPhone, "1234567890");
            senderPhone = new string(senderPhone.Where(char.IsDigit).ToArray());
            if (string.IsNullOrEmpty(senderPhone))
                senderPhone = "1234567890";

            var recipientPhone = Safe(r.RecipientPhone, "1234567890");
            recipientPhone = new string(recipientPhone.Where(char.IsDigit).ToArray());
            if (string.IsNullOrEmpty(recipientPhone))
                recipientPhone = "1234567890";

            // Normalize country/state for FedEx – shipper + recipient
            var countryCode = NormalizeCountry(r.Country, "US");          // shipper (your office – USA)
            var stateCode = NormalizeState(r.State, "TN");

            var recipientCountryCode = NormalizeCountry(r.RecipientCountry, "CA");
            var recipientStateCode = NormalizeState(r.RecipientState, "BC");

            // 🇺🇸 Check if this is a US-domestic shipment (USA -> USA)
            var isDomesticUs =
    string.Equals(countryCode, "US", StringComparison.OrdinalIgnoreCase) &&
    string.Equals(recipientCountryCode, "US", StringComparison.OrdinalIgnoreCase);

            var serviceType = isDomesticUs ? "PRIORITY_OVERNIGHT" : "INTERNATIONAL_PRIORITY";

            var payload = new
            {
                labelResponseOptions = "URL_ONLY",
                accountNumber = new
                {
                    value = _settings.AccountNumber
                },
                requestedShipment = new
                {
                    shipDatestamp = shipDate,
                    serviceType = serviceType,
                    packagingType = "YOUR_PACKAGING",
                    pickupType = "USE_SCHEDULED_PICKUP",
                    blockInsightVisibility = false,

                    shipper = new
                    {
                        contact = new
                        {
                            personName = Safe(r.SenderName, "SHIPPER NAME"),
                            phoneNumber = senderPhone,
                            companyName = Safe(r.CompanyName, "Shipper Company Name")
                        },
                        address = new
                        {
                            streetLines = new[]
                            {
                    Safe(r.AddressLine1, "10 FedEx Parkway"),
                    r.AddressLine2
                }.Where(x => !string.IsNullOrWhiteSpace(x)).ToArray(),
                            city = Safe(r.City, "Memphis"),
                            stateOrProvinceCode = stateCode,
                            postalCode = Safe(r.PostCode, "38116"),
                            countryCode = countryCode
                        }
                    },

                    recipients = new[]
                    {
            new
            {
                contact = new
                {
                    personName = Safe(r.RecipientName, "RECIPIENT NAME"),
                    phoneNumber = recipientPhone,
                    companyName = Safe(r.RecipientCompanyName, "Recipient Company Name")
                },
                address = new
                {
                    streetLines = new[]
                    {
                        Safe(r.RecipientAddressLine1, "RECIPIENT STREET LINE 1"),
                        r.RecipientAddressLine2,
                        r.RecipientAddressLine3
                    }.Where(x => !string.IsNullOrWhiteSpace(x)).ToArray(),
                    city = Safe(r.RecipientCity, "RICHMOND"),
                    stateOrProvinceCode = recipientStateCode,
                    postalCode = Safe(r.RecipientPostalCode, "V7C4V7"),
                    countryCode = recipientCountryCode
                }
            }
        },

                    shippingChargesPayment = new
                    {
                        paymentType = "SENDER"
                    },

                    labelSpecification = new
                    {
                        imageType = "PDF",
                        labelStockType = "PAPER_85X11_TOP_HALF_LABEL"
                    },

                    customsClearanceDetail = isDomesticUs ? null : new
                    {
                        dutiesPayment = new
                        {
                            paymentType = "SENDER"
                        },
                        isDocumentOnly = false,
                        commercialInvoice = new
                        {
                            shipmentPurpose = "SOLD",
                            declarationStatement = "Samples for laboratory testing",
                            termsOfSale = "FCA"
                        },
                        totalCustomsValue = new
                        {
                            amount = declaredValue,
                            currency = "USD"
                        },
                        commodities = new[]
                        {
                new
                {
                    description = description,
                    countryOfManufacture = countryCode,
                    quantity = 1,
                    quantityUnits = "PCS",
                    unitPrice = new
                    {
                        amount = declaredValue,
                        currency = "USD"
                    },
                    customsValue = new
                    {
                        amount = declaredValue,
                        currency = "USD"
                    },
                    weight = new
                    {
                        units = "LB",
                        value = pkgWeightLb
                    }
                }
            }
                    },

                    shippingDocumentSpecification = isDomesticUs ? null : new
                    {
                        shippingDocumentTypes = new[] { "COMMERCIAL_INVOICE" },
                        commercialInvoiceDetail = new
                        {
                            documentFormat = new
                            {
                                stockType = "PAPER_LETTER",
                                docType = "PDF"
                            }
                        }
                    },

                    requestedPackageLineItems = new[]
                    {
            new
            {
                weight = new
                {
                    units = "LB",
                    value = pkgWeightLb
                }
            }
        }
                }
            };

            var json = System.Text.Json.JsonSerializer.Serialize(
                payload,
                new JsonSerializerOptions { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull });

            var reqMsg = new HttpRequestMessage(HttpMethod.Post, shipUrl);
            reqMsg.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
            reqMsg.Headers.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            reqMsg.Content = new StringContent(json,Encoding.UTF8,"application/json");

            _log.LogInformation("FedEx Ship JSON: {Json}", json);

            var resp = await _http.SendAsync(reqMsg, ct);
            var body = await resp.Content.ReadAsStringAsync(ct);

            _log.LogInformation("FedEx Ship response ({Status}): {Body}",
                resp.StatusCode, body);

            if (!resp.IsSuccessStatusCode)
            {
                result.Success = false;
                result.Error =
                    $"FedEx Ship failed ({(int)resp.StatusCode} {resp.StatusCode}): {body}";
                return result;
            }

            // 4) Parse response
            using var doc = JsonDocument.Parse(body);
            var root = doc.RootElement;
            var output = root.GetProperty("output");
            var shipment = output.GetProperty("transactionShipments")[0];

            var masterTracking = shipment
                .GetProperty("masterTrackingNumber")
                .GetString();

            var docsArray = shipment
                .GetProperty("shipmentDocuments")
                .EnumerateArray();

            string? mergedLabelUrl = null;
            string? ciUrl = null;

            foreach (var d in docsArray)
            {
                var contentType = d.GetProperty("contentType").GetString();
                var url = d.GetProperty("url").GetString();

                if (contentType == "MERGED_LABEL_DOCUMENTS")
                    mergedLabelUrl = url;
                else if (contentType == "COMMERCIAL_INVOICE")
                    ciUrl = url;
            }

            // store the raw FedEx URLs here:
            result.FedExLabelUrl = mergedLabelUrl ?? ciUrl;
            result.FedExInvoiceUrl = ciUrl;

            result.Success = true;
            result.TrackingNumber = masterTracking;
            result.LabelUrl = mergedLabelUrl ?? ciUrl;
            result.CommercialInvoiceUrl = ciUrl;

            return result;
        }

        // Sample OrderKit 
        public async Task<string> GetAccessTokenAsync(System.Threading.CancellationToken ct)
        {
            var tokenUrl = $"{_settings.BaseUrl.TrimEnd('/')}/oauth/token";

            var form = new Dictionary<string, string>
            {
                ["grant_type"] = "client_credentials",
                ["client_id"] = _settings.ApiKey,
                ["client_secret"] = _settings.ApiSecret
            };

            var resp = await _http.PostAsync(tokenUrl, new FormUrlEncodedContent(form), ct);
            var body = await resp.Content.ReadAsStringAsync(ct);

            resp.EnsureSuccessStatusCode();

            using var doc = JsonDocument.Parse(body);
            return doc.RootElement.GetProperty("access_token").GetString();
        }


        public async Task<byte[]> DownloadDocumentAsync(string fedexUrl, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(fedexUrl))
                return Array.Empty<byte>();

            // If somehow encoded content/base64 is passed instead of URL
            if (!fedexUrl.StartsWith("http", StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    return Convert.FromBase64String(fedexUrl);
                }
                catch (FormatException)
                {
                    _log.LogError("FedEx document value is neither a valid URL nor valid base64.");
                    return Array.Empty<byte>();
                }
            }

            var token = await GetAccessTokenAsync(ct);

            using var req = new HttpRequestMessage(HttpMethod.Get, fedexUrl);
            req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            using var resp = await _http.SendAsync(req, ct);
            var bytes = await resp.Content.ReadAsByteArrayAsync(ct);

            if (!resp.IsSuccessStatusCode)
            {
                var body = Encoding.UTF8.GetString(bytes);
                _log.LogError("FedEx doc download failed ({Status}): {Body}",
                    resp.StatusCode, body);

                return Array.Empty<byte>();
            }

            return bytes;
        }

        public async Task<AddressValidationResponseDto> ValidateAddressAsync(
    AddressValidationRequestDto dto,
    CancellationToken cancellationToken)
        {
            var accessToken = await GetAccessTokenAsync(cancellationToken);

            var requestPayload = new
            {
                addressesToValidate = new[]
                {
            new
            {
                address = new
                {
                    streetLines = new[]
                    {
                        dto.AddressLine1,
                        dto.AddressLine2,
                        dto.AddressLine3
                    }.Where(x => !string.IsNullOrWhiteSpace(x)).ToArray(),
                    city = dto.City,
                    stateOrProvinceCode = dto.StateOrProvinceCode,
                    postalCode = dto.PostalCode,
                    countryCode = dto.CountryCode
                }
            }
        }
            };

            var request = new HttpRequestMessage(
                HttpMethod.Post,
                $"{_settings.BaseUrl}/address/v1/addresses/resolve");

            request.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", accessToken);

            request.Content = new StringContent(
                JsonConvert.SerializeObject(requestPayload),
                Encoding.UTF8,
                "application/json");

            var response = await _http.SendAsync(request, cancellationToken);
            var json = await response.Content.ReadAsStringAsync(cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                return new AddressValidationResponseDto
                {
                    IsValid = false,
                    Message = "Unable to validate address with FedEx."
                };
            }

            var fedexResponse = JObject.Parse(json);

            var resolution = fedexResponse["output"]?["resolvedAddresses"]?.FirstOrDefault();
            if (resolution == null)
            {
                return new AddressValidationResponseDto
                {
                    IsValid = false,
                    Message = "Address could not be validated."
                };
            }

            var classification = resolution["classification"]?.ToString() ?? string.Empty;
            var standardizedStatusName = resolution["standardizedStatusName"]?.ToString() ?? string.Empty;
            var resolutionMethodName = resolution["resolutionMethodName"]?.ToString() ?? string.Empty;

            // ✅ IMPORTANT:
            // Do not mark invalid only because classification = UNKNOWN.
            // Mark invalid only when FedEx explicitly indicates invalid / undeliverable,
            // or when no usable resolved address is returned.
            bool explicitlyInvalid =
                standardizedStatusName.Equals("INVALID", StringComparison.OrdinalIgnoreCase) ||
                standardizedStatusName.Equals("UNDELIVERABLE", StringComparison.OrdinalIgnoreCase) ||
                classification.Equals("INVALID", StringComparison.OrdinalIgnoreCase) ||
                classification.Equals("UNDELIVERABLE", StringComparison.OrdinalIgnoreCase);

            if (explicitlyInvalid)
            {
                return new AddressValidationResponseDto
                {
                    IsValid = false,
                    Message = "The provided address is invalid or undeliverable."
                };
            }

            // ✅ resolved address can be inside "address" OR directly on "resolution"
            var resolvedAddress = resolution["address"] ?? resolution;

            string? line1 = null;
            string? line2 = null;

            var streetLines = resolvedAddress["streetLines"] as JArray;
            var streetLinesToken = resolvedAddress["streetLinesToken"] as JArray;

            if (streetLines != null && streetLines.Count > 0)
            {
                line1 = streetLines.Count > 0 ? streetLines[0]?.ToString() : null;
                line2 = streetLines.Count > 1 ? streetLines[1]?.ToString() : null;
            }
            else if (streetLinesToken != null && streetLinesToken.Count > 0)
            {
                line1 = streetLinesToken.Count > 0 ? streetLinesToken[0]?.ToString() : null;
                line2 = streetLinesToken.Count > 1 ? streetLinesToken[1]?.ToString() : null;
            }
            else if (resolvedAddress["streetLinesToken"] != null)
            {
                line1 = resolvedAddress["streetLinesToken"]?.ToString();
            }

            var city = resolvedAddress["city"]?.ToString();
            var stateOrProvinceCode = resolvedAddress["stateOrProvinceCode"]?.ToString();
            var postalCode = resolvedAddress["postalCode"]?.ToString();
            var countryCode = resolvedAddress["countryCode"]?.ToString();

            // ✅ If FedEx returned no usable address fields at all, treat as invalid
            bool hasResolvedAddress =
                !string.IsNullOrWhiteSpace(line1) ||
                !string.IsNullOrWhiteSpace(city) ||
                !string.IsNullOrWhiteSpace(stateOrProvinceCode) ||
                !string.IsNullOrWhiteSpace(postalCode) ||
                !string.IsNullOrWhiteSpace(countryCode);

            if (!hasResolvedAddress)
            {
                return new AddressValidationResponseDto
                {
                    IsValid = false,
                    Message = "The provided address is invalid or undeliverable."
                };
            }

            return new AddressValidationResponseDto
            {
                IsValid = true,
                Message = string.IsNullOrWhiteSpace(classification)
                    ? "Address is valid."
                    : $"Address is valid ({classification}).",
                SuggestedAddress = new AddressValidationRequestDto
                {
                    AddressLine1 = line1,
                    AddressLine2 = line2,
                    City = city,
                    StateOrProvinceCode = stateOrProvinceCode,
                    PostalCode = postalCode,
                    CountryCode = countryCode
                }
            };
        }

        private static string ResolveFedExServiceType(string? requestedServiceType, string shipperCountryCode, string recipientCountryCode)
        {
            if (!string.IsNullOrWhiteSpace(requestedServiceType))
                return requestedServiceType.Trim().ToUpperInvariant();

            bool isDomesticUs =
                string.Equals(shipperCountryCode, "US", StringComparison.OrdinalIgnoreCase) &&
                string.Equals(recipientCountryCode, "US", StringComparison.OrdinalIgnoreCase);

            return isDomesticUs
                ? "PRIORITY_OVERNIGHT"
                : "INTERNATIONAL_PRIORITY";
        }
    }
}
