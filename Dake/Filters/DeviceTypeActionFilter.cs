using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Dake.Filters
{
    public class DeviceTypeActionFilter : IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext context)
        {
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.HttpContext.Items.ContainsKey("DeviceType"))
            {
                var deviceType = context.HttpContext.Items["DeviceType"];
                var controller = context.Controller as Controller;
                controller.ViewBag.DeviceType = deviceType;
            }
        }
    }
}
