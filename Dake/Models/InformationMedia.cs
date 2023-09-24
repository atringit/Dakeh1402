using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dake.Models
{
	public class InformationMedia
	{
		public int Id { get; set;}
		public string Image { get; set; }
		public int InformationId { get; set; }
		public Information Information { get; set;  }
	}
}
