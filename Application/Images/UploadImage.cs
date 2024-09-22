using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using MediatR.Pipeline;
using Microsoft.AspNetCore.Http;

namespace Application.Images
{
    public class UploadImage
    {
        public class Command : IRequest
        {
            public required IFormFile formFile { get; set; }
            public required string Path { get; set; }
            public required string Name { get; set; }
        }

        public class Handler : IRequestHandler<Command>
        {
            private readonly IMediator _mediator;
            public Handler(IMediator mediator)
            {
                _mediator = mediator;
            }
            public async Task<Unit> Handle(Command command, CancellationToken cancellationToken)
            {
                var file = command.formFile;
                IFormFile? jpegImage = null;
                try
                {
                    jpegImage = await _mediator.Send(new ConvertToJPG.Command { FormFile = file }, cancellationToken);
                }
                catch (Exception)
                {
                    throw;
                }

                var filePath = command.Path;
                var directoryPath = Path.GetDirectoryName(filePath);

                if (!String.IsNullOrEmpty(filePath) && !Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }

                using (var fileStream = new FileStream(Path.Combine(filePath, command.Name + ".jpg"), FileMode.Create, FileAccess.Write))
                {
                    await jpegImage.CopyToAsync(fileStream, cancellationToken);
                }

                return Unit.Value;
            }
        }
    }
}