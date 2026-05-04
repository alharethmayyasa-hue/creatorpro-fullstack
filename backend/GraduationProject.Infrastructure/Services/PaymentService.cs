using GraduationProject.Application.Contracts.Services;
using GraduationProject.Application.DTOs.Discount;
using Stripe;

namespace GraduationProject.Infrastructure.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IDiscountService _discountService;

        public PaymentService(IDiscountService discountService)
        {
            _discountService = discountService;
        }

        public async Task<PaymentIntentDto> CreatePaymentIntentAsync(int userId, string? discountCode, decimal amount)
        {
            decimal finalAmount = amount;

            if (!string.IsNullOrEmpty(discountCode))
            {
                var discountResult = await _discountService.ValidateDiscountAsync(discountCode, amount);
                if (discountResult.IsValid)
                {
                    finalAmount = discountResult.FinalPrice;
                }
            }

            var options = new PaymentIntentCreateOptions
            {
                Amount = (long)(finalAmount * 100),
                Currency = "usd",
                Metadata = new Dictionary<string, string>
                {
                    { "userId", userId.ToString() },
                    { "discountCode", discountCode ?? "" },
                    { "creditsAmount", "1000" }
                }
            };

            var service = new PaymentIntentService();
            var intent = await service.CreateAsync(options);

            return new PaymentIntentDto
            {
                ClientSecret = intent.ClientSecret,
                PaymentIntentId = intent.Id
            };
        }
    }
}

