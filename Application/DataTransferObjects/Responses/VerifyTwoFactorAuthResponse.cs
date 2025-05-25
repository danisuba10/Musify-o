using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.DataTransferObjects.Responses
{
    public class VerifyTwoFactorAuthResponse
    {
        public bool Success { get; set; }
        public string? Token { get; set; }
    }
}