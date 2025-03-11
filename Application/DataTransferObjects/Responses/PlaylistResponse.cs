using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain;

namespace Application.DataTransferObjects.Responses
{
    public class PlaylistResponse
    {
        public required Guid Id { get; set; }
        public required Guid UserId { get; set; }
        public required string Description { get; set; }
        public UserResponseCompactWImage? User { get; set; }
        public required string Name { get; set; }
        public ImageResponse? Image { get; set; } = null;
        public required List<Guid> SongIds { get; set; }
        public List<SongResponse>? Songs { get; set; } = null;
        public required int Duration { get; set; }
        public required int SongCount { get; set; }
        public required Visibility Visibility { get; set; }
    }
}