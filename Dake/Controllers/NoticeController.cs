using Dake.DAL;
using Dake.Models;
using Dake.Models.ViewModels;
using Dake.Service;
using Dake.Service.Interface;
using Dake.Utility;
using Dake.ViewModel;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PagedList.Core;
using SmsIrRestfulNetCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Dake.Controllers
{
    public class NoticeController : Controller
    {
        private readonly Context _context;
        //private readonly UserManager<User> _userManager;
        private INotice _Notice;
        private IMessage _message;
        private readonly IHostingEnvironment environment;
        private readonly IPushNotificationService _pushNotificationService;
        public NoticeController(Context context, INotice Notice
            , IHostingEnvironment environment
            , IMessage message
            , IPushNotificationService pushNotificationService)
        {
            this.environment = environment;
            _context = context;
            _Notice = Notice;
            _message = message;
            _pushNotificationService = pushNotificationService;
        }

        // GET: Products
        public IActionResult Index(int? filtercategory, int page = 1, string filterTitle = "")
        {
            var curentUser = HttpContext.User.FindFirst(ClaimTypes.MobilePhone).Value; // کاربر جاری

            var model = _Notice.GetNotices(curentUser, filtercategory, page, filterTitle);
            ViewData["category"] = new SelectList(_context.Categorys, "id", "name");
            return View(model);
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = _context.Notices.Include(x => x.user).Include(x => x.category).Include(s => s.city).FirstOrDefault(m => m.id == id);
            ViewData["allImage"] = _context.NoticeImages.Where(x => x.noticeId == id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        public async Task<IActionResult> Charts(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = _context.Notices.Include(x => x.user).Include(x => x.category).FirstOrDefault(m => m.id == id);

            if (product == null)
            {
                return NotFound();
            }
            var visitNotice = _context.VisitNotices.Where(x => x.noticeId == id).OrderBy(x => x.id).Take(7).ToList();

            return View(visitNotice);
        }
        [HttpPost]
        public async Task<ActionResult> InActive(NoticeViewModel page)
        {
            try
            {
                var curentUser = HttpContext.User.FindFirst(ClaimTypes.MobilePhone).Value; // کاربر جاری
                var userId = _context.Users.Where(w => w.cellphone == curentUser).Select(s => s.id).FirstOrDefault();

                //
                SmsIrRestfulNetCore.Token tokenInstance = new SmsIrRestfulNetCore.Token();

                var token = tokenInstance.GetToken("3ca938cd34f116822bad458f", "Atrin2020");

                var Notice = _context.Notices.Include(x => x.user).FirstOrDefault(x => x.id == page.id);

                Notice.AdminUserAccepted = curentUser;  //ثبت ادمین تایید کننده
                Notice.AcceptedDate = DateTime.Now;

                /////ثبت فعالیت ادمین  
                //_context.AdminActivities.Add(new AdminActivity { 

                //    activityType = page.adminConfirmStatus, 
                //    date = DateTime.Now,
                //    notice = Notice,
                //    userId = userId
                //});

                string adminconfirmsms = "";
                int tempId = 0;
                if (page.adminConfirmsms == 1)
                {
                    adminconfirmsms = "دسته بندی آگهی شما اشتباه است.";
                    tempId = 23322;
                }
                if (page.adminConfirmsms == 2)
                {
                    adminconfirmsms = "فیلم آگهی شما دارای جهت نامناسب یا کیفیت پایین یا محتوای ضد اخلاقی است.";
                    tempId = 23324;

                }
                if (page.adminConfirmsms == 3)
                {
                    adminconfirmsms = "متن نوشته شده را مجدد بررسی و جمله بندی ها و کلمات را اصلاح کنید.";
                    tempId = 23325;

                }
                if (page.adminConfirmsms == 4)
                {
                    adminconfirmsms = "تصاویر آگهی شما مناسب نیست(مراجعه به قوانین و مقررات سایت).";
                    tempId = 23326;

                }
                if (page.adminConfirmsms == 5)
                {
                    adminconfirmsms = "لینک درج شده دارای محتوای خلاف قوانین فضای مجازی کشور است.";
                    tempId = 23327;

                }
                //page.notConfirmDescription = adminconfirmsms + " " + page.notConfirmDescription;
                //page.notConfirmDescription = adminconfirmsms;

                page.notConfirmDescription = adminconfirmsms.Substring(0, Math.Min(adminconfirmsms.Length, 50));

                if (page.adminConfirmStatus == EnumStatus.NotAccept)
                {

                    var ultraFastSend = new UltraFastSend()
                    {
                        Mobile = Convert.ToInt64(Notice.user.cellphone),
                        TemplateId = 60219,
                        ParameterArray = new List<UltraFastParameters>()
                        {
                         new UltraFastParameters()
                          {
                             Parameter = "Title" , ParameterValue = Notice.title,
                          },
                         new UltraFastParameters()
                          {
                             Parameter = "Description" , ParameterValue = page.notConfirmDescription,
                          }
                     }.ToArray()

                    };

                    UltraFastSendRespone ultraFastSendRespone = new UltraFast().Send(token, ultraFastSend);

                    if (!ultraFastSendRespone.IsSuccessful)
                    {
                        Notice.adminConfirmStatus = page.adminConfirmStatus;
                        Notice.notConfirmDescription = page.notConfirmDescription;
                        _context.SaveChanges();
                        return Json("Failed");
                    }

                    var _VmPushNotification = new VmPushNotification
                    {
                        Body = page.notConfirmDescription,
                        Title = Notice.title,
                        Url = "https://dakeh.net",
                        UserId = Notice.userId
                    };
                    await _pushNotificationService.SendNotifToSpecialUser(_VmPushNotification);
                }
                if (page.adminConfirmStatus == EnumStatus.Accept)
                {

                    var ultraFastSend = new UltraFastSend()
                    {
                        Mobile = Convert.ToInt64(Notice.user.cellphone),
                        TemplateId = 60218,
                        ParameterArray = new List<UltraFastParameters>()
                        {
                            new UltraFastParameters()
                            {
                              Parameter = "Title" , ParameterValue = Notice.title
                            }
                        }.ToArray()
                    };

                    UltraFastSendRespone ultraFastSendRespone = new UltraFast().Send(token, ultraFastSend);

                    if (!ultraFastSendRespone.IsSuccessful)
                    {

                        Notice.adminConfirmStatus = page.adminConfirmStatus;
                        Notice.notConfirmDescription = page.notConfirmDescription;
                        _context.SaveChanges();
                        //TempData["SendSmsError"] = "تغییرات اعمال شد ، اما پیامک ارسال نشد ، لطفا پنل پیامک را برسی نمایید";
                        return Json("Failed");

                    }

                    var _VmPushNotification = new VmPushNotification
                    {
                        Body = "آگهی شما تایید شد",
                        Title = Notice.title,
                        Url = "https://dakeh.net",
                        UserId = Notice.userId
                    };
                    await _pushNotificationService.SendNotifToSpecialUser(_VmPushNotification);
                }

                Notice.adminConfirmStatus = page.adminConfirmStatus;
                Notice.notConfirmDescription = page.notConfirmDescription;
                _context.SaveChanges();
                return Json("Done");
            }
            catch (Exception ex)
            {
                return Json("Failed");
            }
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var Notice = await _context.Notices
                .FirstOrDefaultAsync(m => m.id == id);
            if (Notice == null)
            {
                return NotFound();
            }

            return View(Notice);
        }

        // POST: Sliders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var Notice = await _context.Notices.FindAsync(id);
            
            Notice.deletedAt = DateTime.Now;
            _context.SaveChanges();

            /*if (Notice.image != null)
            {

                string deletePath = environment.WebRootPath + Notice.image;

                if (System.IO.File.Exists(deletePath))
                {
                    System.IO.File.Delete(deletePath);
                }
            }
            if (Notice.movie != null)
            {

                string deletePath = environment.WebRootPath + Notice.movie;

                if (System.IO.File.Exists(deletePath))
                {
                    System.IO.File.Delete(deletePath);
                }
            }
            var noticeImage = _context.NoticeImages.Where(x => x.noticeId == id);
            foreach (var item in noticeImage)
            {
                string deletePath = environment.WebRootPath + item.image;
                if (System.IO.File.Exists(deletePath))
                {
                    System.IO.File.Delete(deletePath);
                }
                _context.NoticeImages.Remove(item);
            }
            var factor = _context.Factors.Where(x => x.noticeId == id);
            foreach (var item in factor)
            {
                _context.Factors.Remove(item);
            }
            var userFavorite = _context.UserFavorites.Where(x => x.noticeId == id);
            foreach (var item in userFavorite)
            {
                _context.UserFavorites.Remove(item);
            }
            _context.Notices.Remove(Notice);

            _context.VisitNotices.RemoveRange(_context.VisitNotices.Where(s => s.noticeId == id).ToList());
            _context.VisitNoticeUsers.RemoveRange(_context.VisitNoticeUsers.Where(s => s.noticeId == id).ToList());


            await _context.SaveChangesAsync();*/
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public JsonResult GetNotice(long id)
        {
            var notice = new Notice();

            if (id != 0)
            {
                notice = _context.Notices.Find(id);

                return Json(notice);
            }
            else
            {
                return Json(notice);
            }
        }


        [HttpGet]
        public IActionResult AllChats(int id, int page = 1)
        {

            var result = _context.Messages
                .Where(n => n.MessageType == MessageType.Notice && n.ItemId == id)
                .ToList()
                .Select(n => new CustomMessage
                {
                    id = n.id,
                    senderId = n.ssenderId,
                    itemId = n.ItemId,
                    Title = _context.Notices.Single(u => u.id == n.ItemId).title,
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