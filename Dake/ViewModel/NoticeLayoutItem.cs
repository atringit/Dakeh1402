using Dake.Models;

namespace Dake.ViewModel
{
    public class NoticeLayoutItem
    {
        public Notice Notice { get; set; }
        public bool IsSpecial { get; set; }
        public bool IsRight { get; set; }
        public bool IsEmpty { get; set; }
    }
}
