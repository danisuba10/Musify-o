using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.DataTransferObjects.Requests
{
    public class PlaylistSongRequest
    {
        public required Guid PlaylistId { get; set; }
        public required Guid SongId { get; set; }
    }
}