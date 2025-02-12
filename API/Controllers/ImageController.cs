using Application.Images;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class ImageController : BaseController
    {
        [HttpPost("uploadImage")]
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

        [HttpGet("image/{path}")]
        public async Task<IActionResult> getImage(string path)
        {
            string decodedPath = Uri.UnescapeDataString(path);
            string imagePath = Path.Combine(ImageFolderPath, decodedPath);

            if (!System.IO.File.Exists(imagePath))
            {
                return NotFound("Image not found." + "\n" + imagePath);
            }

            var image = await System.IO.File.ReadAllBytesAsync(imagePath);
            return File(image, "image/jpeg");

        }
    }
}
