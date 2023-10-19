using Dake.DAL;
using Dake.Models;
using Microsoft.AspNetCore.Mvc;

namespace Dake.Controllers
{
    public class PaymentsController: Controller
    {
        private readonly Context _context;
        public PaymentsController(Context context)
        {
            _context = context;
        }
        public IActionResult Index(int id)
        {
            if (HttpContext.Request.Query["Status"] != "" && HttpContext.Request.Query["Status"].ToString().ToLower() == "ok"
                && HttpContext.Request.Query["Authority"] != "")
            {
                string Authority = HttpContext.Request.Query["Authority"].ToString();
                var factor = _context.Factors.Find(id);
                int total = (int)factor.totalPrice;
                var pay = new ZarinpalSandbox.Payment(total);
                var res = pay.Verification(Authority).Result;
                if(res.Status == 100)
                {
                    factor.state = State.IsPay;
                    var notice = _context.Notices.Find(factor.noticeId);
                    notice.isPaid = true;
                    _context.Notices.Update(notice);
                    _context.SaveChanges();
                    _context.Factors.Update(factor);
                    _context.SaveChanges();
                    ViewBag.code = res.RefId;
                    return View();
                }
            }
            return NotFound();
        }
    }
}
