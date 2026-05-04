namespace GraduationProject.Application.DTOs.Credit
{
    public class CreditTransactionDto
    {
        public int TransactionId { get; set; }
        public int Amount { get; set; }
        public int BalanceAfter { get; set; }
        public string TransactionType { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}

