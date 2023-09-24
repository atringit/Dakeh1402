using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dake.Models
{
	public class AdminsInProvice
	{
		public int id { get; set; }
		public int? userId { get; set; }
		public User user { get; set; }
		public int? adminsInCityId { get; set;  }
		public AdminsInCity adminsInCity { get; set;  }
		public int? provinceId { get; set;  }
		public Province province { get; set; }

	}
}
