using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Exceptions.Common;
using Application.ImageAccents;
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
            private readonly IMediator _mediator;
            public Handler(ApplicationDbContext context, IMediator mediator)
            {
                _context = context;
                _mediator = mediator;
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

                try
                {
                    await _mediator.Send(new DeleteImageAccent.Command { Path = filePath });
                }
                catch (NotExistingObjectExceptions)
                {
                    throw;
                }
                catch (Exception)
                {
                    throw;
                }


                return Unit.Value;
            }
        }
    }
}