using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain;

namespace Application.DataTransferObjects.Responses
{
    public class UserResponse
    {
        public required Guid Id { get; set; }
        public required string DisplayName { get; set; }
    }
}