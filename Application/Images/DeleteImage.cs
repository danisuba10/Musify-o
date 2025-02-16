using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Persistence;

namespace Application.Images
{
    public class DeleteImage
    {
        public class Command : IRequest
        {
            public required string Path { get; set; }
        }

        public class Handler : IRequestHandler<Command>
        {
            private readonly ApplicationDbContext _context;
            public Handler(ApplicationDbContext context)
            {
                _context = context;
            }

            public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
            {
                var filePath = request.Path;

                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
                else
                {
                    throw new Exception("File does not exist at path!");
                }

                var imageAccent = _context.ImageAccents.FirstOrDefault(a => a.ImagePath == filePath);
                if (imageAccent != null)
                {
                    _context.ImageAccents.Remove(imageAccent);
                    await _context.SaveChangesAsync(cancellationToken);
                }

                return Unit.Value;
            }
        }
    }
}