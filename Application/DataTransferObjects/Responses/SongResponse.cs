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
        public Guid? Id = null;
        public String Title { get; set; } = "";
        public TimeSpan Duration { get; set; } = TimeSpan.FromSeconds(0);
        public Guid? AlbumId { get; set; } = null;
        public Album Album { get; set; } = null!;
        public int PositionInAlbum { get; set; } = -1;
        public List<ArtistResponse> Artists { get; set; } = new List<ArtistResponse>();
        public List<Guid> ArtistIds { get; set; } = new List<Guid>();
    }
}