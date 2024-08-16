using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain
{
    public class AlbumArtistRelation
    {
        public Guid AlbumId { get; set; }
        public Album Album { get; set; } = null!;
        public Guid ArtistId { get; set; }
        public Artist Artist { get; set; } = null!;
    }
}