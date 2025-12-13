using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using API.Hubs;
using Microsoft.AspNetCore.SignalR;
using API.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BaseController : ControllerBase
    {
        private IMediator _mediator;
        private IConfiguration _config;
        protected IMediator Mediator => _mediator ??= HttpContext.RequestServices.GetService<IMediator>();
        protected IConfiguration Config => _config ??= HttpContext.RequestServices.GetService<IConfiguration>();
        protected string ImageFolderPath => Path.Combine(Directory.GetCurrentDirectory(), Config["ImageSettings:ImageFolderPath"]);
        protected string SoundFolderPath => Path.Combine(Directory.GetCurrentDirectory(), Config["SoundSettings:SoundFolderPath"]);
        protected IFileLogger FileLogger => HttpContext.RequestServices.GetService<IFileLogger>();
        protected IHubContext<NotificationHub> HubContext => HttpContext.RequestServices.GetService<IHubContext<NotificationHub>>();
        protected string GetCurrentUserIdentifier()
        {
            return User?.Claims?.FirstOrDefault(c => c.Type == "Identifier")?.Value ?? "anonymous";
        }

        protected async Task NotifyAndLog(string entityType, string operation, Guid? id = null, string details = null)
        {
            try
            {
                var user = GetCurrentUserIdentifier();
                var idString = id?.ToString() ?? string.Empty;
                var message = $"User: {user} Operation: {operation} Entity: {entityType} Id: {idString} Details: {details}";
                if (FileLogger != null)
                {
                    await FileLogger.LogAsync(message);
                }
                if (HubContext != null)
                {
                    try
                    {
                        // Notify all listeners about the entity modification
                        await HubContext.Clients.All.SendAsync("EntityModified", new { EntityType = entityType, Operation = operation, Id = idString });

                        // Log that signaling was successful and how many local connections exist
                        if (FileLogger != null)
                        {
                            var local = API.Hubs.NotificationHub.ConnectionCount;
                            var successMsg = $"SignalingSuccess: User: {user} Operation: {operation} Entity: {entityType} Id: {idString} LocalConnections: {local}";
                            await FileLogger.LogAsync(successMsg);
                        }
                    }
                    catch (Exception ex)
                    {
                        // If signaling fails, log the failure so the operator can inspect
                        if (FileLogger != null)
                        {
                            var failMsg = $"SignalingFailed: User: {user} Operation: {operation} Entity: {entityType} Id: {idString} Error: {ex.Message}";
                            try
                            {
                                await FileLogger.LogAsync(failMsg);
                            }
                            catch
                            {
                                // ignore logging failures to avoid secondary exceptions
                            }
                        }
                    }
                }
            }
            catch
            {
                // Swallow exceptions from logging/notifications to avoid interfering with controller flows
            }
        }

        protected async Task LogOnly(string entityType, string operation, Guid? id = null, string details = null)
        {
            try
            {
                var user = GetCurrentUserIdentifier();
                var idString = id?.ToString() ?? string.Empty;
                var message = $"User: {user} Operation: {operation} Entity: {entityType} Id: {idString} Details: {details}";
                if (FileLogger != null)
                {
                    await FileLogger.LogAsync(message);
                }
            }
            catch
            {
                // ignore
            }
        }
    }
}