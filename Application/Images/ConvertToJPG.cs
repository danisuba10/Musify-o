using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Drawing;
using System.Drawing.Imaging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;

namespace Application.Images
{
    public class ConvertToJPG
    {
        public class Command : IRequest<IFormFile>
        {
            public required IFormFile FormFile { get; set; }
        }
        public class Handler : IRequestHandler<Command, IFormFile>
        {
            public async Task<IFormFile> Handle(Command command, CancellationToken cancellationToken)
            {
                var file = command.FormFile;

                if (IsJPEG(file))
                {
                    var jpgFile = await ConvertPNGToJPG(file);
                    return jpgFile;
                }
                else if (IsPNG(file))
                {
                    var jpgFile = await ConvertPNGToJPG(file);
                    return jpgFile;
                }

                throw new Exception("File format not supported! Only JPEG and PNG are supported.");
            }

            public bool IsJPEG(IFormFile file)
            {
                return file.ContentType.Equals("image/jpeg", StringComparison.OrdinalIgnoreCase) ||
                file.FileName.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
                file.FileName.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase);
            }

            public bool IsPNG(IFormFile file)
            {
                return file.ContentType.Equals("image/png", StringComparison.OrdinalIgnoreCase) ||
                file.FileName.EndsWith(".png", StringComparison.OrdinalIgnoreCase);
            }

            private async Task<IFormFile> ConvertPNGToJPG(IFormFile file)
            {
                using (var inputStream = file.OpenReadStream())
                {
                    using (var image = await Image.LoadAsync(inputStream))
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            var Encoder = new JpegEncoder { Quality = 60 };

                            await image.SaveAsJpegAsync(memoryStream, Encoder);
                            memoryStream.Position = 0;

                            var jpgFile = new FormFile
                                (memoryStream, 0, memoryStream.Length, file.Name + "edit", Path.ChangeExtension(file.FileName, "jpg"))
                            {
                                Headers = file.Headers,
                                ContentType = "image/jpeg"
                            };

                            return jpgFile;
                        }
                    }
                }
            }
        }
    }
}