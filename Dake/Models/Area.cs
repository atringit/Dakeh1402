using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Dake.Models
{
    public class Area
    {
        public int id { get; set; }
        [MaxLength(50)]
        public string name { get; set; }
       
        [ForeignKey("province")]
        public int provinceId { get; set; }
        public virtual Province province { get; set; }
    }
}
