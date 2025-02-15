using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;
using Application.Users;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("user")]
    public class UserController : BaseController
    {
        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser(string userName, string password, CancellationToken cancellationToken)
        {

            if (userName == null)
            {
                return BadRequest("Username is empty!");
            }

            if (password == null)
            {
                return BadRequest("Password is empty!");
            }

            try
            {
                string token = await Mediator.Send(new RegisterUser.Command { UserName = userName, Password = password });
                return Ok(token);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginUser([FromForm] string userName, [FromForm] string password, CancellationToken cancellationToken)
        {

            if (userName == null)
            {
                return BadRequest("Username is empty!");
            }

            if (password == null)
            {
                return BadRequest("Password is empty!");
            }

            try
            {
                string token = await Mediator.Send(new LoginUser.Command { UserName = userName, Password = password });
                return Ok(token);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
    }
}