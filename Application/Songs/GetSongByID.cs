using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Songs
{
    public class GetSongByID
    {
        public class Query : IRequest<Song>
        {
            public Guid Id { get; set; }
        }

        public class Handler : IRequestHandler<Query, Song>
        {
            private readonly ApplicationDbContext _context;
            public Handler(ApplicationDbContext context)
            {
                _context = context;
            }

            public async Task<Song> Handle(Query query, CancellationToken cancellationToken)
            {
                var song = await _context.Songs
                    .Include(s => s.Album)
                    .FirstOrDefaultAsync(s => s.Id == query.Id, cancellationToken);

                if (song == null)
                {
                    throw new Exception($"Song with ID {query.Id} does not exists.");
                }

                return song;
            }

        }
    }
}