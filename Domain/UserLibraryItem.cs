using System;
using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public class UserLibraryItem
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required]
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;
        [Required]
        public Guid ItemId { get; set; }
        [Required]
        [StringLength(10)]
        public string ItemType { get; set; } = string.Empty;
        [Required]
        public DateTime SavedAt { get; set; } = DateTime.UtcNow;
    }
}
