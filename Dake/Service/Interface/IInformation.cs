using Dake.Models;
using PagedList.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dake.Service.Interface
{
    public interface IInformation
    {
        object GetInformations(int pageId = 1, int pagesize = 10);
        PagedList<Information> GetInformations(int pageId = 1, string filterTitle = "");

    }
}
