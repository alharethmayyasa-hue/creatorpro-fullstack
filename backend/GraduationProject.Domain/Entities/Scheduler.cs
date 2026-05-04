using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GraduationProject.Domain.Entities
{
    public class Scheduler
    {
        [Key]
        public int ScheduleId { get; set; }

        public int? PlatformPublicationId { get; set; }

        [Required]
        public DateTime PublishDateTime { get; set; }

        [Required]
        [StringLength(20)]
        public string ScheduleStatus { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public virtual PlatformPublication PlatformPublication { get; set; }
    }
}
