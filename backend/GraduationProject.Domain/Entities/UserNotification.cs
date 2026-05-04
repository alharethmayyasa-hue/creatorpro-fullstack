using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GraduationProject.Domain.Entities
{
    public class UserNotification
    {
        [Key]
        public int UserNotificationId { get; set; }

        public int? UserId { get; set; }
        public int? NotificationId { get; set; }

        public bool IsRead { get; set; } = false;
        public DateTime? ReadAt { get; set; }
        public bool IsPushed { get; set; } = false;
        public DateTime? UpdatedAt { get; set; }

        public virtual User User { get; set; }
        public virtual Notification Notification { get; set; }
    }
}
