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
        public async Task Remove
    }
}