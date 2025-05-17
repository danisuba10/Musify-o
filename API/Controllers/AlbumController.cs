using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Application.DataTransferObjects;
using Domain;
using Application.Albums;
using Application.Artists;
using MediatR;
using Application.Images;
using Application.Songs;
using Application.DataTransferObjects.Requests;
using Application.DataTransferObjects.Responses;
using Application.Mappers;
using Application;
using Microsoft.AspNetCore.Authorization;
using Application.ImageAccents;

namespace API.Controllers
{
    [Route("album/")]
    public class AlbumController : BaseController
    {
        private async Task<string> AddImage(IFormFile file, string name)
        {
            string imagePath;
            try
            {
                imagePath = await Mediator.Send(new UploadImage.Command
                {
                    formFile = file,
                    Path = Path.Combine(ImageFolderPath, "album"),
                    Name = name,
                    RootImagePath = ImageFolderPath
                });
            }
            catch (Exception)
            {
                throw;
            }

            return Path.Combine("album", name + ".jpg");
        }

        // private async Task<Artist> GetOrCreateArtist
        // (ArtistDTO artistDTO, HashSet<Artist> usedArtists, CancellationToken cancellationToken)
        // {
        //     var Artist = usedArtists.FirstOrDefault(a => a.Name == artistDTO.Name);

        //     if (Artist == null)
        //     {
        //         Artist = await Mediator.Send(new GetArtist.Query { Name = artistDTO.Name });

        //         if (Artist == null)
        //         {
        //             Guid ArtistID;

        //             if (artistDTO.Id != null)
        //             {
        //                 ArtistID = (Guid)artistDTO.Id;
        //             }
        //             else
        //             {
        //                 ArtistID = Guid.NewGuid();
        //             }

        //             Artist = new Artist()
        //             {
        //                 Id = ArtistID,
        //                 Name = artistDTO.Name
        //             };
        //             Artist.ImageLocation = "artists/" + Artist.Id;
        //         }

        //         usedArtists.Add(Artist);
        //     }
        //     return Artist;
        // }

        // [HttpPost("AddAlbumWithComplexData")]
        // public async Task<IActionResult> AddAlbumWithComplexData
        // ([FromBody] AlbumDTO albumDto, CancellationToken cancellationToken)
        // {
        //     HashSet<Artist> UsedArtists = new HashSet<Artist>();

        //     Guid AlbumID;
        //     if (albumDto.Id != null)
        //     {
        //         AlbumID = (Guid)albumDto.Id;
        //     }
        //     else
        //     {
        //         AlbumID = Guid.NewGuid();
        //     }
        //     var Album = new Album
        //     {
        //         Id = AlbumID,
        //         Name = albumDto.Name,
        //         Songs = new HashSet<Song>(),
        //         AlbumArtistRelations = new List<AlbumArtistRelation>()
        //     };
        //     Album.ImageLocation = "albums/" + Album.Id;

        //     List<String> ArtistsString = new List<String>();
        //     foreach (var ArtistDTO in albumDto.Artists)
        //     {

        //         var Artist = await GetOrCreateArtist(ArtistDTO, UsedArtists, cancellationToken);

        //         Album.AlbumArtistRelations.Add(
        //             new AlbumArtistRelation
        //             {
        //                 Album = Album,
        //                 Artist = Artist
        //             }
        //             );
        //         ArtistsString.Add(Artist.Name);
        //     }

        //     var ExistingAlbum = await Mediator.Send(
        //         new SearchAlbums.Query { Name = Album.Name, Artists = ArtistsString, AllArtistsPresent = true });

        //     if (ExistingAlbum != null)
        //     {
        //         return BadRequest("Album is already added!");
        //     }

        //     foreach (var SongDTO in albumDto.Songs)
        //     {
        //         Guid SongID;
        //         if (SongDTO.Id != null)
        //         {
        //             SongID = (Guid)SongDTO.Id;
        //         }
        //         else
        //         {
        //             SongID = Guid.NewGuid();
        //         }
        //         var Song = new Song()
        //         {
        //             Id = SongID,
        //             Title = SongDTO.Title,
        //             Duration = SongDTO.Duration,
        //             PositionInAlbum = SongDTO.PositionInAlbum,
        //             Album = Album,
        //             SongArtistRelations = new List<SongArtistRelation>()
        //         };

        //         foreach (var ArtistDTO in SongDTO.Artists)
        //         {
        //             var Artist = await GetOrCreateArtist(ArtistDTO, UsedArtists, cancellationToken);

        //             Song.SongArtistRelations.Add(new SongArtistRelation
        //             {
        //                 Song = Song,
        //                 Artist = Artist
        //             }
        //             );
        //         }

        //         Album.Songs.Add(Song);
        //     }

        //     await Mediator.Send(new AddAlbum.Command { Album = Album });

        //     return Ok(Album);
        // }

