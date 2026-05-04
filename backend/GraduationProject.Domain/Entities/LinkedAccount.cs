using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GraduationProject.Domain.Entities
{
    public class LinkedAccount
    {
        [Key]
        public int LinkedAccountId { get; set; }

        public int? UserId { get; set; }

        [Required]
        [StringLength(100)]
        public string PlatformName { get; set; }

        [StringLength(255)]
        public string ExternalAccountId { get; set; }

        [StringLength(255)]
        public string PlatformUsername { get; set; }

        [Required]
        public string AccessToken { get; set; }

        public string RefreshToken { get; set; }
        public DateTime? TokenExpiry { get; set; }
        public string ProviderResponse { get; set; }

        [StringLength(500)]
        public string Scopes { get; set; }

        [StringLength(50)]
        public string AccountStatus { get; set; } = "Active";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Navigation
        public virtual User User { get; set; }
        public virtual ICollection<PlatformPublication> PlatformPublications { get; set; } = new HashSet<PlatformPublication>();
    }
}
