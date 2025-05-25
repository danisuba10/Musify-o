using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain;

namespace Application.DataTransferObjects.Responses
{
    public class SongResponse
    {
        public Guid Id { get; set; }
        public String Title { get; set; } = "";
        public int Duration { get; set; } = 0;
        public int PositionInAlbum { get; set; } = -1;
        public Guid AlbumId { get; set; }
        public List<Guid> ArtistIds { get; set; } = new List<Guid>();
        public List<ArtistResponse>? Artists { get; set; } = new List<ArtistResponse>();

    }
}