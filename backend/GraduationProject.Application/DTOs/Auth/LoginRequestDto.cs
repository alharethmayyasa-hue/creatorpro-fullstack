using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace GraduationProject.Application.DTOs.Auth
{
    public class LoginRequestDto : IValidatableObject
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public bool RememberMe { get; set; } = false;

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            // 1. التحقق من الإيميل أولاً
            if (string.IsNullOrWhiteSpace(Email))
            {
                yield return new ValidationResult("Email is required.", new[] { nameof(Email) });
                yield break;
            }
            var emailRegex = new Regex(@"^[^\s@]+@[^\s@]+\.[^\s@]+$", RegexOptions.Compiled);
            if (!emailRegex.IsMatch(Email.Trim()))
            {
                yield return new ValidationResult("Invalid email format.", new[] { nameof(Email) });
                yield break;
            }

            // 2. التحقق من الباسورد
            if (string.IsNullOrWhiteSpace(Password))
            {
                yield return new ValidationResult("Password is required.", new[] { nameof(Password) });
                yield break;
            }
            if (Password.Length < 6)
            {
                yield return new ValidationResult("Password must be at least 6 characters.", new[] { nameof(Password) });
                yield break;
            }
        }
    }
}

