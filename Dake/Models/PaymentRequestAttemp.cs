using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dake.Models
{
	public class PaymentRequestAttemp
	{
		public int Id { get; set; }
		public int UserId { get; set; }
		public long NoticeId { get; set; }
		public int FactorId { get; set; }
		public pursheType pursheType { get; set; }
	}


	public enum pursheType
	{
		RegisterNotice ,
		Ladders , 
		MakeSpecial,
		Extend,
		Special,
		emergency

	}
}
