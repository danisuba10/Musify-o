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
        [HttpPost("mixed")]
        public async Task<GlobalSearchResult> mixedSearch(string term, CancellationToken cancellationToken)
        {
            GlobalSearchResult result = await Mediator.Send(new GlobalSearch.Query { SearchString = term }, cancellationToken);
            return result;
        }
    }
}