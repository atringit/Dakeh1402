using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Dake.Models
{
	public class StaticPrice
	{
		public int id { get; set; }
		[DisplayName("قیمت")]
		public long price { get; set; }
		[DisplayName("کد")]
		public string code { get; set; }
	}
}
