using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.DataTransferObjects;
using Application.DataTransferObjects.Responses;
using Application.Search;
using Domain;

namespace Application.Mappers
{
    public class ArtistMapper
    {
        public static ArtistResponse MapToResponse(Artist artist, ImageAccent? imageAccent, List<Album>? topAlbums)
        {
            ArtistResponse response = new ArtistResponse
            {
                Id = artist.Id,
                Name = artist.Name,
                ImageLocation = artist.ImageLocation,
                Image = new ImageResponse { ImageLocation = artist.ImageLocation }
            };

            if (imageAccent != null)
            {
                response.Image.LowColor = imageAccent.LowAccent;
                response.Image.MiddleColor = imageAccent.MiddleAccent;
                response.Image.HighColor = imageAccent.HighAccent;
            }

            if (topAlbums != null)
            {
                response.TopAlbums = AlbumMapper.MapToSearchResultList(topAlbums);
            }

            return response;
        }

        public static List<ArtistResponse> MapToResponseList(List<Artist> artists)
        {
            return artists.Select(artist => MapToResponse(artist, null, null)).ToList();
        }

        public static SearchResult MapToSearchResult(Artist artist)
        {
            return new SearchResult
            {
                Id = artist.Id,
                Name = artist.Name,
                Type = "Artist",
                ImageLocation = artist.ImageLocation
            };
        }

        public static List<SearchResult> MapToSearchResultList(List<Artist> artists)
        {
            return artists.Select(MapToSearchResult).ToList();
        }
    }
}