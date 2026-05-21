using Data;
using SeaHawkServices.Domain.Entities;
using static SeaHawkServices.Domain.Entities.Enums;

namespace SeaHawkServices.Web.ViewModels
{
    public class SampleTrackingVM
    {
        public CourierProvider CourierType { get; set; }
        public string AWBNumber { get; set; } 
        public TrackingResult? Result { get; set; }

        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public string UserRole { get; set; }

        // Pickup tab
        public SamplePickupRequestVM PickupRequest { get; set; } = new SamplePickupRequestVM();
        public SamplePickupResultVM PickupResult { get; set; }

        public List<ShipmentTrackingRowVM> TrackingShipments { get; set; } = new();
        public bool IsAdmin { get; set; }

        public List<ShipmentRowVM> AllUsersShipments { get; set; } = new();

    }
    public class ShipmentRowVM
    {
        public string Source { get; set; } = ""; // "Sample Kit" / "Sample Collection"
        public string VesselName { get; set; } = "";
        public string TrackingNumber { get; set; } = "";
        public string Status { get; set; } = "";
        public string DetailsUrl { get; set; } = "";
        public CourierProvider CourierProvider { get; set; }
    }
}
