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

namespace API.Controllers
{
    public class AlbumController : BaseController
    {
        private async Task<Artist> GetOrCreateArtist
        (ArtistDTO artistDTO, HashSet<Artist> usedArtists, CancellationToken cancellationToken)
        {
            var Artist = usedArtists.FirstOrDefault(a => a.Name == artistDTO.ArtistName);

            if (Artist == null)
            {
                Artist = await Mediator.Send(new GetArtist.Query { Name = artistDTO.ArtistName });

                if (Artist == null)
                {
                    Guid ArtistID;

                    if (artistDTO.Id != null)
                    {
                        ArtistID = (Guid)artistDTO.Id;
                    }
                    else
                    {
                        ArtistID = Guid.NewGuid();
                    }

                    Artist = new Artist()
                    {
                        Id = ArtistID,
                        Name = artistDTO.ArtistName
                    };
                    Artist.ImageLocation = "artists/" + Artist.Id;
                }

                usedArtists.Add(Artist);
            }
            return Artist;
        }

        [HttpPost("AddAlbumWithComplexData")]
        public async Task<IActionResult> AddAlbumWithComplexData
        ([FromBody] AlbumDTO albumDto, CancellationToken cancellationToken)
        {
            HashSet<Artist> UsedArtists = new HashSet<Artist>();

            Guid AlbumID;
            if (albumDto.Id != null)
            {
                AlbumID = (Guid)albumDto.Id;
            }
            else
            {
                AlbumID = Guid.NewGuid();
            }
            var Album = new Album
            {
                Id = AlbumID,
                Name = albumDto.AlbumName,
                Songs = new HashSet<Song>(),
                AlbumArtistRelations = new List<AlbumArtistRelation>()
            };
            Album.ImageLocation = "albums/" + Album.Id;

            List<String> ArtistsString = new List<String>();
            foreach (var ArtistDTO in albumDto.Artists)
            {

                var Artist = await GetOrCreateArtist(ArtistDTO, UsedArtists, cancellationToken);

                Album.AlbumArtistRelations.Add(
                    new AlbumArtistRelation
                    {
                        Album = Album,
                        Artist = Artist
                    }
                    );
                ArtistsString.Add(Artist.Name);
            }

            var ExistingAlbum = await Mediator.Send(
                new GetAlbum.Query { Name = Album.Name, Artists = ArtistsString, AllArtistsPresent = true });

            if (ExistingAlbum != null)
            {
                return BadRequest("Album is already added!");
            }

            foreach (var SongDTO in albumDto.Songs)
            {
                Guid SongID;
                if (SongDTO.Id != null)
                {
                    SongID = (Guid)SongDTO.Id;
                }
                else
                {
                    SongID = Guid.NewGuid();
                }
                var Song = new Song()
                {
                    Id = SongID,
                    Title = SongDTO.Title,
                    Duration = SongDTO.Duration,
                    PositionInAlbum = SongDTO.PositionInAlbum,
                    Album = Album,
                    SongArtistRelations = new List<SongArtistRelation>()
                };

                foreach (var ArtistDTO in SongDTO.Artists)
                {
                    var Artist = await GetOrCreateArtist(ArtistDTO, UsedArtists, cancellationToken);

                    Song.SongArtistRelations.Add(new SongArtistRelation
                    {
                        Song = Song,
                        Artist = Artist
                    }
                    );
                }

                Album.Songs.Add(Song);
            }

            await Mediator.Send(new AddAlbum.Command { Album = Album });

            return Ok(Album);
        }

        [HttpGet("GetAlbumID")]
        public async Task<IActionResult> GetAlbumId(string name, CancellationToken cancellationToken)
        {
            var album = await Mediator.Send(new GetAlbum.Query { Name = name });
            if (album != null)
            {
                return Ok(album.Id);
            }
            else
            {
                return BadRequest("Album does not exist!");
            }
        }

        [HttpPost("AddAlbumImage")]
        public async Task<IActionResult> AddAlbumImage(Guid AlbumID, IFormFile formFile, CancellationToken cancellationToken)
        {
            var album = await Mediator.Send(new GetAlbum.Query { Id = AlbumID });
            if (album != null)
            {
                await Mediator.Send(new UploadImage.Command
                {
                    formFile = formFile,
                    Path = Path.Combine(ImageFolderPath, "Albums"),
                    Name = AlbumID.ToString()
                });
                return Ok();
            }
            else
            {
                return BadRequest("Album does not exist!");
            }
        }
    }
}