using Dake.DAL;
using Dake.Models;
using Dake.Service.Interface;
using Microsoft.EntityFrameworkCore;
using PagedList.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dake.Service
{
    public class informationService : IInformation
    {
         private Context _context;
         public informationService(Context context)
        {
            _context = context;
        }
        public object GetInformations(int page = 1, int pagesize = 10)
        {
             IQueryable<Information> result = _context.Informations.Include(s => s.InformationMedias);
            int skip = (page - 1) * pagesize;
            List<Information> res=result.OrderByDescending(u => u.id).Skip(skip).Take(pagesize).ToList();
            return new { data=res,totalCount=result.Count() };
        }
         public PagedList<Information> GetInformations(int page = 1, string filterTitle = "")
        {
             IQueryable<Information> result = _context.Informations
                .Include(s => s.InformationMedias)
                .OrderByDescending(x=>x.id);
            if (!string.IsNullOrEmpty(filterTitle))
            {
                result = result.Where(u => u.title.Contains(filterTitle));
            }
            PagedList<Information> res=new PagedList<Information>(result,page,20);
            return res;
        }
    }
}
