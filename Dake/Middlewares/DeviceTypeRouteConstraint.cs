using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Dake.Middlewares
{
    public class DeviceTypeRouteConstraint : IRouteConstraint
    {
        private readonly string _deviceType;

        public DeviceTypeRouteConstraint(string deviceType)
        {
            _deviceType = deviceType.ToLower();
        }

        public bool Match(HttpContext httpContext, IRouter route, string routeKey, RouteValueDictionary values, RouteDirection routeDirection)
        {
            var userAgent = httpContext.Request.Headers["User-Agent"].ToString().ToLower();

            // Check the device type based on User-Agent
            if (userAgent.Contains("android"))
            {
                httpContext.Items["DeviceType"] = "Android";
                return true;
            }
            if (_deviceType == "ios" && (userAgent.Contains("iphone") || userAgent.Contains("ipad") || userAgent.Contains("ipod")))
            {
                httpContext.Items["DeviceType"] = "iOS";
                return true;
            }

            return false;
        }
    }
}
