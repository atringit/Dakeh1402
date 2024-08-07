using Dake.Attributes.ValidationAttributes;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dake.Models
{
    public class Category
    {
        [Key]
        public int id { get; set; }

        [Display(Name = "قیمت ثبت برای این دسته بندی")]
        [Required(ErrorMessage = "لطفا قیمت ثبت را وارد کنید")]
        [PriceValidation(ErrorMessage = "قیمت واردشده باید از {0} تومان بیشتر باشد")]
        public long registerPrice { get; set; }

        [Display(Name = "قیمت تمدید برای این دسته بندی")]
        [Required(ErrorMessage = "لطفا قیمت تمدید آگهی را وارد کنید")]
        [PriceValidation(ErrorMessage = "قیمت واردشده باید از {0} تومان بیشتر باشد")]
        public long expirePrice { get; set; }

        [Display(Name = "قیمت ویژه برای این دسته بندی")]
        [Required(ErrorMessage = "لطفا قیمت ویژه کردن آگهی را وارد کنید")]
        [PriceValidation(ErrorMessage = "قیمت واردشده باید از {0} تومان بیشتر باشد")]
        public long espacialPrice { get; set; }

        [ForeignKey("parentCategory")]
        public int? parentCategoryId { get; set; }

        public virtual Category parentCategory { get; set; }

        [Display(Name = "نام دسته بندی")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(200, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        public string name { get; set; }
        [Display(Name = "تصویر دسته بندی ")]
        [MaxLength(500, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        public string image { get; set; }
        [NotMapped]
        [Display(Name = "تصویر ")]
        public IFormFile imageUrl { get; set; }

        [Display(Name = "قیمت نردبان برای این دسته بندی")]
        [Required(ErrorMessage = "لطفا قیمت نردبان کردن آگهی را وارد کنید")]
        [PriceValidation(ErrorMessage = "قیمت واردشده باید از {0} تومان بیشتر باشد")]
        public long laderPrice { get; set; }

        [Display(Name = "قیمت اضطراری برای این دسته بندی")]
        [Required(ErrorMessage = "لطفا اضطراری کردن آگهی را وارد کنید")]
        [PriceValidation(ErrorMessage = "قیمت واردشده باید از {0} تومان بیشتر باشد")]
        public long emergencyPrice { get; set; }
        public string staticemergencyPriceId { get; set; }
        public string staticregisterPriceId { get; set; }
        public string staticexpirePriceId { get; set; }
        public string staticespacialPriceId { get; set; }
        public string staticladerPriceId { get; set; }
    }
}
