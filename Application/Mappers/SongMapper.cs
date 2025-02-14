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
        public static SongResponse MapToResponse(Song song)
        {
            SongResponse response = new SongResponse
            {
                Id = song.Id,
                Title = song.Title,
                Duration = song.Duration,
                AlbumId = song.AlbumId,
                Album = song.Album,
                PositionInAlbum = song.PositionInAlbum,
            };

            List<Artist> artists = song.SongArtistRelations.Select(ar => ar.Artist).ToList();
            response.Artists = ArtistMapper.MapToResponseList(artists);

            foreach (Artist artist in artists)
            {
                response.ArtistIds.Add(artist.Id);
            }

            return response;
        }

        public static List<SongResponse> MapToResponseList(List<Song> songs)
        {
            return songs.Select(MapToResponse).ToList();
        }

        public static SearchResult MapToSearchResult(Song song)
        {
            return new SearchResult
            {
                Id = song.Id,
                Name = song.Title,
                Type = "Album",
                ImageLocation = song.Album?.ImageLocation
            };
        }

        public static List<SearchResult> MapToSearchResultList(List<Song> songs)
        {
            return songs.Select(MapToSearchResult).ToList();
        }
    }
}