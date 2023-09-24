using Dake.DAL;
using Dake.Models.ViewModels;
using Dake.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Dake.Controllers
{
    public class PushNotificationController : Controller
    {
        private readonly IPushNotificationService _pushNotificationService;
        public PushNotificationController(IPushNotificationService pushNotificationService)
        {
            _pushNotificationService = pushNotificationService;
        }

        [HttpPost]
        public async Task<IActionResult> SendPushNotificationToAll([FromBody] VmPushNotification pushModel)
        {
            var result = await _pushNotificationService.SendNotifToAll(pushModel);
            if (result.IsSuccessful)
            {
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }
    }
}
