using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SeaHawkServices.Domain.Entities.Enums;


namespace SeaHawkServices.Domain.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; }
        public Role Role { get; set; }
        public bool IsApprove { get; set; }
        public bool IsDeleted { get; set; }
        // 🔹 NEW: what role they selected during registration
        public Role? RegisteredRole { get; set; } // Management or Vessel

        public DateTime? LastLoginAtUtc { get; set; }
        public DateTime? LastActivityAtUtc { get; set; }
    
        public string CompanyName { get; set; }
        public string Address { get; set; }
    }
}
