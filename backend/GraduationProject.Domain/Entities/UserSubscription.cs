using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GraduationProject.Domain.Enums;

namespace GraduationProject.Domain.Entities
{
    public class UserSubscription
    {
        [Key]
        public int UserSubscriptionId { get; set; }

        public int UserId { get; set; }
        public int PlanId { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal PaidPrice { get; set; }

        public DateTime StartDate { get; set; } = DateTime.UtcNow;
        public DateTime EndDate { get; set; }

        [StringLength(50)]
        public SubscriptionStatus Status { get; set; } =  SubscriptionStatus.Active;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Navigation Properties
        public virtual User User { get; set; } = default!;
        public virtual SubscriptionPlan Plan { get; set; } = default!;
        public virtual ICollection<Payment> Payments { get; set; } = new HashSet<Payment>();
    }
}
