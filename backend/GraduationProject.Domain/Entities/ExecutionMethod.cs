using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GraduationProject.Domain.Entities
{
    public class ExecutionMethod
    {
        [Key]
        public int MethodId { get; set; }

        [Required]
        [StringLength(20)]
        public string Name { get; set; }

        public bool IsActive { get; set; } = true;

        public virtual ICollection<OperationPricing> OperationPricings { get; set; } = new HashSet<OperationPricing>();
    }
}
