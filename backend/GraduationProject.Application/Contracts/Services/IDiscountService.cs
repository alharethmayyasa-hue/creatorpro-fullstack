using GraduationProject.Application.DTOs.Discount;

namespace GraduationProject.Application.Contracts.Services
{
    public interface IDiscountService
    {
        Task<DiscountResponseDto> ValidateDiscountAsync(string code, decimal originalPrice);
        Task<int> CreateDiscountAsync(CreateDiscountDto dto);
        Task<IEnumerable<DiscountResponseDto>> GetAllDiscountsAsync();
        Task DeactivateDiscountAsync(int id);
    }
}

