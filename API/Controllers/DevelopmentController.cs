using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Development;
using Microsoft.AspNetCore.Mvc;
using Application.Albums;
using Domain;
using System.Runtime.CompilerServices;

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

        [HttpPost("TestAlbumSearch")]
        public async Task<IActionResult> TestAlbumSearch()
        {
            List<String> songs = new List<String>();
            List<String> artists = new List<String>();
            songs.Add("obod");
            songs.Add("MUL");
            List<Album>? albums = await Mediator.Send(new SearchAlbums.Query { Name = "Trip", Songs = songs, IncludeSongs = true });
            int a = 0;
            return Ok();
        }
    }
}