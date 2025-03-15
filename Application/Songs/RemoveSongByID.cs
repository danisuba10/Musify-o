using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Persistence;
using Microsoft.EntityFrameworkCore;
using Application.Exceptions.Song;
using Application.Sounds;

namespace Application.Songs
{
    public class RemoveSongByID
    {
        public class Command : IRequest<string>
        {
            public Guid Id { get; set; }
            public required string SoundFolderPath { get; set; }
        }
        public class Handler : IRequestHandler<Command, string>
        {
            private readonly ApplicationDbContext _context;
            private readonly IMediator _mediator;
            public Handler(ApplicationDbContext context, IMediator mediator)
            {
                _context = context;
                _mediator = mediator;
            }
            public async Task<string> Handle(Command command, CancellationToken cancellationToken)
            {
                var Song = await _context.Songs
                    .Include(s => s.SongArtistRelations)
                    .FirstOrDefaultAsync(s => s.Id == command.Id, cancellationToken);

                if (Song == null)
                {
                    throw new SongDoesNotExistException();
                }

                string soundLocation = Song.SoundLocation;

                var songArtistRelations = Song.SongArtistRelations.ToList();
                if (songArtistRelations != null && songArtistRelations.Count > 0)
                {
                    _context.SongArtistRelations.RemoveRange(songArtistRelations);
                    await _context.SaveChangesAsync(cancellationToken);
                }

                _context.Songs.Remove(Song);
                await _context.SaveChangesAsync(cancellationToken);

                try
                {
                    await _mediator.Send(new DeleteSound.Command { Path = Path.Combine(command.SoundFolderPath, soundLocation) });
                }
                catch (FileNotFoundException)
                {
                    throw;
                }
                catch (Exception e)
                {
                    throw;
                }

                return soundLocation;
            }
        }
    }
}