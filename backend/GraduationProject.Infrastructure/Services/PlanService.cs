using GraduationProject.Application.Common.Exceptions;
using GraduationProject.Application.Contracts.Services;
using GraduationProject.Application.DTOs.Plan;
using GraduationProject.Application.Mappings;
using GraduationProject.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GraduationProject.Infrastructure.Services;

public class PlanService : IPlanService
{
    private readonly AppDbContext _context;

    public PlanService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<PlanResponseDto> CreatePlanAsync(PlanCreateDto dto)
    {
        var plan = dto.ToEntity();

        await _context.SubscriptionPlans.AddAsync(plan);
        await _context.SaveChangesAsync();

        return plan.ToDto();
    }

    public async Task<IEnumerable<PlanResponseDto>> GetAllPlansAsync()
    {
        var plans = await _context.SubscriptionPlans
            .AsNoTracking()
            .ToListAsync();

        return plans.Select(p => p.ToDto());
    }

    public async Task<PlanResponseDto?> GetPlanByIdAsync(int id)
    {
        var plan = await _context.SubscriptionPlans.FindAsync(id);
        return plan?.ToDto();
    }

    public async Task<PlanResponseDto?> UpdatePlanAsync(int id, PlanUpdateDto dto)
    {
        var plan = await _context.SubscriptionPlans.FindAsync(id);

        if (plan == null)
            throw new NotFoundException("Plan not found");

        plan.Name = dto.Name;
        plan.Description = dto.Description;
        plan.Price = dto.Price;
        plan.CreditsAmount = dto.CreditsAmount;
        plan.DurationDays = dto.DurationDays;
        plan.IsTrial = dto.IsTrial;
        plan.IsActive = dto.IsActive;
        plan.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return plan.ToDto();
    }

    public async Task DeletePlanAsync(int id)
    {
        var plan = await _context.SubscriptionPlans.FindAsync(id);

        if (plan == null)
            throw new NotFoundException("Plan not found");

        var hasSubscriptions = await _context.UserSubscriptions
            .AnyAsync(x => x.PlanId == id);

        if (hasSubscriptions)
            throw new BadRequestException("Cannot delete plan because it has active or historical subscriptions");

        _context.SubscriptionPlans.Remove(plan);
        await _context.SaveChangesAsync();
    }

    public async Task<PlanResponseDto?> TogglePlanStatusAsync(int id)
    {
        var plan = await _context.SubscriptionPlans.FindAsync(id);

        if (plan == null)
            throw new NotFoundException("Plan not found");

        plan.IsActive = !plan.IsActive;
        plan.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return plan.ToDto();
    }
}
