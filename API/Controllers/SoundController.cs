using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using FFMpegCore;
using Application.Sounds;
using Microsoft.AspNetCore.Authorization;

namespace API.Controllers
{
    [Authorize(Policy = "Admin")]
    [Route("sound/")]
    public class SoundController : BaseController
    {

        [HttpDelete("{path}/delete")]
        public async Task<IActionResult> deleteSound(string path)
        {
            try
            {
                await Mediator.Send(new DeleteSound.Command { Path = Path.Combine(SoundFolderPath, path) });
                return Ok("Sound file successfully deleted!");
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

        [HttpGet("{path}")]
        public async Task<IActionResult> getSound(string path, [FromQuery] string format = "opus", [FromQuery] int bitrate = 128)
        {
            string decodedPath = Uri.UnescapeDataString(path);
            if (decodedPath.StartsWith("/song"))
            {
                decodedPath = decodedPath.Substring(1);
            }

            string soundFilePath = Path.Combine(SoundFolderPath, decodedPath);

            if (!System.IO.File.Exists(soundFilePath))
            {
                return NotFound("Sound not found." + "\n" + soundFilePath);
            }

            var sound = await System.IO.File.ReadAllBytesAsync(soundFilePath);

            string outputFileName = Path.GetFileNameWithoutExtension(soundFilePath) + $".{format}";
            string outputFilePath = Path.Combine(SoundFolderPath, outputFileName);

            if (!System.IO.File.Exists(outputFilePath))
            {
                await TranscodeToFormat(soundFilePath, outputFilePath, format, bitrate);
            }

            string mimeType = GetMimeType(format);

            var fileStream = new FileStream(outputFilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            var fileStreamResult = new FileStreamResult(fileStream, mimeType)
            {
                EnableRangeProcessing = true
            };
            return fileStreamResult;
        }

        private async Task TranscodeToFormat(string inputFilePath, string outputFilePath, string format, int bitrate)
        {
            await FFMpegArguments
                .FromFileInput(inputFilePath)
                .OutputToFile(outputFilePath, true, options => options
                    .WithAudioCodec(GetCodec(format))
                    .WithAudioBitrate(bitrate))
                .ProcessAsynchronously();
        }

        private string GetCodec(string format)
        {
            return format.ToLower() switch
            {
                "mp3" => "libmp3lame",
                "aac" => "aac",
                "opus" => "libopus",
                _ => throw new NotSupportedException($"Format '{format}' is not supported.")
            };
        }

        private string GetMimeType(string format)
        {
            return format.ToLower() switch
            {
                "mp3" => "audio/mpeg",
                "opus" => "audio/ogg", // Opus files typically use the Ogg container
                "aac" => "audio/aac",
                _ => "application/octet-stream" // Default MIME type for unknown formats
            };
        }
    }
}