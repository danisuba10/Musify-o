using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.DataTransferObjects.Responses
{
    public class LoginResponse
    {
        public string Token { get; set; }
        public bool RequiresTwoFactor { get; set; }
    }
}