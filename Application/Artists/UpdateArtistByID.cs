using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Domain;
using MediatR;
using Persistence;
using Microsoft.EntityFrameworkCore;

namespace Application.Artists
{
    public class UpdateArtistByID
    {
        public class Command : IRequest
        {
            public int Id { get; set; }
            public required Artist Artist { get; set; }
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
                var NewArtist = command.Artist;
                var OldArtist = await _context.Artists.FirstOrDefaultAsync(art => art.Id == command.Id);

                if (OldArtist == null)
                {
                    throw new Exception("Artist does not exist! Can't edit it.");
                }

                if (!String.IsNullOrEmpty(NewArtist.Name))
                {
                    OldArtist.Name = NewArtist.Name;
                }

                if (!String.IsNullOrEmpty(NewArtist.ImageLocation))
                {
                    OldArtist.ImageLocation = NewArtist.ImageLocation;
                }

                return Unit.Value;
            }
        }
    }
}