using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain;
using MediatR;
using Microsoft.VisualBasic;
using Persistence;

namespace Application.Development
{
    public class EmptyMusicTables
    {
        public class Command : IRequest { }
        public class Handler : IRequestHandler<Command>
        {
            private readonly ApplicationDbContext _context;
            public Handler(ApplicationDbContext context)
            {
                _context = context;
            }
            public async Task<Unit> Handle(Command command, CancellationToken cancellationToken)
            {
                var Songs = _context.Songs;
                _context.Songs.RemoveRange(Songs);
                var Albums = _context.Albums;
                _context.Albums.RemoveRange(Albums);
                var Artists = _context.Artists;
                _context.Artists.RemoveRange(Artists);
                var SongArtistRelations = _context.SongArtistRelations;
                _context.SongArtistRelations.RemoveRange(SongArtistRelations);
                var AlbumArtistRelations = _context.AlbumArtistRelations;
                _context.AlbumArtistRelations.RemoveRange(AlbumArtistRelations);
                await _context.SaveChangesAsync(cancellationToken);

                return Unit.Value;
            }

        }
    }
}