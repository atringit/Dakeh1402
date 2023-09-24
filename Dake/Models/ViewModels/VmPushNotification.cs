using System.Collections.Generic;

namespace Dake.Models.ViewModels
{
    public class VmPushNotification
    {
        public string Title { get; set; }
        public string Body { get; set; }
        public string ImgUrl { get; set; }
        public string Url { get; set; }
        public int UserId { get; set; }
        public List<int> UsersId { get; set; }
        public OnclickAction OnclickAction { get; set; } = OnclickAction.OpenApp;
    }

    public enum OnclickAction
    {
        OpenLink = 0,
        OpenApp = 1
    }
}
