using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.DataTransferObjects.Responses
{
    public class ImageResponse
    {
        public string ImageLocation { get; set; } = "";
        public string LowColor { get; set; } = "";
        public string MiddleColor { get; set; } = "";
        public string HighColor { get; set; } = "";
    }
}