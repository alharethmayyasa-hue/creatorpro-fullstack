using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GraduationProject.Domain.Entities
{
    public class Publication
    {
        [Key]
        public int PublicationId { get; set; }

        public int? UserId { get; set; }

        [Required]
        [StringLength(50)]
        public string PublicationType { get; set; }

        public string ContentText { get; set; }

        public int? Duration { get; set; }

        [Required]
        [StringLength(50)]
        public string PublicationStatus { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Navigation
        public virtual User User { get; set; }
        public virtual ICollection<PlatformPublication> PlatformPublications { get; set; } = new HashSet<PlatformPublication>();
        public virtual ICollection<PublicationMedia> PublicationMedia { get; set; } = new HashSet<PublicationMedia>();
        public virtual ICollection<PublicationReplyRule> PublicationReplyRules { get; set; } = new HashSet<PublicationReplyRule>();
        public virtual ICollection<OperationExecution> OperationExecutions { get; set; } = new HashSet<OperationExecution>();
    }
}
