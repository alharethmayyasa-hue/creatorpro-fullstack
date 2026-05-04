using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GraduationProject.Domain.Entities
{
    public class Notification
    {
        [Key]
        public int NotificationId { get; set; }

        [Required]
        [StringLength(255)]
        public string Title { get; set; }

        [Required]
        [StringLength(500)]
        public string BodyText { get; set; }

        [StringLength(50)]
        public string NotificationType { get; set; }

        [StringLength(20)]
        public string Priority { get; set; } = "Medium";

        [StringLength(10)]
        public string Language { get; set; } = "ar";

        [StringLength(50)]
        public string ReferenceType { get; set; }

        public int? ReferenceId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public virtual ICollection<UserNotification> UserNotifications { get; set; } = new HashSet<UserNotification>();
    }
}
