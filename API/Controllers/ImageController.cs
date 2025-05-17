using Application.Images;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;

namespace API.Controllers
{
    [Route("image/")]
    public class ImageController : BaseController
    {
        private readonly IOutputCacheStore _outputCacheStore;

        public ImageController(IOutputCacheStore outputCacheStore)
        {
            _outputCacheStore = outputCacheStore;
        }

        [Authorize(Policy = "Admin")]
        [HttpPost("upload-image")]
        public async Task<IActionResult> uploadImage(string fileName, string path, IFormFile file, CancellationToken cancellationToken)
        {
            try
            {
                var resultFilePath = await Mediator.Send(new UploadImage.Command
                {
                    formFile = file,
                    Path = Path.Combine(ImageFolderPath, path),
                    Name = fileName,
                    RootImagePath = ImageFolderPath
                });

                return Ok(resultFilePath);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{*path}")]
        [OutputCache(PolicyName = "Images")]
        public async Task<IActionResult> getImage(string path)
        {
            string decodedPath = Uri.UnescapeDataString(path);
            if (decodedPath.StartsWith("/artist") || decodedPath.StartsWith("/album") || decodedPath.StartsWith("/playlist"))
            {
                decodedPath = decodedPath.Substring(1);
            }

            string imagePath = Path.Combine(ImageFolderPath, decodedPath);

            if (!System.IO.File.Exists(imagePath))
            {
                return NotFound("Image not found." + "\n" + imagePath);
            }

            var image = await System.IO.File.ReadAllBytesAsync(imagePath);
            return File(image, "image/jpeg");

        }

        // [HttpGet("cache-info/{path}")]
        // public async Task<IActionResult> GetCacheInfo(string path)
        // {
        //     string decodedPath = Uri.UnescapeDataString(path);
        //     if (decodedPath.StartsWith("/"))
        //     {
        //         decodedPath = decodedPath.Substring(1);
        //     }

        //     Console.WriteLine($"Cache info for path: {decodedPath}");

        //     var cached = await _outputCacheStore.GetAsync(decodedPath, CancellationToken.None);

        //     return Ok(new
        //     {
        //         IsCached = cached != null,
        //         CacheKey = decodedPath
        //     });
        // }

        [Authorize(Policy = "Admin")]
        [HttpPost("remove-image")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> removeImage([FromForm] string path)
        {
            try
            {
                await Mediator.Send(new DeleteImage.Command
                {
                    Path = Path.Combine(ImageFolderPath, path),
                    RootImagePath = ImageFolderPath
                });
                return Ok("Image removed successfully!");
            }
            catch (Exception e)
            {
                return NotFound(e.Message);
            }
        }
    }
}
