using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Dake.Models
{
    public class Setting
    {
         [Key]
        public int id { get; set; }

        [Display(Name = "کلمات اشتباه در درج آگهی")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(2000, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        public string wrongWord { get; set; }

        [Display(Name = "تعداد روزها برای تمدید آگهی")]
        public int? countExpireDate { get; set; }
        [Display(Name = "تعداد روزها برای ویژه بودن آگهی")]
        public int? countExpireDateIsespacial { get; set; }

        [Display(Name = "تعداد روزها برای اضطراری بودن آگهی")]
        public int? countExpireDateEmergency { get; set; }

        [Display(Name = "نمایش قیمت برای خودروها")]
        public bool showPriceForCars { get; set;  }
        [Display(Name = "نمایش بنر ها")]
        public int? countExpireDateBanner { get; set; }
		[Display(Name = "ویژه شدن خودکار اگهی")]
		public int? countToSpecialNotice { get; set; }
        /// <summary>
        /// برای مواقعی که ادمینی برای تایید آگهی ها وجود ندارد، 
        /// </summary>
        [Display(Name = "تایید خودکار آگهی")]
        public bool AutoAccept { get; set;  }


    }
}
