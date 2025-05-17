using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.DataTransferObjects;
using Application.DataTransferObjects.Requests;
using Application.DataTransferObjects.Responses;
using Application.Exceptions.Common;
using Application.ImageAccents;
using Application.Images;
using Application.Mappers;
using Application.Playlists;
using Application.Songs;
using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("playlist/")]
    public class PlaylistController : BaseController
    {
        private async Task<string> AddImage(IFormFile file, string name)
        {
            try
            {
                await Mediator.Send(new UploadImage.Command
                {
                    formFile = file,
                    Path = Path.Combine(ImageFolderPath, "playlist"),
                    Name = name,
                    RootImagePath = ImageFolderPath
                });
            }
            catch (Exception)
            {
                throw;
            }

            return Path.Combine("playlist", name + ".jpg");
        }

        [Authorize]
        [HttpPost("add")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> addPlaylist([FromForm] AddPlaylistRequest request)
        {
            Guid id = Guid.NewGuid();
            string? imagePath = "";
            string errorMessage = "";

            Guid userId;
            if (!Guid.TryParse(User.Claims.FirstOrDefault(c => c.Type == "Identifier")?.Value, out userId))
            {
                return Unauthorized("User identifier not found.");
            }

            if (request.FormFile != null)
            {
                try
                {
                    imagePath = await AddImage(request.FormFile, id.ToString());
                }
                catch (Exception ex)
                {
                    errorMessage += "Image upload failed!: " + ex.Message + "\n";
                }
            }
            else if (request.FirstSongId != null)
            {
                var song = await Mediator.Send(new GetSongByID.Query { Id = (Guid)request.FirstSongId, IncludeAlbum = true });
                if (song != null)
                {
                    try
                    {
                        var fullPath = Path.Combine(ImageFolderPath, song.Album.ImageLocation);
                        var image = await System.IO.File.ReadAllBytesAsync(Path.Combine(ImageFolderPath, song.Album.ImageLocation));
                        var stream = new MemoryStream(image);
                        var formFile = new FormFile(stream, 0, stream.Length, "formFile", Path.GetFileName(song.Album.ImageLocation))
                        {
                            Headers = new HeaderDictionary(),
                            ContentType = "image/jpeg"
                        };
                        await AddImage(formFile, id.ToString());
                        imagePath = Path.Combine("playlist", id.ToString() + ".jpg");
                    }
                    catch (Exception ex)
                    {
                        errorMessage += "Error getting image from first song!: " + ex.Message + "\n";
                    }
                }
            }

            try
            {
                await Mediator.Send(new AddPlaylist.Command { dto = request, UserId = userId, Id = id, ImagePath = imagePath });
            }
            catch (NotExistingObjectExceptions ex)
            {
                errorMessage += "First song when adding playlist error!: " + ex.Message + "\n";
            }
            catch (Exception e)
            {
                return BadRequest(e.Message + "\nInner exception: " + e.InnerException?.Message);
            }

            if (!String.IsNullOrWhiteSpace(errorMessage))
            {
                return BadRequest(new { Id = id, Message = errorMessage });
            }

            return Ok(new { Id = id, Message = "Playlist added successfully!" });
        }

        [Authorize]
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> removePlaylist(Guid id, CancellationToken cancellationToken)
        {
            try
            {
                Guid userId;
                if (!Guid.TryParse(User.Claims.FirstOrDefault(c => c.Type == "Identifier")?.Value, out userId))
                {
                    return Unauthorized("User identifier not found.");
                }

                var userRole = User.Claims.FirstOrDefault(c => c.Type == "Role")?.Value;

                string imageLocation = await Mediator.Send(new RemovePlaylist.Command { Id = id, UserId = userId, Role = userRole });
                await Mediator.Send(new DeleteImage.Command { Path = Path.Combine(ImageFolderPath, imageLocation), RootImagePath = ImageFolderPath });
                return Ok("Playlist successfully removed!");
            }
            catch (NotExistingObjectExceptions ex)
            {
                return NotFound(ex.Message);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized("Playlist can not be removed as it is not created by the user logged in!");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [Authorize]
        [HttpPost("{id}/add-song")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> addSongToPlaylist(Guid id, [FromForm] Guid SongId)
        {
            try
            {
                Guid userId;
                if (!Guid.TryParse(User.Claims.FirstOrDefault(c => c.Type == "Identifier")?.Value, out userId))
                {
                    return Unauthorized("User identifier not found.");
                }

                var userRole = User.Claims.FirstOrDefault(c => c.Type == "Role")?.Value;

                await Mediator.Send(new AddSongToPlaylist.Command
                {
                    req = new PlaylistSongRequest { PlaylistId = id, SongId = SongId },
                    UserId = userId,
                    Role = userRole
                });
                return Ok("Song successfully added to playlist!");
            }
            catch (NotExistingObjectExceptions ex)
            {
                return NotFound(ex.Message);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized("Song cannot be added to playlist as it was not created by the user logged in!");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [Authorize]
        [HttpPost("{id}/remove-song")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> removeSongFromPlaylist(Guid id, [FromForm] Guid SongId)
        {
            try
            {
                Guid userId;
                if (!Guid.TryParse(User.Claims.FirstOrDefault(c => c.Type == "Identifier")?.Value, out userId))
                {
                    return Unauthorized("User identifier not found.");
                }

                var userRole = User.Claims.FirstOrDefault(c => c.Type == "Role")?.Value;

                await Mediator.Send(new RemoveSongFromPlaylist.Command
                {
                    req = new PlaylistSongRequest { PlaylistId = id, SongId = SongId },
                    UserId = userId,
                    Role = userRole
                });
                return Ok("Song successfully removed from playlist!");
            }
            catch (NotExistingObjectExceptions ex)
            {
                return NotFound(ex.Message);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized("Song cannot be added to playlist as it was not created by the user logged in!");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> getPlaylist(Guid id)
        {
            Guid? userId = null;
            if (Guid.TryParse(User.Claims.FirstOrDefault(c => c.Type == "Identifier")?.Value, out Guid parsedUserId))
            {
                userId = parsedUserId;
            }

            var userRole = User.Claims.FirstOrDefault(c => c.Type == "Role")?.Value;

            try
            {
                Playlist playlist = await Mediator.Send(new GetPlaylistById.Query { Id = id, UserId = userId, Role = userRole });
                ImageAccent? playlistAccent = await Mediator.Send(new GetImageAccentByPath.Query { Path = Path.Combine(ImageFolderPath, playlist.ImageLocation) });
                ImageAccent? userAccent = await Mediator.Send(new GetImageAccentByPath.Query { Path = Path.Combine(ImageFolderPath, playlist.User.ImageLocation) });
                return Ok(PlaylistMapper.MapToResponse(playlist, playlistAccent, playlist.User, userAccent));
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (NotExistingObjectExceptions nonEx)
            {
                return NotFound(nonEx.Message);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("search")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> searchPlaylists([FromQuery] SearchRequest searchRequest)
        {

            try
            {
                Guid? userId = null;
                if (Guid.TryParse(User.Claims.FirstOrDefault(c => c.Type == "Identifier")?.Value, out Guid parsedUserId))
                {
                    userId = parsedUserId;
                }

                var userRole = User.Claims.FirstOrDefault(c => c.Type == "Role")?.Value;

                List<Playlist> playlists = await Mediator.Send(new SearchPlaylist.Query { Request = searchRequest, UserId = userId, Role = userRole });
                List<SearchResult> results = PlaylistMapper.MapToSearchResultList(playlists);

                if (playlists.Count == 0)
                {
                    return NotFound("No results were found");
                }

                return Ok(new SearchResponse
                {
                    SearchResults = results,
                    LastCreatedAt = playlists.Last().CreatedAt,
                    LastName = playlists.Last().Name
                });
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }

        }

        [Authorize]
        [HttpPost("{id}/update")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> updatePlaylist(Guid id, [FromForm] PlaylistUpdateRequest request)
        {
            try
            {
                Guid? userId = null;
                if (Guid.TryParse(User.Claims.FirstOrDefault(c => c.Type == "Identifier")?.Value, out Guid parsedUserId))
                {
                    userId = parsedUserId;
                }

                var userRole = User.Claims.FirstOrDefault(c => c.Type == "Role")?.Value;

                if (userId == null)
                {
                    return Unauthorized("User not logged in!");
                }

                await Mediator.Send(new UpdatePlaylist.Command { Id = id, request = request, UserId = (Guid)userId, Role = userRole, ImageFolderPath = ImageFolderPath });
                return Ok(new { message = "Playlist successfully updated!", id = id });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex);
            }
            catch (NotExistingObjectExceptions ex)
            {
                return NotFound(ex);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

    }
}