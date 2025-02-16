using Application.Images;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("image/")]
    public class ImageController : BaseController
    {
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
                    Name = fileName
                });

                return Ok(resultFilePath);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{path}")]
        public async Task<IActionResult> getImage(string path)
        {
            string decodedPath = Uri.UnescapeDataString(path);
            if (decodedPath.StartsWith("/artist") || decodedPath.StartsWith("/album"))
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

        [Authorize(Policy = "Admin")]
        [HttpPost("remove-image")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> removeImage([FromForm] string path)
        {
            try
            {
                await Mediator.Send(new DeleteImage.Command { Path = Path.Combine(ImageFolderPath, path) });
                return Ok("Image removed successfully!");
            }
            catch (Exception e)
            {
                return NotFound(e.Message);
            }
        }
    }
}
