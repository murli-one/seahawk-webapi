using Data;
using SeaHawkServices.Domain.Entities;
using System;
using System.Linq;
using System.Text.Json;

namespace SeaHawkServices.Infrastructure.FedEx
{
    public static class FedExXmlParser
    {
        /// <summary>
        /// Parses FedEx REST Track API JSON response into TrackingResult.
        /// Method name is kept same to avoid changing existing callers.
        /// </summary>
        public static TrackingResult ParseTrackingResponse(string json)
        {
            var result = new TrackingResult
            {
                Success = false
            };

            if (string.IsNullOrWhiteSpace(json))
            {
                result.Error = "Empty FedEx tracking response.";
                return result;
            }

            try
            {
                using JsonDocument doc = JsonDocument.Parse(json);
                var root = doc.RootElement;

                // Top-level FedEx API errors.
                if (root.TryGetProperty("errors", out var errorsElement) &&
                    errorsElement.ValueKind == JsonValueKind.Array &&
                    errorsElement.GetArrayLength() > 0)
                {
                    var firstError = errorsElement[0];
                    result.Error = firstError.TryGetProperty("message", out var msgProp)
                        ? msgProp.GetString() ?? "FedEx tracking error."
                        : "FedEx tracking error.";
                    return result;
                }

                if (!root.TryGetProperty("output", out var outputEl))
                {
                    result.Error = "No output section in FedEx tracking response.";
                    return result;
                }

                // Important:
                // FedEx sandbox can include output.alerts code VIRTUAL.RESPONSE even for valid
                // sample tracking numbers, for example 123456789012. So this alert must not be
                // treated as shipment-not-found. We only fail when trackResults contains an error
                // or when no useful tracking data is returned.

                if (!outputEl.TryGetProperty("completeTrackResults", out var completeResultsEl) ||
                    completeResultsEl.ValueKind != JsonValueKind.Array ||
                    completeResultsEl.GetArrayLength() == 0)
                {
                    result.Error = "No tracking results found.";
                    return result;
                }

                var firstComplete = completeResultsEl[0];

                if (!firstComplete.TryGetProperty("trackResults", out var trackResultsEl) ||
                    trackResultsEl.ValueKind != JsonValueKind.Array ||
                    trackResultsEl.GetArrayLength() == 0)
                {
                    result.Error = "No track results found.";
                    return result;
                }

                var trackResultEl = trackResultsEl[0];

                // Embedded tracking error = actual not found/error response.
                if (trackResultEl.TryGetProperty("error", out var embeddedErrorEl) &&
                    embeddedErrorEl.ValueKind == JsonValueKind.Object)
                {
                    result.Error = embeddedErrorEl.TryGetProperty("message", out var embeddedMsgProp)
                        ? embeddedMsgProp.GetString() ?? "Shipment not found."
                        : "Shipment not found.";
                    return result;
                }

                // Waybill / tracking number.
                if (trackResultEl.TryGetProperty("trackingNumberInfo", out var trackingInfoEl) &&
                    trackingInfoEl.TryGetProperty("trackingNumber", out var tnProp))
                {
                    result.Waybill = tnProp.GetString() ?? string.Empty;
                }
                else if (firstComplete.TryGetProperty("trackingNumber", out var tn2Prop))
                {
                    result.Waybill = tn2Prop.GetString() ?? string.Empty;
                }

                // Latest status.
                if (trackResultEl.TryGetProperty("latestStatusDetail", out var latestStatusEl) &&
                    latestStatusEl.ValueKind == JsonValueKind.Object)
                {
                    string? statusByLocale = latestStatusEl.TryGetProperty("statusByLocale", out var sblProp)
                        ? sblProp.GetString()
                        : null;

                    string? description = latestStatusEl.TryGetProperty("description", out var descProp)
                        ? descProp.GetString()
                        : null;

                    result.StatusText = !string.IsNullOrWhiteSpace(statusByLocale)
                        ? statusByLocale
                        : description ?? string.Empty;
                }

                // Service description.
                if (trackResultEl.TryGetProperty("serviceDetail", out var serviceEl) &&
                    serviceEl.ValueKind == JsonValueKind.Object &&
                    serviceEl.TryGetProperty("description", out var serviceDescProp))
                {
                    result.ServiceDescription = serviceDescProp.GetString() ?? string.Empty;
                }

                // Shipper/origin.
                if (trackResultEl.TryGetProperty("shipperInformation", out var shipperInfoEl) &&
                    shipperInfoEl.TryGetProperty("address", out var shipperAddressEl))
                {
                    if (shipperAddressEl.TryGetProperty("city", out var cityProp))
                        result.ShipperCity = cityProp.GetString() ?? string.Empty;

                    if (shipperAddressEl.TryGetProperty("countryCode", out var countryProp))
                        result.ShipperCountryCode = countryProp.GetString() ?? string.Empty;
                }

                // Recipient/destination.
                if (trackResultEl.TryGetProperty("recipientInformation", out var recipientInfoEl) &&
                    recipientInfoEl.TryGetProperty("address", out var recipientAddressEl))
                {
                    if (recipientAddressEl.TryGetProperty("city", out var cityProp))
                        result.RecipientCity = cityProp.GetString() ?? string.Empty;

                    if (recipientAddressEl.TryGetProperty("countryCode", out var countryProp))
                        result.RecipientCountryCode = countryProp.GetString() ?? string.Empty;
                }
                else if (trackResultEl.TryGetProperty("lastUpdatedDestinationAddress", out var destinationAddressEl))
                {
                    if (destinationAddressEl.TryGetProperty("city", out var cityProp))
                        result.RecipientCity = cityProp.GetString() ?? string.Empty;

                    if (destinationAddressEl.TryGetProperty("countryCode", out var countryProp))
                        result.RecipientCountryCode = countryProp.GetString() ?? string.Empty;
                }

                // Ship/pickup dates.
                if (trackResultEl.TryGetProperty("dateAndTimes", out var dateTimesEl) &&
                    dateTimesEl.ValueKind == JsonValueKind.Array)
                {
                    foreach (var dateItem in dateTimesEl.EnumerateArray())
                    {
                        if (!dateItem.TryGetProperty("type", out var typeProp) ||
                            !dateItem.TryGetProperty("dateTime", out var dateTimeProp))
                            continue;

                        var type = typeProp.GetString();
                        var dateTime = dateTimeProp.GetString();

                        if (string.IsNullOrWhiteSpace(dateTime) ||
                            !DateTimeOffset.TryParse(dateTime, out var parsedDate))
                            continue;

                        if (string.Equals(type, "SHIP", StringComparison.OrdinalIgnoreCase))
                            result.ShipDate = parsedDate;

                        if (string.Equals(type, "ACTUAL_PICKUP", StringComparison.OrdinalIgnoreCase))
                            result.ActualPickupDate = parsedDate;
                    }
                }

                // Scan events.
                if (trackResultEl.TryGetProperty("scanEvents", out var scanEventsEl) &&
                    scanEventsEl.ValueKind == JsonValueKind.Array)
                {
                    foreach (var ev in scanEventsEl.EnumerateArray())
                    {
                        DateTimeOffset when = DateTimeOffset.MinValue;
                        if (ev.TryGetProperty("date", out var dateProp))
                        {
                            DateTimeOffset.TryParse(dateProp.GetString(), out when);
                        }

                        string? code = ev.TryGetProperty("eventType", out var eventTypeProp)
                            ? eventTypeProp.GetString()
                            : null;

                        string? description = ev.TryGetProperty("eventDescription", out var eventDescProp)
                            ? eventDescProp.GetString()
                            : null;

                        string? derivedStatus = ev.TryGetProperty("derivedStatus", out var derivedStatusProp)
                            ? derivedStatusProp.GetString()
                            : null;

                        string? exceptionCode = ev.TryGetProperty("exceptionCode", out var exceptionCodeProp)
                            ? exceptionCodeProp.GetString()
                            : null;

                        string? exceptionDescription = ev.TryGetProperty("exceptionDescription", out var exceptionDescProp)
                            ? exceptionDescProp.GetString()
                            : null;

                        string? city = null;
                        string? state = null;
                        string? postalCode = null;
                        string? countryCode = null;

                        if (ev.TryGetProperty("scanLocation", out var locationEl) &&
                            locationEl.ValueKind == JsonValueKind.Object)
                        {
                            if (locationEl.TryGetProperty("city", out var cityProp))
                                city = cityProp.GetString();

                            if (locationEl.TryGetProperty("stateOrProvinceCode", out var stateProp))
                                state = stateProp.GetString();

                            if (locationEl.TryGetProperty("postalCode", out var postalProp))
                                postalCode = postalProp.GetString();

                            if (locationEl.TryGetProperty("countryCode", out var countryProp))
                                countryCode = countryProp.GetString();
                        }

                        result.Events.Add(new TrackingEvent
                        {
                            When = when,
                            Code = code ?? string.Empty,
                            Description = description ?? string.Empty,
                            City = city ?? string.Empty,
                            CountryCode = countryCode ?? string.Empty,
                            StateOrProvince = state ?? string.Empty,
                            PostalCode = postalCode ?? string.Empty,
                            DerivedStatus = derivedStatus ?? string.Empty,
                            ExceptionCode = exceptionCode ?? string.Empty,
                            ExceptionDescription = exceptionDescription ?? string.Empty
                        });
                    }
                }

                result.Events = result.Events
                    .OrderByDescending(e => e.When)
                    .ToList();

                if (!result.Events.Any() &&
                    string.IsNullOrWhiteSpace(result.StatusText) &&
                    string.IsNullOrWhiteSpace(result.Waybill))
                {
                    result.Error = "Shipment not found.";
                    return result;
                }

                if (string.IsNullOrWhiteSpace(result.StatusText) && result.Events.Any())
                {
                    result.StatusText = result.Events.First().DerivedStatus;
                    if (string.IsNullOrWhiteSpace(result.StatusText))
                        result.StatusText = result.Events.First().Description;
                }

                result.Success = true;
                return result;
            }
            catch (Exception ex)
            {
                result.Error = $"Failed to parse FedEx tracking JSON: {ex.Message}";
                return result;
            }
        }
    }
}
