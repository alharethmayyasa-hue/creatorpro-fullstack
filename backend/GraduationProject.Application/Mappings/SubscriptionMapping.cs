using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GraduationProject.Domain.Entities;
using GraduationProject.Application.DTOs.Plan;

namespace GraduationProject.Application.Mappings;

public static class SubscriptionMapping
{
    // تحويل من Entity لـ DTO (للعرض)
    public static PlanResponseDto? ToDto(this SubscriptionPlan? entity)
    {
        if (entity == null) return null;

        return new PlanResponseDto
        {
            PlanId = entity.PlanId,
            Name = entity.Name,
            Price = entity.Price,
            CreditsAmount = entity.CreditsAmount,
            DurationDays = entity.DurationDays,
            Description = entity.Description,
            IsTrial = entity.IsTrial,
            IsActive = entity.IsActive,
            Features = entity.Features
        };
    }

    // تحويل من CreateDto لـ Entity (للحفظ)
public static SubscriptionPlan ToEntity(this PlanCreateDto dto)
    {
        if (dto == null) throw new ArgumentNullException(nameof(dto));

        return new SubscriptionPlan
        {
            Name = dto.Name,
            Description = dto.Description,
            Price = dto.Price,
            CreditsAmount = dto.CreditsAmount,
            DurationDays = dto.DurationDays,
            IsTrial = dto.IsTrial,
            Features = dto.Features,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
    }
}
