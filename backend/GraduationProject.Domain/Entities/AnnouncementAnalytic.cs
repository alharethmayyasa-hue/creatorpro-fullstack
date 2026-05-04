using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GraduationProject.Domain.Entities
{
    public class AnnouncementAnalytic
    {
        [Key]
        public int AnnouncementAnalyticsId { get; set; }

        public int? AnnouncementId { get; set; }

        public int LikesCount { get; set; } = 0;
        public int ClicksCount { get; set; } = 0;

        public DateTime RecordedAt { get; set; } = DateTime.UtcNow;

        public virtual Announcement Announcement { get; set; }
    }
}
