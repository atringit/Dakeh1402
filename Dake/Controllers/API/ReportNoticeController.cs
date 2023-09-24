using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Dake.DAL;
using Dake.Models;
using Dake.Service.Interface;
using Newtonsoft.Json.Linq;
using Dake.Utility;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Dake.ViewModel;

namespace Dake.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportNoticeController : ControllerBase
    {
        private readonly Context _context;
       
        public ReportNoticeController(Context context)
        {
            _context = context;
        }
       
         [HttpPost]
        public object ReportNotice(ReportNoticeViewModel  reportNoticeViewModel)
        {
            string Token = HttpContext.Request?.Headers["Token"];
            var user = _context.Users.Where(p => p.token == Token).FirstOrDefault();
            if (user == null)
                return new { status = 3, message = "چنین کاربری وجود ندارد." };
            if(_context.ReportNotices.Any(x=>x.noticeId==reportNoticeViewModel.noticeId && x.userId==user.id))
                return new { status = 4, message = "قبلا گزارش شما ثبت شده است." };
            var reportNotice = new ReportNotice();
            reportNotice.noticeId = reportNoticeViewModel.noticeId;
            reportNotice.message = reportNoticeViewModel.message;
            reportNotice.userId =user.id;
            _context.ReportNotices.Add(reportNotice);
            _context.SaveChanges();
            return new { status = 1,title="گزارش آگهی", message = "گزارش آگهی با موفقیت انجام شد." };
        }
       
    }
}