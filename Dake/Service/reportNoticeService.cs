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
    public class reportNoticeService : IReportNotice
    {
        private Context _context;
        public  reportNoticeService(Context context)
        {
            _context = context;
        }
       
        
        public PagedList<ReportNotice> GetReportNotice(int pageId = 1)
        {
            IQueryable<ReportNotice> result = _context.ReportNotices.Include(x=>x.notice).Include(x=>x.user).OrderByDescending(x => x.id);
            PagedList<ReportNotice> res = new PagedList<ReportNotice>(result, pageId, 10);
            return res;
        }

        
    }
}
