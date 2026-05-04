using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GraduationProject.Domain.Entities
{
    public class Announcement
    {
        [Key]
        public int AnnouncementId { get; set; }

        public int? UserId { get; set; }

        [Required]
        [StringLength(255)]
        public string Title { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        [Required]
        public string ContentBody { get; set; }

        [StringLength(500)]
        public string ImageUrl { get; set; }

        [StringLength(500)]
        public string SourceUrl { get; set; }

        [Required]
        [StringLength(50)]
        public string AnnouncementType { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; } = 0;

        [StringLength(30)]
        public string Status { get; set; } = "draft";

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public virtual User User { get; set; }
        public virtual ICollection<AnnouncementAnalytic> AnnouncementAnalytics { get; set; } = new HashSet<AnnouncementAnalytic>();
    }
}
