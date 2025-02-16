using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain;
using MediatR;
using MediatR.Pipeline;
using Microsoft.AspNetCore.Http;
using Persistence;

namespace Application.Images
{
    public class UploadImage
    {

        public class Command : IRequest<string>
        {
            public required IFormFile formFile { get; set; }
            public required string Path { get; set; }
            public required string Name { get; set; }
        }

        public class Handler : IRequestHandler<Command, string>
        {
            private readonly IMediator _mediator;
            private readonly ApplicationDbContext _context;
            public Handler(IMediator mediator, ApplicationDbContext context)
            {
                _mediator = mediator;
                _context = context;
            }
            public async Task<string> Handle(Command command, CancellationToken cancellationToken)
            {
                var file = command.formFile;
                IFormFile? jpegImage;
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

                string resultFilePath = Path.Combine(filePath, command.Name + ".jpg");
                using (var fileStream = new FileStream(resultFilePath, FileMode.Create, FileAccess.Write))
                {
                    await jpegImage.CopyToAsync(fileStream, cancellationToken);
                }

                ImageAccent accent = await _mediator.Send(new GetImageAccent.Command { FormFile = jpegImage });
                accent.ImagePath = resultFilePath;

                var existingAccent = _context.ImageAccents.FirstOrDefault(a => a.ImagePath == resultFilePath);
                if (existingAccent != null)
                {
                    _context.Entry(existingAccent).CurrentValues.SetValues(accent);
                }
                else
                {
                    _context.Add(accent);
                }
                await _context.SaveChangesAsync(cancellationToken);

                return resultFilePath;
            }
        }
    }
}