namespace GraduationProject.Application.DTOs.Discount
{
    public class DiscountResponseDto
    {
        public bool IsValid { get; set; }
        public string Code { get; set; } = string.Empty;
        public decimal FinalPrice { get; set; }
        public decimal SavedAmount { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}

