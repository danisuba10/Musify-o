using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Development;
using Microsoft.AspNetCore.Mvc;
using Application.Albums;
using Domain;
using System.Runtime.CompilerServices;
using Application.Users;
using Application.DataTransferObjects.Requests;
using Microsoft.AspNetCore.Authorization;

namespace API.Controllers
{
    [Route("dev")]
    public class DevelopmentController : BaseController
    {

        [Authorize(Policy = "Admin")]
        [HttpPost("DeleteAllDataFromMusicTables")]
        public async Task<IActionResult> DeleteAllDataFromMusicTables()
        {
            await Mediator.Send(new EmptyMusicTables.Command { });
            return Ok();
        }

        // [HttpPost("TestAlbumSearch")]
        // public async Task<IActionResult> TestAlbumSearch()
        // {
        //     List<String> songs = new List<String>();
        //     List<String> artists = new List<String>();
        //     songs.Add("obod");
        //     songs.Add("MUL");
        //     List<Album>? albums = await Mediator.Send(new SearchAlbums.Query { Name = "Trip", Songs = songs, IncludeSongs = true });
        //     int a = 0;
        //     return Ok();
        // }


        [Authorize(Policy = "Admin")]
        [HttpPost("make-admin")]
        public async Task<IActionResult> makeAdmin(Guid id)
        {
            try
            {
                await Mediator.Send(new UpdateUserByID.Query { Request = new UpdateUserRequest { Id = id, Role = "Admin" } });
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}