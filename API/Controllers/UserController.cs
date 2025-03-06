using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Security.Authentication;
using System.Threading.Tasks;
using Application.DataTransferObjects.Requests;
using Application.Exceptions.User;
using Application.ImageAccents;
using Application.Mappers;
using Application.Playlists;
using Application.Users;
using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("user")]
    public class UserController : BaseController
    {

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser([FromForm] RegisterRequest request, CancellationToken cancellationToken)
        {

            try
            {
                string token = await Mediator.Send(new RegisterUser.Command
                {
                    Email = request.Email,
                    Password = request.Password,
                    DisplayName = request.DisplayName
                });

                return Ok(token);
            }
            catch (UserAlreadyExistsException ue)
            {
                return Conflict(ue.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost("login")]
        public async Task<IActionResult> LoginUser([FromForm] string userName, [FromForm] string password, CancellationToken cancellationToken)
        {

            try
            {
                string token = await Mediator.Send(new LoginUser.Command { UserName = userName, Password = password });
                return Ok(token);
            }
            catch (UserDoesNotExistException udne)
            {
                return NotFound(udne.Message);
            }
            catch (IncorrectCredentialsException invCred)
            {
                return Unauthorized(invCred.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [HttpGet("playlists")]
        public async Task<IActionResult> GetUserPlaylists(CancellationToken cancellationToken)
        {
            Guid userId;
            if (!Guid.TryParse(User.Claims.FirstOrDefault(c => c.Type == "Identifier")?.Value, out userId))
            {
                return Unauthorized("User identifier not found.");
            }

            try
            {
                var playlists = await Mediator.Send(new GetPlaylistsOfUser.Query { UserId = userId });
                var playlistsWithImageAccents = new List<PlaylistWithImageAccent>();

                foreach (var playlist in playlists)
                {
                    var imageAccent = await Mediator.Send(new GetImageAccentByPath.Query { Path = Path.Combine(ImageFolderPath, playlist.ImageLocation) });
                    playlistsWithImageAccents.Add(new PlaylistWithImageAccent
                    {
                        Playlist = playlist,
                        ImageAccent = imageAccent
                    });
                }

                var user = playlists.First().User;
                if (user == null)
                {
                    return BadRequest("User not found for the playlist.");
                }
                return Ok(PlaylistMapper.MapToResponseList(playlistsWithImageAccents, user));
            }
            catch (ArgumentException ex)
            {
                return BadRequest("User of this playlist does not exist!");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [HttpGet("search-playlists")]
        public async Task<IActionResult> SearchUserPlaylist(string term)
        {
            try
            {
                Guid userId;
                if (!Guid.TryParse(User.Claims.FirstOrDefault(c => c.Type == "Identifier")?.Value, out userId))
                {
                    return Unauthorized("User identifier not found.");
                }

                var playlists = await Mediator.Send(new SearchPlaylistOfUser.Query { Term = term, UserId = userId });
                var playlistsWithImageAccents = new List<PlaylistWithImageAccent>();

                foreach (var playlist in playlists)
                {
                    var imageAccent = await Mediator.Send(new GetImageAccentByPath.Query { Path = Path.Combine(ImageFolderPath, playlist.ImageLocation) });
                    playlistsWithImageAccents.Add(new PlaylistWithImageAccent
                    {
                        Playlist = playlist,
                        ImageAccent = imageAccent
                    });
                }

                var user = await Mediator.Send(new GetUserById.Query { Id = userId });
                if (user == null)
                {
                    return BadRequest("User not found for the playlist.");
                }
                return Ok(PlaylistMapper.MapToResponseList(playlistsWithImageAccents, user));
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }
    }
}