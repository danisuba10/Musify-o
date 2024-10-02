using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.DataTransferObjects.Requests
{
    public class AlbumSearchRequest
    {
        public string? Name { get; set; } = null;
        public List<String>? Artists { get; set; } = null;
        public List<String>? Songs { get; set; } = null;
        public bool IncludeSongs { get; set; } = false;
        public bool IncludeArtists { get; set; } = false;
        public bool AllArtistsPresent { get; set; } = false;
    }
}