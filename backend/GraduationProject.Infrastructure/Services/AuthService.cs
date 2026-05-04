using GraduationProject.Application.Contracts.Services;
using GraduationProject.Application.DTOs.Auth;
using GraduationProject.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;

namespace GraduationProject.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _configuration;
        private readonly ITokenService _tokenService;

        public AuthService(UserManager<User> userManager, IConfiguration configuration, ITokenService tokenService)
        {
            _userManager = userManager;
            _configuration = configuration;
            _tokenService = tokenService;
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto dto)
        {
            // تم نقل التحقق من البيانات إلى DTO باستخدام IValidatableObject
            // التحقق الوحيد المتبقي هو تكرار الإيميل في قاعدة البيانات
            if (await _userManager.FindByEmailAsync(dto.Email) != null)
            {
                return new AuthResponseDto { IsAuthenticated = false, Message = "Email already exists." };
            }

            var user = new User
            {
                Email = dto.Email,
                UserName = dto.UserName ?? dto.Email,
                FullName = dto.FullName,
                CreatedAt = DateTime.UtcNow,
                AccountStatus = "Active"
            };

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
            {
                return new AuthResponseDto
                {
                    IsAuthenticated = false,
                    Message = string.Join(", ", result.Errors.Select(e => e.Description))
                };
            }

            await _userManager.AddToRoleAsync(user, "User");

            var token = await _tokenService.GenerateJwtTokenAsync(user);
            var refreshToken = await _tokenService.GenerateRefreshTokenAsync();

            user.LastLogin = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);

            return new AuthResponseDto
            {
                IsAuthenticated = true,
                Message = "Registration successful",
                Email = user.Email,
                UserName = user.UserName,
                Token = token,
                RefreshToken = refreshToken,
                ExpiresOn = DateTime.UtcNow.AddDays(1),
                Roles = new List<string> { "User" }
            };
        }

        public async Task<AuthResponseDto> LoginAsync(LoginRequestDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, dto.Password))
            {
                return new AuthResponseDto { IsAuthenticated = false, Message = "Invalid email or password." };
            }

            var token = await _tokenService.GenerateJwtTokenAsync(user, dto.RememberMe);
            var refreshToken = await _tokenService.GenerateRefreshTokenAsync();

            user.LastLogin = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);

            var roles = await _userManager.GetRolesAsync(user);

            // RememberMe: 30 days | Normal: 1 day
            var expiry = dto.RememberMe ? DateTime.UtcNow.AddDays(30) : DateTime.UtcNow.AddDays(1);

            return new AuthResponseDto
            {
                IsAuthenticated = true,
                Message = "Login successful",
                Email = user.Email,
                UserName = user.UserName,
                Token = token,
                RefreshToken = refreshToken,
                ExpiresOn = expiry,
                Roles = roles.ToList()
            };
        }

        public async Task<AuthResponseDto> GetRefreshTokenAsync(RefreshTokenRequestDto dto)
        {
            // TODO: Implement refresh token validation and generation
            return new AuthResponseDto
            {
                IsAuthenticated = false,
                Message = "Refresh token functionality not fully implemented."
            };
        }

        public async Task<bool> LogoutAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return false;

            user.LastLogout = DateTime.UtcNow;
            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }
    }
}

