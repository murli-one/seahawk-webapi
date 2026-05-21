using Data;
using SeaHawkServices.Domain.Entities;

namespace SeaHawkServices.Web.ViewModels
{
    public static class DhlJsonParser
    {
        public static TrackingResult ParseJson(string json)
        {
            var result = new TrackingResult();

            dynamic data = Newtonsoft.Json.JsonConvert.DeserializeObject(json);

            if (data.shipments == null || data.shipments.Count == 0)
            {
                result.Success = false;
                result.Error = "No shipment found.";
                return result;
            }

            var shipment = data.shipments[0];

            result.StatusText = shipment.status.description;
            result.Success = true;

            foreach (var ev in shipment.events)
            {
                result.Events.Add(new TrackingEvent
                {
                    When = ev.timestamp,
                    Code = ev.code,
                    Description = ev.description,
                    City = ev.location?.address?.addressLocality,
                    CountryCode = ev.location?.address?.countryCode
                });
            }

            result.Events = result.Events.OrderByDescending(e => e.When).ToList();

            return result;
        }
    }
}
