using Dake.Models;
using PagedList.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dake.Service.Interface
{
    public interface Icategory
    {
       PagedList<Category> GetCategories(int pageId = 1, string filterTitle = "");
       object GetCategories(int pageId = 1,int pagesize=10);
    }
}
