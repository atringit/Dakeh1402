using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dake.Models
{
	public class AdminActivity
	{
		public int id { get; set;  }
		public int? userId { get; set; }
		public  User user { get; set; }
        public DateTime date { get; set; }
		public EnumStatus activityType { get; set;  }
		public long? noticeId { get; set; }
		public Notice notice { get; set; }
	}
	//public enum ActivityType
	//{
	//	accept  , 
	//	reject 
	//}
}
