using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Domain;
using Persistence;
using Application.DataTransferObjects.Requests;

namespace Application.Songs
{
    public class AddSong
    {
        public class Command : IRequest<Guid>
        {
            public required Song Song;
        };

        public class Handler : IRequestHandler<Command, Guid>
        {
            private readonly ApplicationDbContext _context;
            public Handler(ApplicationDbContext context)
            {
                _context = context;
            }
            public async Task<Guid> Handle(Command command, CancellationToken cancellationToken)
            {
                _context.Songs.Add(command.Song);
                await _context.SaveChangesAsync();
                return command.Song.Id;
            }
        }
    }
}