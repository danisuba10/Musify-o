using System.Threading.Tasks;

namespace API.Logging
{
    public interface IFileLogger
    {
        Task LogAsync(string message);
    }
}
