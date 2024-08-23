using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.DataTransferObjects
{
    public class SongDTO
    {
        public Guid? Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public TimeSpan Duration { get; set; } = TimeSpan.FromSeconds(0);
        public int PositionInAlbum { get; set; } = -1;
        public List<ArtistDTO> Artists { get; set; } = new List<ArtistDTO>();
    }
}