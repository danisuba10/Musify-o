using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Security.Authentication;
using System.Threading.Tasks;
using Application.DataTransferObjects.Requests;
using Application.Exceptions.User;
using Application.Users;
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
    }
}