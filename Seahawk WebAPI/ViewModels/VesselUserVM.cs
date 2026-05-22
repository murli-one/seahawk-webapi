using Data;
using Microsoft.AspNetCore.Mvc.Rendering;
//using Microsoft.CodeAnalysis.Diagnostics;
using SeaHawkServices.Domain.Entities;

namespace SeaHawkServices.Web.ViewModels
{
    public class VesselUserVM
    {
        public List<VesselDetail> Vessels { get; set; }
        public List<ApplicationUser> ApplicationUsers { get; set; }
        public List<VesselUser> VesselUser { get; set; } = new List<VesselUser>();

        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 15;
        public int TotalCount { get; set; } = 0;

        public int TotalPages =>
            PageSize == 0 ? 0 : (int)Math.Ceiling((double)TotalCount / PageSize);

        public string? FilterUserName { get; set; }
        public string? FilterVesselName { get; set; }

        // ✅ NEW: one row per user (we will store 1 record per user here)
        public List<VesselUser> VesselUserGrouped { get; set; } = new List<VesselUser>();

        // ✅ NEW: vessel names mapped by userId (comma-separated)
        public Dictionary<string, string> UserVesselNames { get; set; } = new();

        public string SelectedVesselIdsCsv { get; set; }
        public List<int> SelectedVesselIds { get; set; } = new();

        ////public int? SelectedVesselId { get; set; }
        public string SelectedUser { get; set; } = string.Empty;
        public string SelectedUserEmail { get; set; } = string.Empty;
        public string SelectedUserRole { get; set; } = string.Empty;

        /* DDL*/
        public List<SelectListItem> UsersDDL
        {
            get
            {
                var DDL = new List<SelectListItem>();
                var itemx = new SelectListItem();
                itemx.Value = "0";
                itemx.Text = "";
                DDL.Add(itemx);

                if (ApplicationUsers != null)
                    foreach (var i in ApplicationUsers)
                    {
                        var item = new SelectListItem();
                        item.Value = i.Id.ToString();
                        item.Text = i.UserName;
                        DDL.Add(item);
                    }
                return DDL;
            }
        }

        public List<SelectListItem> VesselsDDL
        {
            get
            {
                var DDL = new List<SelectListItem>();
                var itemx = new SelectListItem();
                itemx.Value = "0";
                itemx.Text = "";
                DDL.Add(itemx);

                if (Vessels != null)
                    foreach (var i in Vessels)
                    {
                        var item = new SelectListItem();
                        item.Value = i.Id.ToString();
                        item.Text = i.VesselName;
                        DDL.Add(item);
                    }
                return DDL;
            }
        }
    }
}
