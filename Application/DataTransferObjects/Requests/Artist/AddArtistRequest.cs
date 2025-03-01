using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Application.DataTransferObjects.Requests
{
    public class AddArtistRequest
    {
        public required string Name { get; set; }
        public IFormFile? FormFile { get; set; }
    }
}