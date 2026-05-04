using GraduationProject.Application.Contracts.Repositories;
using GraduationProject.Domain.Entities;
using GraduationProject.Domain.Enums;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace GraduationProject.Infrastructure.BackgroundServices;

public class SubscriptionCleanupService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<SubscriptionCleanupService> _logger;

    public SubscriptionCleanupService(IServiceProvider serviceProvider, ILogger<SubscriptionCleanupService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Subscription Cleanup Service is starting.");

        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Checking for expired subscriptions at: {time}", DateTimeOffset.Now);

            using (var scope = _serviceProvider.CreateScope())
            {
                var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                var subscriptionRepo = unitOfWork.Repository<UserSubscription>();

                // جلب كل الاشتراكات النشطة التي انتهى تاريخها
                var expiredSubscriptions = subscriptionRepo.Query(track: true)
                    .Where(s => s.Status == SubscriptionStatus.Active && s.EndDate < DateTime.UtcNow)
                    .ToList();

                if (expiredSubscriptions.Any())
                {
                    foreach (var sub in expiredSubscriptions)
                    {
                        sub.Status = SubscriptionStatus.Expired;
                        sub.UpdatedAt = DateTime.UtcNow;
                        _logger.LogInformation($"Subscription {sub.UserSubscriptionId} marked as Expired.");
                    }

                    await unitOfWork.SaveChangesAsync();
                }
            }

            // الانتظار لمدة 24 ساعة قبل الفحص التالي
            await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
        }
    }
}