using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GraduationProject.Domain.Entities
{
    public class AIProvider
    {
        [Key]
        public int ProviderId { get; set; }

        [Required]
        [StringLength(100)]
        public string ProviderName { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public virtual ICollection<AIModel> AIModels { get; set; } = new HashSet<AIModel>();
    }
}
