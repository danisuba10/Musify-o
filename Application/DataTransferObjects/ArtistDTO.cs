using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.DataTransferObjects
{
    public class ArtistDTO
    {
        public Guid? Id { get; set; } = null;
        public string Name { get; set; } = string.Empty;
        public string? ImageLocation { get; set; } = null;
    }
}