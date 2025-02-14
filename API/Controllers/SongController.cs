using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Albums;
using Application.Artists;
using Application.DataTransferObjects.Requests;
using Application.DataTransferObjects.Responses;
using Application.Mappers;
using Application.Songs;
using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("songs/")]
    public class SongController : BaseController
    {

        [HttpGet("search")]
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

        [Authorize(Policy = "Admin")]
        [HttpPost("add-song")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> addSong([FromForm] AddSongRequest request)
        {
            try
            {
                await Mediator.Send(new AddSong.Command { songRequest = request });
                return Ok("Song added succesfully!");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [Authorize(Policy = "Admin")]
        [HttpPost("remove-song")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> removeSong(Guid id)
        {
            try
            {
                await Mediator.Send(new RemoveSongByID.Command { Id = id });
                return Ok("Song removed succesfully!");
            }
            catch (Exception e)
            {
                return NotFound(e.Message);
            }
        }

        [Authorize(Policy = "Admin")]
        [HttpPost("remove-songs")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> removeSongs([FromForm] List<Guid> songIds)
        {
            try
            {
                int failures = await Mediator.Send(new RemoveSongs.Command { SongIds = songIds });
                if (failures != 0)
                {
                    return BadRequest(failures.ToString() + " songs failed to be removed!");
                }
                return Ok("Song removed succesfully!");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [Authorize(Policy = "Admin")]
        [HttpPost("update-song")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> updateSong([FromForm] UpdateSongRequest request)
        {
            try
            {
                await Mediator.Send(
                    new UpdateSongByID.Command
                    {
                        Id = request.ID,
                        Title = request.Title,
                        Duration = request.Duration.HasValue ? TimeSpan.FromSeconds((double)request.Duration) : null,
                        PositionInAlbum = request.PositionInAlbum,
                        AlbumID = request.AlbumId
                    });
                return Ok("Song updated successfully!");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

    }
}