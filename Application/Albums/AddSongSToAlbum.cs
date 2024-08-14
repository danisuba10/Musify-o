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
    public class AddSongToAlbum
    {
        public class Query : IRequest
        {
            public int AlbumId { get; set; }
            public List<int> SongIds { get; set; } = [];
        }

        public class Handler : IRequestHandler<Query>
        {
            private readonly ApplicationDbContext _context;
            private readonly IMediator _mediator;
            public Handler(ApplicationDbContext context, IMediator mediator)
            {
                _context = context;
                _mediator = mediator;
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

                foreach (int SongID in query.SongIds)
                {
                    var song = await _mediator.Send(new GetSongByID.Query() { Id = SongID }, cancellationToken);
                    Album.Songs.Add(song);
                }

                return Unit.Value;
            }
        }
    }
}