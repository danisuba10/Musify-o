using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.DataTransferObjects;
using Application.DataTransferObjects.Requests;
using Application.DataTransferObjects.Responses;
using Domain;

namespace Application.Mappers
{
    public class PlaylistMapper
    {
        public static PlaylistResponse MapToResponse(Playlist playlist, ImageAccent? playlistImageAccent, User? user, ImageAccent? userImageAccent)
        {
            PlaylistResponse response = new PlaylistResponse
            {
                Id = playlist.Id,
                Name = playlist.Name,
                UserId = playlist.UserId,
                Description = playlist.Description,
                Visibility = playlist.Visibility,
                SongIds = playlist.PlaylistSongRelations.Select(relation => relation.SongId).ToList(),
                SongCount = playlist.PlaylistSongRelations.Count,
                Duration = (int)playlist.PlaylistSongRelations
                    .Where(relation => relation.Song != null)
                    .Select(relation => relation.Song.Duration.TotalSeconds).Sum(),
                Songs = SongMapper.MapToResponseList(playlist.PlaylistSongRelations
                    .Where(relation => relation.Song != null)
                    .Select(relation => new SongMapperDTO
                    {
                        Song = relation.Song,
                        PositionInPlaylist = relation.PositionInPlaylist
                    }).ToList(), true),
            };

            if (playlistImageAccent != null)
            {
                response.Image = new ImageResponse();
                response.Image.ImageLocation = playlistImageAccent.ImagePath;
                response.Image.LowColor = playlistImageAccent.LowAccent;
                response.Image.MiddleColor = playlistImageAccent.MiddleAccent;
                response.Image.HighColor = playlistImageAccent.HighAccent;
            }

            if (user != null)
            {

                response.User = UserMapper.mapToResponseCompactWithImage(user, userImageAccent);
            }

            return response;
        }

        public static List<PlaylistResponse> MapToResponseList(List<PlaylistWithImageAccent> playlists, User user)
        {
            return playlists.Select(playlist => MapToResponse(playlist.Playlist, playlist.ImageAccent, user, null)).ToList();
        }

        public static SearchResult MapToSearchResult(Playlist playlist)
        {
            return new SearchResult
            {
                Id = playlist.Id,
                Name = playlist.Name,
                Type = "Playlist",
                ImageLocation = playlist.ImageLocation
            };
        }

        public static List<SearchResult> MapToSearchResultList(List<Playlist> playlists)
        {
            return playlists.Select(MapToSearchResult).ToList();
        }
    }
}