using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Dake.Models
{
    public class VisitNoticeUser
    {
        [Key]
        public int id { get; set; }
        [ForeignKey("notice")]
        public long noticeId { get; set; }
        public virtual Notice notice { get; set; }
        [ForeignKey("user")]
        public int userId { get; set; }
        public virtual User user { get; set; }
    }
}
