namespace GraduationProject.Application.DTOs.User
{
    public class UserProfileDto
    {
        public string FullName { get; set; } = string.Empty;
        public string? ProfilePictureUrl { get; set; }
        public string? Email { get; set; }
    }
}

