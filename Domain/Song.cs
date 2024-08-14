using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public class Song
    {
        public int Id { get; set; }
        [Required]
        [StringLength(256)]
        public string Title { get; set; } = string.Empty;
        [Required]
        public TimeSpan Duration { get; set; } = TimeSpan.FromSeconds(0);
        public int? AlbumId { get; set; } = null;
        public Album Album { get; set; } = null!;
        public int PositionInAlbum { get; set; } = -1;
        public ICollection<SongArtistRelation> SongArtistRelations { get; set; } = new List<SongArtistRelation>();

        public Song() { }
        public Song(string title, TimeSpan duration)
        {
            Title = title;
            Duration = duration;
        }
        public string GetDurationFormatted()
        {
            return Duration.ToString(@"hh\:mm\:ss");
        }

    }
}