using GraduationProject.Application.Common.Exceptions;
using GraduationProject.Application.Contracts.Services;
using GraduationProject.Application.DTOs.Subscription;
using GraduationProject.Domain.Entities;
using GraduationProject.Domain.Enums;
using GraduationProject.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GraduationProject.Infrastructure.Services;

public class SubscriptionService : ISubscriptionService
{
    private readonly AppDbContext _context;

    public SubscriptionService(AppDbContext context)
    {
        _context = context;
    }

public async Task<UserSubscriptionResponseDto> SubscribeUserAsync(int userId, int planId)
    {
        var plan = await _context.SubscriptionPlans.FindAsync(planId);
        if (plan == null || !plan.IsActive)
            throw new BadRequestException("Invalid or inactive plan");

        var hasActiveSubscription = await _context.UserSubscriptions
            .AnyAsync(x =>
                x.UserId == userId &&
                x.Status == SubscriptionStatus.Active &&
                x.EndDate > DateTime.UtcNow);

        if (hasActiveSubscription)
            throw new BadRequestException("You already have an active subscription");

        if (plan.IsTrial)
        {
            var usedTrial = await _context.UserSubscriptions
                .Include(x => x.Plan)
                .AnyAsync(x => x.UserId == userId && x.Plan.IsTrial);

            if (usedTrial)
                throw new BadRequestException("Trial already used");
        }

        var sub = new UserSubscription
        {
            UserId = userId,
            PlanId = planId,
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(plan.DurationDays),
            Status = SubscriptionStatus.Active,
            PaidPrice = plan.IsTrial ? 0 : plan.Price,
            CreatedAt = DateTime.UtcNow
        };

        await _context.UserSubscriptions.AddAsync(sub);
        await _context.SaveChangesAsync();

        return new UserSubscriptionResponseDto
        {
            SubscriptionId = sub.UserSubscriptionId,
            PlanName = plan.Name,
            StartDate = sub.StartDate,
            EndDate = sub.EndDate,
            IsActive = true
        };
    }

    public async Task<UserSubscriptionResponseDto> CancelSubscriptionAsync(int userId)
    {
        var sub = await _context.UserSubscriptions
            .Include(s => s.Plan)
            .FirstOrDefaultAsync(s =>
                s.UserId == userId &&
                s.Status == SubscriptionStatus.Active);

        if (sub == null)
            throw new NotFoundException("No active subscription");

        if (sub.EndDate <= DateTime.UtcNow)
            throw new BadRequestException("Subscription already expired");

        sub.Status = SubscriptionStatus.Cancelled;
        sub.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return new UserSubscriptionResponseDto
        {
            SubscriptionId = sub.UserSubscriptionId,
            PlanName = sub.Plan.Name,
            StartDate = sub.StartDate,
            EndDate = sub.EndDate,
            IsActive = false
        };
    }

    public async Task<bool> IsSubscriptionValidAsync(int userId)
    {
        return await _context.UserSubscriptions
            .AnyAsync(x =>
                x.UserId == userId &&
                x.Status == SubscriptionStatus.Active &&
                x.EndDate > DateTime.UtcNow);
    }

    public async Task<UserSubscriptionResponseDto?> GetSubscriptionStatusAsync(int userId)
    {
        var sub = await _context.UserSubscriptions
            .Include(x => x.Plan)
            .FirstOrDefaultAsync(x =>
                x.UserId == userId &&
                x.Status == SubscriptionStatus.Active &&
                x.EndDate > DateTime.UtcNow);

        if (sub == null)
            return null;

        return new UserSubscriptionResponseDto
        {
            SubscriptionId = sub.UserSubscriptionId,
            PlanName = sub.Plan.Name,
            StartDate = sub.StartDate,
            EndDate = sub.EndDate,
            IsActive = true
        };
    }

    public async Task<ActiveSubscriptionDto?> GetUserActiveSubscriptionAsync(int userId)
    {
        return await _context.UserSubscriptions
            .Where(x =>
                x.UserId == userId &&
                x.Status == SubscriptionStatus.Active &&
                x.EndDate > DateTime.UtcNow)
            .Select(x => new ActiveSubscriptionDto
            {
                UserId = x.UserId,
                PlanId = x.PlanId,
                Status = x.Status,
                StartDate = x.StartDate,
                EndDate = x.EndDate
            })
            .FirstOrDefaultAsync();
    }
}
