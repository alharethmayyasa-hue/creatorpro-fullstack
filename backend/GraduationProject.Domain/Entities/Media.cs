using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GraduationProject.Domain.Entities
{
    public class Media
    {
        [Key]
        public int MediaId { get; set; }

        [Required]
        [StringLength(500)]
        public string MediaUrl { get; set; }

        [Required]
        [StringLength(20)]
        public string MediaType { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public virtual ICollection<PublicationMedia> PublicationMedia { get; set; } = new HashSet<PublicationMedia>();
    }
}
