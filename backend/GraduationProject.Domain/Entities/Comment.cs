using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GraduationProject.Domain.Entities
{
    public class Comment
    {
        [Key]
        public int CommentId { get; set; }

        public int? PlatformPublicationId { get; set; }

        [StringLength(255)]
        public string ExternalCommentId { get; set; }

        [Required]
        public string CommentText { get; set; }

        public int? ParentCommentId { get; set; }

        public bool IsReplied { get; set; } = false;

        [StringLength(50)]
        public string CommentStatus { get; set; } = "active";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Navigation
        public virtual PlatformPublication PlatformPublication { get; set; }
        public virtual Comment ParentComment { get; set; }
        public virtual ICollection<Comment> Replies { get; set; } = new HashSet<Comment>();
    }
}
