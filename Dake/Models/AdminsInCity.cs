using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dake.Models
{
	public class AdminsInCity
	{
		public int id { get; set; }
		public int userid { get; set; }
		public virtual User user { get; set; }
		public int  cityId { get; set; }
		public virtual City city { get; set;  }

	}
}
