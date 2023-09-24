using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dake.Models
{
    public class Banner
    {
        public long Id { get; set; }
        public string title { get; set; }
        public virtual ICollection<BannerImage> BannerImage { get; set; }
          

        //[Display(Name = "توضیحات")]
        //[MaxLength(1000, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        //public string description { get; set; }



        [Display(Name = "دلیل رد")]
        [MaxLength(1000, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        public string notConfirmDescription { get; set; }

        [Display(Name = "لینک")]
        [MaxLength(1000, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        public string Link { get; set; }

        public EnumStatus adminConfirmStatus { get; set; } = EnumStatus.Pending;
        [ForeignKey("user")]
        public int userId { get; set; }
        public virtual User user { get; set; }

        [Display(Name = "کد")]
        public string code { get; set; }

        [Display(Name = "تاریخ ایجاد")]
        //[MaxLength(50, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        public DateTime createDate { get; set; } = DateTime.Now;

        [Display(Name = "تاریخ انقضا")]
       // [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        //[MaxLength(50, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        public DateTime expireDate { get; set; }

        [Display(Name = "تاریخ انقضا ویژه")]
       // [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        //[MaxLength(50, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        public DateTime? expireDateIsespacial { get; set; }

        [Display(Name = "تعداد کل کلیک")]
        public int countView { get; set; }
        
        public bool isSpecial { get; set; }
        public bool isEmergency { get; set; }
        public DateTime? ExpireDateEmergency { get; set; }
        public bool isPaid { get; set; }

        public string AdminUserAccepted { get; set; }

        public DateTime? AcceptedDate { get; set; }

    }
}
