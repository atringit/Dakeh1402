using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dake.Models
{
    public enum EnumStatus
    {
        Pending = 1,
        Accept = 2,
        NotAccept = 3,
    }
    public class Notice
    {
        [Key]
        public long id { get; set; }

        [Display(Name = "فیلم ")]
        [MaxLength(500, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        public string movie { get; set; }

        [Display(Name = "تصویر اصلی ")]
        [MaxLength(500, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        public string image { get; set; }

        [Display(Name = "عنوان آگهی")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(200, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        public string title { get; set; }

        [Display(Name = "قیمت")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public long price { get; set; }

        [Display(Name = "قیمت آخر")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public long lastPrice { get; set; }

        [Display(Name = "توضیحات")]
        [MaxLength(1000, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        public string description { get; set; }



        [Display(Name = "دلیل رد")]
        [MaxLength(1000, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        public string notConfirmDescription { get; set; }

        [Display(Name = "لینک")]
        [MaxLength(1000, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        public string link { get; set; }

        public EnumStatus adminConfirmStatus { get; set; }
        [ForeignKey("user")]
        public int userId { get; set; }
        public virtual User user { get; set; }

        [Display(Name = "کد")]
        public string code { get; set; }

        [Display(Name = "تاریخ ایجاد")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(50, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        public DateTime createDate { get; set; }

        [Display(Name = "تاریخ انقضا")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(50, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        public DateTime expireDate { get; set; }

        [Display(Name = "تاریخ انقضا ویژه")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(50, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        public DateTime? expireDateIsespacial { get; set; }

        [ForeignKey("category")]
        public int categoryId { get; set; }
        public virtual Category category { get; set; }
        [Display(Name = "تعداد کل بازدید")]
        public int countView { get; set; }
        [ForeignKey("city")]
        public int cityId { get; set; }
        public virtual City city { get; set; }

        [ForeignKey("province")]
        public int provinceId { get; set; }
        public virtual Province province { get; set; }

        [ForeignKey("area")]
        public int areaId { get; set; }
        public virtual Area area { get; set; }
        public bool isSpecial { get; set; }
        public bool isEmergency { get; set; }
        public DateTime? ExpireDateEmergency { get; set; }
        public bool isPaid { get; set; }
        
        public DateTime? deletedAt { get; set; }

        public string AdminUserAccepted { get; set; }

        public DateTime? AcceptedDate { get; set; }

    }
}
