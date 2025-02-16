using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.DataTransferObjects;
using Application.DataTransferObjects.Responses;
using AutoMapper.Configuration.Conventions;
using Domain;
using SixLabors.ImageSharp;

namespace Application.Mappers
{
    public class AlbumMapper
    {
        public static AlbumResponse MapToResponse(Album album, ImageAccent? imageAccent)
        {
            AlbumResponse response = new AlbumResponse
            {
                Id = album.Id,
                Name = album.Name,
                Year = album.ReleaseYear,
                Image = new ImageResponse { ImageLocation = album.ImageLocation }
            };

            if (imageAccent != null)
            {
                response.Image.LowColor = imageAccent.LowAccent;
                response.Image.MiddleColor = imageAccent.MiddleAccent;
                response.Image.HighColor = imageAccent.HighAccent;
            }

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

            response.SongCount = album.Songs.Count;
            response.Duration = album.Songs
                .Select(s => (int)s.Duration.TotalSeconds)
                .Sum();

            return response;
        }

        public static List<AlbumResponse> MapToResponseList(List<Album> albums)
        {
            return albums.Select(album => MapToResponse(album, null)).ToList();
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