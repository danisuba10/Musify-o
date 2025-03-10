using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application
{
    public class RemoveAlbumByID
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
            public async Task<string> Handle(Command request, CancellationToken cancellationToken)
            {
                var album = await _context.Albums
                    .FirstOrDefaultAsync(album => album.Id == request.Id, cancellationToken);

                if (album == null)
                {
                    throw new Exception("Album does not exist!");
                }

                string imageLocation = album.ImageLocation;

                _context.Albums.Remove(album);
                await _context.SaveChangesAsync(cancellationToken);

                return imageLocation;
            }
        }
    }
}