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
    public class RemoveArtistByID
    {
        public class Command : IRequest<string>
        {
            public Guid Id { get; set; }
        }

        public class Handler : IRequestHandler<Command, string>
        {
            private readonly ApplicationDbContext _context;
            public Handler(ApplicationDbContext context)
            {
                _context = context;
            }

            public async Task<string> Handle(Command command, CancellationToken cancellationToken)
            {
                var artist = await _context.Artists
                    .FirstOrDefaultAsync(art => art.Id == command.Id);

                if (artist == null)
                {
                    throw new Exception("Album does not exist!Can't be deleted!");
                }

                string imageLocation = artist.ImageLocation;

                _context.Artists.Remove(artist);
                await _context.SaveChangesAsync(cancellationToken);

                return imageLocation;
            }
        }
    }
}