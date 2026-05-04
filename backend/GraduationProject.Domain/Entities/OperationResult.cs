using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GraduationProject.Domain.Entities
{
    public class OperationResult
    {
        [Key]
        public int ResultId { get; set; }

        public int? ExecutionId { get; set; }

        [Required]
        public string ResultData { get; set; }

        public int ResultOrder { get; set; } = 1;
        public bool IsSelected { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public virtual OperationExecution Execution { get; set; }
    }
}
