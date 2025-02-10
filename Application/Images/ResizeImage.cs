using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace Application.Images
{
    public class ResizeImage
    {
        public class Command : IRequest<IFormFile>
        {
            public required IFormFile FormFile { get; set; }
            public int Width { get; set; }
            public int Height { get; set; }
        }

        public class Handler : IRequestHandler<Command, IFormFile>
        {
            public async Task<IFormFile> Handle(Command command, CancellationToken cancellationToken)
            {
                var file = command.FormFile;
                var width = command.Width;
                var height = command.Height;

                if (!IsSupportedImageFormat(file))
                {
                    throw new Exception("File format not supported! Only JPEG and PNG are supported!");
                }

                var croppedFile = await CropToCenterAndResize(file, width, height);
                return croppedFile;
            }

            private bool IsSupportedImageFormat(IFormFile file)
            {
                return file.ContentType.Equals("image/jpeg", StringComparison.OrdinalIgnoreCase) ||
                       file.ContentType.Equals("image/png", StringComparison.OrdinalIgnoreCase) ||
                       file.FileName.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
                       file.FileName.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase) ||
                       file.FileName.EndsWith(".png", StringComparison.OrdinalIgnoreCase);
            }

            private async Task<IFormFile> CropToCenterAndResize(IFormFile file, int targetWidth, int targetHeight)
            {
                using (var inputStream = file.OpenReadStream())
                {
                    using (var image = await Image.LoadAsync(inputStream))
                    {
                        ResizeToCenterCrop(image, targetWidth, targetHeight);

                        var memoryStream = new MemoryStream();

                        if (file.ContentType.Equals("image/png", StringComparison.OrdinalIgnoreCase))
                        {
                            await image.SaveAsPngAsync(memoryStream);
                        }
                        else if (file.ContentType.Equals("image/jpeg", StringComparison.OrdinalIgnoreCase))
                        {
                            await image.SaveAsJpegAsync(memoryStream);
                        }

                        memoryStream.Position = 0;

                        var resizedFile = new FormFile(memoryStream, 0, memoryStream.Length, file.Name, file.FileName)
                        {
                            Headers = file.Headers,
                            ContentType = file.ContentType
                        };
                        return resizedFile;
                    }
                }
            }

            private void ResizeToCenterCrop(Image image, int targetWidth, int targetHeight)
            {
                // Calculate the scale ratio to cover the target dimensions
                float scaleRatio = Math.Max(
                    (float)targetWidth / image.Width,
                    (float)targetHeight / image.Height
                );

                // Resize first to cover target area
                image.Mutate(x => x.Resize(new ResizeOptions
                {
                    Size = new Size(
                        (int)(image.Width * scaleRatio),
                        (int)(image.Height * scaleRatio)
                    ),
                    Mode = ResizeMode.Stretch
                }));

                // Then center crop to exact dimensions
                image.Mutate(x => x.Crop(new Rectangle(
                    (image.Width - targetWidth) / 2,
                    (image.Height - targetHeight) / 2,
                    targetWidth,
                    targetHeight
                )));
            }
        }
    }
}