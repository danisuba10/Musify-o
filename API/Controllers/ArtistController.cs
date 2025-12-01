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
using Application.DataTransferObjects.Requests;
using Microsoft.AspNetCore.Authorization;
using Application.ImageAccents;
using Application.Albums;
using Microsoft.AspNetCore.Http.HttpResults;

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
                    Name = name,
                    RootImagePath = ImageFolderPath
                });
            }
            catch (Exception)
            {
                throw;
            }

            return Path.Combine("artist", name + ".jpg");
        }

        // [HttpPost("GetArtistByName")]
        // [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ArtistDTO))]
        // [ProducesResponseType(StatusCodes.Status404NotFound)]
        // public async Task<IActionResult> GetArtistByName(string Name, CancellationToken cancellationToken)
        // {
        //     var artist = await Mediator.Send(new GetArtist.Query { Name = Name });

        //     if (artist == null)
        //     {
        //         return NotFound(new { Message = "Artist not found" });
        //     }

        //     return Ok(new ArtistDTO { Name = artist.Name, ImageLocation = artist.ImageLocation, Id = artist.Id });
        // }

        // [HttpGet("GetArtistID")]
        // [ProducesResponseType(StatusCodes.Status200OK)]
        // [ProducesResponseType(StatusCodes.Status400BadRequest)]
        // public async Task<IActionResult> GetArtistID(string name, CancellationToken cancellationToken)
        // {
        //     var artist = await Mediator.Send(new GetArtist.Query { Name = name }, cancellationToken);
        //     if (artist != null)
        //     {
        //         return Ok(artist.Id);
        //     }
        //     else
        //     {
        //         return BadRequest("Artist does not exist!");
        //     }
        // }

        // [HttpGet("GetAllArtistIds")]
        // [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<Guid>))]
        // public async Task<List<Guid>> GetAllArtistIDs()
        // {
        //     List<Guid> Ids = new List<Guid>();
        //     var artists = await Mediator.Send(new GetAllArtists.Query { });

        //     foreach (var artist in artists)
        //     {
        //         Ids.Add(artist.Id);
        //     }

        //     return Ids;
        // }

        // [HttpPost("AddArtistImage")]
        // [ProducesResponseType(StatusCodes.Status200OK)]
        // [ProducesResponseType(StatusCodes.Status400BadRequest)]
        // public async Task<IActionResult> addArtistImage(Guid ArtistID, IFormFile formFile, CancellationToken cancellationToken)
        // {
        //     var album = await Mediator.Send(new GetArtist.Query { Id = ArtistID });
        //     if (album != null)
        //     {
        //         try
        //         {
        //             await Mediator.Send(new UploadImage.Command
        //             {
        //                 formFile = formFile,
        //                 Path = Path.Combine(ImageFolderPath, "Artists1"),
        //                 Name = ArtistID.ToString()
        //             });
        //         }
        //         catch (Exception ex)
        //         {
        //             return BadRequest(ex.Message);
        //         }
        //         return Ok();
        //     }
        //     else
        //     {
        //         return BadRequest("Album does not exist!");
        //     }
        // }

        [Authorize(Policy = "Admin")]
        [HttpPost("add-artist")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> addArtist([FromForm] AddArtistRequest request, CancellationToken cancellationToken)
        {
            Guid id = Guid.NewGuid();

            IActionResult imageUploadResult = null;
            string? imagePath = "";
            string? errorMessage = null;

            if (request.FormFile != null)
            {
                try
                {
                    imagePath = await AddImage(request.FormFile, id.ToString());
                }
                catch (Exception ex)
                {
                    errorMessage = ex.Message;
                }
            }

            Artist artist = new Artist
            {
                Name = request.Name,
                Id = id,
                ImageLocation = imagePath
            };

            await Mediator.Send(new AddArtist.Command { Artist = artist });

            if (request.FormFile != null && imageUploadResult is BadRequestObjectResult)
            {
                await LogOnly("Artist", "CreateFailed", id, "Artist created but failed to upload image: " + errorMessage);
                return BadRequest("Artist created, but failed to upload image.\n" + errorMessage);
            }

            await NotifyAndLog("Artist", "Create", id, $"Name: {request.Name}");
            return Ok(new { Id = id, Message = "Artist added successfully!" });
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ArtistResponse))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> getArtistById(Guid id)
        {
            Artist? artist = await Mediator.Send(new GetArtist.Query { Id = id });
            if (artist == null)
            {
                // Read failed - not logging per configuration
                return NotFound("Artist with this ID not found!");
            }
            List<Album> topTenAlbums = await Mediator.Send(new GetTopAlbumsOfArtist.Query { Id = artist.Id });

            ImageAccent? imageAccent = await Mediator.Send(
                new GetImageAccentByPath.Query
                { Path = Path.Combine(ImageFolderPath, artist.ImageLocation) });
            // No notifications or logs for read operations per request
            return Ok(ArtistMapper.MapToResponse(artist, imageAccent, topTenAlbums));
        }

        [HttpGet("{id}/albums")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ArtistResponse))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> getAlbums(Guid id, [FromQuery] SearchRequest request)
        {
            try
            {
                List<Album> result = await Mediator.Send(new SearchArtistAlbums.Query { Request = request, ArtistId = id });
                if (result.Count == 0)
                {
                    return NotFound("No results!");
                }
                List<SearchResult> artists = AlbumMapper.MapToSearchResultList(result);
                // No notifications or logs for read operations per request
                return Ok(new SearchResponse
                {
                    SearchResults = artists,
                    LastName = result.Last().Name,
                    LastCreatedAt = result.Last().CreatedAt
                });
            }
            catch (Exception e)
            {
                // Read failed - not logging per configuration
                return BadRequest(e.Message);
            }
        }

        [Authorize(Policy = "Admin")]
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> removeArtist(Guid id)
        {
            try
            {
                string imageLocation = await Mediator.Send(new RemoveArtistByID.Command { Id = id, ImageFolderPath = ImageFolderPath, SoundFolderPath = SoundFolderPath });
                await NotifyAndLog("Artist", "Delete", id);
                return Ok("Artist successfully removed!");
            }
            catch (Exception e)
            {
                await LogOnly("Artist", "DeleteFailed", id, e.Message);
                return NotFound(e.Message);
            }
        }

        [Authorize(Policy = "Admin")]
        [HttpPost("update-artist")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> updateArtist([FromForm] UpdateArtistRequest request)
        {
            try
            {
                await Mediator.Send(new UpdateArtistByID.Command
                { Id = request.Id, File = request.FormFile, Name = request.Name, ImageFolderPath = ImageFolderPath });
                await NotifyAndLog("Artist", "Update", request.Id, $"Name: {request.Name}");
                return Ok("Artist succesfully modified!");
            }
            catch (Exception e)
            {
                await LogOnly("Artist", "UpdateFailed", request.Id, e.Message);
                return BadRequest(e.Message);
            }
        }

        [HttpGet("search")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> searchArtist([FromQuery] SearchRequest request)
        {
            try
            {
                List<Artist> result = await Mediator.Send(new SearchArtist.Query { Request = request });
                List<SearchResult> artists = ArtistMapper.MapToSearchResultList(result);

                if (result.Count == 0)
                {
                    return NotFound("No results were found");
                }

                // No notifications or logs for read operations per request
                return Ok(new SearchResponse
                {
                    SearchResults = artists,
                    LastName = result.Last().Name,
                    LastCreatedAt = result.Last().CreatedAt
                });
            }
            catch (Exception e)
            {
                // Read failed - not logging per configuration
                return BadRequest(e.Message);
            }
        }
    }
}