using Data;
using Microsoft.AspNetCore.Mvc.Rendering;
//using Microsoft.CodeAnalysis.Diagnostics;

namespace SeaHawkServices.Web.ViewModels
{
    public class VesselDetailVM
    {
        public VesselDetail Vessel { get; set; }
        public IEnumerable<VesselDetail> VesselDetailList { get; set; }
        public IEnumerable<SelectListItem> CompanyList { get; set; }
        public string ErrorMessage {  get; set; }   
        public int Id {  get; set; }

        public string? CurrentUserRole { get; set; }
        public string? CurrentUser { get; set; }
        public string? CurrentUserEmail { get; set; }

        // Filters 
        public string? FilterCompanyName { get; set; }
        public string? FilterVesselName { get; set; }
        public string? FilterIMONumber { get; set; }
        public string? FilterOwner { get; set; }
        public string? FilterCallSign { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 15;
        public int TotalCount { get; set; }

        public int TotalPages =>
            PageSize <= 0 ? 0 : (int)Math.Ceiling((double)TotalCount / PageSize);

    }
}
