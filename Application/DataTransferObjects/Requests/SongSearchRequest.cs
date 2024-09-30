using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.DataTransferObjects.Requests
{
    public class SongSearchRequest
    {
        public string? Title { get; set; }
        public List<String>? Artists { get; set; }
        public string? AlbumName { get; set; }
        public bool IncludeAlbum { get; set; } = false;
        public bool IncludeArtists { get; set; } = false;
    }
}