        // [HttpGet("GetAlbumID")]
        // public async Task<IActionResult> GetAlbumId(string name, CancellationToken cancellationToken)
        // {
        //     var albums = await Mediator.Send(new SearchAlbums.Query { Name = name });
        //     if (albums != null)
        //     {
        //         return Ok(albums[0].Id);
        //     }
        //     else
        //     {
        //         return BadRequest("Album does not exist!");
        //     }
        // }

        // [HttpPost("AddAlbumImage")]
        // public async Task<IActionResult> AddAlbumImage(Guid AlbumID, IFormFile formFile, CancellationToken cancellationToken)
        // {
        //     var album = await Mediator.Send(new GetAlbumByID.Query { Id = AlbumID });
        //     if (album != null)
        //     {
        //         try
        //         {
        //             await Mediator.Send(new UploadImage.Command
        //             {
        //                 formFile = formFile,
        //                 Path = Path.Combine(ImageFolderPath, "Albums"),
        //                 Name = AlbumID.ToString()
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

        // [HttpPost("AddSongToAlbum")]
        // public async Task<IActionResult> AddSongToAlbum(Guid AlbumID, Guid SongID)
        // {
        //     var album = await Mediator.Send(new GetAlbum.Query { Id = AlbumID, ExtendedQuery = true });
        //     var song = await Mediator.Send(new GetSong.Query { Id = SongID, ExtendedQuery = false });
        // }

        // [HttpPost("/search")]
        // public async Task<List<AlbumResponse>> search([FromBody] AlbumSearchRequest request)
        // {
        //     var query = new SearchAlbums.Query
        //     {
        //         Name = request.Name,
        //         Artists = request.Artists,
        //         Songs = request.Songs,
        //         IncludeSongs = request.IncludeSongs,
        //         IncludeArtists = request.IncludeArtists,
        //         AllArtistsPresent = request.AllArtistsPresent
        //     };

        //     var albums = await Mediator.Send(query);
        //     if (albums != null)
        //     {
        //         return AlbumMapper.MapToResponseList(albums);
        //     }
        //     else
        //     {
        //         return new List<AlbumResponse>();
        //     }
        // }

        [Authorize(Policy = "Admin")]
        [HttpPost("add-album")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> addAlbum(
            [FromForm] AddAlbumRequest request
        )
        {
            Guid id = Guid.NewGuid();
            string? imagePath = "";
            string errorMessage = "";

            if (request.FormFile != null)
            {
                try
                {
                    imagePath = await AddImage(request.FormFile, id.ToString());
                }
                catch (Exception ex)
                {
                    errorMessage += "Image upload failed!: " + ex.Message + "\n";
                }
            }

            Album album = new Album
            {
                Id = id,
                ReleaseYear = request.Year,
                ImageLocation = imagePath,
                Name = request.Name
            };

            await Mediator.Send(new AddAlbum.Command { Album = album });

            if (request.ArtistIds != null && request.ArtistIds.Count > 0)
            {
                int failures = await Mediator.Send(new AddArtistsToAlbum.Query { AlbumId = id, ArtistIds = request.ArtistIds });
                if (failures != 0)
                {
                    errorMessage += "Out of " + request.ArtistIds.Count.ToString() + " artists " + failures.ToString() + " could not be added!\n";
                }
            }

            if (!String.IsNullOrWhiteSpace(errorMessage))
            {
                return BadRequest(new { Id = id, Message = errorMessage });
            }

            return Ok(new { Id = id, Message = "Album added successfully!" });
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AlbumResponse))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> getAlbumById(Guid id)
        {

            try
            {
                Album album = await Mediator.Send(new GetAlbumByID.Query { Id = id, IncludeArtists = true, IncludeSongs = true });
                ImageAccent? imageAccent = await Mediator.Send(new GetImageAccentByPath.Query
                { Path = Path.Combine(ImageFolderPath, album.ImageLocation) });
                return Ok(AlbumMapper.MapToResponse(album, imageAccent));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [Authorize(Policy = "Admin")]
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> removeAlbum(Guid id)
        {
            try
            {
                await Mediator.Send(new RemoveAlbumByID.Command { Id = id, ImageFolderPath = ImageFolderPath, SoundFolderPath = SoundFolderPath });
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

            return Ok("Album removed successfully!");
        }

        [Authorize(Policy = "Admin")]
        [HttpPost("update-album")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> updateAlbum([FromForm] UpdateAlbumRequest request)
        {
            try
            {
                await Mediator.Send(new UpdateAlbumByID.Query { Id = request.Id, Name = request.Name, Year = request.Year, File = request.FormFile, ArtistIds = request.ArtistIds, ImageFolderPath = ImageFolderPath });
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("search")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> searchAlbum([FromQuery] SearchRequest request)
        {
            try
            {
                List<Album> albums = await Mediator.Send(new SearchAlbum.Query { Request = request });
                List<SearchResult> results = AlbumMapper.MapToSearchResultList(albums);

                if (albums.Count == 0)
                {
                    return NotFound("No results were found");
                }

                return Ok(new SearchResponse
                {
                    SearchResults = results,
                    LastCreatedAt = albums.Last().CreatedAt,
                    LastName = albums.Last().Name
                });
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}