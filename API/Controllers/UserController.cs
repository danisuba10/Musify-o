using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;
using Application.Users;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
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
                await Mediator.Send(new RegisterUser.Command { UserName = userName, Password = password });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok();
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginUser(string userName, string password, CancellationToken cancellationToken)
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
                if (await Mediator.Send(new LoginUser.Command { UserName = userName, Password = password }))
                {
                    return Ok();
                }
                else
                {
                    return BadRequest("Incorrect password!");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

    }
}