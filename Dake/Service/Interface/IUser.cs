using Dake.Models;
using Dake.Models.ApiDto;
using Dake.ViewModel;
using PagedList.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dake.Service.Interface
{
   public interface IUser
    {
       PagedList<User> GetUsers(int pageId = 1, string filterFullName = "", int CityId = 0);
       PagedList<UserDTO> GetAdminUsers(int pageId = 1, string filterFullName = "");
    }
}
