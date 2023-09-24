using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Dake.Models
{
    public class Slider
    {
        [Key]
        public int id { get; set; }
        [Display(Name = "تصویر ")]
       
        [MaxLength(500, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        public string image { get; set; }

        [NotMapped]
        [Display(Name = "تصویر ")]

        public IFormFile imageUrl { get; set; }

        [Display(Name = "لینک")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(200, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        public string link { get; set; }
    }
}
