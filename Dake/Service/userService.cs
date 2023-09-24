using Dake.DAL;
using Dake.Models;
using Dake.Models.ApiDto;
using Dake.Service.Interface;
using Dake.ViewModel;
using Microsoft.EntityFrameworkCore;
using PagedList.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dake.Service
{
    public class userService : IUser
    {
        private Context _context;
        public userService(Context context)
        {
            _context = context;
        }


        public PagedList<User> GetUsers(int pageId = 1, string filtercellphone = "", int CityId = 0)
        {
            IQueryable<User> result = _context.Users.Include(s=>s.province.city).IgnoreQueryFilters().Where(x=>x.role.RoleNameEn=="Member"&& x.deleted == null).OrderByDescending(x=>x.id);
            if(!string.IsNullOrEmpty(filtercellphone))
            {
                result = result.Where(x => x.cellphone.Contains(filtercellphone));
            }
            if (CityId != 0)
            {
                result = result.Where(x => x.province.cityId == CityId);
            }
            PagedList<User> res = new PagedList<User>(result, pageId, 10);
            return res;
        }
         public PagedList<UserDTO> GetAdminUsers(int pageId = 1, string filtercellphone = "")
         {
            IQueryable<UserDTO> result = _context.Users.IgnoreQueryFilters().Where(x => x.role.RoleNameEn == "Admin"&&x.deleted == null).OrderByDescending(x => x.id).Select(p => new UserDTO
            {
                id = p.id,
                cellphone = p.cellphone,
                Acceptcount = _context.Notices.Where(r => r.adminConfirmStatus == EnumStatus.Accept && r.AdminUserAccepted == p.cellphone).Count(),
                NotAcceptcount = _context.Notices.Where(r => r.adminConfirmStatus == EnumStatus.NotAccept && r.AdminUserAccepted == p.cellphone).Count()

            });
            if (!string.IsNullOrEmpty(filtercellphone))
            {
                result = result.Where(x => x.cellphone.Contains(filtercellphone));
            }
            PagedList<UserDTO> res = new PagedList<UserDTO>(result, pageId, 10);
            return res;
        }
    }
}
