using Dake.DAL;
using Dake.Models;
using Dake.Service.Interface;
using PagedList.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dake.Service
{
     public class categoryService : Icategory
    {
        private Context _context;
        public categoryService(Context context)
        {
            _context = context;
        }
        public object GetCategories(int page = 1, int pagesize = 10)
        {
            IQueryable<Category> result = _context.Categorys;
            int skip = (page - 1) * pagesize;
            List<Category> res = result.OrderBy(u => u.id).Skip(skip).Take(pagesize).ToList();
            return new { data = res, totalCount = result.Count() };
        }
         
        private bool HasChild(int id)
        {
            if (_context.Categorys.Any(x => x.parentCategoryId == id))
                return true;
            else
                return false;
        }
        public PagedList<Category> GetCategories(int pageId = 1, string filterTitle = "")
        {
            IQueryable<Category> result = _context.Categorys.Where(x=>x.parentCategoryId==null).OrderByDescending(x=>x.id);
            if (!string.IsNullOrEmpty(filterTitle))
            {
                result = result.Where(u => u.name.Contains(filterTitle));
            }
            PagedList<Category> res = new PagedList<Category>(result, pageId, 10);
            return res;
        }
    }
}
