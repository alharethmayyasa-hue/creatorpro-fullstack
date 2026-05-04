using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GraduationProject.Domain.Entities
{
    public class DiscountCode
    {
        [Key]
        public int DiscountId { get; set; }

        [Required]
        [StringLength(50)]
        public string Code { get; set; }

        [StringLength(255)]
        public string Description { get; set; }

        [Required]
        [StringLength(20)]
        public string DiscountType { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal DiscountValue { get; set; }

        public int MaxUsage { get; set; } = 1;
        public int UsedCount { get; set; } = 0;

        public DateTime StartDate { get; set; } = DateTime.UtcNow;
        public DateTime? EndDate { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public virtual ICollection<PaymentDiscount> PaymentDiscounts { get; set; } = new HashSet<PaymentDiscount>();
    }
}
