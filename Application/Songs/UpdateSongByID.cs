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
            public Guid Id { get; set; }
            public string? Title { get; set; }
            public TimeSpan? Duration { get; set; }
            public Guid? AlbumID { get; set; }
            public int? PositionInAlbum { get; set; }
        }

        public class Handler : IRequestHandler<Command>
        {
            private readonly ApplicationDbContext _context;
            private readonly IMediator _mediator;
            public Handler(ApplicationDbContext context, IMediator mediator)
            {
                _context = context;
                _mediator = mediator;
            }
            public async Task<Unit> Handle(Command command, CancellationToken cancellationToken)
            {
                var existingSong = await _context.Songs
                    .FirstOrDefaultAsync(s => s.Id == command.Id);

                if (existingSong == null)
                {
                    throw new Exception("Song was not found!");
                }

                if (!string.IsNullOrWhiteSpace(command.Title))
                {
                    existingSong.Title = command.Title;
                }

                if (command.Duration != null && command.Duration != TimeSpan.FromSeconds(0))
                {
                    existingSong.Duration = (TimeSpan)command.Duration;
                }

                if (command.AlbumID != null)
                {

                    var albumExists = await _context.Albums
                        .AnyAsync(a => a.Id == command.AlbumID);

                    if (!albumExists)
                    {
                        throw new Exception("Album does not exist! Cant update Song's album!");
                    }

                    existingSong.AlbumId = command.AlbumID;
                }

                if (command.PositionInAlbum != null)
                {
                    existingSong.PositionInAlbum = (int)command.PositionInAlbum;
                }

                _context.Songs.Update(existingSong);
                await _context.SaveChangesAsync();

                return Unit.Value;
            }
        }
    }
}