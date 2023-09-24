using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dake.Models
{
	public class DiscountCode
	{
		public int id { get; set; }
		public  int code { get; set; }
		public int count { get; set; }
		public int remain { get; set; }
		public long price { get; set; }
	}
}
