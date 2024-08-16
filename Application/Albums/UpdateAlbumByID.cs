using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Albums
{
    public class UpdateAlbumByID
    {
        public class Query : IRequest
        {
            public Guid Id { get; set; }
            public Album Album { get; set; } = new Album();
        }

        public class Handler : IRequestHandler<Query>
        {
            private readonly ApplicationDbContext _context;
            public Handler(ApplicationDbContext context)
            {
                _context = context;
            }
            public async Task<Unit> Handle(Query query, CancellationToken cancellationToken)
            {
                var existingAlbum = await _context.Albums
                    .FirstOrDefaultAsync(a => a.Id == query.Id, cancellationToken);

                if (existingAlbum == null)
                {
                    throw new Exception("Album does not exist!");
                }

                Album newAlbum = query.Album;
                if (newAlbum.Name != "")
                {
                    existingAlbum.Name = newAlbum.Name;
                }

                if (newAlbum.ImageLocation != null)
                {
                    existingAlbum.ImageLocation = newAlbum.ImageLocation;
                }

                if (newAlbum.Songs != null && newAlbum.Songs.Any())
                {
                    existingAlbum.Songs = newAlbum.Songs;
                }

                _context.Albums.Update(existingAlbum);
                await _context.SaveChangesAsync();

                return Unit.Value;
            }
        }
    }
}