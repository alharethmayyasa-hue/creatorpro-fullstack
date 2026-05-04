using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Infrastructure;
using GraduationProject.API.Controllers.Base;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Threading.Tasks;
using GraduationProject.Infrastructure.Persistence;
using GraduationProject.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using GraduationProject.Application.Contracts.Services;
using GraduationProject.Domain.Enums;
using System;
using System.Collections.Generic;

namespace GraduationProject.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StripeWebhookController : BaseController
    {
        private readonly string _webhookSecret;
        private readonly AppDbContext _context;
        private readonly ILogger<StripeWebhookController> _logger;
        private readonly IDiscountService _discountService;

        public StripeWebhookController(IConfiguration configuration, AppDbContext context, ILogger<StripeWebhookController> logger, IDiscountService discountService)
        {
            _webhookSecret = configuration["Stripe:WebhookSecret"]!;
            _context = context;
            _logger = logger;
            _discountService = discountService;
        }

        [HttpPost]
        public async Task<IActionResult> Index()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();

            Event stripeEvent;
            try
            {
                stripeEvent = EventUtility.ConstructEvent(
                    json, 
                    Request.Headers["Stripe-Signature"], 
                    _webhookSecret
                );
            }
            catch (StripeException ex)
            {
                _logger.LogWarning("Invalid Stripe signature: {Message}", ex.Message);
                return BadRequest();
            }

_logger.LogInformation("Stripe Webhook | Event: {Type}", stripeEvent.Type);

            if (stripeEvent.Type == "payment_intent.succeeded")
            {
                var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
                if (paymentIntent == null) 
                {
                    _logger.LogWarning("Invalid payment_intent.succeeded event data");
                    return BadRequest();
                }
                await HandlePaymentIntentSucceeded(paymentIntent);
            }
            else if (stripeEvent.Type == "payment_intent.payment_failed")
            {
                var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
                var paymentIntentId = paymentIntent?.Id ?? "unknown";
                _logger.LogError("❌ PaymentIntent FAILED: {Id}", paymentIntentId);
            }

            return Ok();
        }

        private async Task HandlePaymentIntentSucceeded(PaymentIntent paymentIntent)
        {
            var paymentIntentId = paymentIntent.Id;

            // Idempotency
            if (await _context.Payments.AnyAsync(p => p.ExternalTransactionId == paymentIntentId))
            {
                _logger.LogInformation("PaymentIntent {Id} already processed", paymentIntentId);
                return;
            }

            // Metadata
            var metadata = paymentIntent.Metadata;
            if (!int.TryParse(metadata.GetValueOrDefault("userId", "0"), out int userId) || userId <= 0)
            {
                _logger.LogWarning("Missing userId metadata: {Id}", paymentIntentId);
                return;
            }

            string discountCode = metadata.GetValueOrDefault("discountCode", "");

            // Payment
            var payment = new Payment
            {
                UserId = userId,
                ExternalTransactionId = paymentIntentId,
                Amount = paymentIntent.Amount / 100m,
                Currency = paymentIntent.Currency,
                PaymentMethod = paymentIntent.PaymentMethodId,
                Status = "succeeded",
                CreatedAt = DateTime.UtcNow
            };
            await _context.Payments.AddAsync(payment);
            await _context.SaveChangesAsync();

            decimal finalAmount = payment.Amount;
            decimal originalAmount = payment.Amount;
            if (!string.IsNullOrEmpty(discountCode))
            {
                var discountResp = await _discountService.ValidateDiscountAsync(discountCode, originalAmount);
                if (discountResp.IsValid)
                {
                    finalAmount = discountResp.FinalPrice;
                    payment.Amount = finalAmount;

                    var discount = await _context.DiscountCodes
                        .FirstOrDefaultAsync(d => d.Code.ToLower() == discountCode.ToLower());
                    if (discount != null)
                    {
                        discount.UsedCount++;
                        var pd = new PaymentDiscount
                        {
                            PaymentId = payment.PaymentId,
                            DiscountId = discount.DiscountId,
                            AppliedAmount = discountResp.SavedAmount
                        };
                        await _context.PaymentDiscounts.AddAsync(pd);
                    }
                }
            }
            await _context.SaveChangesAsync();

            // Add credits (configurable amount or from discount metadata)
            var creditsAmount = int.Parse(metadata.GetValueOrDefault("creditsAmount", "1000")); // Default 1000 credits
            var credit = new CreditTransaction
            {
                UserId = userId,
                Amount = creditsAmount,
                BalanceAfter = creditsAmount,
                TransactionType = "Payment",
                ReferenceId = paymentIntentId
            };
            await _context.CreditTransactions.AddAsync(credit);
            await _context.SaveChangesAsync();

            _logger.LogInformation("✅ PaymentIntent {Id} processed for User {UserId}", paymentIntentId, userId);
        }

        private async Task HandlePaymentIntentFailed(string paymentIntentId)
        {
            _logger.LogError("❌ PaymentIntent FAILED: {Id}", paymentIntentId);
        }
    }
}

