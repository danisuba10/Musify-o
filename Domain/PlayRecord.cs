using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Domain
{
    public abstract class PlayRecord
    {
        public Guid Id { get; set; }
        [Required]
        public PlayedItemType PlayedItemType { get; set; }
        [Required]
        public Guid PlayedItemId { get; set; }
        [Required]
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;
        [Required]
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }

    public class SongPlayRecord : PlayRecord { }
    public class AlbumPlayRecord : PlayRecord { }
    public class PlaylistPlayRecord : PlayRecord { }


}