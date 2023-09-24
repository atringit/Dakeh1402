using Dake.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dake.ViewModel
{
    public class DetailNoticeViewModel
    {
        public Notice notice { get; set; }
        public List<NoticeImage> noticeImages { get; set; }
        public List<Category> Categorys { get; set; }
        public int userId { get; set; }

        ///
        public List<Notice> Relatednotices { get; set; }

    }
}
