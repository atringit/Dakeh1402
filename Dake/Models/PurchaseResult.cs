using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dake.Models
{
	public class PurchaseResult
	{
		public string OrderId { get; set; }
		public string Token { get; set; }
		public string ResCode { get; set; }
		public VerifyResultData VerifyResultData { get; set; }


	}
}
