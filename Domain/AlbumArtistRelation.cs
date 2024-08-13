using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain
{
    public class AlbumArtistRelation
    {
        public int AlbumId { get; set; }
        public Album Album { get; set; } = null!;
        public int ArtistId { get; set; }
        public Artist Artist { get; set; } = null!;
    }
}