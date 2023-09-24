using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Dake.DAL;
using Dake.Models;
using Dake.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Dake.Utility;
using PagedList.Core;
using Dake.Models.ViewModels;

namespace Dake.Controllers
{
    [Authorize]

    public class ReportNoticeController : Controller
    {
        private readonly Context _context;
        private IReportNotice _ReportNotice;

        public ReportNoticeController(Context context, IReportNotice ReportNotice)
        {
            _context = context;
            _ReportNotice = ReportNotice;
        }

        // GET: Information
        public IActionResult Index(int page = 1)
        {
            var model = _ReportNotice.GetReportNotice(page);
            return View(model);
        }
       public IActionResult GetReportNotice(int id)
        {
            if (id == 0)
            {
                return NotFound();
            }

            var ReportNotice =  _context.ReportNotices.Include(x=>x.notice).Include(x=>x.user).FirstOrDefault(x=>x.id==id);
            if (ReportNotice == null)
            {
                return NotFound();
            }
            return Json(ReportNotice);

        }

        [HttpGet]
        public IActionResult AllChats(int id, int page = 1)
        {

            var result = _context.Messages
                .Where(n => n.MessageType == MessageType.CrashReport && n.ItemId == id)
                .ToList()
                .Select(n => new CustomMessage
                {
                    id = n.id,
                    senderId = n.ssenderId,
                    itemId = n.ItemId,
                    Title = _context.ReportNotices.Include(p=>p.notice).Single(u => u.id == n.ItemId).notice.title,
                    text = n.text,
                    date = n.date.ToPersianDateString(),
                    userSenderPhone = _context.Users
                        .Single(u => u.id == n.ssenderId)
                        .cellphone,
                    userReceiverPhone = _context.Users
                        .Single(u => u.id == n.rreceiverId)
                        .cellphone
                })
                .ToList();

            var response = new List<CustomMessage>();
            response.AddRange(result);

            var result2 = response.AsQueryable();

            PagedList<CustomMessage> res = new PagedList<CustomMessage>(result2, page, 20);

            return View(res);
        }

    }
}
