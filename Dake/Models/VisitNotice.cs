using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Dake.Models
{
    public class VisitNotice
    {
        [Key]
        public int id { get; set; }
        [ForeignKey("notice")]
        public long noticeId { get; set; }
        public virtual Notice notice { get; set; }

        

        [Display(Name = "تاریخ ")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(50, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        public DateTime date { get; set; }
        public int countView { get; set; }
    }
}
