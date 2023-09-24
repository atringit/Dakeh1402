using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Dake.Models
{
    public class Information
    {
        [Key]
        public int id { get; set; }

        [Display(Name = "عنوان ")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(500, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        public string title { get; set; }

         [Display(Name = "توضیحات ")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(5000, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        public string description { get; set; }


        [Display(Name = "لینک ")]
        /*[Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(5000, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]*/
        public string Link { get; set; }

        public ICollection<InformationMedia> InformationMedias { get; set;  }
    }
}
