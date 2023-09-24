using System;

namespace Dake.Models
{
    public class BaseEntity
    {
        public BaseEntity()
        {
            CreateDate = DateTime.Now;
        }
        public long Id { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
    }
}
