namespace GraduationProject.Application.DTOs.Auth
{
    public class AuthResponseDto
    {
        public bool IsAuthenticated { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? Token { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? ExpiresOn { get; set; }
        public string? Email { get; set; }
        public string? UserName { get; set; }
        public List<string>? Roles { get; set; }
    }
}

