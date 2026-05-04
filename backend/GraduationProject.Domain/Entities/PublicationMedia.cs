using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GraduationProject.Domain.Entities
{
    public class PublicationMedia
    {
        [Key]
        public int PublicationMediaId { get; set; }

        public int? PublicationId { get; set; }
        public int? MediaId { get; set; }

        public int OrderIndex { get; set; } = 1;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public virtual Publication Publication { get; set; }
        public virtual Media Media { get; set; }
    }
}
