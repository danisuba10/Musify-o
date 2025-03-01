using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.DataTransferObjects.Requests
{
    public class UpdateSongRequest
    {
        public required Guid Id { get; set; }
        public string? Title { get; set; }
        public int? Duration { get; set; }
        public Guid? AlbumId { get; set; }
        public int? PositionInAlbum { get; set; }
        public List<Guid>? ArtistIds { get; set; }
    }
}