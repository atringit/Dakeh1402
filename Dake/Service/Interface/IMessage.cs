using Dake.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dake.Service.Interface
{
	public interface IMessage
	{
		IEnumerable<Message> get(int senderId, int reciverId , int noticeId); 
		 void Add(Message model);
	}
}
