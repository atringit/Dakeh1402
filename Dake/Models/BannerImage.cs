using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dake.Models
{
    public class BannerImage : BaseEntity
    {
        public string FileLocation { get; set; }
        public string Name { get; set; }
        public long BannerId { get; set; }
        public virtual Banner Banner { get; set; }
    }
}
