using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using MediatR;
using Persistence;
using Microsoft.EntityFrameworkCore;

namespace Application.Artists
{
    public class DeleteArtistByID
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
                var album = await _context.Artists
                    .FirstOrDefaultAsync(art => art.Id == command.Id);

                if (album == null)
                {
                    throw new Exception("Album does not exist!Can't be deleted!");
                }

                _context.Artists.Remove(album);
                await _context.SaveChangesAsync(cancellationToken);

                return Unit.Value;
            }
        }
    }
}