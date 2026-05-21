using Data;
using Microsoft.AspNetCore.Mvc.Rendering;
using SeaHawkServices.Domain.Entities;

namespace SeaHawkServices.Web.ViewModels
{
    public class SampLogicVM
    {

        public SampleCollections SampleCollections { get; set; }
        public IEnumerable<SampleCollections> SampleCollectionsList { get; set; }
        //public SampCollection SampleCollection { get; set; }
        public string CurrentTab { get; set; }
        public int SelectedCountry { get; set; }
        public int SelectedHourEnum { get; set; }
        public int SelectedMinuteEnum { get; set; }

        public string UserEmail { get; set; }
        public string UserRole{ get; set; }

        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 15;
        public int TotalCount { get; set; } = 0;
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);

        // filters (match action params)
        public string? Search { get; set; }
        public string? FilterTracking { get; set; }
        public string? FilterVesselName { get; set; }
        public string? FilterIMO { get; set; }
        public string? FilterCompany { get; set; }
        public string? FilterStatus { get; set; }
        public DateTime? FilterFromDate { get; set; }
        public DateTime? FilterToDate { get; set; }

        /// <summary>
        /// Request history grouped by SampleCollections Id.
        /// Key = RequestId, Value = list of history entries (sorted by time).
        /// </summary>
        public Dictionary<int, List<RequestHistory>> RequestHistoriesByRequestId { get; set; }
            = new Dictionary<int, List<RequestHistory>>();
    }
}
