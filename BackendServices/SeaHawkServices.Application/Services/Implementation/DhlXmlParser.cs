using Data;
using SeaHawkServices.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SeaHawkServices.Infrastructure.DHL
{
    public static class DhlXmlParser
    {
        public static TrackingResult ParseTrackingResponse(string xml)
        {
            var result = new TrackingResult();

            var doc = XDocument.Parse(xml);
            XNamespace ns = "http://www.dhl.com";

            // AWBInfo (no namespace in your XML)
            var awbInfo = doc.Descendants("AWBInfo").FirstOrDefault();
            if (awbInfo == null)
            {
                result.Success = false;
                result.Error = "AWBInfo not found in DHL response.";
                return result;
            }

            // Status
            var status = awbInfo
                .Element("Status")?
                .Element("ActionStatus")?
                .Value ?? "Unknown";

            result.StatusText = status;
            result.Success = string.Equals(status, "success", StringComparison.OrdinalIgnoreCase);

            // ShipmentInfo (no namespace)
            var shipmentInfo = awbInfo.Element("ShipmentInfo");
            if (shipmentInfo == null)
                return result; // nothing else to show

            // 1) Try to read detailed ShipmentEvent nodes (if they exist)
            bool hasEvents = false;

            foreach (var ev in shipmentInfo.Elements(ns + "ShipmentEvent")
                                           .Concat(shipmentInfo.Elements("ShipmentEvent")))
            {
                hasEvents = true;

                var date = (ev.Element(ns + "Date") ?? ev.Element("Date"))?.Value;
                var time = (ev.Element(ns + "Time") ?? ev.Element("Time"))?.Value;

                var svc = ev.Element(ns + "ServiceEvent") ?? ev.Element("ServiceEvent");
                var loc = ev.Element(ns + "EventLocation") ?? ev.Element("EventLocation");
                var addr = loc?.Element(ns + "Address") ?? loc?.Element("Address");

                result.Events.Add(new TrackingEvent
                {
                    When = ParseDateTime(date, time),
                    Code = (svc?.Element(ns + "EventCode") ?? svc?.Element("EventCode"))?.Value ?? "",
                    Description = (svc?.Element(ns + "Description") ?? svc?.Element("Description"))?.Value ?? "",
                    City = (addr?.Element(ns + "City") ?? addr?.Element("City"))?.Value ?? "",
                    CountryCode = (addr?.Element(ns + "CountryCode") ?? addr?.Element("CountryCode"))?.Value ?? ""
                });
            }

            // 2) If there are NO ShipmentEvent elements (your current XML case),
            //    create a single summary row from ShipmentInfo
            if (!hasEvents)
            {
                // ShipmentDate
                DateTime when = DateTime.MinValue;
                var shipmentDateStr = shipmentInfo.Element("ShipmentDate")?.Value;
                if (DateTime.TryParse(shipmentDateStr, out var shipmentDt))
                    when = shipmentDt;

                // Origin & Destination descriptions
                var origin = shipmentInfo.Element("OriginServiceArea");
                var dest = shipmentInfo.Element("DestinationServiceArea");

                string originDesc = origin?.Element("Description")?.Value ?? "";
                string destDesc = dest?.Element("Description")?.Value ?? "";

                // Shipper / Consignee country codes
                var shipper = shipmentInfo.Element("Shipper");
                var consignee = shipmentInfo.Element("Consignee");

                string shipperCountry = shipper?.Element("CountryCode")?.Value ?? "";
                string consigneeCountry = consignee?.Element("CountryCode")?.Value ?? "";

                // Product code & description
                var productCode = shipmentInfo.Element("GlobalProductCode")?.Value;
                var shipmentDesc = shipmentInfo.Element("ShipmentDesc")?.Value;

                result.Events.Add(new TrackingEvent
                {
                    When = when,
                    Code = productCode ?? "",
                    Description = shipmentDesc ?? "",
                    City = $"{originDesc} → {destDesc}",
                    CountryCode = $"{shipperCountry} → {consigneeCountry}"
                });
            }

            // Order newest first
            result.Events = result.Events.OrderByDescending(e => e.When).ToList();

            return result;
        }

        private static DateTime ParseDateTime(string date, string time)
        {
            if (string.IsNullOrWhiteSpace(date) && string.IsNullOrWhiteSpace(time))
                return DateTime.MinValue;

            if (DateTime.TryParse($"{date} {time}", out var dt))
                return dt;

            if (DateTime.TryParse(date, out dt))
                return dt;

            return DateTime.MinValue;
        }

    }



    public static class FedExXmlParser
    {
        //public static TrackingResult ParseTrackingResponse(string xml)
        //{
        //    var result = new TrackingResult
        //    {
        //        Success = false
        //    };

        //    var doc = XDocument.Parse(xml);

        //    XNamespace t = "http://fedex.com/ws/track/v16";

        //    // Read top-level notification (success / error)
        //    var notification = doc.Descendants(t + "Notifications").FirstOrDefault();
        //    if (notification != null)
        //    {
        //        var severity = (string)notification.Element(t + "Severity");
        //        var message = (string)notification.Element(t + "Message");

        //        if (!string.Equals(severity, "SUCCESS", StringComparison.OrdinalIgnoreCase) &&
        //            !string.Equals(severity, "NOTE", StringComparison.OrdinalIgnoreCase) &&
        //            !string.Equals(severity, "WARNING", StringComparison.OrdinalIgnoreCase))
        //        {
        //            result.Error = message ?? "FedEx tracking error.";
        //            return result;
        //        }
        //    }

        //    var trackDetails = doc.Descendants(t + "TrackDetails").FirstOrDefault();
        //    if (trackDetails == null)
        //    {
        //        result.Error = "No tracking details found.";
        //        return result;
        //    }

        //    result.Success = true;
        //    result.StatusText = (string)trackDetails.Element(t + "StatusDescription");

        //    foreach (var ev in trackDetails.Elements(t + "Events"))
        //    {
        //        var addr = ev.Element(t + "Address");

        //        result.Events.Add(new TrackingEvent
        //        {
        //            When = (DateTime?)ev.Element(t + "Timestamp") ?? DateTime.MinValue,
        //            Code = (string)ev.Element(t + "EventType"),
        //            Description = (string)ev.Element(t + "EventDescription"),
        //            City = (string?)addr?.Element(t + "City"),
        //            CountryCode = (string?)addr?.Element(t + "CountryCode")
        //        });
        //    }

        //    // ----- 2) If no Events, create a single summary row from TrackDetails -----
        //    if (!result.Events.Any())
        //    {
        //        // When: prefer ActualDeliveryTimestamp, then ShipTimestamp
        //        var actualDeliveryTsEl = trackDetails.Element(t + "ActualDeliveryTimestamp") ?? trackDetails.Element("ActualDeliveryTimestamp");
        //        var shipTsEl = trackDetails.Element(t + "ShipTimestamp") ?? trackDetails.Element("ShipTimestamp");

        //        DateTime when = DateTime.MinValue;
        //        if (actualDeliveryTsEl != null && DateTime.TryParse(actualDeliveryTsEl.Value, out var dtDelivery))
        //            when = dtDelivery;
        //        else if (shipTsEl != null && DateTime.TryParse(shipTsEl.Value, out var dtShip))
        //            when = dtShip;

        //        // Status code / description
        //        var statusCodeEl = trackDetails.Element(t + "StatusCode") ?? trackDetails.Element("StatusCode");
        //        var statusCode = (string?)statusCodeEl ?? "";

        //        var summaryDesc = result.StatusText; // already set above

        //        // Origin / Destination
        //        var originAddr = trackDetails.Element(t + "OriginAddress") ?? trackDetails.Element("OriginAddress");
        //        var destAddr = trackDetails.Element(t + "DestinationAddress") ?? trackDetails.Element("DestinationAddress");

        //        string originCity = (string?)(originAddr?.Element(t + "City") ?? originAddr?.Element("City")) ?? "";
        //        string originCountry = (string?)(originAddr?.Element(t + "CountryCode") ?? originAddr?.Element("CountryCode")) ?? "";

        //        string destCity = (string?)(destAddr?.Element(t + "City") ?? destAddr?.Element("City")) ?? "";
        //        string destCountry = (string?)(destAddr?.Element(t + "CountryCode") ?? destAddr?.Element("CountryCode")) ?? "";

        //        var citySummary = $"{originCity} {originCountry}".Trim();
        //        var destSummary = $"{destCity} {destCountry}".Trim();
        //        var cityCombined = string.IsNullOrWhiteSpace(destSummary)
        //                                ? citySummary
        //                                : $"{citySummary} → {destSummary}";

        //        var countryCombined = string.IsNullOrWhiteSpace(originCountry + destCountry)
        //                                ? ""
        //                                : $"{originCountry} → {destCountry}";

        //        result.Events.Add(new TrackingEvent
        //        {
        //            When = when,
        //            Code = statusCode,      // e.g. "DL"
        //            Description = summaryDesc,     // e.g. "Delivered"
        //            City = cityCombined,    // Origin → Destination
        //            CountryCode = countryCombined  // Country → Country
        //        });
        //    }

        //    // Order newest first
        //    result.Events = result.Events
        //        .OrderByDescending(e => e.When)
        //        .ToList();


        //    return result;
        //}

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
                using var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;

                // --------- 1) Check errors ----------
                if (root.TryGetProperty("errors", out var errorsEl) &&
                    errorsEl.ValueKind == JsonValueKind.Array &&
                    errorsEl.GetArrayLength() > 0)
                {
                    var firstErr = errorsEl[0];
                    var msg = firstErr.TryGetProperty("message", out var msgEl)
                        ? msgEl.GetString()
                        : "FedEx tracking error.";
                    result.Error = msg ?? "FedEx tracking error.";
                    return result;
                }

                if (!root.TryGetProperty("output", out var outputEl))
                {
                    result.Error = "No output section in FedEx tracking response.";
                    return result;
                }

                // alerts (e.g. VIRTUAL.RESPONSE) are informational, not errors
                // so we just ignore them for now

                // --------- 2) Navigate to the first trackResult ----------
                if (!outputEl.TryGetProperty("completeTrackResults", out var ctrArray) ||
                    ctrArray.ValueKind != JsonValueKind.Array ||
                    ctrArray.GetArrayLength() == 0)
                {
                    result.Error = "No tracking results found.";
                    return result;
                }

                var ctr0 = ctrArray[0];

                if (!ctr0.TryGetProperty("trackResults", out var trArray) ||
                    trArray.ValueKind != JsonValueKind.Array ||
                    trArray.GetArrayLength() == 0)
                {
                    result.Error = "No track results found.";
                    return result;
                }

                var tr0 = trArray[0];

                // FedEx can return an embedded error inside the first track result when the
                // tracking number is not found. This is the reliable not-found condition.
                // Do not treat output.alerts code VIRTUAL.RESPONSE as not-found, because
                // FedEx sandbox also returns this alert for valid/sample tracking numbers
                // such as 123456789012.
                if (tr0.TryGetProperty("error", out var trackingErrorEl) &&
                    trackingErrorEl.ValueKind == JsonValueKind.Object)
                {
                    var msg = trackingErrorEl.TryGetProperty("message", out var errorMsgEl)
                        ? errorMsgEl.GetString()
                        : "Shipment not found.";

                    result.Error = string.IsNullOrWhiteSpace(msg) ? "Shipment not found." : msg;
                    return result;
                }

                // --------- 3) Waybill / tracking number ----------
                if (tr0.TryGetProperty("trackingNumberInfo", out var tniEl) &&
                    tniEl.TryGetProperty("trackingNumber", out var tnEl))
                {
                    result.Waybill = tnEl.GetString() ?? "";
                }
                else if (ctr0.TryGetProperty("trackingNumber", out var tn2El))
                {
                    result.Waybill = tn2El.GetString() ?? "";
                }

                // --------- 4) Overall status text ----------
                if (tr0.TryGetProperty("latestStatusDetail", out var latestStatusEl))
                {
                    string statusByLocale = latestStatusEl.TryGetProperty("statusByLocale", out var sblEl)
                        ? sblEl.GetString()
                        : null;

                    string description = latestStatusEl.TryGetProperty("description", out var descEl)
                        ? descEl.GetString()
                        : null;

                    result.StatusText = !string.IsNullOrWhiteSpace(statusByLocale)
                        ? statusByLocale
                        : description;
                }

                // --------- 5) Service description ----------
                if (tr0.TryGetProperty("serviceDetail", out var serviceEl))
                {
                    if (serviceEl.TryGetProperty("description", out var sDescEl))
                        result.ServiceDescription = sDescEl.GetString() ?? "";
                }

                // --------- 6) Shipper & recipient summary ----------
                // Shipper (origin)
                if (tr0.TryGetProperty("shipperInformation", out var shipInfoEl) &&
                    shipInfoEl.TryGetProperty("address", out var shipAddrEl))
                {
                    if (shipAddrEl.TryGetProperty("city", out var sCityEl))
                        result.ShipperCity = sCityEl.GetString() ?? "";

                    if (shipAddrEl.TryGetProperty("countryCode", out var sCountryEl))
                        result.ShipperCountryCode = sCountryEl.GetString() ?? "";
                }

                // Recipient (destination)
                if (tr0.TryGetProperty("recipientInformation", out var recInfoEl) &&
                    recInfoEl.TryGetProperty("address", out var recAddrEl))
                {
                    if (recAddrEl.TryGetProperty("city", out var rCityEl))
                        result.RecipientCity = rCityEl.GetString() ?? "";

                    if (recAddrEl.TryGetProperty("countryCode", out var rCountryEl))
                        result.RecipientCountryCode = rCountryEl.GetString() ?? "";
                }
                else if (tr0.TryGetProperty("lastUpdatedDestinationAddress", out var destAddrEl))
                {
                    if (destAddrEl.TryGetProperty("city", out var rCityEl))
                        result.RecipientCity = rCityEl.GetString() ?? "";

                    if (destAddrEl.TryGetProperty("countryCode", out var rCountryEl))
                        result.RecipientCountryCode = rCountryEl.GetString() ?? "";
                }

                // --------- 7) Ship / pickup dates from dateAndTimes ----------
                if (tr0.TryGetProperty("dateAndTimes", out var dtArray) &&
                    dtArray.ValueKind == JsonValueKind.Array)
                {
                    foreach (var dtItem in dtArray.EnumerateArray())
                    {
                        if (!dtItem.TryGetProperty("type", out var typeEl) ||
                            !dtItem.TryGetProperty("dateTime", out var dtEl))
                            continue;

                        var typeStr = typeEl.GetString();
                        var dtStr = dtEl.GetString();

                        if (string.IsNullOrWhiteSpace(dtStr))
                            continue;

                        if (!DateTimeOffset.TryParse(dtStr, out var parsed))
                            continue;

                        if (string.Equals(typeStr, "SHIP", StringComparison.OrdinalIgnoreCase))
                            result.ShipDate = parsed;

                        if (string.Equals(typeStr, "ACTUAL_PICKUP", StringComparison.OrdinalIgnoreCase))
                            result.ActualPickupDate = parsed;
                    }
                }

                // --------- 8) Detailed scan events ----------
                if (tr0.TryGetProperty("scanEvents", out var scanEventsEl) &&
                    scanEventsEl.ValueKind == JsonValueKind.Array)
                {
                    foreach (var ev in scanEventsEl.EnumerateArray())
                    {
                        // When
                        DateTimeOffset when = DateTimeOffset.MinValue;
                        if (ev.TryGetProperty("date", out var dateEl))
                        {
                            var dateStr = dateEl.GetString();
                            DateTimeOffset.TryParse(dateStr, out when);
                        }

                        // Event code & description
                        string code = ev.TryGetProperty("eventType", out var evTypeEl)
                            ? evTypeEl.GetString()
                            : null;

                        string description = ev.TryGetProperty("eventDescription", out var evDescEl)
                            ? evDescEl.GetString()
                            : null;

                        string derivedStatus = ev.TryGetProperty("derivedStatus", out var dsEl)
                            ? dsEl.GetString()
                            : null;

                        string exceptionCode = ev.TryGetProperty("exceptionCode", out var exCodeEl)
                            ? exCodeEl.GetString()
                            : null;

                        string exceptionDescription = ev.TryGetProperty("exceptionDescription", out var exDescEl)
                            ? exDescEl.GetString()
                            : null;

                        // Location
                        string city = null;
                        string state = null;
                        string postalCode = null;
                        string countryCode = null;

                        if (ev.TryGetProperty("scanLocation", out var locEl) &&
                            locEl.ValueKind == JsonValueKind.Object)
                        {
                            if (locEl.TryGetProperty("city", out var cityEl))
                                city = cityEl.GetString();

                            if (locEl.TryGetProperty("stateOrProvinceCode", out var stEl))
                                state = stEl.GetString();

                            if (locEl.TryGetProperty("postalCode", out var pcEl))
                                postalCode = pcEl.GetString();

                            if (locEl.TryGetProperty("countryCode", out var ccEl))
                                countryCode = ccEl.GetString();
                        }

                        result.Events.Add(new TrackingEvent
                        {
                            When = when,
                            Code = code ?? "",
                            Description = description ?? "",
                            City = city ?? "",
                            CountryCode = countryCode ?? "",
                            StateOrProvince = state ?? "",
                            PostalCode = postalCode ?? "",
                            DerivedStatus = derivedStatus ?? "",
                            ExceptionCode = exceptionCode ?? "",
                            ExceptionDescription = exceptionDescription ?? ""
                        });
                    }
                }

                // If still no events, we could add a fallback summary row (optional),
                // but in your sample we always have scanEvents.

                result.Events = result.Events
                    .OrderByDescending(e => e.When)
                    .ToList();

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
