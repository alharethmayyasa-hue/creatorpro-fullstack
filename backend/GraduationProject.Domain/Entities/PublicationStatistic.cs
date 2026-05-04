using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GraduationProject.Domain.Entities
{
    public class PublicationStatistic
    {
        [Key]
        public int StatisticId { get; set; }

        public int? PlatformPublicationId { get; set; }

        public int ViewsCount { get; set; } = 0;
        public int LikesCount { get; set; } = 0;
        public int SharesCount { get; set; } = 0;
        public int CommentsCount { get; set; } = 0;

        public DateTime CheckedAt { get; set; } = DateTime.UtcNow;

        public virtual PlatformPublication PlatformPublication { get; set; }
    }
}
