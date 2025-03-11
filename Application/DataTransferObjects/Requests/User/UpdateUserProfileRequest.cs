using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Application.DataTransferObjects.Requests.User
{
    public class UpdateUserProfileRequest
    {
        public string? DisplayName { get; set; }
        public IFormFile? File { get; set; }
    }
}