using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Artists;
using Application.Images;
using Application.DataTransferObjects;
using Microsoft.AspNetCore.Mvc;
using Domain;
using Application.DataTransferObjects.Responses;
using Application.Mappers;
using Microsoft.AspNetCore.Http;
using System.Threading;

namespace API.Controllers
{
    [Route("artist/")]
    public class ArtistController : BaseController
    {
        private async Task<String> AddImage(IFormFile file, string name)
        {
            string imagePath;
            try
            {
                imagePath = await Mediator.Send(new UploadImage.Command
                {
                    formFile = file,
                    Path = Path.Combine(ImageFolderPath, "artist"),
                    Name = name
                });
            }
            catch (Exception)
            {
                throw;
            }

            return Path.Combine("/artist", name);
        }

        [HttpPost("GetArtistByName")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ArtistDTO))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetArtistByName(string Name, CancellationToken cancellationToken)
        {
            var artist = await Mediator.Send(new GetArtist.Query { Name = Name });

            if (artist == null)
            {
                return NotFound(new { Message = "Artist not found" });
            }

            return Ok(new ArtistDTO { Name = artist.Name, ImageLocation = artist.ImageLocation, Id = artist.Id });
        }

        [HttpGet("GetArtistID")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
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
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<Guid>))]
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
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
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

        [HttpPost("add-artist")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> addArtist(string name, IFormFile? formFile, CancellationToken cancellationToken)
        {
            Guid id = Guid.NewGuid();

            IActionResult imageUploadResult = null;
            string? imagePath = null;
            string? errorMessage = null;

            if (formFile != null)
            {
                try
                {
                    imagePath = await AddImage(formFile, id.ToString());
                }
                catch (Exception ex)
                {
                    errorMessage = ex.Message;
                }
            }

            Artist artist = new Artist
            {
                Name = name,
                Id = id,
                ImageLocation = imagePath
            };

            await Mediator.Send(new AddArtist.Command { Artist = artist });

            if (formFile != null && imageUploadResult is BadRequestObjectResult)
            {
                return BadRequest("Artist created, but failed to upload image.\n" + errorMessage);
            }

            return Ok(new { Id = id, Message = "Artist and image added successfully!" });
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ArtistResponse))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> getArtistById(Guid id)
        {
            Artist? artist = await Mediator.Send(new GetArtist.Query { Id = id });
            if (artist == null)
            {
                return NotFound("Artist with this ID not found!");
            }
            return Ok(ArtistMapper.MapToResponse(artist));
        }
    }
}