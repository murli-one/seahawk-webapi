using Microsoft.AspNetCore.Mvc.Rendering;
using static SeaHawkServices.Domain.Entities.Enums;

namespace SeaHawkServices.Web.ViewModels
{
    public class VesselUserEditVM
    {
        public int VesselUserId { get; set; }
        public string UserId { get; set; } = string.Empty;

        // ✅ Selected Vessel (for adding new vessel)
        //public int SelectedVesselId { get; set; }

        public List<int> SelectedVesselIds { get; set; } = new();


        // ✅ Dropdown data (available vessels)
        public List<SelectListItem> VesselOptions { get; set; } = new();

        // ✅ NEW: already assigned vessels list
        public List<AssignedVesselVM> AssignedVessels { get; set; } = new();

        // Read-only display (can keep / optional)
        public string VesselName { get; set; } = string.Empty;
        public string IMONumber { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }

        public Role Role { get; set; }
        public bool IsApprove { get; set; }

        public DateTime? LastLoginAtUtc { get; set; }
        public DateTime? LastActivityAtUtc { get; set; }
    }

    // ✅ NEW: row model for assigned vessels
    public class AssignedVesselVM
    {
        public int VesselUserMappingId { get; set; }   // mapping table id (VesselUser.Id)
        public int VesselId { get; set; }
        public string VesselName { get; set; } = string.Empty;
        public string? IMONumber { get; set; }
    }
}
