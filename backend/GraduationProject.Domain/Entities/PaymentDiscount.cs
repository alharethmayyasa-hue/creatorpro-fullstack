using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GraduationProject.Domain.Entities
{
    public class PaymentDiscount
    {
        [Key]
        public int PaymentDiscountId { get; set; }

        public int? PaymentId { get; set; }
        public int? DiscountId { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal AppliedAmount { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public virtual Payment Payment { get; set; }
        public virtual DiscountCode DiscountCode { get; set; }
    }
}
