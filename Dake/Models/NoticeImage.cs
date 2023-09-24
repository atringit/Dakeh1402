using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Dake.Models
{
    public class NoticeImage
    {
        [Key]
        public long id { get; set; }
        [Display(Name = "تصویر ")]
       
        [MaxLength(500, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        public string image { get; set; }

        [ForeignKey("notice")]
        public long noticeId { get; set; }
        public virtual Notice notice { get; set; }
    }
}
