

namespace Dake.Models.ViewModels
{
    
    public class CustomMessage
    {
        public long id { get; set; }

        public int itemId { get; set; }
        public string text { get; set; }
        public string userSenderPhone { get; set; }
        public string userReceiverPhone { get; set; }
        public string date { get; set; }
        public string receiverId { get; set; }
        public int senderId { get; set; }
        public string Title { get; set; }
        

    }
}
