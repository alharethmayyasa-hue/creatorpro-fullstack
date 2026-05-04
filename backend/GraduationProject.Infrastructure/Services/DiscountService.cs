using GraduationProject.Application.Contracts.Services;
using GraduationProject.Application.DTOs.Discount;
using GraduationProject.Domain.Entities;
using GraduationProject.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GraduationProject.Infrastructure.Services
{
    public class DiscountService : IDiscountService
    {
        private readonly AppDbContext _context;

        public DiscountService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<DiscountResponseDto> ValidateDiscountAsync(string code, decimal originalPrice)
        {
            var discount = await _context.DiscountCodes
                .FirstOrDefaultAsync(d => d.Code.ToLower() == code.ToLower() && d.IsActive);

            if (discount == null)
            {
                return new DiscountResponseDto
                {
                    IsValid = false,
                    Message = "Invalid discount code",
                    FinalPrice = originalPrice
                };
            }

            if (discount.EndDate.HasValue && discount.EndDate < DateTime.UtcNow)
            {
                return new DiscountResponseDto
                {
                    IsValid = false,
                    Message = "Discount code expired",
                    FinalPrice = originalPrice
                };
            }

            if (discount.UsedCount >= discount.MaxUsage)
            {
                return new DiscountResponseDto
                {
                    IsValid = false,
                    Message = "Discount code usage limit reached",
                    FinalPrice = originalPrice
                };
            }

            decimal finalPrice = originalPrice;
            decimal savedAmount = 0;

            if (discount.DiscountType.ToLower() == "percentage")
            {
                savedAmount = originalPrice * (discount.DiscountValue / 100);
                finalPrice = originalPrice - savedAmount;
            }
            else if (discount.DiscountType.ToLower() == "fixed")
            {
                savedAmount = discount.DiscountValue;
                finalPrice = originalPrice - savedAmount;
            }

            if (finalPrice < 0) finalPrice = 0;

            return new DiscountResponseDto
            {
                IsValid = true,
                Code = code,
                FinalPrice = finalPrice,
                SavedAmount = savedAmount,
                Message = "Discount applied successfully"
            };
        }

        public async Task<int> CreateDiscountAsync(CreateDiscountDto dto)
        {
            var discount = new DiscountCode
            {
                Code = dto.Code,
                Description = dto.Description,
                DiscountType = dto.DiscountType,
                DiscountValue = dto.DiscountValue,
                MaxUsage = dto.MaxUsage,
                EndDate = dto.EndDate,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.DiscountCodes.Add(discount);
            await _context.SaveChangesAsync();
            return discount.DiscountId;
        }

        public async Task<IEnumerable<DiscountResponseDto>> GetAllDiscountsAsync()
        {
            var discounts = await _context.DiscountCodes
                .Select(d => new DiscountResponseDto
                {
                    IsValid = d.IsActive && (d.EndDate == null || d.EndDate > DateTime.UtcNow) && d.UsedCount < d.MaxUsage,
                    Code = d.Code,
                    FinalPrice = 0,
                    SavedAmount = 0,
                    Message = d.Description ?? string.Empty
                })
                .ToListAsync();

            return discounts;
        }

        public async Task DeactivateDiscountAsync(int id)
        {
            var discount = await _context.DiscountCodes.FindAsync(id);
            if (discount != null)
            {
                discount.IsActive = false;
                discount.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }
    }
}

