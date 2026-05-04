using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GraduationProject.Domain.Entities
{
    public class OperationExecution
    {
        [Key]
        public int ExecutionId { get; set; }

        public int? OperationId { get; set; }
        public int? ModelId { get; set; }
        public int? UserId { get; set; }
        public int? PublicationId { get; set; }

        [StringLength(20)]
        public string ExecutionStatus { get; set; }

        public int? CostCredits { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public virtual Operation Operation { get; set; }
        public virtual AIModel Model { get; set; }
        public virtual User User { get; set; }
        public virtual Publication Publication { get; set; }
        public virtual ICollection<CreditTransaction> CreditTransactions { get; set; } = new HashSet<CreditTransaction>();
        public virtual ICollection<OperationResult> OperationResults { get; set; } = new HashSet<OperationResult>();
    }
}
