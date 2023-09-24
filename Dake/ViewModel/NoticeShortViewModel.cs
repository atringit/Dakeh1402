using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dake.ViewModel
{
	public class NoticeShortViewModel
	{
		public long id { get; set;  }
		public string title { get; set; }
		public string url { get; set;  }
		public int categoryId { get; set; }
	}
}
