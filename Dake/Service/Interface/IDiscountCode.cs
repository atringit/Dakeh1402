using Dake.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dake.Service.Interface
{
	public interface IDiscountCode
	{
		IEnumerable<DiscountCode> GetAll( );
		void Remove(int Id);
		void AddOrUpdate(DiscountCode model);
		DiscountCode GetById(int Id);
		bool CheckCode(int code);
		long GetDiscountPrice(int code);
		bool IsAlreadyUsed(int userid, int code);
		void AddUserToDiscountCode(int userid, int code);
		int GetIdByCode(int code);
		List<DiscountCode> GetDiscountCodeForUser(int userid);
	}
}
