using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Domain;
using MediatR;
using Persistence;

namespace Application.Albums
{
    public class AddAlbum
    {
        public class Command : IRequest
        {
            public required Album Album { get; set; }
        }

        public class Handler : IRequestHandler<Command>
        {
            private readonly ApplicationDbContext _context;

            public Handler(ApplicationDbContext context)
            {
                _context = context;
            }

            public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
            {
                await _context.Albums.AddAsync(request.Album);
                await _context.SaveChangesAsync();
                return Unit.Value;
            }
        }
    }
}