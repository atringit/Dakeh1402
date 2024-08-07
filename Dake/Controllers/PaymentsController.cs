using Dake.DAL;
using Dake.Models;
using Dake.Service;
using Dake.Service.Common;
using Dake.Service.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.SqlServer.Management.Smo;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Dake.Controllers
{
    public class PaymentsController: Controller
    {
        private readonly Context _context;
        private readonly IBannerSevice _bannerSevice;
        private readonly IPaymentService _paymentService;

        public PaymentsController(Context context, IBannerSevice bannerSevice, IPaymentService paymentService)
        {
            _context = context;
            _bannerSevice = bannerSevice;
            _paymentService = paymentService;
        }
        public async Task<IActionResult> Index(int id)
        {
            var status = HttpContext.Request.Query["Status"].ToString();
            var authority = HttpContext.Request.Query["Authority"].ToString();

            if (!string.IsNullOrEmpty(status) &&
                status.Equals("ok", StringComparison.OrdinalIgnoreCase)
                && !string.IsNullOrEmpty(authority))
            {
                ViewBag.Status = true;
                var factor = _context.Factors.Find(id);
                int total = (int)factor.totalPrice;
                
                (var isVerified, var refId) = await _paymentService.VerifyPayment(authority, total);

                if(isVerified)
                {
                    var autoAccept = await _context
                        .Settings
                        .Select(s => s.AutoAccept)
                        .FirstOrDefaultAsync();

                    factor.state = State.IsPay;
                    var notice = _context.Notices.Find(factor.noticeId);
                    notice.isPaid = true;

                    ////تایید خودکار آگهی //////////////////////
                    if (autoAccept)
                    {
                        var userCellphone = await _context
                            .Users
                            .Where(w => w.id == notice.userId)
                            .Select(s => s.cellphone)
                            .FirstOrDefaultAsync();

                        notice.adminConfirmStatus = EnumStatus.Accept;

                        CommonService.SendSMS_Accept(userCellphone, notice.title);
                    }

                    _context.Notices.Update(notice);
                    _context.SaveChanges();
                    _context.Factors.Update(factor);
                    _context.SaveChanges();
                    ViewBag.code = refId;
                    return View();
                }
            }
            ViewBag.Status = false;
            return View();
        }
        public async Task<IActionResult> Banner(int id)
        {
            var status = HttpContext.Request.Query["Status"].ToString();
            var authority = HttpContext.Request.Query["Authority"].ToString();

            if (!string.IsNullOrEmpty(status) &&
                status.Equals("ok", StringComparison.OrdinalIgnoreCase)
                && !string.IsNullOrEmpty(authority))
            {
                ViewBag.Status = true;
                var factor = _context.Factors.Find(id);
                int total = (int)factor.totalPrice;

                (var isVerified, var refId) = await _paymentService.VerifyPayment(authority, total);

                if (isVerified)
                {
                    factor.state = State.IsPay;
                    var banner = _context.Banner.Find(factor.bannerId);
                    banner.isPaid = true;
                    _context.Banner.Update(banner);
                    _context.SaveChanges();
                    _context.Factors.Update(factor);
                    _context.SaveChanges();
                    ViewBag.code = refId;

                    await _bannerSevice.ConfirmBanner(banner);

                    return View();
                }
            }
            ViewBag.Status = false;
            return View();
        }

        public async Task<IActionResult> Purshe(int id)
        {
            var status = HttpContext.Request.Query["Status"].ToString();
            var authority = HttpContext.Request.Query["Authority"].ToString();

            if (!string.IsNullOrEmpty(status) &&
                status.Equals("ok", StringComparison.OrdinalIgnoreCase)
                && !string.IsNullOrEmpty(authority))
            {
                var paymentAttempt = await _context.PaymentRequestAttemps.Where(w => w.FactorId == id).FirstOrDefaultAsync();
                if (paymentAttempt != null)
                {
                    var factor = _context.Factors.Find(id);
                    int total = (int)factor.totalPrice;

                    (var isVerified, var refId) = await _paymentService.VerifyPayment(authority, total);

                    if (isVerified)
                    {
                        ViewBag.code = refId;
                        var notice = await _context.Notices.Where(w => w.id == paymentAttempt.NoticeId).FirstOrDefaultAsync();

                        switch (paymentAttempt.pursheType)
                        {
                            case pursheType.RegisterNotice:
                                notice.isPaid = true;
                                ViewBag.PursheResult = "کاربر گرامی ، عملیات پرداخت موفقیت آمیز بود ، پس از برسی ادمین ، آگهی شما قابل مشاهده می باشد.";
                                break;
                            case pursheType.Ladders:
                                notice.createDate = DateTime.Now;
                                ViewBag.PursheResult = $"کاربرگرامی ، عملیات نردبان کردن آگهی {notice.title} با موفقیت انجام شد";
                                break;
                            case pursheType.Extend:
                                var countExpireDate = await _context.Settings.Select(s => s.countExpireDate).FirstOrDefaultAsync();

                                notice.expireDate = notice.expireDate.AddDays((double)countExpireDate);
                                var daysofextend = countExpireDate ?? 0;

                                ViewBag.PursheResult = $"کاربر گرامی ، عملیات تمدید آگهی {notice.title} به مدت {daysofextend} روز با موفقیت انجام شد";

                                break;
                            case pursheType.Special:
                                var countSpecialExpireDate = await _context.Settings.Select(s => s.countToSpecialNotice).FirstOrDefaultAsync();

                                notice.isSpecial = true;
                                notice.expireDateIsespacial = notice.expireDate.AddDays((double)countSpecialExpireDate);
                                ViewBag.PursheResult = $"کاربر گرامی ، اکنون آگهی {notice.title} جزو آگهی های ویژه است ";

                                break;
                            case pursheType.emergency:
                                var countEmergencyExpireDate = await _context.Settings.Select(s => s.countExpireDateEmergency).FirstOrDefaultAsync();

                                notice.isEmergency = true;
                                notice.ExpireDateEmergency = notice.expireDate.AddDays((double)countEmergencyExpireDate);
                                ViewBag.PursheResult = $"کاربر گرامی ، اکنون آگهی {notice.title} جزو آگهی های اضطراری است ";
                                break;
                            default:
                                ViewBag.PursheResult = "پرداخت شما با موفقیت انجام شد.";
                                break;
                        }

                        await _context.SaveChangesAsync();

                        return View();
                    }
                }
            }

            return NotFound();
        }
    }
}
