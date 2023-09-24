using Dake.DAL;
using Dake.Models;
using Dake.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dake.Service
{
	public class discountCodeService : IDiscountCode
	{
		private Context _context;
		public discountCodeService(Context context)
		{
			_context = context;
		}

		public IEnumerable<DiscountCode> GetAll()
		{
			return _context.DiscountCodes.ToList();
		}
		public void Remove(int Id)
		{
			var Item = _context.DiscountCodes.Find(Id);
			if(Item != null)
			{
				_context.DiscountCodes.Remove(Item);
				_context.SaveChanges();
			}

		}
		public void AddOrUpdate(DiscountCode model)
		{
			if (model.id == 0)
			{
			
				_context.DiscountCodes.Add(model);
			}
			else
			{
				var item = _context.DiscountCodes.FirstOrDefault(s => s.id == model.id);
				item.count = model.count;
				item.price = model.price; 
			
			}
			_context.SaveChanges();
		}
		public DiscountCode GetById(int Id)
		{
			var Item = _context.DiscountCodes.Find(Id);
			if (Item != null)
			{
				return Item;
			}

			else
			{
				return null;
			}

		}

		public bool CheckCode(int code)
		{
			DiscountCode discountCodeItem = _context.DiscountCodes.FirstOrDefault(s => s.code == code); 
			if(discountCodeItem == null || discountCodeItem.count ==0)
			{
				return false; 
			}
			else
			{
				return true; 
			}
		}
		public long GetDiscountPrice(int code)
		{
			var Item = _context.DiscountCodes.FirstOrDefault(s => s.code == code);
			return Item.price; 
		}

		public bool IsAlreadyUsed(int userid , int code) 
		{
			return _context.UsersToDiscountCodes.Any(s => s.DiscountCodeId == code && s.UserId == userid);
		}
		public void AddUserToDiscountCode(int userid, int code)
		{
			User userItem = _context.Users.FirstOrDefault(s => s.id == userid);
			DiscountCode discountCodeItem = _context.DiscountCodes.FirstOrDefault(s => s.code == code);
			UsersToDiscountCode usersToDiscountCode = new UsersToDiscountCode();
			if(userItem !=null && discountCodeItem != null)
			{
				usersToDiscountCode.DiscountCodeId = discountCodeItem.id;
				usersToDiscountCode.UserId = userItem.id;
				_context.UsersToDiscountCodes.Add(usersToDiscountCode);
				discountCodeItem.count -= 1;
				discountCodeItem.remain += 1;

				_context.SaveChanges();
			}
		}
		public int GetIdByCode(int code)
		{
			var Item = _context.DiscountCodes.FirstOrDefault(s=>s.code == code);
			if (Item != null)
			{
				return Item.id;
			}
			else
			{
				return 0;
			}
		}

		public List<DiscountCode> GetDiscountCodeForUser(int userid)
		{
			User userItem = _context.Users.FirstOrDefault(s => s.id == userid);
			List<DiscountCode> mainlist = new List<DiscountCode>();
			if(userItem != null)
			{
				var userstodiscountcode = _context.UsersToDiscountCodes.Where(s => s.UserId == userid).ToList();
				foreach (var item in _context.DiscountCodes.ToList())
				{
					if(! userstodiscountcode.Any(s=>s.DiscountCodeId == item.id))
					{
						mainlist.Add(item);
					}

				}
				return mainlist; 

			}
			return null;

		}


	}
}
