using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GraduationProject.Domain.Enums;

namespace GraduationProject.Application.DTOs.Subscription
{
    public class ActiveSubscriptionDto
    {
        public int UserId { get; set; }

        public int PlanId { get; set; }

        public SubscriptionStatus Status { get; set; } = default!;

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }
    }
}
