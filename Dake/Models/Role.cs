using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Dake.Models
{
    public class Role
    {
         [Key]
        public int Id { get; set; }
       [Display(Name = "نام نقش فارسی")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(1000, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        public string RoleNameFa { get; set; }
        [Display(Name = "نام نقش لاتین")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(50, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        public string RoleNameEn{ get; set; }
        public List<User> Users{ get; set; }
    }
}
