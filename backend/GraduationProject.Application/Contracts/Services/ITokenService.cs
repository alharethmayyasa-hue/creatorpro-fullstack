using GraduationProject.Domain.Entities;

namespace GraduationProject.Application.Contracts.Services
{
    public interface ITokenService
    {
        Task<string> GenerateJwtTokenAsync(User user, bool rememberMe = false);
        Task<string> GenerateRefreshTokenAsync();
        Task<bool> ValidateTokenAsync(string token);
    }
}

