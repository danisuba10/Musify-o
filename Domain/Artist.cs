using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public class Artist
    {
        public Guid Id { get; set; }
        [Required]
        [StringLength(256)]
        public string Name { get; set; } = string.Empty;
        [Required]
        public string ImageLocation { get; set; } = string.Empty;
        public ICollection<SongArtistRelation> SongArtistRelations { get; set; } = new List<SongArtistRelation>();
        public ICollection<AlbumArtistRelation> AlbumArtistRelations { get; set; } = new List<AlbumArtistRelation>();
    }
}