using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Exceptions.Common;
using Application.ImageAccents;
using MediatR;
using Microsoft.AspNetCore.OutputCaching;
using Persistence;

namespace Application.Images
{
    public class DeleteImage
    {
        public class Command : IRequest
        {
            public required string Path { get; set; }
            public required string RootImagePath { get; set; }
        }

        public class Handler : IRequestHandler<Command>
        {
            private readonly IMediator _mediator;
            private readonly IOutputCacheStore _outputCacheStore;

            public Handler(IMediator mediator, IOutputCacheStore outputCacheStore)
            {
                _mediator = mediator;
                _outputCacheStore = outputCacheStore;
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

                await _outputCacheStore.EvictByTagAsync("image", cancellationToken);

                return Unit.Value;
            }
        }
    }
}