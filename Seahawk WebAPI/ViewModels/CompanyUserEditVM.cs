using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using static SeaHawkServices.Domain.Entities.Enums;

namespace SeaHawkServices.Web.ViewModels
{
    public class CompanyUserEditVM
    {
        // This is still the "current opened mapping row id" (used only to open Edit screen)
        public int CompanyUserId { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        // ✅ list of current assigned companies for this user (to render in table)
        public List<AssignedCompanyRowVM> AssignedCompanies { get; set; } = new();

        // ✅ multi-select checkboxes (same pattern as SelectedVesselIds)
        public List<int> SelectedCompanyIds { get; set; } = new();

        // ✅ checkbox options list (same idea as VesselOptions)
        public List<SelectListItem> CompanyOptions { get; set; } = new();

        // Display-only
        public string UserName { get; set; } = string.Empty;

        // Editable (kept as-is)
        [Required]
        public string Name { get; set; } = string.Empty;

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        public string? PhoneNumber { get; set; }

        [Required]
        public Role Role { get; set; }

        public bool IsApprove { get; set; }

        public DateTime? LastLoginAtUtc { get; set; }
        public DateTime? LastActivityAtUtc { get; set; }
    }

    public class AssignedCompanyRowVM
    {
        public int CompanyUserMappingId { get; set; }
        public int CompanyId { get; set; }
        public string CompanyName { get; set; } = string.Empty;
    }
}
