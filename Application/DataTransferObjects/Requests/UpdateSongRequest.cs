using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.DataTransferObjects.Requests
{
    public class UpdateSongRequest
    {
        public Guid ID;
        public string? Title { get; set; }
        public int? Duration { get; set; }
        public Guid? AlbumId { get; set; }
        public int? PositionInAlbum { get; set; }
    }
}