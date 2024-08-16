using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain
{
    public class SongArtistRelation
    {
        public Guid SongId { get; set; }
        public Song Song { get; set; } = null!;
        public Guid ArtistId { get; set; }
        public Artist Artist { get; set; } = null!;
    }
}