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
        public static ArtistResponse MapToResponse(Artist artist)
        {
            return new ArtistResponse
            {
                Id = artist.Id,
                Name = artist.Name,
                ImageLocation = artist.ImageLocation
            };
        }

        public static List<ArtistResponse> MapToResponseList(List<Artist> artists)
        {
            return artists.Select(MapToResponse).ToList();
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