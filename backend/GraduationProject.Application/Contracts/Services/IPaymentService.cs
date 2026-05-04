using GraduationProject.Application.DTOs.Discount;

namespace GraduationProject.Application.Contracts.Services
{
    public interface IPaymentService
    {
        Task<PaymentIntentDto> CreatePaymentIntentAsync(int userId, string? discountCode, decimal amount);
    }
}

