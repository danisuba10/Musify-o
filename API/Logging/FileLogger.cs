using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace API.Logging
{
    public class FileLogger : IFileLogger
    {
        private readonly string _filePath;
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        public FileLogger(IConfiguration configuration)
        {
            try
            {
                var logsFolder = Path.Combine(Directory.GetCurrentDirectory(), "Logs");
                if (!Directory.Exists(logsFolder)) Directory.CreateDirectory(logsFolder);
                _filePath = Path.Combine(logsFolder, configuration["Logging:CrudLogFileName"] ?? "crud-operations.txt");
            }
            catch
            {
                _filePath = Path.Combine(Directory.GetCurrentDirectory(), "Logs", "crud-operations.txt");
            }
        }

        public async Task LogAsync(string message)
        {
            var timestamp = DateTime.UtcNow.ToString("o");
            var line = $"[{timestamp}] {message}{Environment.NewLine}";
            await _semaphore.WaitAsync();
            try
            {
                await File.AppendAllTextAsync(_filePath, line, Encoding.UTF8);
            }
            finally
            {
                _semaphore.Release();
            }
        }
    }
}
