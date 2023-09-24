using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Dake.Models
{
    public class Province
    {
        public int id { get; set; }
        [MaxLength(50)]
        public string name { get; set; }
       
        [ForeignKey("city")]
        public int cityId { get; set; }
        public virtual City city { get; set; }
    }
}
