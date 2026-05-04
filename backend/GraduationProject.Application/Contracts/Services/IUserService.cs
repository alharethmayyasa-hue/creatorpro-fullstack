using GraduationProject.Application.DTOs.User;

namespace GraduationProject.Application.Contracts.Services
{
    public interface IUserService
    {
        Task<UserProfileDto?> GetProfileAsync(string userId);
        Task<bool> UpdateProfileAsync(string userId, UserProfileDto dto);
        Task<bool> SoftDeleteUserAsync(string userId);
    }
}

