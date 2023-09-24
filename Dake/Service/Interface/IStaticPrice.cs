using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dake.Models;
namespace Dake.Service.Interface
{
public	interface IStaticPrice
	{
		IEnumerable<StaticPrice>  GetAll(string txtsearch);
		void Remove(int Id);
		void AddOrUpdate(StaticPrice model);
		StaticPrice GetById(int Id);
		bool CheckCode(string Code , long Price);
		bool CheckByPrice(long Price);
		string GetPriceCodeByPrice(long Price); 
	}
}
