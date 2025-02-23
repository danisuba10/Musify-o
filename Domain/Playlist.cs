using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Domain
{
    public class Playlist
    {
        public Guid Id { get; set; }
        [Required]
        [StringLength(256)]
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        [Required]
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;
        public ICollection<PlaylistSongRelation> PlaylistSongRelations { get; set; } = new List<PlaylistSongRelation>();
        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        [Required]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}