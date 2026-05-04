using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GraduationProject.Domain.Entities
{
    public class OperationPricing
    {
        [Key]
        public int PricingId { get; set; }

        public int? OperationId { get; set; }
        public int? ExecutionMethodId { get; set; }
        public int? ModelId { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal? ProviderCost { get; set; }

        public int? UserPriceCredits { get; set; }

        [StringLength(50)]
        public string UnitType { get; set; }

        public DateTime EffectiveFrom { get; set; } = DateTime.UtcNow;
        public DateTime? EffectiveTo { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public virtual Operation Operation { get; set; }
        public virtual ExecutionMethod ExecutionMethod { get; set; }
        public virtual AIModel Model { get; set; }
    }
}
