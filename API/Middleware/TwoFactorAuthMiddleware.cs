using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Middleware
{
    public class TwoFactorAuthMiddleware
    {
        private readonly RequestDelegate _next;

        public TwoFactorAuthMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            // Skip middleware for these endpoints
            if (ShouldSkipMiddleware(context))
            {
                await _next(context);
                return;
            }

            var twoFactorPending = context.User?.Claims?
                .FirstOrDefault(c => c.Type == "TwoFactorPending")?.Value;

            var twoFactorEnabled = context.User?.Claims?
                .FirstOrDefault(c => c.Type == "TwoFactorEnabled")?.Value;

            if (twoFactorEnabled.Equals("true") && !twoFactorPending.Equals("false"))
            {
                context.Response.StatusCode = 403; // Forbidden
                await context.Response.WriteAsync("Two-factor authentication required");
                return;
            }

            await _next(context);
        }

        private bool ShouldSkipMiddleware(HttpContext context)
        {
            var path = context.Request.Path.Value;

            // Skip for these endpoints
            return path.StartsWith("/user/login") ||
                   path.StartsWith("/user/register") ||
                   path.StartsWith("/user/2fa/verify") ||
                   path.StartsWith("/user/2fa/recovery") ||
                   path.StartsWith("/user/2fa/qrcode") ||
                   path.StartsWith("/swagger") ||
                   path.StartsWith("/health");
        }
    }
}
