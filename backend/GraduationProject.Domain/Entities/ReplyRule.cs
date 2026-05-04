using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GraduationProject.Domain.Entities
{
    public class ReplyRule
    {
        [Key]
        public int ReplyRuleId { get; set; }

        public int? UserId { get; set; }

        [Required]
        [StringLength(255)]
        public string Keyword { get; set; }

        [Required]
        public string ReplyText { get; set; }

        public int Priority { get; set; } = 1;
        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public virtual User User { get; set; }
        public virtual ICollection<PublicationReplyRule> PublicationReplyRules { get; set; } = new HashSet<PublicationReplyRule>();
    }
}
