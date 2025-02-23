using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain
{
    public class PlaylistSongRelation
    {
        public Guid PlaylistId { get; set; }
        public Playlist Playlist { get; set; } = null!;
        public Guid SongId { get; set; }
        public Song Song { get; set; } = null!;
        public int PositionInPlaylist { get; set; }
    }
}