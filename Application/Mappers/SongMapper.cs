using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Core;
using Application.DataTransferObjects;
using Application.DataTransferObjects.Responses;
using Domain;

namespace Application.Mappers
{
    public class SongMapper
    {
        public static SongResponse MapToResponse(Song song, bool includeArtists = false)
        {
            SongResponse response = new SongResponse
            {
                Id = song.Id,
                Title = song.Title,
                Duration = (int)song.Duration.TotalSeconds,
                AlbumId = (Guid)song.AlbumId,
                PositionInAlbum = song.PositionInAlbum,
            };

            List<Artist> artists = song.SongArtistRelations.Select(ar => ar.Artist).ToList();

            if (includeArtists)
            {
                response.Artists = ArtistMapper.MapToResponseList(artists);
            }

            List<Guid> artistIds = [.. artists.Select(artist => artist.Id)];
            response.ArtistIds = artistIds;

            return response;
        }

        public static List<SongResponse> MapToResponseList(List<Song> songs, bool includeArtists = false)
        {
            return songs.Select(song => MapToResponse(song, includeArtists)).ToList();
        }

        public static SearchResult MapToSearchResult(Song song)
        {
            return new SearchResult
            {
                Id = song.Id,
                Name = song.Title,
                Type = "Album",
                ImageLocation = song.Album?.ImageLocation,
                ParentId = song.Album?.Id
            };
        }

        public static List<SearchResult> MapToSearchResultList(List<Song> songs)
        {
            return songs.Select(MapToSearchResult).ToList();
        }
    }
}