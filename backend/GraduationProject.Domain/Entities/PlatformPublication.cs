using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GraduationProject.Domain.Entities
{
    public class PlatformPublication
    {
        [Key]
        public int PlatformPublicationId { get; set; }

        public int? PublicationId { get; set; }
        public int? LinkedAccountId { get; set; }

        [StringLength(255)]
        public string ExternalPostId { get; set; }

        [StringLength(500)]
        public string ExternalPostUrl { get; set; }

        [Required]
        [StringLength(50)]
        public string Status { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Navigation
        public virtual Publication Publication { get; set; }
        public virtual LinkedAccount LinkedAccount { get; set; }
        public virtual ICollection<Comment> Comments { get; set; } = new HashSet<Comment>();
        public virtual ICollection<PublicationStatistic> PublicationStatistics { get; set; } = new HashSet<PublicationStatistic>();
        public virtual ICollection<Scheduler> Schedulers { get; set; } = new HashSet<Scheduler>();
    }
}
