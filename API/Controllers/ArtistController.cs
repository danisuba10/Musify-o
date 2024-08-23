using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Artists;
using Application.DataTransferObjects;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class ArtistController : BaseController
    {
        [HttpPost("GetArtistByName")]
        public async Task<IActionResult> GetArtistByName(string Name, CancellationToken cancellationToken)
        {
            var artist = await Mediator.Send(new GetArtistByName.Query { Name = Name });

            if (artist == null)
            {
                return NotFound(new { Message = "Artist not found" });
            }

            return Ok(new ArtistDTO { Name = artist.Name, ImgLocation = artist.ImageLocation });
        }

        [HttpGet("GetArtistID")]
        public async Task<IActionResult> GetArtistID(string name, CancellationToken cancellationToken)
        {
            var artist = await Mediator.Send(new GetArtistByName.Query { Name = name }, cancellationToken);
            if (artist != null)
            {
                return Ok(artist.Id);
            }
            else
            {
                return BadRequest("Artist does not exist!");
            }
        }

        [HttpGet("GetAllArtistIds")]
        public async Task<List<Guid>> GetAllArtistIDs()
        {
            List<Guid> Ids = new List<Guid>();
            var artists = await Mediator.Send(new GetAllArtists.Query { });

            foreach (var artist in artists)
            {
                Ids.Add(artist.Id);
            }

            return Ids;
        }
    }
}