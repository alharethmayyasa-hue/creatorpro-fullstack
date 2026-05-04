using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GraduationProject.Application.Contracts.Services;
using GraduationProject.Application.DTOs.User;
using System.Security.Claims;

namespace GraduationProject.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IAuthService _authService;


        public ProfileController(IUserService userService, IAuthService authService)
        {
            _userService = userService;
            _authService = authService;
        }

        // Helper Method لتجنب تكرار كود استخراج UserId
        private string GetUserId()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                   ?? throw new UnauthorizedAccessException("User ID not found in token.");
        }


        [HttpGet]
        public async Task<IActionResult> GetProfile()
        {
            try
            {
                var userId = GetUserId();
                var profile = await _userService.GetProfileAsync(userId);

                if (profile == null)
                    return NotFound(new { message = "Profile not found." });

                return Ok(profile);
            }
            catch (Exception ex)
            {
                // يفضل تسجيل الخطأ Log Error هنا
                return StatusCode(500, new { message = "An error occurred while fetching profile.", error = ex.Message });
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateProfile([FromBody] UserProfileDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var userId = GetUserId();
                var result = await _userService.UpdateProfileAsync(userId, model);

                if (!result)
                    return BadRequest(new { message = "Failed to update profile. Please check the data." });

                return Ok(new { message = "Profile updated successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating profile.", error = ex.Message });
            }
        }

        /// <summary>
        /// حذف الحساب نهائياً أو نرمياً (حسب منطق الخدمة) وتسجيل الخروج
        /// </summary>
        [HttpDelete]
        public async Task<IActionResult> DeleteAccount()
        {
            try
            {
                var userId = GetUserId();

                // 1. حذف الحساب عبر خدمة المستخدم
                var deleteResult = await _userService.SoftDeleteUserAsync(userId);

                if (!deleteResult)
                    return BadRequest(new { message = "Failed to delete account." });

                // 2. إبطال التوكن الحالي (Logout) لضمان عدم استخدام الحساب بعد الحذف
                await _authService.LogoutAsync(userId);

                return Ok(new { message = "Account deleted and logged out successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting account.", error = ex.Message });
            }
        }
    }

}

