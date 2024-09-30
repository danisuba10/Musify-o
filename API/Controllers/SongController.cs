using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.DataTransferObjects.Requests;
using Application.DataTransferObjects.Responses;
using Application.Mappers;
using Application.Songs;
using Domain;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class SongController : BaseController
    {

        [HttpPost("songs/search")]
        public async Task<List<SongResponse>> SearchSongs([FromBody] SongSearchRequest Request)
        {
            var query = new SearchSongs.Query
            {
                Title = Request.Title,
                Artists = Request.Artists,
                AlbumName = Request.AlbumName,
                IncludeAlbum = Request.IncludeAlbum,
                IncludeArtists = Request.IncludeArtists
            };

            var songs = await Mediator.Send(query);
            var songResponses = SongMapper.MapToResponseList(songs);
            return songResponses;
        }

    }
}