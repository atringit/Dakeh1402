using Dake.Models.ViewModels;
using Microsoft.Extensions.Configuration;
using RestSharp;
using System.Threading.Tasks;

namespace Dake.Service
{
    public interface IPushNotificationService
    {
        IConfiguration Configuration { get; }

        Task<IRestResponse> SendNotifToAll(VmPushNotification pushModel);
        Task<IRestResponse> SendNotifToSpecialUser(VmPushNotification pushModel);
        Task<IRestResponse> SendNotifToSpecialUsers(VmPushNotification pushModel);
    }
}