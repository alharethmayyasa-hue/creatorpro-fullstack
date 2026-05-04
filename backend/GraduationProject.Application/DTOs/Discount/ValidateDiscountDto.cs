namespace GraduationProject.Application.DTOs.Discount
{
    public class ValidateDiscountDto
    {
        public string Code { get; set; } = string.Empty;
        public decimal OriginalPrice { get; set; }
    }
}

