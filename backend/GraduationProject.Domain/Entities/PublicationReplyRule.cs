using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GraduationProject.Domain.Entities
{
    public class PublicationReplyRule
    {
        public int PublicationId { get; set; }

        public int ReplyRuleId { get; set; }

        // Navigation
        public virtual Publication Publication { get; set; } = default!;
        public virtual ReplyRule ReplyRule { get; set; }=default!;
    }
}
