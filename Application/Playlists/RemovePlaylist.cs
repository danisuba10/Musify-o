using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Exceptions.Common;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Playlists
{
    public class RemovePlaylist
    {
        public class Command : IRequest<Unit>
        {
            public Guid Id { get; set; }
            public Guid UserId { get; set; }
            public string? Role { get; set; }
        }
        public class Handler : IRequestHandler<Command, Unit>
        {
            ApplicationDbContext _context;
            public Handler(ApplicationDbContext context)
            {
                _context = context;
            }

            public async Task<Unit> Handle(Command cmd, CancellationToken cancellationToken)
            {
                var playlist = await _context.Playlists
                    .FirstOrDefaultAsync(pl => pl.Id == cmd.Id);

                if (playlist == null)
                {
                    throw new NotExistingObjectExceptions("Playlist");
                }

                if (playlist.UserId != cmd.UserId && !(cmd.Role?.Equals("Admin") ?? false))
                {
                    throw new UnauthorizedAccessException();
                }

                _context.Playlists.Remove(playlist);
                await _context.SaveChangesAsync(cancellationToken);

                return Unit.Value;
            }
        }
    }
}