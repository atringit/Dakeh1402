using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dake.Models
{
    public class FactorItem
    {
        [Key]
        public int id { get; set; }
        [Display(Name = "قیمت محصول")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public long price { get; set; }
        [ForeignKey("Product")]
        public long ProductId { get; set; }
        public virtual Notice Product { get; set; }
        [ForeignKey("Factor")]
        public int? FactorId { get; set; }
        public virtual Factor Factor { get; set; }
    }
}
