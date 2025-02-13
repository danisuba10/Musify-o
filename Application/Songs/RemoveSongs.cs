using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Songs
{
    public class RemoveSongs
    {
        public class Command : IRequest<int>
        {
            public List<Guid> SongIds { get; set; }
        }

        public class Handler : IRequestHandler<Command, int>
        {
            private readonly ApplicationDbContext _context;
            public Handler(ApplicationDbContext context)
            {
                _context = context;
            }
            public async Task<int> Handle(Command command, CancellationToken cancellationToken)
            {
                if (command.SongIds == null || !command.SongIds.Any())
                {
                    throw new Exception("No song IDs provided!");
                }

                var songsToRemove = await _context.Songs
                    .Where(s => command.SongIds.Contains(s.Id))
                    .ToListAsync(cancellationToken);

                if (songsToRemove.Count == 0)
                {
                    throw new Exception("No songs found with the provided IDs.");
                }

                _context.Songs.RemoveRange(songsToRemove);
                await _context.SaveChangesAsync(cancellationToken);

                return command.SongIds.Count - songsToRemove.Count;
            }
        }
    }
}