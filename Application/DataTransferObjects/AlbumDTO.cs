using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.DataTransferObjects
{
    public class AlbumDTO
    {
        public string AlbumName { get; set; } = string.Empty;
        public string? ImageLocation { get; set; } = null;
        public Guid? Id { get; set; }
        public List<SongDTO> Songs { get; set; } = new List<SongDTO>();
        public List<ArtistDTO> Artists { get; set; } = new List<ArtistDTO>();
    }
}