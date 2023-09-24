using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Dake.Utility
{
    public class LoginUserAdmin
    {
        [Display(Name = "موبایل")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(11, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        [MinLength(11, ErrorMessage = "{0} نمی تواند کمتر از {1} کاراکتر باشد .")]
        public string cellphone { get; set; }
        [Display(Name = "پسورد")]
        [MaxLength(200, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        public string password { get; set; }
         [Display(Name = "مرا به خاطر بسپار")]
        public bool RememberMe { get; set; }
        public int? majorId { get; set; }


    }
}
