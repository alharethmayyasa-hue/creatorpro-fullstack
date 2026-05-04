using GraduationProject.Domain.Enums;

namespace GraduationProject.Application.DTOs.Credit
{
    public class AddCreditRequest
    {
        public int UserId { get; set; }
        public int Amount { get; set; }
        public CreditTransactionType Type { get; set; }
        public string? Description { get; set; }
    }
}

