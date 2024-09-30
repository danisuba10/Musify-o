using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.DataTransferObjects.Responses;
using Domain;

namespace Application.Mappers
{
    public class SongMapper
    {
        public static SongResponse MapToResponse(Song song)
        {
            return new SongResponse
            {
                Id = song.Id,
                Title = song.Title,
                Duration = song.Duration,
                AlbumId = song.AlbumId,
                Album = song.Album,
                PositionInAlbum = song.PositionInAlbum,
                Artists = song.SongArtistRelations.Select(ar => ar.Artist).ToList()
            };
        }

        public static List<SongResponse> MapToResponseList(List<Song> songs)
        {
            return songs.Select(MapToResponse).ToList();
        }
    }
}