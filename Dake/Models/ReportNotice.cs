using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Dake.Models
{
    
    
    public class ReportNotice
    {
        [Key]
        public int id { get; set; }
        [ForeignKey("user")]
        public int userId { get; set; }
        public virtual User user { get; set; }
        [ForeignKey("notice")]
        public long noticeId { get; set; }
        public virtual Notice notice { get; set; }
        [Display(Name = "پیغام")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(5000, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        public string message { get; set; }

    }
}
