using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SeaHawkServices.Domain.Entities
{
    public class UserLoginHistory
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }

        [MaxLength(256)]
        public string UserName { get; set; }

        [MaxLength(50)]
        public string Role { get; set; }

        public DateTime LoginTimeUtc { get; set; }

        // NEW: logout time (nullable)
        public DateTime? LogoutTimeUtc { get; set; }

        // NEW: session id to link login + logout
        [MaxLength(100)]
        public string? SessionId { get; set; }

        // Optional but useful:
        // Manual = user clicked logout
        // Timeout = cookie expired / session ended
        [MaxLength(20)]
        public string? LogoutType { get; set; }

        [MaxLength(50)]
        public string IpAddress { get; set; }

        [MaxLength(1024)]
        public string UserAgent { get; set; }

        public bool IsSuccess { get; set; }

        // Optional navigation
        [ForeignKey(nameof(UserId))]
        public ApplicationUser User { get; set; }

        // Not stored in DB (helper)
        [NotMapped]
        public TimeSpan? SessionDuration =>
            LogoutTimeUtc.HasValue ? LogoutTimeUtc.Value - LoginTimeUtc : null;
    }
}
