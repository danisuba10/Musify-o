using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.DataTransferObjects;
using Application.DataTransferObjects.Responses;
using AutoMapper.Configuration.Conventions;
using Domain;

namespace Application.Mappers
{
    public class AlbumMapper
    {
        public static AlbumResponse MapToResponse(Album album)
        {
            AlbumResponse response = new AlbumResponse
            {
                Id = album.Id,
                Name = album.Name,
                Year = album.ReleaseYear,
                ImageLocation = album.ImageLocation
            };

            response.ArtistIds = album.AlbumArtistRelations
                .Select(relation => relation.ArtistId)
                .ToList();
            response.SongIds = album.Songs
                .Select(song => song.Id)
                .ToList();
            response.Artists = ArtistMapper.MapToResponseList(
                album.AlbumArtistRelations
                    .Select(aar => aar.Artist)
                    .ToList()
            );
            response.Songs = SongMapper.MapToResponseList(
                album.Songs.ToList()
            );
            return response;
        }

        public static List<AlbumResponse> MapToResponseList(List<Album> albums)
        {
            return albums.Select(MapToResponse).ToList();
        }

        public static SearchResult MapToSearchResult(Album album)
        {
            return new SearchResult
            {
                Id = album.Id,
                Name = album.Name,
                Type = "Album",
                ImageLocation = album.ImageLocation
            };
        }

        public static List<SearchResult> MapToSearchResultList(List<Album> albums)
        {
            return albums.Select(MapToSearchResult).ToList();
        }
    }
}