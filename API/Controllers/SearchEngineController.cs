using Microsoft.AspNetCore.Mvc;
using Search.Abstractions;

namespace API.Controllers;

[ApiController]
[Route("search/engine")]
public class SearchEngineController : ControllerBase
{
    private readonly ISearchEngine _engine;

    public SearchEngineController(ISearchEngine engine) => _engine = engine;

    [HttpGet]
    public async Task<IActionResult> Search(
        [FromQuery] string? term,
        [FromQuery] int pageSize = 20,
        [FromQuery] int skip = 0,
        [FromQuery] string? entityFilter = "All",
        [FromQuery] string? lastName = null,
        [FromQuery] DateTime? lastCreatedAt = null,
        CancellationToken ct = default)
    {
        if (!Enum.TryParse<SearchEntityType>(entityFilter, ignoreCase: true, out var filter))
            filter = SearchEntityType.All;

        var query = new SearchEngineQuery
        {
            Term          = term ?? string.Empty,
            EntityFilter  = filter,
            PageSize      = pageSize,
            Skip          = skip,
            LastName      = lastName,
            LastCreatedAt = lastCreatedAt
        };

        var result = await _engine.SearchAsync(query, ct);
        return Ok(result);
    }
}
