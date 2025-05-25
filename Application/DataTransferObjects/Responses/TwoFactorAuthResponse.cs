using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.DataTransferObjects.Responses
{
    public class TwoFactorAuthResponse
    {
        public bool IsTwoFactorEnabled { get; set; }
        public string SecretKey { get; set; }
        public string QrCodeUri { get; set; }
        public IEnumerable<string> RecoveryCodes { get; set; }
    }

}