using Dake.DAL;
using Dake.Models;
using Dake.Service.Interface;
using Dake.Utility;
using Dake.ViewModel;
using Microsoft.EntityFrameworkCore;
using PagedList.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Dake.Service
{
    public class noticeService : INotice
    {
        private Context _context;
        public noticeService(Context context)
        {
            _context = context;
        }

        //public int NoticeFromAdmin(CreateNoticeViewModel Notice)
        //{
        //    Notice Notice = new Notice();
        //    Notice.description = Notice.description;
        //    Notice.title = Notice.title;

        //    #region Save Image

        //    if (Notice.image != null)
        //    {
        //        string imagePath = "";
        //        Notice.image = Guid.NewGuid().ToString().Replace("-", "") + Path.GetExtension(Notice.image.FileName);
        //        imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/Notice", Notice.image);
        //        using (var stream = new FileStream(imagePath, FileMode.Create))
        //        {
        //            Notice.image.CopyTo(stream);
        //        }
        //        Notice.image = "/images/Notice/" + Notice.image;
        //    }

        //    #endregion

        //    return Notice(Notice);
        //}
        public long Notice(Notice Notice)
        {
            _context.Notices.Add(Notice);
            _context.SaveChanges();
            return Notice.id;
        }

        public object GetLastNotices(int page = 1)
        {
            throw new NotImplementedException();
        }

        //public void EditNotice(CreateNoticeViewModel Notice)
        //{
        //    Notice _Notice = _context.Notices.Find(Notice.id);
        //    _Notice.title = Notice.title;
        //    _Notice.description = Notice.description;
        //    if (Notice.image != null)
        //    {
        //        if (_Notice.image != null)
        //        {
        //            string deletePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/Notice/", _Notice.image);
        //            if (File.Exists(deletePath))
        //            {
        //                File.Delete(deletePath);
        //            }
        //        }


        //        string imagePath = "";
        //        _Notice.image = Guid.NewGuid().ToString().Replace("-", "") + Path.GetExtension(Notice.image.FileName);
        //        imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/Notice", _Notice.image);
        //        using (var stream = new FileStream(imagePath, FileMode.Create))
        //        {
        //            Notice.image.CopyTo(stream);
        //        }
        //        _Notice.image = "/images/Notice/" + _Notice.image;

        //    }

        //    _context.Notices.Update(_Notice);
        //    _context.SaveChanges();
        //}
        public object GetNotices(int page = 1)
        {
            IQueryable<Notice> result = _context.Notices;
            int skip = (page - 1) * 10;
            List<Notice> res = result.ToList();
            return new { data = res.OrderByDescending(u => u.id).Skip(skip).Take(10), totalCount = res.Count() };
        }
        public object GetNotices(string Token, int page = 1, int pagesize = 10)
        {
            var user = _context.Users.Where(p => p.token == Token).FirstOrDefault();
            if (user == null)
                return new { status = 3, message = "چنین کاربری وجود ندارد." };
            List<Notice> result = new List<Notice>();
            List<Notice> result2 = new List<Notice>();
            List<Notice> resultEspacial = new List<Notice>();

            if (user.provinceId != null && user.provinceId != 33)
            {
                result = _context.Notices.Include(s => s.category).Where(x => x.expireDate >= DateTime.Now && x.adminConfirmStatus == EnumStatus.Accept && x.provinceId == user.provinceId && x.deletedAt == null).OrderByDescending(u => u.createDate).ToList();

                resultEspacial = _context.Notices.Where(x => x.expireDate >= DateTime.Now && x.adminConfirmStatus == EnumStatus.Accept && x.isSpecial && x.expireDateIsespacial >= DateTime.Now && x.provinceId == user.provinceId && x.deletedAt == null).OrderByDescending(x => x.expireDateIsespacial).ToList();
            }
            else
            {
                result = _context.Notices.Include(s => s.category).Where(x => x.expireDate >= DateTime.Now && x.adminConfirmStatus == EnumStatus.Accept && x.deletedAt == null).OrderByDescending(u => u.createDate).ToList();

                resultEspacial = _context.Notices.Where(x => x.expireDate >= DateTime.Now && x.adminConfirmStatus == EnumStatus.Accept && x.isSpecial && x.expireDateIsespacial >= DateTime.Now && x.deletedAt == null).OrderByDescending(x => x.expireDateIsespacial).ToList();
            }

            foreach (var item in result)
            {
                if (string.IsNullOrEmpty(item.image) == false && item.image.Contains("/images/Category/"))
                {
                    GetNoticeCategoryImage getNoticeCategoryImage = new GetNoticeCategoryImage(_context);
                    item.image = getNoticeCategoryImage.getCategoryImage(item.categoryId);
                }
            }
            foreach (var item in resultEspacial)
            {
                if (string.IsNullOrEmpty(item.image) == false && item.image.Contains("/images/Category/"))
                {
                    GetNoticeCategoryImage getNoticeCategoryImage = new GetNoticeCategoryImage(_context);
                    item.image = getNoticeCategoryImage.getCategoryImage(item.categoryId);
                }
            }
            // int skip = (page - 1) * pagesize;
            var res = result.Skip((page - 1) * pagesize).Take(pagesize).Select(x => new
            {
                x.id,
                x.title,
                x.description,
                x.image,
                x.category.name,
                x.categoryId,
                x.isEmergency,
                x.price,
                x.lastPrice
            }).ToList();

            var resEspacial = resultEspacial.Skip((page - 1) * pagesize).Take(pagesize).Select(x => new
            {
                x.id,
                x.title,
                x.description,
                x.image,
                x.category.name,
                x.movie,
                x.isSpecial,
                x.price,
                x.lastPrice
            }).ToList();
            return new { data = res, resEspacial, totalCount = result.Count() };
        }

        public object GetAllEspacialNotices(string Token, long noticeId, string scroll)
        {
            var user = _context.Users.Where(p => p.token == Token).FirstOrDefault();
            if (user == null)
                return new { status = 3, message = "چنین کاربری وجود ندارد." };

            IQueryable<Notice> resultEspacial = null;
            var notice = _context.Notices.Find(noticeId);
            if (scroll == "Up")
            {
                if (user.provinceId != null)

                    resultEspacial = _context.Notices.Where(x => x.expireDate >= DateTime.Now && x.adminConfirmStatus == EnumStatus.Accept && x.isSpecial && x.expireDateIsespacial >= DateTime.Now && x.provinceId == user.provinceId && x.expireDateIsespacial <= notice.expireDateIsespacial).OrderByDescending(x => x.expireDateIsespacial);
                else
                    resultEspacial = _context.Notices.Where(x => x.expireDate >= DateTime.Now && x.adminConfirmStatus == EnumStatus.Accept && x.isSpecial && x.expireDateIsespacial >= DateTime.Now && x.expireDateIsespacial <= notice.expireDateIsespacial).OrderByDescending(x => x.expireDateIsespacial);

            }
            if (scroll == "Down")
            {
                if (user.provinceId != null)

                    resultEspacial = _context.Notices.Where(x => x.expireDate >= DateTime.Now && x.adminConfirmStatus == EnumStatus.Accept && x.isSpecial && x.expireDateIsespacial >= DateTime.Now && x.provinceId == user.provinceId && x.expireDateIsespacial > notice.expireDateIsespacial).OrderByDescending(x => x.expireDateIsespacial);
                else
                    resultEspacial = _context.Notices.Where(x => x.expireDate >= DateTime.Now && x.adminConfirmStatus == EnumStatus.Accept && x.isSpecial && x.expireDateIsespacial >= DateTime.Now && x.expireDateIsespacial > notice.expireDateIsespacial).OrderByDescending(x => x.expireDateIsespacial);

            }

            notice.countView = notice.countView + 1;
            var visitNotice = _context.VisitNotices.FirstOrDefault(x => x.noticeId == noticeId && DateTime.Compare(x.date.Date, DateTime.Now.Date) == 0);
            if (visitNotice != null)
                visitNotice.countView++;
            else
            {
                _context.VisitNotices.Add(new VisitNotice { countView = 1, date = DateTime.Now, noticeId = noticeId });
            }
            _context.SaveChanges();
            var res = resultEspacial.Take(1).Select(p => new { p.id, p.countView, areaName = p.area.name, provinceName = p.province.name, cityName = p.city.name, p.code, createDate = DateToUnix(p.createDate), p.description, categoryName = p.category.name, user = p.user.cellphone, p.title, p.price, p.movie, p.image, p.isSpecial, p.lastPrice, dailyVisit = DailyVisit(p.id) });

            return res;

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

        //public object GetAllEspacialNotices(string Token, int page, int noticeId, string scroll)
        //{
        //    var user = _context.Users.Where(p => p.token == Token).FirstOrDefault();
        //    if (user == null)
        //        return new { status = 3, message = "چنین کاربری وجود ندارد." };

        //    IQueryable<Notice> resultEspacial = null;
        //    var notice = _context.Notices.Find(noticeId);
        //    if (scroll == "Up")
        //    {
        //        if (user.provinceId != null)

        //            resultEspacial = _context.Notices.Where(x => x.expireDate >= DateTime.Now && x.adminConfirmStatus == EnumStatus.Accept && x.isSpecial && x.expireDateIsespacial >= DateTime.Now && x.provinceId == user.provinceId && x.expireDateIsespacial <= notice.expireDateIsespacial).OrderByDescending(x => x.expireDateIsespacial);
        //        else
        //            resultEspacial = _context.Notices.Where(x => x.expireDate >= DateTime.Now && x.adminConfirmStatus == EnumStatus.Accept && x.isSpecial && x.expireDateIsespacial >= DateTime.Now  && x.expireDateIsespacial <= notice.expireDateIsespacial).OrderByDescending(x => x.expireDateIsespacial);

        //    }
        //    if (scroll == "Down")
        //    {
        //        if (user.provinceId != null)

        //            resultEspacial = _context.Notices.Where(x => x.expireDate >= DateTime.Now && x.adminConfirmStatus == EnumStatus.Accept && x.isSpecial && x.expireDateIsespacial >= DateTime.Now && x.provinceId == user.provinceId && x.expireDateIsespacial > notice.expireDateIsespacial).OrderByDescending(x => x.expireDateIsespacial);
        //        else
        //            resultEspacial = _context.Notices.Where(x => x.expireDate >= DateTime.Now && x.adminConfirmStatus == EnumStatus.Accept && x.isSpecial && x.expireDateIsespacial >= DateTime.Now  && x.expireDateIsespacial > notice.expireDateIsespacial).OrderByDescending(x => x.expireDateIsespacial);

        //    }
        //    int skip = (page - 1) * 10;
        //    var res = resultEspacial.Skip(skip).Take(10).Select(x => new { x.id, x.title, x.description, x.image, x.category.name }).ToList();

        //    return new { data = res,  totalCount = resultEspacial.Count() };

        //}



        public PagedList<Notice> GetNotices(string currentUser, int? filtercategory, int page = 1, string filterTitle = "")
        {
            //IQueryable<Notice> result = _context.Notices.IgnoreQueryFilters().OrderByDescending(x => x.id);

            //var userCitys = _context.AdminsInCities.Where(w=> w.user.cellphone == currentUser).Select(s=> s.cityId);
            ///var userProvices = _context.AdminsInProvices.Where(w=> w.user.cellphone == currentUser).Select(s=> s.provinceId);

            //result = result.Where(w => userCitys.Contains(w.cityId));

            IQueryable<Notice> result = _context.Notices.Where(p=>p.deletedAt == null).IgnoreQueryFilters().OrderByDescending(x => x.id);
            if (!string.IsNullOrEmpty(filterTitle))
            {
                result = result.Where(u => u.title.Contains(filterTitle));
            }
            if (filtercategory != null)
            {
                result = result.Where(u => u.categoryId == filtercategory);
            }

            PagedList<Notice> res = new PagedList<Notice>(result, page, 20);
            return res;
        }

        //public object GetLastNotices(int page = 1)
        //{
        //    IQueryable<Notice> result = _context.Notices;
        //    int skip = (page - 1) * 10;
        //    var res = result.OrderByDescending(u => u.id).Skip(skip).Take(10).Select(x => new { x.id, x.image1,x.image2,x.image3, x.title, x.description }).ToList();
        //    return new { data = res, totalCount = result.Count() };
        //}

        //public object GetNoticesByCatAndType(NoticeSearch NoticeSearch)
        //{
        //    var result = _context.Notices.AsQueryable();
        //    int skip = (NoticeSearch.page - 1) * 10;

        //    List<int> cats = new List<int>();
        //    cats.Add(Convert.ToInt32(NoticeSearch.category_id));
        //    int catId = 0;


        //    return new { data = result.OrderByDescending(p => p.id).Skip(10 * (NoticeSearch.page - 1)).Take(10).Select(x => new { x.id, x.image, x.title, x.description }).ToList(), totalCount = result.Count() };
        //}

        //public object GetNoticesByTitle(NoticeSearch2 NoticeSearch)
        //{
        //    var result = _context.Notices.AsQueryable();
        //    int skip = (NoticeSearch.page - 1) * 10;
        //    if (NoticeSearch.title != "")
        //        result = result.Where(x => x.title.Contains(NoticeSearch.title));
        //    return new { data = result.OrderByDescending(p => p.id).Skip(10 * (NoticeSearch.page - 1)).Take(10).Select(x => new { x.id, x.image,  x.title, x.description }).ToList(), totalCount = result.Count() };
        //}

        //public object NoticeToFactor(BuyNotice buyNotice)
        //{
        //    try
        //    {
        //        string Token = buyNotice.token;
        //        var user = _context.Users.Where(p => p.token == Token).FirstOrDefault();
        //        List<Factor> factors = new List<Factor>();
        //        List<FactorItem> FactorItems = new List<FactorItem>();

        //        //foreach (var item in buyNotice.Notices)
        //        //{
        //        //    var Notice = _context.Notices.FirstOrDefault(x => x.id == item);
        //        //    if (Notice == null)
        //        //    {
        //        //        return new { status = 1, title = "خطای ثبت فاکتور", message = "چنین محصولی یافت نشد" };

        //        //    }
        //        //    else
        //        //    {
        //        //        Factor factor = new Factor();
        //        //        factor.NoticeId = item;
        //        //        factor.isAdminCheck = false;
        //        //        factor.state = State.IsPay;
        //        //        factor.userId = user.id;
        //        //        factor.createDatePersian = PersianCalendarDate.PersianCalendarResult(DateTime.Now);;
        //        //        factors.Add(factor);
        //        //    }
        //        //}
        //        //_context.Factors.AddRange(factors);
        //        long totalPrice = 0;


        //        Factor factor = new Factor();
        //        //factor.NoticeId = buyNotice.Notices.ToString();

        //        factor.state = State.IsPay;
        //        factor.userId = user.id;
        //        factor.createDatePersian = PersianCalendarDate.PersianCalendarResult(DateTime.Now); ;

        //        foreach (var item in buyNotice.Notices)
        //        {
        //            var Notice = _context.Notices.FirstOrDefault(x => x.id == item);
        //            if (Notice == null)
        //            {
        //                return new { status = 1, title = "خطای ثبت فاکتور", message = "چنین محصولی یافت نشد" };
        //            }
        //            //totalPrice += Notice.price;
        //        }
        //        factor.totalPrice = totalPrice;
        //        _context.Factors.Add(factor);
        //        foreach (var item in buyNotice.Notices)
        //        {
        //            var Notice = _context.Notices.FirstOrDefault(x => x.id == item);
        //            if (Notice == null)
        //            {
        //                return new { status = 1, title = "خطای ثبت فاکتور", message = "چنین محصولی یافت نشد" };
        //            }
        //            FactorItem factorItem = new FactorItem();
        //            factorItem.NoticeId = Notice.id;
        //            factorItem.FactorId = factor.id;
        //            FactorItems.Add(factorItem);
        //        }
        //        _context.FactorItems.AddRange(FactorItems);
        //        _context.SaveChanges();
        //    }
        //    catch (Exception ex)
        //    {
        //        return new { status = 2, title = "خطای ثبت فاکتور", message = "خطایی رخ داده است." };
        //    }
        //    return new { status = 0, title = " ثبت فاکتور", message = "ثبت فاکتور با موفقیت انجام شد." };

        //}

        //public object GetFactorOfUser(GetFactor getFactor)
        //{
        //    var user = _context.Users.Where(p => p.token == getFactor.token).FirstOrDefault();
        //    //List<int?> Noticeids = _context.Factors.Where(x => x.userId == user.id).Select(x => x.NoticeId).ToList();
        //    //var result = _context.Notices.Where(x => Noticeids.Contains(x.id)).AsQueryable();
        //    var result = _context.Factors.Where(x => x.userId == user.id).ToList().Select(x => new { x.id, x.createDatePersian, x.totalPrice }).AsQueryable();
        //    int skip = (getFactor.page - 1) * 10;
        //    var res = result.OrderByDescending(u => u.id).Skip(skip).Take(10).ToList();
        //    return new { data = res, totalCount = result.Count() };
        //}
        //public object GetFactorItems(AllFactorItem allFactorItem)
        //{
        //    List<int> NoticeIds = _context.FactorItems.Include(x => x.Factor).Where(x => x.FactorId == allFactorItem.factorId).Select(x=>x.NoticeId).ToList();
        //    var result = _context.Notices.Where(x => NoticeIds.Contains(x.id)).Select(x => new { x.id, x.image, x.title, x.description }).AsQueryable();
        //    int skip = (allFactorItem.page - 1) * 10;
        //    var res = result.OrderByDescending(u => u.id).Skip(skip).Take(10).ToList();
        //    return new { data = res, totalCount = result.Count() };
        //}

        //public object GetLinkOfNotices(GetLinkOfNotice getLinkOfNotice)
        //{
        //    var user = _context.Users.Where(p => p.token == getLinkOfNotice.token).FirstOrDefault();
        //    var factor = _context.Factors.FirstOrDefault(x => x.userId == user.id);
        //    if (factor == null)
        //    {
        //        return new { status = 1, title = "خطای دریافت لینک", message = "این لینک توسط شما خریداری نشده است." };
        //    }
        //    else
        //    {
        //        var Notice = _context.Notices.FirstOrDefault(x => x.id == getLinkOfNotice.NoticeId);
        //        return new { status = 0, title = "لینک محصول", message = "لینک محصول شما.", data = Notice };
        //    }

        //}

        /// <summary>
        /// دریافت تعداد آگهی های تایید شده توسط هر ادمین در بازه زمانی مشخص
        /// </summary>
        /// <param name="fromd">از تاریخ</param>
        /// <param name="tod">تا تاریخ</param>
        /// <param name="username">نام کاربری ادمین</param>
        /// <returns></returns>
        public int GetAdminAcceptedNoticeCount(string fromd, string tod, string username)
        {
            try
            {

                var fromD = PersianCalendarDate.ConvertShamsiToGareg(fromd);
                var toD = PersianCalendarDate.ConvertShamsiToGareg(tod);

                return  _context.Notices.Where(w=> w.AcceptedDate >= fromD && w.AcceptedDate <= toD
                && w.AdminUserAccepted == username).Count(); 
            }
            catch(Exception er)
            {
                return 0;
            }
        }
    }
  
}
