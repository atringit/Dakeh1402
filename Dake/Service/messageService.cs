using Dake.DAL;
using Dake.Models;
using Dake.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dake.Service
{
	public class messageService : IMessage
	{
		private Context _context;
		public messageService(Context context)
		{
			_context = context;
		}
		public void Add(Message model)
		{
			model.date = DateTime.Now; 
			_context.Messages.Add(model);
			_context.SaveChanges();
		}
		public IEnumerable<Message> get(int senderId, int reciverId , int noticeId)
		{
			var Items = _context.Messages.Where(s => s.ssenderId == senderId && s.rreceiverId == reciverId && s.ItemId == noticeId).ToList();
			var Items2 = _context.Messages.Where(s => s.ssenderId == reciverId && s.rreceiverId == senderId && s.ItemId == noticeId).ToList();
			List<Message> finallist = new List<Message>();
			foreach (var item in Items)
			{
				finallist.Add(item);
			}
			foreach (var item in Items2)
			{
				finallist.Add(item);
			}


			return finallist.OrderByDescending(s=>s.date).ToList(); 
		}

	}
}
