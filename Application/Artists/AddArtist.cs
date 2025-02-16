using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Artists
{
    public class AddArtist
    {
        public class Command : IRequest<Guid>
        {
            public required Artist Artist { get; set; }
        }

        public class Handler : IRequestHandler<Command, Guid>
        {
            private readonly ApplicationDbContext _context;
            public Handler(ApplicationDbContext context)
            {
                _context = context;
            }

            public async Task<Guid> Handle(Command command, CancellationToken cancellationToken)
            {
                await _context.Artists.AddAsync(command.Artist, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);
                return command.Artist.Id;
            }
        }
    }
}