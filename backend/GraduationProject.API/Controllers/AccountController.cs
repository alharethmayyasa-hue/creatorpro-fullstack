using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GraduationProject.API.Controllers.Base;
using GraduationProject.Application.Contracts.Services;
using GraduationProject.Application.DTOs.Auth;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using GraduationProject.Domain.Entities;


namespace GraduationProject.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IUserService _userService;
        public AccountController(UserManager<User> usermanager, IConfiguration configuration, IAuthService authService, IUserService userService)
        {
            _userManager = usermanager;
            this.configuration = configuration;
            _authService = authService;
            _userService = userService;

        }
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration configuration;
        private readonly IAuthService _authService;
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto model)
        {
            // 1. التحقق من صحة البيانات المدخلة (ModelState)
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // 2. استدعاء خدمة المصادقة لتسجيل المستخدم
            // هذه الخدمة تتحقق داخلياً من تكرار الإيميل/الاسم، وتنشئ المستخدم، وتولد التوكنات
            var result = await _authService.RegisterAsync(model);

            // 3. التحقق من نتيجة العملية
            if (!result.IsAuthenticated)
            {
                // إذا فشلت العملية (مثلاً الإيميل مكرر)، نرجع رسالة الخطأ
                return BadRequest(new { message = result.Message });
            }

            // 4. إرجاع الاستجابة الناجحة التي تحتوي على الـ Tokens وبيانات المستخدم
            return Ok(result);
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.LoginAsync(model);

            if (!result.IsAuthenticated)
                return Unauthorized(new { message = result.Message });

            return Ok(result);
        }
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDto model)
        {
            var result = await _authService.GetRefreshTokenAsync(model);

            if (!result.IsAuthenticated)
                return Unauthorized(result.Message);

            return Ok(result);
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
                return BadRequest(new { message = "Invalid user ID." });

            var result = await _authService.LogoutAsync(userId);

            if (result)
                return Ok(new { message = "Logged out successfully." });

            return BadRequest(new { message = "Logout failed." });
        }
    }
}

