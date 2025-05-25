using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Domain
{
    public class User
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public required string Email { get; set; }
        [Required]
        public string ImageLocation { get; set; } = string.Empty;
        public string? PasswordHash { get; set; }
        public string Role { get; set; } = "User";
        public string DisplayName { get; set; } = String.Empty;
        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        [Required]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public ICollection<Playlist> Playlists { get; set; } = new List<Playlist>();
        public bool IsTwoFactorEnabled { get; set; }
        public string? TwoFactorSecret { get; set; }
        public string? TwoFactorRecoveryCodes { get; set; }
    }
}