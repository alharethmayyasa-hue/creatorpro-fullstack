namespace GraduationProject.Application.DTOs.Payment
{
    public class PaymentStatusDto
    {
        public int PaymentId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string DiscountCode { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}

