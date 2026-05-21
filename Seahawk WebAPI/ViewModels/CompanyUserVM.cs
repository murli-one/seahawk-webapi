using Data;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.Diagnostics;
using SeaHawkServices.Domain.Entities;

namespace SeaHawkServices.Web.ViewModels
{
    public class CompanyUserVM
    {
        /* List */
        public List<Company> Companies { get; set; }
        public List<ApplicationUser> ApplicationUsers { get; set; }
        public List<CompanyUser> CompanyUsers { get; set; } = new List<CompanyUser>();

        /* int */
        public int? SelectedCompanyId{ get; set; }
        public string SelectedUser { get; set; } = string.Empty;
        public string SelectedUserEmail { get; set; } = string.Empty;
        public string SelectedUserRole { get; set; } = string.Empty;

        // 🔍 Filters
        public string? FilterUserName { get; set; }
        public string? FilterCompany{ get; set; }
        public string? FilterVesselName{ get; set; }
        /* DateTime */
        public DateTime? FilterByFromDate { get; set; }
        public DateTime? FilterByToDate { get; set; }

        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 15;
        public int TotalCount { get; set; }

        public int TotalPages =>
            PageSize == 0 ? 0 : (int)Math.Ceiling((double)TotalCount / PageSize);

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

        public List<SelectListItem> CompaniesDDL
        {
            get
            {
                var DDL = new List<SelectListItem>();
                var itemx = new SelectListItem();
                itemx.Value = "0";
                itemx.Text = "";
                DDL.Add(itemx);

                if (Companies != null)
                    foreach (var i in Companies)
                    {
                        var item = new SelectListItem();
                        item.Value = i.Id.ToString();
                        item.Text = i.CompanyName;
                        DDL.Add(item);
                    }
                return DDL;
            }
        }
    }
}
