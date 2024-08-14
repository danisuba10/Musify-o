using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Domain;
using Persistence;

namespace Application.Songs
{
    public class AddSong
    {
        public class Command : IRequest
        {
            public required Song Song { get; set; }
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
                _context.Songs.Add(command.Song);
                await _context.SaveChangesAsync();
                return Unit.Value;
            }
        }
    }
}