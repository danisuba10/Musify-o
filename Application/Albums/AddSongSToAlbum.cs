using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;
using Application.Songs;

namespace Application.Albums
{
    public class AddSongsToAlbum
    {
        public class Query : IRequest
        {
            public int AlbumId { get; set; }
            public List<Song> Songs { get; set; } = [];
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
                var Album = await _context.Albums
                    .Include(a => a.Songs)
                    .FirstOrDefaultAsync(a => a.Id == query.AlbumId, cancellationToken);

                if (Album == null)
                {
                    throw new Exception("Album does not exist!");
                }

                foreach (Song Song in query.Songs)
                {
                    Album.Songs.Add(Song);
                }

                _context.Albums.Update(Album);
                await _context.SaveChangesAsync();

                return Unit.Value;
            }
        }
    }
}