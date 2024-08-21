using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Development;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class DevelopmentController : BaseController
    {
        [HttpPost("DeleteAllDataFromMusicTables")]
        public async Task<IActionResult> DeleteAllDataFromMusicTables()
        {
            await Mediator.Send(new EmptyMusicTables.Command { });
            return Ok();
        }
    }
}