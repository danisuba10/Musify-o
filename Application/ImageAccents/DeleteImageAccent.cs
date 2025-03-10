using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Exceptions.Common;
using MediatR;
using Persistence;

namespace Application.ImageAccents
{
    public class DeleteImageAccent
    {
        public class Command : IRequest<Unit>
        {
            public required string Path { get; set; }
        }
        public class Handler : IRequestHandler<Command, Unit>
        {
            private readonly ApplicationDbContext _context;
            public Handler(ApplicationDbContext context)
            {
                _context = context;
            }
            public async Task<Unit> Handle(Command cmd, CancellationToken cancellationToken)
            {
                var imageAccent = _context.ImageAccents
                    .FirstOrDefault(ia => ia.ImagePath.Equals(cmd.Path));

                if (imageAccent == null)
                {
                    throw new NotExistingObjectExceptions("Image accent");
                }

                _context.Remove(imageAccent);
                await _context.SaveChangesAsync(cancellationToken);
                return Unit.Value;
            }
        }
    }
}