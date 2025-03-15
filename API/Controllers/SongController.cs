using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Albums;
using Application.Artists;
using Application.DataTransferObjects;
using Application.DataTransferObjects.Requests;
using Application.DataTransferObjects.Responses;
using Application.Exceptions.Common;
using Application.Exceptions.Song;
using Application.Mappers;
using Application.PlayRecords;
using Application.Songs;
using Application.Sounds;
using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("song/")]
    public class SongController : BaseController
    {

        // [HttpGet("search")]
        // public async Task<List<SongResponse>> SearchSongs([FromBody] SongSearchRequest Request)
        // {
        //     var query = new SearchSongs.Query
        //     {
        //         Title = Request.Title,
        //         Artists = Request.Artists,
        //         AlbumName = Request.AlbumName,
        //         IncludeAlbum = Request.IncludeAlbum,
        //         IncludeArtists = Request.IncludeArtists
        //     };

        //     var songs = await Mediator.Send(query);

        //     var songResponses = SongMapper.MapToResponseList(songs);
        //     return songResponses;
        // }

        private async Task<String> AddSound(IFormFile file, string name)
        {
            string imagePath;
            try
            {
                imagePath = await Mediator.Send(new UploadSound.Command
                {
                    FormFile = file,
                    Path = Path.Combine(SoundFolderPath, "song"),
                    Name = name
                });
            }
            catch (Exception)
            {
                throw;
            }

            return Path.Combine("song", name + ".opus");
        }

        [Authorize(Policy = "Admin")]
        [HttpPost("add-song")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> addSong([FromForm] AddSongRequest request)
        {
            try
            {
                Guid id = Guid.NewGuid();
                Song song = new Song
                {
                    Id = id,
                    Title = request.Title,
                    PositionInAlbum = request.PositionInAlbum ?? -1,
                    AlbumId = request.AlbumId,
                    Duration = TimeSpan.FromSeconds(request.Duration)
                };

                string soundPath = "";
                string? errorMessage = null;

                if (request.SoundFile != null)
                {
                    try
                    {
                        soundPath = await AddSound(request.SoundFile, id.ToString());
                        song.SoundLocation = soundPath;

                    }
                    catch (Exception ex)
                    {
                        errorMessage += "Sound upload error:" + ex.Message;
                    }
                }

                await Mediator.Send(new AddSong.Command { Song = song });

                if (request.ArtistIds != null && request.ArtistIds.Count > 0)
                {
                    int failures = await Mediator.Send(new AddArtistsToSong.Query { SongId = id, ArtistIds = request.ArtistIds });
                    if (failures != 0)
                    {
                        errorMessage += "Out of " + request.ArtistIds.Count.ToString() + " artists " + failures.ToString() + " could not be added!\n";
                    }
                }

                if (!String.IsNullOrWhiteSpace(errorMessage))
                {
                    return BadRequest(new { Id = id, Message = errorMessage });
                }

                return Ok(new { Id = id, Message = "Song added successfully!" });
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [Authorize(Policy = "Admin")]
        [HttpPost("{id}/remove")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> removeSong(Guid id)
        {
            try
            {
                string soundLocation = await Mediator.Send(new RemoveSongByID.Command { Id = id, SoundFolderPath = SoundFolderPath });
                return Ok("Song removed succesfully!");
            }
            catch (FileNotFoundException)
            {
                return BadRequest("Song successfully removed, but sound file failed to be deleted.");
            }
            catch (Exception e)
            {
                return NotFound(e.Message);
            }
        }

        [Authorize(Policy = "Admin")]
        [HttpPost("remove-songs")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
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
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> updateSong([FromForm] UpdateSongRequest request)
        {
            try
            {
                await Mediator.Send(
                    new UpdateSongByID.Command
                    {
                        Id = request.Id,
                        Title = request.Title,
                        Duration = request.Duration.HasValue ? TimeSpan.FromSeconds((double)request.Duration) : null,
                        PositionInAlbum = request.PositionInAlbum,
                        AlbumID = request.AlbumId,
                        ArtistIds = request.ArtistIds
                    });
                return Ok("Song updated successfully!");
            }
            catch (Exception e)
            {
                return BadRequest($"{e.Message} - {e.InnerException?.Message}");
            }

        }

        [HttpGet("search")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> searchSong([FromQuery] SearchRequest request)
        {
            try
            {
                List<Song> songs = await Mediator.Send(new SearchSong.Query { Request = request });
                List<SearchResult> results = SongMapper.MapToSearchResultList(songs);

                if (songs.Count == 0)
                {
                    return NotFound("No results were found");
                }

                return Ok(new SearchResponse
                {
                    SearchResults = results,
                    LastCreatedAt = songs.Last().CreatedAt,
                    LastName = songs.Last().Title
                });
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("{id}/play")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> playSong(Guid id)
        {
            try
            {
                Guid? userId = null;
                if (Guid.TryParse(User.Claims.FirstOrDefault(c => c.Type == "Identifier")?.Value, out Guid parsedUserId))
                {
                    userId = parsedUserId;
                }

                var Song = await Mediator.Send(new GetSongByID.Query { Id = id, IncludeAlbum = true, IncludeArtists = true });

                if (userId != null)
                {
                    await Mediator.Send(new RecordPlay.Query { UserId = (Guid)userId, PlayedItemType = PlayedItemType.Song, PlayedItemId = id, TimeStamp = DateTime.Now });
                }

                return Ok(new { Song = SongMapper.MapToResponse(Song, null, true), songImage = Song.Album.ImageLocation, songFileUrl = Song.SoundLocation });
            }
            catch (NotExistingObjectExceptions sDNE)
            {
                return NotFound(sDNE.Message);
            }
            catch (ArgumentException ae)
            {
                return BadRequest("Internal error:" + ae.Message);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }

}