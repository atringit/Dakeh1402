using Dake.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dake.ViewModel
{
    public class FirstHomeViewModel
    {
        public List<Notice> notices { get; set; }
        public List<NoticeImage> NoticeImage { get; set; }
        public List<Notice> espacialNotices { get; set; }
        public List<Category> Categories { get; set; }
        public List<Banner> Banner { get; set; }
        public string subCat { get; set; }
    }
}
