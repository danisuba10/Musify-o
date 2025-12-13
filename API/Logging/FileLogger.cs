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
        private readonly bool _consoleLoggingEnabled = true;

        public FileLogger(IConfiguration configuration)
        {
            try
            {
                var logsFolder = Path.Combine(Directory.GetCurrentDirectory(), "Logs");
                if (!Directory.Exists(logsFolder)) Directory.CreateDirectory(logsFolder);
                // Prefer environment variables (loaded via .env) but fall back to IConfiguration
                var filenameEnv = Environment.GetEnvironmentVariable("Logging__CrudLogFileName");
                var fileName = !string.IsNullOrEmpty(filenameEnv) ? filenameEnv : (configuration["Logging:CrudLogFileName"] ?? "crud-operations.txt");
                _filePath = Path.Combine(logsFolder, fileName);

                // Allow toggling console logging via environment variable (prefer .env) or configuration
                var consoleEnv = Environment.GetEnvironmentVariable("Logging__ConsoleLoggingEnabled");
                if (!string.IsNullOrEmpty(consoleEnv) && bool.TryParse(consoleEnv, out var envEnabled))
                {
                    _consoleLoggingEnabled = envEnabled;
                }
                else
                {
                    var consoleConfig = configuration["Logging:ConsoleLoggingEnabled"];
                    if (!string.IsNullOrEmpty(consoleConfig) && bool.TryParse(consoleConfig, out var enabledConfig))
                    {
                        _consoleLoggingEnabled = enabledConfig;
                    }
                }
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
                if (_consoleLoggingEnabled)
                {
                    try
                    {
                        Console.WriteLine(line.TrimEnd());
                    }
                    catch
                    {
                        // Ignore console logging failure
                    }
                }
            }
            finally
            {
                _semaphore.Release();
            }
        }
    }
}
