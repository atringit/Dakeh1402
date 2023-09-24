using Dake.DAL;
using Dake.Models;
using Dake.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dake.Service
{
	public class staticPriceService : IStaticPrice
	{
		private Context _context;
		public staticPriceService(Context context)
		{
			_context = context;
		}
		public IEnumerable<StaticPrice> GetAll(string txtsearch)
		{
			int n;
			if (!string.IsNullOrEmpty(txtsearch))
			{
				if(int.TryParse(txtsearch, out n))
				{
				return _context.StaticPrices.Where(s=>s.code == txtsearch  || s.price == Convert.ToInt32(txtsearch) ).ToList();
				}

			}
			return _context.StaticPrices.ToList();
		}
		public void Remove(int Id)
		{
			var Item = _context.StaticPrices.Find(Id);
			if(Item != null)
			{
				var Categorys = _context.Categorys.Where(s => s.staticespacialPriceId == Item.code).ToList();
				var Categorys2 = _context.Categorys.Where(s => s.staticexpirePriceId == Item.code).ToList();
				var Categorys3 = _context.Categorys.Where(s => s.staticregisterPriceId == Item.code).ToList();
				var Categorys4 = _context.Categorys.Where(s => s.staticladerPriceId == Item.code).ToList();
				foreach (var item in Categorys)
				{
					item.staticespacialPriceId ="0";
				}
				foreach (var item in Categorys2)
				{
					item.staticexpirePriceId = "0";
				}
				foreach (var item in Categorys3)
				{
					item.staticregisterPriceId = "0";
				}
				foreach (var item in Categorys4)
				{
					item.staticladerPriceId = "0";
				}
				_context.StaticPrices.Remove(Item);
				_context.SaveChanges();
			}
		}
		public void AddOrUpdate(StaticPrice model)
		{
			if(model.id == 0)
			{
				_context.StaticPrices.Add(model);
			}
			else
			{
				_context.Update(model);
			}
			_context.SaveChanges();
		}

		public StaticPrice GetById(int Id)
		{
			var Item = _context.StaticPrices.Find(Id);
			if(Item != null)
			{
				return Item; 
			}

			else
			{
				return null;
			}

		}

		public bool CheckCode(string Code , long Price)
		{
			if (_context.StaticPrices.Any(s => s.code == Code) || _context.StaticPrices.Any(s=>s.price == Price))
			{
				return true;
			}
			else
			{
				return false; 
			}
		}

		public bool CheckByPrice(long Price)
		{
			if (_context.StaticPrices.Any(s => s.price == Price))
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		public string GetPriceCodeByPrice(long Price)
		{
			return _context.StaticPrices.FirstOrDefault(s => s.price == Price).code;
		}




	}
}
