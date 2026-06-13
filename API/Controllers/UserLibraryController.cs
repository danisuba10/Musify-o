using Application.DataTransferObjects.Responses;
using Application.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("library")]
    public class UserLibraryController : BaseController
    {
        [Authorize]
        [HttpPost("toggle")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> ToggleItem([FromForm] Guid itemId, [FromForm] string itemType)
        {
            Guid userId;
            if (!Guid.TryParse(User.Claims.FirstOrDefault(c => c.Type == "Identifier")?.Value, out userId))
                return Unauthorized("User identifier not found.");

            if (itemType != "Album" && itemType != "Artist" && itemType != "Playlist")
                return BadRequest("itemType must be 'Album', 'Artist', or 'Playlist'.");

            bool added = await Mediator.Send(new ToggleLibraryItem.Command
            {
                UserId = userId,
                ItemId = itemId,
                ItemType = itemType
            });

            await NotifyAndLog("UserLibraryItem", added ? "Add" : "Remove", itemId, $"Type: {itemType}");
            return Ok(new { added, itemId, itemType });
        }

        [Authorize]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetLibrary(
            [FromQuery] int pageSize = 25,
            [FromQuery] string? lastSavedAt = null,
            [FromQuery] string? lastItemId = null)
        {
            Guid userId;
            if (!Guid.TryParse(User.Claims.FirstOrDefault(c => c.Type == "Identifier")?.Value, out userId))
                return Unauthorized("User identifier not found.");

            if (pageSize <= 0 || pageSize > 50)
                pageSize = 25;

            var result = await Mediator.Send(new GetUserLibrary.Query
            {
                UserId = userId,
                PageSize = pageSize,
                LastSavedAt = lastSavedAt,
                LastItemId = lastItemId
            });

            return Ok(result);
        }

        [Authorize]
        [HttpGet("check")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> CheckItem([FromQuery] Guid itemId, [FromQuery] string itemType)
        {
            Guid userId;
            if (!Guid.TryParse(User.Claims.FirstOrDefault(c => c.Type == "Identifier")?.Value, out userId))
                return Unauthorized("User identifier not found.");

            if (itemType != "Album" && itemType != "Artist" && itemType != "Playlist")
                return BadRequest("itemType must be 'Album', 'Artist', or 'Playlist'.");

            bool inLibrary = await Mediator.Send(new CheckLibraryItem.Query
            {
                UserId = userId,
                ItemId = itemId,
                ItemType = itemType
            });

            return Ok(new { inLibrary });
        }
    }
}
