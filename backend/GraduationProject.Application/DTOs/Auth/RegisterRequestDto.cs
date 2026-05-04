using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace GraduationProject.Application.DTOs.Auth
{
    public class RegisterRequestDto : IValidatableObject
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
        public string? UserName { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            // 1. التحقق من الاسم أولاً
            if (string.IsNullOrWhiteSpace(FullName))
            {
                yield return new ValidationResult("Full Name is required.", new[] { nameof(FullName) });
                yield break;
            }
            if (FullName.Trim().Length < 2)
            {
                yield return new ValidationResult("Full Name must be at least 2 characters.", new[] { nameof(FullName) });
                yield break;
            }

            // 2. التحقق من الإيميل
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

            // 3. التحقق من الباسورد
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

            // 4. التحقق من تأكيد الباسورد
            if (string.IsNullOrWhiteSpace(ConfirmPassword))
            {
                yield return new ValidationResult("Confirm Password is required.", new[] { nameof(ConfirmPassword) });
                yield break;
            }
            if (!string.Equals(Password, ConfirmPassword))
            {
                yield return new ValidationResult("Password and Confirm Password do not match.", new[] { nameof(ConfirmPassword) });
                yield break;
            }

            // 5. التحقق من اليوزر نيم
            if (string.IsNullOrWhiteSpace(UserName))
            {
                yield return new ValidationResult("UserName is required.", new[] { nameof(UserName) });
                yield break;
            }
            if (UserName.Trim().Length < 3)
            {
                yield return new ValidationResult("UserName must be at least 3 characters.", new[] { nameof(UserName) });
                yield break;
            }
        }
    }
}

