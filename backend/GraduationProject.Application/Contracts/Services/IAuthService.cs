using GraduationProject.Application.DTOs.Auth;

namespace GraduationProject.Application.Contracts.Services
{
    public interface IAuthService
    {
        Task<AuthResponseDto> RegisterAsync(RegisterRequestDto dto);
        Task<AuthResponseDto> LoginAsync(LoginRequestDto dto);
        Task<AuthResponseDto> GetRefreshTokenAsync(RefreshTokenRequestDto dto);
        Task<bool> LogoutAsync(string userId);
    }
}

