using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GraduationProject.API.Controllers.Base;
using GraduationProject.Application.Contracts.Services;
using GraduationProject.Application.DTOs.Discount;
using GraduationProject.Application.DTOs.Plan;

namespace GraduationProject.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DiscountController : BaseController
    {
        private readonly IDiscountService _discountService;
        private readonly IPaymentService _paymentService;

        public DiscountController(IDiscountService discountService, IPaymentService paymentService)
        {
            _discountService = discountService;
            _paymentService = paymentService;
        }

[HttpPost("validate")]
        [AllowAnonymous]
        public async Task<IActionResult> ValidateDiscount([FromBody] ValidateDiscountDto dto)
        {
            var result = await _discountService.ValidateDiscountAsync(dto.Code, dto.OriginalPrice);
            return Success<DiscountResponseDto>(result);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateDiscount([FromBody] CreateDiscountDto dto)
        {
            try
            {
                var id = await _discountService.CreateDiscountAsync(dto);
                return CreatedResponse(id, "Discount created successfully");
            }
            catch (Exception ex)
            {
                return Fail(ex.Message);
            }
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllDiscounts()
        {
            var result = await _discountService.GetAllDiscountsAsync();
            return Success(result, "Discounts retrieved successfully");
        }

        [HttpPut("deactivate/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeactivateDiscount(int id)
        {
            try
            {
                await _discountService.DeactivateDiscountAsync(id);
                return Success<object>(null, "Discount deactivated successfully");
            }
            catch (Exception ex)
            {
                return Fail(ex.Message);
            }
        }

        [HttpPost("payment-intent")]
        [AllowAnonymous]
        public async Task<IActionResult> CreatePaymentIntent([FromQuery] int userId, [FromQuery] string? discountCode, [FromQuery] decimal amount)
        {
            var result = await _paymentService.CreatePaymentIntentAsync(userId, discountCode, amount);
            return Success<PaymentIntentDto>(result, "Payment intent created - Webhook will handle success");
        }
    }
}