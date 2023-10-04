using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Dake.Models
{
    public class User
    {
        [Key]
        public int id { get; set; }        

        [Display(Name = "موبایل")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(11, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        [MinLength(11, ErrorMessage = "{0} نمی تواند کمتر از {1} کاراکتر باشد .")]
        public string cellphone { get; set; }

        [Display(Name = "پسورد")]
        //[Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(200, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        public string password { get; set; }

        [Display(Name = "پسورد")]
        //[Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(20, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        public string passwordShow { get; set; }

       
        [MaxLength(100, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        public string token { get; set; }
        [ForeignKey("role")]
        public int roleId { get; set; }
        public Role role { get; set; }

        [Display(Name = "کد")]
        //[Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(6, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        public string code { get; set; }
        public bool isCodeConfirmed { get; set; }
        [Display(Name = "تاریخ احراز هویت")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(50, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        public DateTime oTPDate { get; set; }
        [ForeignKey("province")]
        public int? provinceId { get; set; }
        public virtual Province province { get; set; }

         [Display(Name = "نقش های کاربر ادمین")]
        [MaxLength(50, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        public string adminRole { get; set; }

        public bool IsBlocked { get; set; } = false;
        public string PushNotifToken { get; set; }
        public string deleted { get; set; }
        [MaxLength(100, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        public string Invite_Link { get; set; }
        public int Invite_Price { get; set; }
    }
}
