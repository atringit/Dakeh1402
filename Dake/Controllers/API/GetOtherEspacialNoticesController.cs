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
    public class GetOtherEspacialNoticesController : ControllerBase
    {
        private readonly Context _context;
        private INotice _notice;
        private readonly IHostingEnvironment environment;

        public GetOtherEspacialNoticesController(Context context, INotice notice, IHostingEnvironment environment)
        {
            this.environment = environment;
            _context = context;
            _notice = notice;
        }
       
        [HttpPost]
        public object GetOtherEspacialNotices(AllEspacial allEspacial)
        {
            string Token = HttpContext.Request?.Headers["Token"];
            var data = _notice.GetAllEspacialNotices(Token, allEspacial.noticeId,allEspacial.scroll);
            return data;
        }
       
       
        
        private object DailyVisit(int noticeId)
        {
            var visitNotice = _context.VisitNotices.Where(x => x.noticeId == noticeId).OrderByDescending(x => x.id).Select(x => new { x.id, x.countView, date = DateToUnix(x.date)/*, date1 = Convert.ToInt32(DateTime.UtcNow.Subtract(x.date).TotalSeconds)*/ });
            var dailyVisitNotice = visitNotice.Take(7);
            //var monthlyVisitNotice = visitNotice.GroupBy(x => x.date.Month).ToList();
            //foreach (var item in visitNotice)
            //{

            //}
            return dailyVisitNotice;

        }
        public long DateToUnix(DateTime date)
        {
             date.AddHours(-3);
            date.AddMinutes(-30);
            TimeSpan timeSpan = date - new DateTime(1970, 1, 1, 0, 0, 0,DateTimeKind.Utc);
            return (long)timeSpan.TotalSeconds;
        }
    }
}