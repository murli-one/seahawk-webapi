using SeaHawkServices.Domain.Entities;
using static SeaHawkServices.Domain.Entities.Enums;

namespace SeaHawkServices.Web.ViewModels
{
    public class SamplingKitAdminEditVM
    {
        public int Id { get; set; }

        // Context (read-only in the view)
        public string VesselName { get; set; }
        public string IMONumber { get; set; }
        public string RequestorName { get; set; }
        public string RequestorEmail { get; set; }
        public string CompanyName { get; set; }

        // Shipment / logistics fields admin can adjust
        public int? NumberOfParcels { get; set; }
        public decimal? DeclaredValue { get; set; }
        public decimal? PackageWeightLb { get; set; }
        public string CommodityDescription { get; set; }

        public CourierProvider CourierProvider { get; set; }
        public string TrackingNumber { get; set; }   // maps to kit.FedExTrackingNumber

        public SampleCollectionStatus RequestStatus { get; set; }

        // Optional: if you later add this to SamplingKit entity
        public bool IsUserCancellationAllowed { get; set; }
    }
}