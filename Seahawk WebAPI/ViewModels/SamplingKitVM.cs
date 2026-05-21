using SeaHawkServices.Domain.Entities;
using static SeaHawkServices.Domain.Entities.Enums;

namespace SeaHawkServices.Web.ViewModels
{
    //public class SamplingKitVM
    //{
    //    public SamplingKit SamplingKit { get; set; }
    //    public IEnumerable<SamplingKit> SamplingKitList { get; set; }
    //    public int Id { get; set; }
    //    public string UserId { get; set; }
    //    public Country SelectedBillingCountry { get; set; }
    //    public Country SelectedCountry { get; set; }

    //    public string SuccessMessage { get; set; }
    //    public string TrackingNumber { get; set; }
    //    public string LabelUrl { get; set; }
    //    public string InvoiceUrl { get; set; }
    //    public string MsdsUrl { get; set; }

    //}
    //}

    public class SamplingKitVM
    {
        public SamplingKit SamplingKit { get; set; }
        public IEnumerable<SamplingKit> SamplingKitList { get; set; }

        // 🔹 Paging
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 15;
        public int TotalCount { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);

        // 🔹 Filters
        public string? FilterTracking { get; set; }
        public string? FilterVesselName { get; set; }
        public string? FilterIMO { get; set; }
        public string? FilterStatus { get; set; }
        public DateTime? FilterFromDate { get; set; }
        public DateTime? FilterToDate { get; set; }

        public int Id { get; set; }
        public string UserId { get; set; }
        public string UserEmail { get; set; }
        public string UserRole { get; set; }
        public Country SelectedBillingCountry { get; set; }
        public Country SelectedCountry { get; set; }

        public string SuccessMessage { get; set; }
        public string TrackingNumber { get; set; }
        public string LabelUrl { get; set; }
        public string InvoiceUrl { get; set; }
        public string MsdsUrl { get; set; }
        public SampleCollectionStatus RequestStatus { get; set; } = SampleCollectionStatus.Pending;
        public string? FedExRequestJson { get; set; }
        public string? FedExErrorMessage { get; set; }

        public Dictionary<int, List<RequestHistory>> RequestHistoriesByRequestId { get; set; }
            = new Dictionary<int, List<RequestHistory>>();
    }

}
