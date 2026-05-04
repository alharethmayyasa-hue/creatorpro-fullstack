using GraduationProject.Application.DTOs.Credit;
using GraduationProject.Domain.Enums;

namespace GraduationProject.Application.Contracts.Services
{
    public interface ICreditService
    {
        Task<int> GetBalanceAsync(int userId);
        Task<IEnumerable<CreditTransactionDto>> GetTransactionsAsync(int userId, int pageSize, int page);
        Task<bool> AddCreditsAsync(int userId, int amount, CreditTransactionType type, string? description);
        Task<bool> DeductCreditsAsync(int userId, int amount, CreditTransactionType type, int? executionId);
        Task<bool> RefundAsync(int transactionId, string? reason);
    }
}

