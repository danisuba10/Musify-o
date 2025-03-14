using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using FFMpegCore;
using FFMpegCore.Enums;

namespace Application.Sounds
{
    public class UploadSound
    {
        public class Command : IRequest<string>
        {
            public required IFormFile FormFile { get; set; }
            public required string Path { get; set; }
            public required string Name { get; set; }
        }
        public class Handler : IRequestHandler<Command, string>
        {
            public async Task<string> Handle(Command command, CancellationToken cancellationToken)
            {
                var file = command.FormFile;
                var filePath = command.Path;
                var directoryPath = Path.GetDirectoryName(filePath);

                if (!String.IsNullOrEmpty(directoryPath) && !Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                var tempFilePath = Path.GetTempFileName();
                using (var stream = new FileStream(tempFilePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream, cancellationToken);
                }

                var outputFilePath = Path.Combine(filePath, command.Name + ".opus");
                await TranscodeToOpus(tempFilePath, outputFilePath, cancellationToken);

                File.Delete(tempFilePath);

                return outputFilePath;
            }

            private async Task TranscodeToOpus(string inputFilePath, string outputFilePath, CancellationToken cancellationToken)
            {
                await FFMpegArguments
                    .FromFileInput(inputFilePath)
                    .OutputToFile(outputFilePath, true, options => options
                        .WithAudioCodec("libopus")
                        .WithAudioBitrate(128) // 128 kbps
                        .WithCustomArgument("-vbr on")) // Variable bitrate
                    .ProcessAsynchronously();
            }
        }
    }
}