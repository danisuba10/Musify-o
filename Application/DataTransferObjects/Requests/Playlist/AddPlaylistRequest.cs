using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Application.DataTransferObjects.Requests
{
    public class AddPlaylistRequest
    {
        public required string Name { get; set; }
        public required Guid UserId { get; set; }
        public string? Description { get; set; }
        public IFormFile? FormFile { get; set; }
    }
}