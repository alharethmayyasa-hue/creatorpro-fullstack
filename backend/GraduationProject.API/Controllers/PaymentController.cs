using Microsoft.AspNetCore.Mvc;
using GraduationProject.API.Controllers.Base;
using GraduationProject.Infrastructure.Persistence;
using GraduationProject.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using GraduationProject.Application.DTOs.Payment;
using Stripe;

namespace GraduationProject.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : BaseController
    {
        private readonly AppDbContext _context;

        public PaymentController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("{paymentIntentId}")]
        public async Task<IActionResult> GetPaymentStatus(string paymentIntentId)
        {
            var payment = await _context.Payments
                .Include(p => p.PaymentDiscounts)
                .ThenInclude(pd => pd.DiscountCode)
                .FirstOrDefaultAsync(p => p.ExternalTransactionId == paymentIntentId);

            if (payment == null)
            {
                return NotFound("Payment not found");
            }

            // Fallback to Stripe API if status is pending/unknown
            if (payment.Status == "pending")
            {
                var service = new PaymentIntentService();
                try
                {
                    var stripePayment = await service.GetAsync(payment.ExternalTransactionId);
                    payment.Status = stripePayment.Status;
                }
                catch (Stripe.StripeException)
                {
                    // Keep DB status if Stripe API fails
                }
            }

            var discountCode = payment.PaymentDiscounts.FirstOrDefault()?.DiscountCode?.Code ?? "";

            var result = new PaymentStatusDto
            {
                PaymentId = payment.PaymentId,
                Amount = payment.Amount,
                Currency = payment.Currency,
                Status = payment.Status,
                DiscountCode = discountCode,
                CreatedAt = payment.CreatedAt
            };

            return Success(result);
        }
    }
}
