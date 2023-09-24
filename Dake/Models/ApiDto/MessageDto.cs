namespace Dake.Models.ApiDto
{
    public class AddMessageDto
    {
        public string text { get; set; }
        public string phone { get; set; }
        public int itemId { get; set; }
        
        public string receiverPhone { get; set; }
        public int messageType { get; set; }
    }
    public class DeleteMessagDto
    {
        public long SenderUserId { get; set; }
        public long itemId { get; set; }
        public string phone { get; set; }
    }

    public class GetNoticeMessageUserDto
    {
        public int noticeId { get; set; }
        public int senderId { get; set; }
    }
    public class GetCrashReportMessageUser
    {
        public int reportId { get; set; }
        public int senderId { get; set; }
    }
}
