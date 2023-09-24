using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dake.Models
{
    public class Message
	{
		//[ForeignKey("id")]
		public long id { get; set;  }
		public string text { get; set;  }
		public DateTime date { get; set;  }
		public int rreceiverId { get; set;  }
		public int ssenderId { get; set;  }
		public MessageType MessageType { get; set; }

		public int ItemId { get; set; }

		//[ForeignKey("Noticeid")]
		//get notice by item id
		public virtual Notice Notice { get; set; }
		
		//get user by sender id
		public virtual User sender { get; set; }
		
		//get user by receiver id
		public virtual User receiver { get; set; }
	}

	public enum MessageType
	{
		Notice = 0,
		CrashReport = 1
    }
}
