using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraduationProject.Application.DTOs.Subscription;

public class UserSubscriptionResponseDto
{
    public int SubscriptionId { get; set; }
    public string PlanName { get; set; } = default!; 
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
public bool IsActive { get; set; }
           
}
