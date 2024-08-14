using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using MediatR;
using Persistence;
using Domain;
using Application.Albums;
using Microsoft.EntityFrameworkCore;
using Moq;
using Application;
using Microsoft.AspNetCore.Mvc.Diagnostics;
using System.Diagnostics;

namespace Tests
{
    public class AlbumHandlerTests
    {
        private readonly ApplicationDbContext _context;
        private readonly IMediator _mediator;

        public AlbumHandlerTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDB")
                .Options;

            _context = new ApplicationDbContext(options);

            var mediatorMock = new Mock<IMediator>();
            _mediator = mediatorMock.Object;
        }

        [Fact]
        public async Task AddAlbum_ShouldAddAlbumToDatabase()
        {
            var handler = new AddAlbum.Handler(_context);
            var album = new Album { Name = "Follow the leader", ImageLocation = "path/followtheleader" };
            var command = new AddAlbum.Command { Album = album };

            //Act
            await handler.Handle(command, CancellationToken.None);

            //Assert
            var returnAlbum = await _context.Albums.FirstOrDefaultAsync(a => a.Name == "Follow the leader");
            Assert.NotNull(returnAlbum);
            Assert.Equal(album.Name, returnAlbum.Name);
        }

        [Fact]
        public async Task GetAlbumByID_ShouldReturnAlbum()
        {
            //Arrange
            var album = new Album { Name = "Follow the leader", ImageLocation = "path/test" };
            _context.Albums.Add(album);
            await _context.SaveChangesAsync();

            var handler = new GetAlbumByID.Handler(_context);
            var query = new GetAlbumByID.Query { Id = album.Id };

            //Act
            var result = await handler.Handle(query, CancellationToken.None);

            //Assert
            Assert.NotNull(result);
            Assert.Equal(album.Name, result.Name);
        }

        [Fact]
        public async Task GetAllAlbums_ShouldReturnAllAlbums()
        {
            //Arrange
            var albums = _context.Albums;
            _context.Albums.RemoveRange(albums);
            await _context.SaveChangesAsync();
            _context.Albums.AddRange(new List<Album>
            {
                new Album {Name = "Follow the leader", ImageLocation = "path/1"},
                new Album {Name = "Surrealistic pillow", ImageLocation = "path/2"},
                new Album {Name = "Reggatta De Blanc", ImageLocation = "path/3"},
                new Album {Name = "Rosenrot", ImageLocation = "path/4"}
            }
            );
            await _context.SaveChangesAsync();

            var handler = new GetAllAlbums.Handler(_context);
            var query = new GetAllAlbums.Query();

            //Act
            var result = await handler.Handle(query, CancellationToken.None);

            //Assert
            Assert.Equal(4, result.Count);
        }

        [Fact]
        public async Task RemoveAlbumByID_ShouldRemoveAlbumAndRelatedSongs()
        {
            //Arrange
            var albums = _context.Albums;
            _context.Albums.RemoveRange(albums);

            var songsToRemove = _context.Songs;
            _context.Songs.RemoveRange(songsToRemove);
            await _context.SaveChangesAsync();

            await _context.SaveChangesAsync();
            List<Album> NewAlbums = new List<Album>
            {
                new Album {Name = "Follow the leader", ImageLocation = "path/1"},
                new Album {Name = "Surrealistic pillow", ImageLocation = "path/2"},
                new Album {Name = "Reggatta De Blanc", ImageLocation = "path/3"},
                new Album {Name = "Rosenrot", ImageLocation = "path/4"}
            };
            _context.Albums.AddRange(NewAlbums);
            List<Song> NewSongs = new List<Song>
            {
                new Song{Title = "Freak On a Leash", Duration = TimeSpan.FromSeconds(256), AlbumId = NewAlbums[0].Id},
                new Song{Title = "Got The Life", Duration = TimeSpan.FromSeconds(248), AlbumId = NewAlbums[0].Id},
                new Song{Title = "White Rabbit", Duration = TimeSpan.FromSeconds(151), AlbumId = NewAlbums[1].Id},
                new Song{Title = "Message In A Bottle", Duration = TimeSpan.FromSeconds(290), AlbumId = NewAlbums[2].Id},
                new Song{Title = "Rosenrot", Duration = TimeSpan.FromSeconds(235), AlbumId = NewAlbums[3].Id}
            };
            _context.Songs.AddRange(NewSongs);
            await _context.SaveChangesAsync();

            var initialAlbumCount = await _context.Albums.CountAsync();
            Assert.Equal(4, initialAlbumCount);

            var handler = new RemoveAlbumByID.Handler(_context);
            var command = new RemoveAlbumByID.Command { Id = NewAlbums[0].Id };

            //Act
            try
            {
                await handler.Handle(command, CancellationToken.None);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            var AlbumsAfterDelete = _context.Albums;
            Assert.Equal(3, AlbumsAfterDelete.Count());
            var SongsAfterDelete = _context.Songs;
            Assert.Equal(3, SongsAfterDelete.Count());
        }

        [Fact]
        public async Task AddSongsToAlbum_ShouldAddSongsToAlbum()
        {
            //Arrange
            var albums = _context.Albums;
            _context.Albums.RemoveRange(albums);

            var songsToRemove = _context.Songs;
            _context.Songs.RemoveRange(songsToRemove);

            List<Album> NewAlbums = new List<Album>
            {
                new Album {Name = "Follow the leader", ImageLocation = "path/1"},
                new Album {Name = "Surrealistic pillow", ImageLocation = "path/2"},
                new Album {Name = "Reggatta De Blanc", ImageLocation = "path/3"},
                new Album {Name = "Rosenrot", ImageLocation = "path/4"}
            };
            _context.Albums.AddRange(NewAlbums);

            List<Song> NewSongs = new List<Song>
            {
                new Song{Title = "Freak On a Leash", Duration = TimeSpan.FromSeconds(256)},
                new Song{Title = "Got The Life", Duration = TimeSpan.FromSeconds(248)},
                new Song{Title = "White Rabbit", Duration = TimeSpan.FromSeconds(151)},
                new Song{Title = "Message In A Bottle", Duration = TimeSpan.FromSeconds(290)},
                new Song{Title = "Rosenrot", Duration = TimeSpan.FromSeconds(235)}
            };

            await _context.SaveChangesAsync();

            //Act
            var handler = new AddSongsToAlbum.Handler(_context);
            var query = new AddSongsToAlbum.Query() { AlbumId = NewAlbums[0].Id, Songs = NewSongs };
            try
            {
                await handler.Handle(query, CancellationToken.None);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            //Assert
            var CheckAlbum = await _context.Albums.FirstOrDefaultAsync(a => a.Id == NewAlbums[0].Id);
            if (CheckAlbum != null)
            {
                Assert.Equal(5, CheckAlbum.Songs.Count());
            }
        }
    }
}