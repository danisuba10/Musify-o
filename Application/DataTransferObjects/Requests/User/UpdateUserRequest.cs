using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Application.DataTransferObjects.Requests
{
    public class UpdateUserRequest
    {
        public Guid Id { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? Role { get; set; }
        public string? DisplayName { get; set; }
        public IFormFile? File { get; set; }
        public bool? IsTwoFactorEnabled { get; set; }
    }
}