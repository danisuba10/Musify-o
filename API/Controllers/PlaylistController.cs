using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.DataTransferObjects.Requests;
using Application.Exceptions.Common;
using Application.Images;
using Application.Playlists;
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
                    Name = name
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

            try
            {
                await Mediator.Send(new AddPlaylist.Command { dto = request, UserId = userId, Id = id });
            }
            catch (NotExistingObjectExceptions ex)
            {
                errorMessage += "First song when adding playlist error!: " + ex.Message + "\n";
            }
            catch (Exception e)
            {
                return BadRequest(e.Message + "\nInner exception: " + e.InnerException?.Message);
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

            if (!String.IsNullOrWhiteSpace(errorMessage))
            {
                return BadRequest(new { Id = id, Message = errorMessage });
            }

            return Ok(new { Id = id, Message = "Playlist added successfully!" });
        }

        [Authorize]
        [HttpPost("remove")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> removePlaylist([FromForm] Guid Id, CancellationToken cancellationToken)
        {
            try
            {
                Guid userId;
                if (!Guid.TryParse(User.Claims.FirstOrDefault(c => c.Type == "Identifier")?.Value, out userId))
                {
                    return Unauthorized("User identifier not found.");
                }

                var userRole = User.Claims.FirstOrDefault(c => c.Type == "Role")?.Value;

                await Mediator.Send(new RemovePlaylist.Command { Id = Id, UserId = userId, Role = userRole });
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
    }
}