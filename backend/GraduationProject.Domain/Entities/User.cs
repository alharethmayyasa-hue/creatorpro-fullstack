using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GraduationProject.Domain.Entities
{
    public class User : IdentityUser<int>
    {
        [Key]
        [Column("UserId")]
        public override int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string FullName { get; set; } = string.Empty;

        [StringLength(500)]
        public string ProfilePictureUrl { get; set; } = string.Empty;

        [StringLength(50)]
        public string RegistrationSource { get; set; } = string.Empty;

        [StringLength(50)]
        public string UserRole { get; set; } = "User";

        public DateTime? LastLogin { get; set; }
        public DateTime? LastLogout { get; set; }

        [StringLength(50)]
        public string AccountStatus { get; set; } = "Active";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Navigation Properties
        public virtual ICollection<UserSubscription> UserSubscriptions { get; set; } = new HashSet<UserSubscription>();
        public virtual ICollection<LinkedAccount> LinkedAccounts { get; set; } = new HashSet<LinkedAccount>();
        public virtual ICollection<Publication> Publications { get; set; } = new HashSet<Publication>();
        public virtual ICollection<Announcement> Announcements { get; set; } = new HashSet<Announcement>();
        public virtual ICollection<ReplyRule> ReplyRules { get; set; } = new HashSet<ReplyRule>();
        public virtual ICollection<OperationExecution> OperationExecutions { get; set; } = new HashSet<OperationExecution>();
        public virtual ICollection<CreditTransaction> CreditTransactions { get; set; } = new HashSet<CreditTransaction>();
        public virtual ICollection<UserNotification> UserNotifications { get; set; } = new HashSet<UserNotification>();
        public virtual ICollection<Payment> Payments { get; set; } = new HashSet<Payment>();
    }
}

