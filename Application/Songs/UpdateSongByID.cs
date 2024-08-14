using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Domain;
using Microsoft.EntityFrameworkCore;
using Persistence;
using System.Reflection.Metadata;

namespace Application.Songs
{
    public class UpdateSongByID
    {
        public class Command : IRequest
        {
            public int Id { get; set; }
            public Song Song { get; set; } = new Song();
        }

        public class Handler : IRequestHandler<Command>
        {
            private readonly ApplicationDbContext _context;
            public Handler(ApplicationDbContext context)
            {
                _context = context;
            }
            public async Task<Unit> Handle(Command command, CancellationToken cancellationToken)
            {
                var existingSong = await _context.Songs
                    .FirstOrDefaultAsync(s => s.Id == command.Id);

                if (existingSong == null)
                {
                    throw new Exception($"Song was not found!");
                }

                Song NewSong = command.Song;

                if (!string.IsNullOrWhiteSpace(NewSong.Title))
                {
                    existingSong.Title = NewSong.Title;
                }

                if (NewSong.Duration != TimeSpan.FromSeconds(0))
                {
                    existingSong.Duration = NewSong.Duration;
                }

                if (NewSong.AlbumId.HasValue)
                {

                    var albumExists = await _context.Albums
                        .AnyAsync(a => a.Id == NewSong.AlbumId);

                    if (!albumExists)
                    {
                        throw new Exception("Album does not exist! Cant update Song's album!");
                    }

                    existingSong.AlbumId = NewSong.AlbumId;
                }

                if (NewSong.PositionInAlbum != -1)
                {
                    existingSong.PositionInAlbum = NewSong.PositionInAlbum;
                }

                _context.Songs.Update(existingSong);
                await _context.SaveChangesAsync();

                return Unit.Value;
            }
        }
    }
}