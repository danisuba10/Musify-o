using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Application.DataTransferObjects.Requests
{
    public class UpdateArtistRequest
    {
        public Guid Id { get; set; }
        public IFormFile? FormFile { get; set; }
        public string? Name { get; set; }
    }
}