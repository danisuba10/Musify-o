using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Persistence;
using Microsoft.EntityFrameworkCore;
using Application.Exceptions.Song;

namespace Application.Songs
{
    public class RemoveSongByID
    {
        public class Command : IRequest
        {
            public Guid Id { get; set; }
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
                var Song = await _context.Songs
                    .FirstOrDefaultAsync(s => s.Id == command.Id, cancellationToken);

                if (Song == null)
                {
                    throw new SongDoesNotExistException();
                }

                _context.Songs.Remove(Song);
                await _context.SaveChangesAsync(cancellationToken);

                return Unit.Value;
            }
        }
    }
}