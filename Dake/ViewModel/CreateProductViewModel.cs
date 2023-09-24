using Dake.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Dake.ViewModel
{
    public class CreateProductViewModel
    {
        [Key]
        public int id { get; set; }

        [Display(Name = "عنوان محصول")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(200, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        public string title { get; set; }

        [Display(Name = "توضیحات")]
        [MaxLength(1000, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        public string description { get; set; }
       
         [Display(Name = "تصویر")]

        public IFormFile image { get; set; }
        public string imageName { get; set; }
         
        public bool isActive { get; set; }

    }
}
