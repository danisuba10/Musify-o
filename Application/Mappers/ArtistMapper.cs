using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.DataTransferObjects.Responses;
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
                Name = artist.Name
            };
        }

        public static List<ArtistResponse> MapToResponseList(List<Artist> artists)
        {
            return artists.Select(MapToResponse).ToList();
        }
    }
}