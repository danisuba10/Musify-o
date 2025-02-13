using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Domain;
using Persistence;
using Application.DataTransferObjects.Requests;

namespace Application.Songs
{
    public class AddSong
    {
        public class Command : IRequest
        {
            public required AddSongRequest songRequest;
        };

        public class Handler : IRequestHandler<Command>
        {
            private readonly ApplicationDbContext _context;
            public Handler(ApplicationDbContext context)
            {
                _context = context;
            }
            public async Task<Unit> Handle(Command command, CancellationToken cancellationToken)
            {
                Song song = new Song
                {
                    Id = Guid.NewGuid(),
                    Title = command.songRequest.Title,
                    PositionInAlbum = command.songRequest.PositionInAlbum ?? -1,
                    AlbumId = command.songRequest.AlbumId,
                    Duration = TimeSpan.FromSeconds(command.songRequest.Duration)
                };
                _context.Songs.Add(song);
                await _context.SaveChangesAsync();
                return Unit.Value;
            }
        }
    }
}