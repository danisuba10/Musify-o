using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Application.DataTransferObjects;
using Domain;
using Application.Albums;
using Application.Artists;

namespace API.Controllers
{
    public class AlbumController : BaseController
    {
        private async Task<Artist> GetOrCreateArtist
        (ArtistDTO artistDTO, HashSet<Artist> usedArtists, CancellationToken cancellationToken)
        {
            var Artist = usedArtists.FirstOrDefault(a => a.Name == artistDTO.Name);

            if (Artist == null)
            {
                Artist = await Mediator.Send(new GetArtistByName.Query { Name = artistDTO.Name });

                if (Artist == null)
                {
                    Artist = new Artist()
                    {
                        Id = Guid.NewGuid(),
                        Name = artistDTO.Name
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

            var Album = new Album
            {
                Id = Guid.NewGuid(),
                Name = albumDto.Name,
                Songs = new HashSet<Song>(),
                AlbumArtistRelations = new List<AlbumArtistRelation>()
            };
            Album.ImageLocation = "albums/" + Album.Id;

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

            }

            foreach (var SongDTO in albumDto.Songs)
            {
                var Song = new Song()
                {
                    Id = Guid.NewGuid(),
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
    }
}