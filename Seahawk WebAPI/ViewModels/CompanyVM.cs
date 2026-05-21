using Data;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SeaHawkServices.Web.ViewModels
{
    public class CompanyVM
    {
        public Company Company { get; set; }
        public IEnumerable<Company> CompanyList { get; set; }
        public string ErrorMessage {  get; set; }   
        public int Id {  get; set; }   
        public string CurrentUser {  get; set; }   
        public string CurrentUserRole {  get; set; }
        public string CurrentUserEmail {  get; set; }

        // 🔍 Filters
        public string? FilterCompanyName { get; set; }
        public string? FilterCity { get; set; }
        public string? FilterCountry { get; set; }

        public int TotalPages =>
            PageSize == 0 ? 0 : (int)Math.Ceiling((double)TotalCount / PageSize);
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 15;
        public int TotalCount { get; set; } = 0;

        // ✅ Assigned vessels (CompanyId == Company.Id)
        public List<VesselDetail> AssignedVessels { get; set; } = new();

        // ✅ inline edit (one row at a time) OR you can redirect to a Vessel edit screen
        public VesselDetail? EditVessel { get; set; }

        // used to decide which vessel row to open in edit mode
        public int? EditVesselId { get; set; }


        // ✅ For Add Vessel multi-select
        public List<int> SelectedVesselIdsToAdd { get; set; } = new();
        public List<SelectListItem> AvailableVesselsDDL { get; set; } = new();

        // ✅ vessels not assigned to THIS company (CompanyId is null OR 0)
        public List<VesselDetail> AvailableVessels { get; set; } = new();

        // ✅ user selection
        public int? SelectedVesselId { get; set; }               // single-select
        public List<int> SelectedVesselIds { get; set; } = new(); // multi-select (optional)

        // toggle UI section
        public bool ShowAddVesselPanel { get; set; }


    }
}
