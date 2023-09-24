using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dake.Models
{
	public class UsersToDiscountCode
	{
		public int id { get; set; }
		public int DiscountCodeId { get; set; }
		public DiscountCode DiscountCode { get; set; }
		public int UserId { get; set; }
		public User  User { get; set; }
	}
}
