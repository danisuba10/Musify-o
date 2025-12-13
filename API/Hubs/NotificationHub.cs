using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System;

namespace API.Hubs
{
    public class NotificationHub : Hub
    {
        // Tracks connections per instance (helpful for debugging and diagnostics)
        private static readonly ConcurrentDictionary<string, string> _connections = new();

        // Number of connections currently handled by this instance
        public static int ConnectionCount => _connections.Count;

        public override Task OnConnectedAsync()
        {
            try
            {
                var instance = Environment.GetEnvironmentVariable("HOSTNAME") ?? "instance";
                _connections[Context.ConnectionId] = instance;
                Console.WriteLine($"Hub Connected: {Context.ConnectionId} on {instance}; Total connections: {ConnectionCount}");
            }
            catch { }
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            try
            {
                _connections.TryRemove(Context.ConnectionId, out _);
                Console.WriteLine($"Hub Disconnected: {Context.ConnectionId}; Total connections: {ConnectionCount}");
            }
            catch { }
            return base.OnDisconnectedAsync(exception);
        }
    }
}
