// Models/User.cs
using System;
using System.ComponentModel.DataAnnotations;

namespace webb_tst_site3.Models
{
    public class User
    {
        public int Id { get; set; }

        [StringLength(50)]
        public string? Username { get; set; }

        public string? PasswordHash { get; set; }

        [Required]
        public string Role { get; set; } = "User"; // "Admin" или "User"

        // Telegram integration
        public long? TelegramId { get; set; }
        public string? TelegramUsername { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? PhotoUrl { get; set; }

        // Auth data
        public string? AuthToken { get; set; }
        public DateTime? AuthTokenExpiry { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime LastLoginAt { get; set; } = DateTime.UtcNow;
    }
}