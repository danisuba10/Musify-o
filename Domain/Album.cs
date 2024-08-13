using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public class Album
    {
        public int Id { get; set; }
        [Required]
        [StringLength(256)]
        public string Name { get; set; } = string.Empty;
        [Required]
        public string ImageLocation { get; set; } = string.Empty;
        public ICollection<Song> Songs { get; set; } = new List<Song>();
        public ICollection<AlbumArtistRelation> AlbumArtistRelations { get; set; } = new List<AlbumArtistRelation>();
    }
}