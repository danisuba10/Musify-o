using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class MiscleController : BaseController
    {
        [HttpGet("GetGUID")]
        public Guid GetGUID()
        {
            return Guid.NewGuid();
        }
    }
}