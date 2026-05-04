using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraduationProject.Application.DTOs.Plan;

public class PlanResponseDto
{
    public int PlanId { get; set; }
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int CreditsAmount { get; set; }
    public int DurationDays { get; set; }
    public bool IsTrial { get; set; }
    public bool IsActive { get; set; }
public string? Features { get; set; }
}
