using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GraduationProject.Domain.Entities
{
    public class Payment
    {
        [Key]
        public int PaymentId { get; set; }

        public int? UserId { get; set; }

        [StringLength(255)]
        public string ExternalTransactionId { get; set; }

        public int? UserSubscriptionId { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [StringLength(10)]
        public string Currency { get; set; } = "USD";

        [StringLength(50)]
        public string PaymentMethod { get; set; }

        [Required]
        [StringLength(50)]
        public string Status { get; set; }

        [StringLength(500)]
        public string ProviderResponse { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public virtual User User { get; set; }
        public virtual UserSubscription UserSubscription { get; set; }
        public virtual ICollection<PaymentDiscount> PaymentDiscounts { get; set; } = new HashSet<PaymentDiscount>();
    }
}
