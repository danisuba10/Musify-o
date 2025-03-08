using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Application.Search;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace API.Controllers
{
    [Route("search/")]
    public class SearchController : BaseController
    {
        [HttpGet("mixed")]
        public async Task<GlobalSearchResult> mixedSearch([FromQuery] string Term, CancellationToken cancellationToken)
        {
            Guid? userId = null;
            if (Guid.TryParse(User.Claims.FirstOrDefault(c => c.Type == "Identifier")?.Value, out Guid parsedUserId))
            {
                userId = parsedUserId;
            }

            var userRole = User.Claims.FirstOrDefault(c => c.Type == "Role")?.Value;

            GlobalSearchResult result = await Mediator.Send(new GlobalSearch.Query { SearchString = Term, UserId = userId, Role = userRole }, cancellationToken);
            return result;
        }
    }
}