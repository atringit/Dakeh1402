using System;

namespace Dake.Models.ApiDto
{
    public class GeNoticeMessageDTO
    {
        public long id { get; set; }
        public string Date { get; set; }
        public string senderPhone { get; set; }
        public string recieverPhone { get; set; }
        public string userCellPhone { get; set; }
        public int senderId { get; set; }
        public int recieverId { get; set; }
        //public string shouldGetPhone { get; set; }
        //public int shouldGetId { get; set; }
        public int itemId { get; set; }
        public string text { get; set; }
    }
}
