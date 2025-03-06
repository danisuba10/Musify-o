using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.DataTransferObjects.Responses;
using Domain;

namespace Application.Mappers
{
    public class PlaylistMapper
    {
        public static PlaylistResponse MapToResponse(Playlist playlist, ImageAccent? imageAccent, User? user)
        {
            PlaylistResponse response = new PlaylistResponse
            {
                Id = playlist.Id,
                Name = playlist.Name,
                UserId = playlist.UserId,
                SongIds = playlist.PlaylistSongRelations.Select(relation => relation.SongId).ToList(),
                SongCount = playlist.PlaylistSongRelations.Count,
                Duration = (int)playlist.PlaylistSongRelations.Select(relation => relation.Song.Duration.TotalSeconds).Sum(),
                Songs = SongMapper.MapToResponseList(playlist.PlaylistSongRelations.Select(relation => relation.Song).ToList(), true),
            };

            if (imageAccent != null)
            {
                response.Image = new ImageResponse();
                response.Image.LowColor = imageAccent.LowAccent;
                response.Image.MiddleColor = imageAccent.MiddleAccent;
                response.Image.HighColor = imageAccent.HighAccent;
            }

            if (user != null)
            {
                response.User = UserMapper.mapToResponseCompact(user);
            }

            return response;
        }
    }
}