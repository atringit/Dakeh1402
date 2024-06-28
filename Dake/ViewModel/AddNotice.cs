using Dake.Attributes.ValidationAttributes;
using Dake.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Dake.ViewModel
{
   
    public class AddNotice
    {
        [Key]
        public long id { get; set; }
        
        

        [Display(Name = "عنوان آگهی")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(200, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        [NoBannedWords(ErrorMessage = "لطفا از کلمات مناسب در {0} استفاده نمایید")]
        public string title { get; set; }

         [Display(Name = "قیمت")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string price { get; set; }

         [Display(Name = "قیمت آخر")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string lastPrice { get; set; }

        [Display(Name = "توضیحات")]
        [MaxLength(1000, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        [NoBannedWords(ErrorMessage = "لطفا از کلمات مناسب در {0} استفاده نمایید")]
        public string description { get; set; }

         [Display(Name = "مشخصات کلی")]
        [MaxLength(1000, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        public string totallDescription { get; set; }

        

        [Display(Name = "لینک")]
        [MaxLength(1000, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        public string link { get; set; }
       

        

         

         
        [Display(Name = "دسته بندی")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public int categoryId { get; set; }
        //[Display(Name = "تعداد کل بازدید")]
        //public int countView { get; set; }
        [Display(Name = "استان")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public int cityId { get; set; }
        [Display(Name = "شهرستان")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public int provinceId { get; set; }
        [Display(Name = "محدوده")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public int areaId { get; set; }
        [Display(Name = "تصویر اصلی ")]
        //public IFormFile image { get; set; }
       
        public IFormFile movie { get; set; }
        public string imageUrl { get; set; }
        public string movieUrl { get; set; }
        public List<NoticeImage> NoticeImages { get; set; }
        public List<IFormFile> image { get; set; }

        [Display(Name = "کد تخفیف")]
        public string discountcode { get; set; }
    }
}
