using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Application.DataTransferObjects.Requests
{
    public class AddSongRequest
    {
        public string Title { get; set; }
        public int Duration { get; set; }
        public Guid? AlbumId { get; set; }
        public int? PositionInAlbum { get; set; }
        public List<Guid>? ArtistIds { get; set; }
        public IFormFile? SoundFile { get; set; }
    }
}