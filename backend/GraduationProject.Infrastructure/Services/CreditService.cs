using GraduationProject.Application.Contracts.Services;
using GraduationProject.Application.DTOs.Credit;
using GraduationProject.Application.DTOs.Payment;
using GraduationProject.Domain.Entities;
using GraduationProject.Domain.Enums;
using GraduationProject.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GraduationProject.Infrastructure.Services;

public class CreditService : ICreditService
{
    private readonly AppDbContext _context;

    public CreditService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<int> GetBalanceAsync(int userId)
    {
        var lastTransaction = await _context.CreditTransactions
            .Where(t => t.UserId == userId)
            .OrderByDescending(t => t.CreatedAt)
            .FirstOrDefaultAsync();

        return lastTransaction?.BalanceAfter ?? 0;
    }

    public async Task<IEnumerable<CreditTransactionDto>> GetTransactionsAsync(int userId, int pageSize, int page)
    {
        var query = _context.CreditTransactions
            .Where(t => t.UserId == userId)
            .OrderByDescending(t => t.CreatedAt);

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(t => new CreditTransactionDto
            {
                TransactionId = t.TransactionId,
                Amount = t.Amount,
                BalanceAfter = t.BalanceAfter,
                TransactionType = t.TransactionType,
                CreatedAt = t.CreatedAt
            })
            .ToListAsync();

        return items;
    }

    public async Task<bool> AddCreditsAsync(int userId, int amount, CreditTransactionType type, string? description)
    {
        if (amount <= 0) return false;

        var lastTransaction = await _context.CreditTransactions
            .Where(t => t.UserId == userId)
            .OrderByDescending(t => t.CreatedAt)
            .FirstOrDefaultAsync();

        var previousBalance = lastTransaction?.BalanceAfter ?? 0;

        var transaction = new CreditTransaction
        {
            UserId = userId,
            Amount = amount,
            BalanceAfter = previousBalance + amount,
            TransactionType = type.ToString(),
            Description = description,
            CreatedAt = DateTime.UtcNow
        };

        await _context.CreditTransactions.AddAsync(transaction);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeductCreditsAsync(int userId, int amount, CreditTransactionType type, int? executionId)
    {
        if (amount <= 0) return false;

        var lastTransaction = await _context.CreditTransactions
            .Where(t => t.UserId == userId)
            .OrderByDescending(t => t.CreatedAt)
            .FirstOrDefaultAsync();

        var previousBalance = lastTransaction?.BalanceAfter ?? 0;

        if (previousBalance < amount) return false;

        var transaction = new CreditTransaction
        {
            UserId = userId,
            Amount = -amount,
            BalanceAfter = previousBalance - amount,
            TransactionType = type.ToString(),
            ExecutionId = executionId == 0 ? null : executionId,
            CreatedAt = DateTime.UtcNow
        };

        await _context.CreditTransactions.AddAsync(transaction);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> RefundAsync(int transactionId, string? reason)
    {
        var transaction = await _context.CreditTransactions.FindAsync(transactionId);
        if (transaction == null || transaction.Amount >= 0) return false;

        var refundAmount = -transaction.Amount;
        var lastTransaction = await _context.CreditTransactions
            .Where(t => t.UserId == transaction.UserId)
            .OrderByDescending(t => t.CreatedAt)
            .FirstOrDefaultAsync();

        var previousBalance = lastTransaction?.BalanceAfter ?? 0;

        var refundTransaction = new CreditTransaction
        {
            UserId = transaction.UserId,
            Amount = refundAmount,
            BalanceAfter = previousBalance + refundAmount,
            TransactionType = "Refund",
            Description = reason,
            CreatedAt = DateTime.UtcNow
        };

        await _context.CreditTransactions.AddAsync(refundTransaction);
        await _context.SaveChangesAsync();
        return true;
    }
}

