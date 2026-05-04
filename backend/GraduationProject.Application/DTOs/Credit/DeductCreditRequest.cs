using GraduationProject.Domain.Enums;

namespace GraduationProject.Application.DTOs.Credit
{
    public class DeductCreditRequest
    {
        public int UserId { get; set; }
        public int Amount { get; set; }
        public CreditTransactionType Type { get; set; }
        public int? ExecutionId { get; set; }
    }
}

