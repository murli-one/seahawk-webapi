namespace SeaHawkServices.Web.ViewModels
{
    public class ShipmentTrackingRowVM
    {
        public string SourceType { get; set; } = ""; // "SampleCollection" / "SampleKit"
        public string VesselName { get; set; } = "";
        public string TrackingNumber { get; set; } = "";
        public string Status { get; set; } = "";
        public string? Carrier { get; set; }
        public DateTime? CreatedAt { get; set; }

        // Where to navigate when tracking number clicked
        public string DetailsUrl { get; set; } = "";
    }
}
