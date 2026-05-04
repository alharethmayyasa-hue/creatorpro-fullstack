namespace GraduationProject.Application.DTOs.Discount
{
    public class CreateDiscountDto
    {
        public string Code { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string DiscountType { get; set; } = string.Empty;
        public decimal DiscountValue { get; set; }
        public int MaxUsage { get; set; } = 1;
        public DateTime? EndDate { get; set; }
    }
}

