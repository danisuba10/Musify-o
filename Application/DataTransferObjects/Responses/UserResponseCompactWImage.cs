using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.DataTransferObjects.Responses
{
    public class UserResponseCompactWImage
    {
        public required Guid Id { get; set; }
        public required String Name { get; set; }
        public ImageResponse? Image { get; set; }
    }
}