using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Dake.Middlewares
{
    public class DeviceBasedRoutingMiddleware
    {
        private readonly RequestDelegate _next;

        public DeviceBasedRoutingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var userAgent = context.Request.Headers["User-Agent"].ToString().ToLower();

            // Detect device type
            if (userAgent.Contains("android"))
            {
                context.Items["DeviceType"] = "Android";
            }
            else if (userAgent.Contains("iphone") || userAgent.Contains("ipad") || userAgent.Contains("ipod"))
            {
                context.Items["DeviceType"] = "iOS";

                // Reroute for iOS to the SPA fallback route
                context.Request.Path = "/Home/Index";
            }
            else
            {
                context.Items["DeviceType"] = "Other";
            }

            // Call the next middleware in the pipeline
            await _next(context);
        }
    }
}
