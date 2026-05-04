using Microsoft.AspNetCore.Mvc;
using GraduationProject.Application.Contracts.Services;
using GraduationProject.Domain.Enums;
using GraduationProject.Application.DTOs.Credit;
using GraduationProject.Domain.Entities;
using GraduationProject.API.Controllers.Base;

namespace GraduationProject.API.Controllers
{
    [Route("api/[controller]")]
    public class CreditController : BaseController
    {
        private readonly ICreditService _creditService;

        public CreditController(ICreditService creditService)
        {
            _creditService = creditService;
        }

        [HttpGet("balance/{userId}")]
        public async Task<IActionResult> GetBalance(int userId)
        {
            var balance = await _creditService.GetBalanceAsync(userId);
            return Success(new { balance });
        }

        [HttpGet("transactions/{userId}")]
        public async Task<IActionResult> GetTransactions(int userId, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            var transactions = await _creditService.GetTransactionsAsync(userId, pageSize, page);
            return Success(transactions);
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddCredits([FromBody] AddCreditRequest request)
        {
            var success = await _creditService.AddCreditsAsync(request.UserId, request.Amount, request.Type, request.Description);
            if (success)
                return Success(new { message = "تم شحن الرصيد بنجاح" });
            return Fail("خطأ في الشحن");
        }

        [HttpPost("deduct")]
        public async Task<IActionResult> DeductCredits([FromBody] DeductCreditRequest request)
        {
            var success = await _creditService.DeductCreditsAsync(request.UserId, request.Amount, request.Type, request.ExecutionId);
            if (success)
                return Success(new { message = "تم خصم الرصيد بنجاح" });
            return Fail("رصيد غير كافي أو خطأ");
        }

        [HttpPost("refund")]
        public async Task<IActionResult> Refund([FromBody] RefundRequest request)
        {
            var success = await _creditService.RefundAsync(request.TransactionId, request.Reason);
            if (success)
                return Success(new { message = "تم الاسترداد بنجاح" });
            return Fail("خطأ في الاسترداد");
        }
    }
}
