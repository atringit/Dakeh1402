using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Dake.Models
{
    public class ContactUs
    {
        public int id { get; set; }
        [Display(Name = "تلفن")]
        [MaxLength(50, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        public string phone { get; set; }

        [Display(Name = "آدرس تلگرام")]
        [MaxLength(200, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        public string pageTelegramUrl { get; set; }

        [Display(Name = "آدرس اینستاگرام")]
        [MaxLength(200, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        public string pageInstagramUrl { get; set; }

        [Display(Name = "آدرس توییتر")]
        [MaxLength(200, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        public string pageTwitterUrl { get; set; }

        [Display(Name = "ایمیل")]
        [MaxLength(200, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        public string email { get; set; }

        [Display(Name = "ورژن اندروید")]
        [MaxLength(20, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        public string androidVersion { get; set; }
		[Display(Name = "ادرس ایتا")]
		[MaxLength(200, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
		public string PageEitta { get; set; }
	}
}
