using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Artists;
using Application.Images;
using Application.DataTransferObjects;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class ArtistController : BaseController
    {
        [HttpPost("GetArtistByName")]
        public async Task<IActionResult> GetArtistByName(string Name, CancellationToken cancellationToken)
        {
            var artist = await Mediator.Send(new GetArtist.Query { Name = Name });

            if (artist == null)
            {
                return NotFound(new { Message = "Artist not found" });
            }

            return Ok(new ArtistDTO { ArtistName = artist.Name, ImgLocation = artist.ImageLocation, Id = artist.Id });
        }

        [HttpGet("GetArtistID")]
        public async Task<IActionResult> GetArtistID(string name, CancellationToken cancellationToken)
        {
            var artist = await Mediator.Send(new GetArtist.Query { Name = name }, cancellationToken);
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

        [HttpPost("AddArtistImage")]
        public async Task<IActionResult> AddAlbumImage(Guid ArtistID, IFormFile formFile, CancellationToken cancellationToken)
        {
            var album = await Mediator.Send(new GetArtist.Query { Id = ArtistID });
            if (album != null)
            {
                try
                {
                    await Mediator.Send(new UploadImage.Command
                    {
                        formFile = formFile,
                        Path = Path.Combine(ImageFolderPath, "Artists1"),
                        Name = ArtistID.ToString()
                    });
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
                return Ok();
            }
            else
            {
                return BadRequest("Album does not exist!");
            }
        }
    }
}