using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GraduationProject.Domain.Entities
{
    public class CreditTransaction
    {
        [Key]
        public int TransactionId { get; set; }

        public int? UserId { get; set; }
        public int? ExecutionId { get; set; }

        public int Amount { get; set; }
        public int BalanceAfter { get; set; }

        [Required]
        [StringLength(50)]
        public string TransactionType { get; set; }

        [StringLength(255)]
        public string ReferenceId { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public virtual User User { get; set; }
        public virtual OperationExecution Execution { get; set; }
    }
}
