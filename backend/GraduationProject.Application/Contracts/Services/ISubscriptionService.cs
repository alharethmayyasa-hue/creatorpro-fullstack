using System.Threading.Tasks;
using GraduationProject.Application.DTOs.Subscription;

namespace GraduationProject.Application.Contracts.Services
{
    public interface ISubscriptionService
    {
        Task<UserSubscriptionResponseDto> SubscribeUserAsync(int userId, int planId);

        Task<UserSubscriptionResponseDto> CancelSubscriptionAsync(int userId);

Task<bool> IsSubscriptionValidAsync(int userId);

        Task<UserSubscriptionResponseDto?> GetSubscriptionStatusAsync(int userId);

        Task<ActiveSubscriptionDto?> GetUserActiveSubscriptionAsync(int userId);
    }
}
