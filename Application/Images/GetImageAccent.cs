using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using Domain;
using MediatR;
using Microsoft.AspNetCore.Http;
using ColorThief.ImageSharp;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.ColorSpaces.Conversion;
using SixLabors.ImageSharp.ColorSpaces;

namespace Application.Images
{
    public class GetImageAccent
    {
        public class Command : IRequest<ImageAccent>
        {
            public required IFormFile FormFile { get; set; }
        }

        public class Handler : IRequestHandler<Command, ImageAccent>
        {
            private readonly IMediator _mediator;
            public Handler(IMediator mediator)
            {
                _mediator = mediator;
            }

            public async Task<ImageAccent> Handle(Command command, CancellationToken cancellationToken)
            {
                var file = command.FormFile;
                byte[] imageBytes;
                using (var memoryStream = new MemoryStream())
                {
                    await file.CopyToAsync(memoryStream, cancellationToken);
                    imageBytes = memoryStream.ToArray();
                }

                using (var image = Image.Load<Rgba32>(imageBytes))
                {
                    var colorThief = new ColorThief.ImageSharp.ColorThief();
                    var dominantColor = colorThief.GetColor(image);
                    var middleAccent = AdjustColorBrightness(dominantColor.Color.ToString(), 18);
                    var lowAccent = AdjustColorBrightness(middleAccent, -55.1);
                    var highAccent = AdjustColorBrightness(middleAccent, 27.92);

                    return new ImageAccent
                    {
                        LowAccent = "#" + lowAccent,
                        MiddleAccent = "#" + middleAccent,
                        HighAccent = "#" + highAccent,
                        ImagePath = ""
                    };
                }
            }

            private static string AdjustColorBrightness(string hexColor, double percentage)
            {
                Rgba32 rgba = Rgba32.ParseHex(hexColor);
                var hsv = ColorSpaceConverter.ToHsv(rgba);

                if (percentage >= 0)
                {
                    //Darker
                    double newV = Math.Max(0, Math.Min(1, hsv.V * (1 - percentage / 100)));
                    hsv = new Hsv(hsv.H, hsv.S, (float)newV);
                }
                else
                {
                    //Lighter
                    double newV = Math.Max(0, Math.Min(1, hsv.V * (1 + Math.Abs(percentage) / 100)));
                    hsv = new Hsv(hsv.H, hsv.S, (float)newV);
                }

                Rgba32 adjustedColor = ColorSpaceConverter.ToRgb(hsv);
                return adjustedColor.ToHex()[0..6];
            }
        }
    }
}