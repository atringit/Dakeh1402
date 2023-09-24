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
using Dake.Models.ViewModels;

namespace Dake.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserNoticeController : ControllerBase
    {
        private readonly Context _context;

        public UserNoticeController(Context context)
        {
            _context = context;
        }

        [HttpGet("{page}/{pagesize}")]
        public async Task<object> UserNotice([FromRoute] int page, [FromRoute] int pagesize)
        {
            List<NoticeViewModelHelper> noticeViewModels = new List<NoticeViewModelHelper>();

            string Token = HttpContext.Request?.Headers["Token"];
            var user = _context.Users.Where(p => p.token == Token).FirstOrDefault();


            if (user == null)
                return new { status = 3, message = "چنین کاربری وجود ندارد." };
            List<Notice> result = _context.Notices.Where(x => x.userId == user.id && x.deletedAt == null).ToList();
            int skip = (page - 1) * pagesize;
            foreach (var item in result)
            {
                if (item.ExpireDateEmergency < DateTime.Now)
                {
                    item.isEmergency = false;
                }
                if (item.expireDateIsespacial < DateTime.Now)
                {
                    item.isSpecial = false;
                }
                if (string.IsNullOrEmpty(item.image) == false && item.image.Contains("/images/Category/"))
                {
                    item.image = getCategoryImage(item.categoryId);
                }
                await _context.SaveChangesAsync();
            }
            var res = result.OrderByDescending(u => u.createDate).Skip(skip).Take(pagesize).ToList();



            foreach (var item in res)
            {
                var PriceItem = GetParent(item.categoryId);
                noticeViewModels.Add(new NoticeViewModelHelper()
                {
                    id = item.id,
                    adminConfirmStatus = item.adminConfirmStatus,
                    dailyVisit = DailyVisit(item.id),
                    description = item.description,
                    espacialPrice = PriceItem.espacialPrice,
                    espacialPriceCode = PriceItem.espacialPriceCode,
                    expireDate = DateToUnix(item.expireDate),
                    expirePrice = PriceItem.expirePrice,
                    expirePriceCode = PriceItem.expirePriceCode,
                    image = item.image,
                    movie = item.movie,
                    isExpire = IsExpire(item.expireDate),
                    ladderPrice = PriceItem.ladderPrice,
                    ladderPriceCode = PriceItem.ladderPriceCode,
                    registerPrice = PriceItem.registerPrice,
                    registerPriceCode = PriceItem.registerPriceCode,
                    title = item.title,
                    isSpecial =  item.isSpecial,
                    expireDateEmergency = item.ExpireDateEmergency==null?0: DateToUnix(item.ExpireDateEmergency.Value),
                    isEmergency = item.isEmergency,
                    LeftDayToExpire = PersianCalendarDate.LeftDayToExpire(item.expireDate),
                    LeftDayToExpireEmergency = PersianCalendarDate.LeftDayToExpire(item.ExpireDateEmergency),
                    LeftDayToExpireSpecial = PersianCalendarDate.LeftDayToExpire(item.expireDateIsespacial)
            });
            }
            return new { data = noticeViewModels, totalCount = result.Count() };
        }
        private string getCategoryImage(int catId)
        {
            var categoryItem = _context.Categorys.FirstOrDefault(s => s.id == catId);
            if (!categoryItem.parentCategoryId.HasValue)
            {
                return categoryItem.image;
            }
            var categoryItem2 = _context.Categorys.FirstOrDefault(s => s.id == categoryItem.parentCategoryId);
            if (!categoryItem2.parentCategoryId.HasValue)
            {
                return categoryItem2.image;
            }
            var categoryItem3 = _context.Categorys.FirstOrDefault(s => s.id == categoryItem2.parentCategoryId);
            if (!categoryItem3.parentCategoryId.HasValue)
            {
                return categoryItem3.image;
            }
            var categoryItem4 = _context.Categorys.FirstOrDefault(s => s.id == categoryItem3.parentCategoryId);
            if (!categoryItem4.parentCategoryId.HasValue)
            {
                return categoryItem4.image;
            }
            var categoryItem5 = _context.Categorys.FirstOrDefault(s => s.id == categoryItem4.parentCategoryId);
            if (!categoryItem5.parentCategoryId.HasValue)
            {
                return categoryItem5.image;
            }
            var categoryItem6 = _context.Categorys.FirstOrDefault(s => s.id == categoryItem5.parentCategoryId);
            if (!categoryItem6.parentCategoryId.HasValue)
            {
                return categoryItem6.image;
            }
            var categoryItem7 = _context.Categorys.FirstOrDefault(s => s.id == categoryItem6.parentCategoryId);
            if (!categoryItem7.parentCategoryId.HasValue)
            {
                return categoryItem7.image;
            }
            var categoryItem8 = _context.Categorys.FirstOrDefault(s => s.id == categoryItem7.parentCategoryId);
            if (!categoryItem8.parentCategoryId.HasValue)
            {
                return categoryItem8.image;
            }
            var categoryItem9 = _context.Categorys.FirstOrDefault(s => s.id == categoryItem8.parentCategoryId);
            if (!categoryItem9.parentCategoryId.HasValue)
            {
                return categoryItem9.image;
            }
            var categoryItem10 = _context.Categorys.FirstOrDefault(s => s.id == categoryItem9.parentCategoryId);
            if (!categoryItem10.parentCategoryId.HasValue)
            {
                return categoryItem10.image;
            }
            return string.Empty;
        }

        private bool IsExpire(DateTime expireDate)
        {
            if (expireDate >= DateTime.Now)
                return false;
            else
                return true;
        }
       private object DailyVisit(long noticeId)
        {
            var visitNotice = _context.VisitNotices.Where(x => x.noticeId == noticeId).OrderByDescending(x => x.id).Select(x => new { x.id, x.countView, date = DateToUnix(x.date)/*, date1 = Convert.ToInt32(DateTime.UtcNow.Subtract(x.date).TotalSeconds)*/ });
            var dailyVisitNotice = visitNotice.Take(7);
            return dailyVisitNotice;
        }
        public long DateToUnix(DateTime date)
        {
            date.AddHours(-3);
            date.AddMinutes(-30);
            TimeSpan timeSpan = date - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            return (long)timeSpan.TotalSeconds;
        }

        private CategoryViewModelHelper GetParent(int CatId)
        {
            CategoryViewModelHelper item = new CategoryViewModelHelper();
            while (CatId > 0)
            {
                var categoryitem = _context.Categorys.Where(s => s.id == CatId).FirstOrDefault();
                if (categoryitem != null)
                {
                    if (categoryitem.parentCategoryId.HasValue)
                    {
                        CatId = categoryitem.parentCategoryId.Value;
                    }
                    else
                    {
                        item.espacialPrice = categoryitem.staticespacialPriceId == "0" ? 0 : _context.StaticPrices.Where(s => s.code == categoryitem.staticespacialPriceId).FirstOrDefault()?.price ?? 0;
                        item.espacialPriceCode = categoryitem.staticespacialPriceId;
                        item.ladderPriceCode = categoryitem.staticladerPriceId;
                        item.registerPriceCode = categoryitem.staticregisterPriceId;
                        item.expirePriceCode = categoryitem.staticexpirePriceId;
                        item.ladderPrice = categoryitem.staticladerPriceId == "0" ? 0 : _context.StaticPrices.Where(s => s.code == categoryitem.staticladerPriceId).FirstOrDefault()?.price ?? 0;
                        item.expirePrice = categoryitem.staticexpirePriceId == "0" ? 0 : _context.StaticPrices.Where(s => s.code == categoryitem.staticexpirePriceId).FirstOrDefault()?.price ?? 0;
                        item.registerPrice = categoryitem.staticregisterPriceId == "0" ? 0 : _context.StaticPrices.Where(s => s.code == categoryitem.staticregisterPriceId).FirstOrDefault()?.price ?? 0;
                        return item;
                    }
                }
            }
            return null;

        }


    }
}