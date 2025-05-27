using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection.Metadata.Ecma335;
using System.Security.Authentication;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Application.DataTransferObjects;
using Application.DataTransferObjects.Requests;
using Application.DataTransferObjects.Requests.User;
using Application.DataTransferObjects.Responses;
using Application.Exceptions.Common;
using Application.Exceptions.User;
using Application.ImageAccents;
using Application.Mappers;
using Application.Playlists;
using Application.Users;
using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QRCoder;

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
            if (!IsValidEmail(request.Email))
            {
                return BadRequest("Invalid email address.");
            }

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

        private bool IsValidEmail(string email)
        {
            var regex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
            if (!regex.IsMatch(email))
            {
                return false;
            }

            try
            {
                var domain = email.Split('@')[1];
                var mxRecords = Dns.GetHostEntry(domain).AddressList;
                return mxRecords.Length > 0;
            }
            catch
            {
                return false;
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
                LoginResponse response = await Mediator.Send(new LoginUser.Command { UserName = userName, Password = password });
                return Ok(response);
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
                if (playlists.Count == 0)
                {
                    return NotFound("No playlists.");
                }
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


        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{id}/profile/playlists")]
        public async Task<IActionResult> getTopPlaylists(Guid id)
        {
            try
            {
                Guid? userId = null;
                if (Guid.TryParse(User.Claims.FirstOrDefault(c => c.Type == "Identifier")?.Value, out Guid parsedUserId))
                {
                    userId = parsedUserId;
                }

                var userRole = User.Claims.FirstOrDefault(c => c.Type == "Role")?.Value;

                var playlists = await Mediator.Send(new GetTopPlaylistsOfUser.Query { UserId = id, RequesterUserId = userId, RequesterRole = userRole });
                if (playlists == null || playlists.Count == 0)
                {
                    return NotFound("No playlists found!");
                }

                return Ok(PlaylistMapper.MapToSearchResultList(playlists));
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{id}/profile")]
        public async Task<IActionResult> getUserProfile(Guid id)
        {
            try
            {
                var user = await Mediator.Send(new GetUserById.Query { Id = id });
                var imageAccent = await Mediator.Send(new GetImageAccentByPath.Query { Path = Path.Combine(ImageFolderPath, user.ImageLocation) });
                int publicPlaylistCount = await Mediator.Send(new PublicPlaylistCountOfUser.Query { UserId = id });
                return Ok(UserMapper.mapToProfileRespose(user, imageAccent, publicPlaylistCount));
            }
            catch (NotExistingObjectExceptions ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost("{id}/profile/update")]
        [Authorize(Policy = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> updateUserProfile(Guid id, [FromForm] UpdateUserProfileRequest request)
        {
            try
            {
                if (String.IsNullOrWhiteSpace(request.DisplayName))
                {
                    return BadRequest("Displayname empty!");
                }

                await Mediator.Send(new UpdateUserByID.Query
                {
                    Request = new UpdateUserRequest { Id = id, DisplayName = request.DisplayName, File = request.File },
                    ImageFolderPath = ImageFolderPath,
                });

                return Ok(new { Id = id, Message = "User with id: " + id.ToString() + " successfully modified!" });
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost("profile/update")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> updateUserProfile([FromForm] UpdateUserProfileRequest request)
        {
            try
            {
                Guid userId;
                if (!Guid.TryParse(User.Claims.FirstOrDefault(c => c.Type == "Identifier")?.Value, out userId))
                {
                    return Unauthorized("User identification failed!");
                }

                await Mediator.Send(new UpdateUserByID.Query
                {
                    Request = new UpdateUserRequest { Id = userId, DisplayName = request.DisplayName, File = request.File },
                    ImageFolderPath = ImageFolderPath,
                });

                return Ok(new { Id = userId, Message = "User with id: " + userId.ToString() + " successfully modified!" });
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
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

        [HttpGet("search")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> searchUsers([FromQuery] SearchRequest searchRequest)
        {

            try
            {
                Guid? userId = null;
                if (Guid.TryParse(User.Claims.FirstOrDefault(c => c.Type == "Identifier")?.Value, out Guid parsedUserId))
                {
                    userId = parsedUserId;
                }

                var userRole = User.Claims.FirstOrDefault(c => c.Type == "Role")?.Value;

                List<User> users = await Mediator.Send(new SearchUser.Query { Request = searchRequest });
                var results = users.Select(UserMapper.MapToSearchResult).ToList();

                if (results.Count == 0)
                {
                    return NotFound("No results were found");
                }

                return Ok(new SearchResponse
                {
                    SearchResults = results,
                    LastCreatedAt = users.Last().CreatedAt,
                    LastName = users.Last().DisplayName
                });
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }

        }

        [Authorize]
        [HttpPost("2fa/enable")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> EnableTwoFactorAuth()
        {
            try
            {
                if (!Guid.TryParse(User.Claims.FirstOrDefault(c => c.Type == "Identifier")?.Value, out var userId))
                {
                    return Unauthorized("User identification failed!");
                }

                var response = await Mediator.Send(new EnableTwoFactorAuth.Command { UserId = userId });
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize]
        [HttpPost("2fa/confirm")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ConfirmTwoFactorAuth([FromForm] string code)
        {
            Guid? userId = null;
            if (Guid.TryParse(User.Claims.FirstOrDefault(c => c.Type == "Identifier")?.Value, out Guid parsedUserId))
            {
                userId = parsedUserId;
            }

            var user = await Mediator.Send(new GetUserById.Query { Id = userId.Value });

            if (user == null || string.IsNullOrEmpty(user.TwoFactorSecret))
                return BadRequest("2FA not initialized");

            // Verify the code
            bool isValid = (await Mediator.Send(new VerifyTwoFactorAuth.Command
            {
                UserId = userId.Value,
                Code = code
            })).Success;

            if (!isValid) return BadRequest("Invalid verification code");

            // Only enable after successful verification
            user.IsTwoFactorEnabled = true;
            await Mediator.Send(new UpdateUserByID.Query
            {
                Request = new UpdateUserRequest { Id = userId.Value, IsTwoFactorEnabled = true },
                ImageFolderPath = ImageFolderPath
            });

            return Ok(new
            {
                Success = true,
                Message = "2FA successfully enabled"
            });
        }

        [Authorize]
        [HttpPost("2fa/verify")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> VerifyTwoFactorAuth([FromForm] string Code)
        {
            try
            {
                if (!Guid.TryParse(User.Claims.FirstOrDefault(c => c.Type == "Identifier")?.Value, out var userId))
                {
                    return Unauthorized("User identification failed!");
                }

                var isValid = await Mediator.Send(new VerifyTwoFactorAuth.Command
                {
                    UserId = userId,
                    Code = Code
                });

                return Ok(new { IsValid = isValid });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize]
        [HttpPost("2fa/recovery")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> VerifyTwoFactorRecoveryCode([FromForm] string RecoveryCode)
        {
            try
            {
                if (!Guid.TryParse(User.Claims.FirstOrDefault(c => c.Type == "Identifier")?.Value, out var userId))
                {
                    return Unauthorized("User identification failed!");
                }

                var isValid = await Mediator.Send(new VerifyTwoFactorRecoveryCode.Command
                {
                    UserId = userId,
                    RecoveryCode = RecoveryCode
                });

                return Ok(new { IsValid = isValid });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize]
        [HttpPost("2fa/disable")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> DisableTwoFactorAuth()
        {
            try
            {
                if (!Guid.TryParse(User.Claims.FirstOrDefault(c => c.Type == "Identifier")?.Value, out var userId))
                {
                    return Unauthorized("User identification failed!");
                }

                var result = await Mediator.Send(new DisableTwoFactorAuth.Command { UserId = userId });
                return Ok(new { Success = result });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize]
        [HttpGet("2fa/qrcode")]
        public async Task<IActionResult> GetTwoFactorQrCode()
        {
            var userId = Guid.Parse(User.FindFirst("Identifier").Value);
            var user = await Mediator.Send(new GetUserById.Query { Id = userId });

            if (user == null || string.IsNullOrEmpty(user.TwoFactorSecret))
            {
                return BadRequest("Two-factor authentication is not enabled for this user.");
            }

            var qrCodeUri = $"otpauth://totp/YourApp:{user.Email}?secret={user.TwoFactorSecret}&issuer=YourApp";

            // Generate QR code image
            var qrGenerator = new QRCodeGenerator();
            var qrCodeData = qrGenerator.CreateQrCode(qrCodeUri, QRCodeGenerator.ECCLevel.Q);
            var qrCode = new PngByteQRCode(qrCodeData);
            var qrCodeBytes = qrCode.GetGraphic(20);

            return File(qrCodeBytes, "image/png");
        }
    }
}