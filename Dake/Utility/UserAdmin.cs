using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Dake.Utility
{
    public class UserAdmin
    {

         [Display(Name = "رمز عبور فعلی")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(20, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        public string passwordShow { get; set; }
         [Display(Name = "رمز عبور جدید")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(20, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        public string newPasswordShow { get; set; }
         [Display(Name = "تکرار رمز عبور")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(20, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        [Compare("newPasswordShow")]
        public string reNewPasswordShow { get; set; }
    }
}
