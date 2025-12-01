using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace API.Hubs
{
    public class NotificationHub : Hub
    {
        // Intentionally left minimal: server will push notifications
        // No client-to-server methods required for this task.
    }
}
