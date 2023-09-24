using Dake.DAL;
using Dake.Models.ViewModels;
using Microsoft.Extensions.Configuration;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Dake.Service
{
    public class PushNotificationService : IPushNotificationService
    {
        private readonly Context _context;

        readonly string token;
        readonly string apiKey;
        string dateSend;
        readonly string iconUrl;
        readonly string imageUrl;
        public IConfiguration Configuration { get; }

        public PushNotificationService(Context context, IConfiguration configuration)
        {
            _context = context;

            apiKey = "dc17d23e-8ff6-477e-a463-fa46f8d95ed4";
            token = "c86d7cf2cd39350c624ef9101b4360e9bd9a6688";
            iconUrl = "https://dakeh.net/assets/img/dakeh-wh512.jpg";
            imageUrl = "https://dakeh.net/assets/img/dakeh-wh512.jpg";
        }

        public async Task<IRestResponse> SendNotifToAll(VmPushNotification pushModel)
        {
            var client = new RestClient("https://app.najva.com/api/v1/notifications/");
            var request = new RestRequest(Method.POST);
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("content-type", "application/json");
            request.AddHeader("authorization", "Token " + token);

            dateSend = DateTime.Now.AddMinutes(1).ToString("yyyy-MM-ddTHH:mm:ss");

            object _body = new
            {
                api_key = apiKey,
                title = pushModel.Title,
                body = pushModel.Body.Length > 150 ? pushModel.Body.Substring(0, 150) : pushModel.Body,
                onclick_action = pushModel.OnclickAction == OnclickAction.OpenApp ? "open-app" : "open-link",
                url = pushModel.Url,
                icon = iconUrl,
                image = string.IsNullOrEmpty(pushModel.ImgUrl)? imageUrl : pushModel.ImgUrl,
                sent_time = dateSend,
                one_signal_enabled = false
            };


            var body = JsonSerializer.Serialize(_body);

            request.AddParameter("application/json", body, ParameterType.RequestBody);
            var response = await client.ExecuteAsync(request);
            return response;
        }
        public async Task<IRestResponse> SendNotifToSpecialUser(VmPushNotification pushModel)
        {
            var client = new RestClient("https://app.najva.com/notification/api/v1/notifications/");
            var request = new RestRequest(Method.POST);
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("content-type", "application/json");
            request.AddHeader("authorization", "Token " + token + "");

            dateSend = DateTime.Now.AddMinutes(1).ToString("yyyy-MM-ddTHH:mm:ss");

            var UserToken = _context.Users.Where(p => p.id == pushModel.UserId).Select(p => p.PushNotifToken).ToList();


            object _body = new // for special user
            {
                api_key = apiKey,
                title = pushModel.Title,
                body = pushModel.Body,
                onclick_action = pushModel.OnclickAction == OnclickAction.OpenApp ? "open-app" : "open-link",
                url = pushModel.Url,
                icon = iconUrl,
                image = string.IsNullOrEmpty(pushModel.ImgUrl) ? imageUrl : pushModel.ImgUrl,
                sent_time = dateSend,
                subscriber_tokens = UserToken
            };

            var body = JsonSerializer.Serialize(_body);

            request.AddParameter("application/json", body, ParameterType.RequestBody);
            var response = await client.ExecuteAsync(request);
            return response;
        }

        public async Task<IRestResponse> SendNotifToSpecialUsers(VmPushNotification pushModel)
        {
            var client = new RestClient("https://app.najva.com/notification/api/v1/notifications/");
            var request = new RestRequest(Method.POST);
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("content-type", "application/json");
            request.AddHeader("authorization", "Token " + token + "");

            dateSend = DateTime.Now.AddMinutes(1).ToString("yyyy-MM-ddTHH:mm:ss");

            List<string> UsersToken = new List<string>();

            foreach (var userId in pushModel.UsersId)
            {
                UsersToken.Add(_context.Users.Where(p => p.id == userId).Select(p => p.PushNotifToken).FirstOrDefault());
            }


            object _body = new // for special user
            {
                api_key = apiKey,
                title = pushModel.Title,
                body = pushModel.Body,
                onclick_action = pushModel.OnclickAction == OnclickAction.OpenApp ? "open-app" : "open-link",
                url = pushModel.Url,
                icon = iconUrl,
                image = string.IsNullOrEmpty(pushModel.ImgUrl) ? imageUrl : pushModel.ImgUrl,
                sent_time = dateSend,
                subscriber_tokens = UsersToken
            };

            var body = JsonSerializer.Serialize(_body);

            request.AddParameter("application/json", body, ParameterType.RequestBody);
            var response = await client.ExecuteAsync(request);
            return response;
        }
    }
}
