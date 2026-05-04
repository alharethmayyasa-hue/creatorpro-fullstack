using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GraduationProject.Domain.Entities
{
    public class AIModel
    {
        [Key]
        public int ModelId { get; set; }

        public int? ProviderId { get; set; }

        [Required]
        [StringLength(100)]
        public string ModelName { get; set; }

        [StringLength(50)]
        public string ModelType { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public virtual AIProvider Provider { get; set; }
        public virtual ICollection<OperationPricing> OperationPricings { get; set; } = new HashSet<OperationPricing>();
        public virtual ICollection<OperationExecution> OperationExecutions { get; set; } = new HashSet<OperationExecution>();
    }
}
