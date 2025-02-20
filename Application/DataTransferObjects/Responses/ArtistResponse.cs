using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.DataTransferObjects.Responses
{
    public class ArtistResponse
    {
        public Guid? Id { get; set; } = null;
        public string ImageLocation { get; set; } = "";
        public string Name { get; set; } = "";
        public ImageResponse? Image { get; set; } = null;
        public List<SearchResult>? TopAlbums { get; set; }
    }
}