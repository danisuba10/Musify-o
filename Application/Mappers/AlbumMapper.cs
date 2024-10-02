using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
                Songs = album.Songs.ToList(),
                Artists = album.AlbumArtistRelations.Select(aar => aar.Artist).ToList(),
                ArtistIds = new List<Guid>()
            };

            foreach (var relation in album.AlbumArtistRelations)
            {
                response.ArtistIds.Add(relation.ArtistId);
            }

            return response;
        }

        public static List<AlbumResponse> MapToResponseList(List<Album> albums)
        {
            return albums.Select(MapToResponse).ToList();
        }
    }
}