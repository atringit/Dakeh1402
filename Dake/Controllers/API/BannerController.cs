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
using Dake.Service;

namespace Dake.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class BannerController : ControllerBase
    {
        private readonly Context _context;
        private readonly IHostingEnvironment environment;
        private readonly IBannerSevice _repository;
        private IDiscountCode _IDiscountCode;



        public BannerController(Context context,  IHostingEnvironment environment , IDiscountCode discountCode, IBannerSevice repository)
        {
            this.environment = environment;
            _context = context;
            _IDiscountCode = discountCode;
            _repository = repository;
        }
   
            


        [HttpPost("{id}")]
        public object ExtendedNotice([FromRoute]long id)
        {
            var Notice = _context.Notices.Find(id);
            long totalPrice = 0; 
            if (Notice == null)
                return new { status = 1, message = "چنین درخواستی یافت نشد." };
            var setting = _context.Settings.FirstOrDefault();
            var category = _context.Categorys.Find(Notice.categoryId);
            Notice.expireDate = Notice.expireDate.AddDays(Convert.ToInt64(setting.countExpireDate));
            var user = _context.Users.Find(Notice.userId);
            var staticPriceItem = _context.StaticPrices.FirstOrDefault(s => s.code == category.staticexpirePriceId);
            if (staticPriceItem != null)
            {
                totalPrice = staticPriceItem.price;
            }
            Factor factor = new Factor();
            factor.state = State.IsPay;
            factor.userId = user.id;
            factor.createDatePersian = PersianCalendarDate.PersianCalendarResult(DateTime.Now); ;
            factor.noticeId = id;
            factor.factorKind = FactorKind.Extend;
            factor.totalPrice = totalPrice;
            _context.Factors.Add(factor);
            _context.SaveChanges();
            return new { status = 1, title = "تمدید آگهی", message = "تمدید آگهی با موفقیت انجام شد." };

        }
        //[HttpPost]
        //public object GetNotices(NoticeSearch NoticeSearch)
        //{
        //    var result = _notice.GetNoticesByCatAndType(NoticeSearch);
        //    return result;
        //}
        //[HttpPost("{page}")]
        //public object GetNotices([FromRoute]int page)
        //{
        //    var data = _notice.GetLastNotices(page);
        //    return data;
        //}

        // GET: api/Notices/5
        [HttpGet("{id}")]
        public object GetNotice([FromRoute] long id)
        {
            var settings = _context.Settings.FirstOrDefault();

            NoticeViewModelHelper noticeViewModels = new NoticeViewModelHelper();

            string Token = HttpContext.Request?.Headers["Token"];
            var user = _context.Users.Where(p => p.token == Token).FirstOrDefault();
            if (user == null)
                return new { status = 3, message = "چنین کاربری وجود ندارد." };
            var Notice = _context.Notices.Find(id);
            if (Notice.userId != user.id)
            {
                if (!_context.VisitNoticeUsers.Any(x => x.noticeId == id && x.userId == user.id))
                {
                        _context.VisitNoticeUsers.Add(new VisitNoticeUser { noticeId = id,userId=user.id });
                    Notice.countView = Notice.countView + 1;
                    var VisitNotice = _context.VisitNotices.LastOrDefault(x => x.noticeId == id);

                    if (VisitNotice == null)
                    {
                        //Notice.countView = Notice.countView + 1;
                        _context.VisitNotices.Add(new VisitNotice { countView = 1, date = DateTime.Now, noticeId = id });
                    }
                    else
                    {
                        if (DateTime.Compare(VisitNotice.date.Date, DateTime.Now.Date) == 0)
                            VisitNotice.countView++;
                        else
                            _context.VisitNotices.Add(new VisitNotice { countView = 1, date = DateTime.Now, noticeId = id });
                    }
                }

            }
            _context.SaveChanges();
            if (Notice == null)
            {
                return NotFound();
            }
            var images = _context.NoticeImages.Where(p => p.noticeId == id).Select(p => new { p.image }).ToList();
            bool IsFavorit = false;
            var userFavorite = _context.UserFavorites.FirstOrDefault(x => x.userId == user.id && x.noticeId == id);

            if (userFavorite != null)
                IsFavorit = true;


         

            var noticeitem = _context.Notices.Select(p => new { p.id, p.link, p.countView, IsFavorit, areaName = p.area.name, provinceName = p.province.name, 
                cityName = p.city.name, p.code, createDate = DateToUnix(p.createDate), p.description, categoryName = p.category.name, user = p.user.cellphone,
                userId = p.user.id ,p.title, p.price, p.movie, p.image, p.isSpecial, p.lastPrice, dailyVisit = DailyVisit(p.id), p.categoryId, p.areaId, p.cityId, p.provinceId, p.isEmergency }).FirstOrDefault(x => x.id == id);



            if (noticeitem != null)
            {
                var PriceItem = GetParent(noticeitem.categoryId);
                noticeViewModels.id = noticeitem.id;
                noticeViewModels.link = noticeitem.link;
                noticeViewModels.IsFavorit = noticeitem.IsFavorit;
                noticeViewModels.countView = noticeitem.countView;
                noticeViewModels.areaName = noticeitem.areaName;
                noticeViewModels.provinceName = noticeitem.provinceName;
                noticeViewModels.cityName = noticeitem.cityName;
                noticeViewModels.code = noticeitem.code;
                noticeViewModels.createDate = noticeitem.createDate;
                noticeViewModels.description = noticeitem.description;
                noticeViewModels.categoryName = noticeitem.categoryName;
                noticeViewModels.user = noticeitem.user;
                noticeViewModels.title = noticeitem.title;
                noticeViewModels.price = noticeitem.price;
                noticeViewModels.movie = noticeitem.movie;

                noticeViewModels.isEmergency = noticeitem.isEmergency;


                if (string.IsNullOrEmpty(noticeitem.image) == false && noticeitem.image.Contains("/images/Category/"))
                {
                    noticeViewModels.image = getCategoryImage(noticeitem.categoryId);
                }
				else
				{
                noticeViewModels.image = noticeitem.image;
				}
                noticeViewModels.isSpecial = noticeitem.isSpecial;
                noticeViewModels.lastPrice = noticeitem.lastPrice;
                noticeViewModels.dailyVisit = noticeitem.dailyVisit;
                noticeViewModels.categoryId = noticeitem.categoryId;
                noticeViewModels.areaId = noticeitem.areaId;
                noticeViewModels.cityId = noticeitem.cityId;
                noticeViewModels.provinceId = noticeitem.provinceId;
                noticeViewModels.espacialPrice = PriceItem.espacialPrice;
                noticeViewModels.espacialPriceCode = PriceItem.espacialPriceCode;
                noticeViewModels.expirePrice = PriceItem.expirePrice;
                noticeViewModels.expirePriceCode = PriceItem.expirePriceCode;
                noticeViewModels.ladderPrice = PriceItem.ladderPrice;
                noticeViewModels.ladderPriceCode = PriceItem.ladderPriceCode;
                noticeViewModels.registerPrice = PriceItem.registerPrice;
                noticeViewModels.registerPriceCode = PriceItem.registerPriceCode;
                noticeViewModels.userId = noticeitem.userId;

            }
            if (settings != null && settings.showPriceForCars == false)
            {
                if (IsDrivingPrice(noticeitem.categoryId))
                {
                    noticeViewModels.price = 0;
                    noticeViewModels.lastPrice = 0;
                }
            }
            return new { Notice = noticeViewModels, images };
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

        [HttpPost("AddBanner")]
        public async Task<IActionResult> AddBanner([FromForm]Banner banner,[FromForm] IList<IFormFile> files)
        {

            try
            {
                string Token = HttpContext.Request?.Headers["Token"];
                var user = _context.Users.FirstOrDefault(x => x.token == Token);
                 banner.user = user;
                
                var bres = _repository.AddBanner(banner, files).Result;

                if (bres.IsSuccess)
                {


                    var bannerId = long.Parse(bres.Data.ToString());

                    Factor factor = new Factor();
                    factor.state = State.IsPay;
                    factor.userId = banner.user.id;
                    factor.createDatePersian = PersianCalendarDate.PersianCalendarResult(DateTime.Now);
                    factor.factorKind = FactorKind.Add;
                    factor.bannerId = bannerId;

                    //factor.totalPrice = havediscount ? category.registerPrice - discountprice : category.registerPrice;
                    _context.Factors.Add(factor);
                    //Payment
                    await _context.SaveChangesAsync();

                    if (factor.totalPrice >= 10000)
                    {
                        try
                        {
                            PaymentRequestAttemp request = new PaymentRequestAttemp();
                            request.FactorId = factor.id;
                            request.NoticeId = bannerId;
                            request.UserId = user.id;
                            request.pursheType = pursheType.RegisterNotice;

                            _context.Add(request);
                            _context.SaveChanges();

                            var res = PaymentHelper.SendRequest(request.Id, 0, "http://dakeh.net/Purshe/VerifyRequest");
                            if (res != null && res.Result != null)
                            {
                                if (res.Result.ResCode == "0")
                                {
                                    bool havediscount = false;
                                    if (havediscount)
                                    {
                                        int _code = 0;
                                        _IDiscountCode.AddUserToDiscountCode(user.id, _code);
                                    }
                                   
                                }
                                return Ok(res.Result.Description);
                            }
                        }
                        catch (Exception)
                        {
                            return NotFound("امکان اتصال به درگاه بانکی وجود ندارد");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return Ok(new { status = 1, title = "ثبت آگهی", BannerId = banner.Id, message = "آگهی شما با موفقیت ثبت گردید." });

        }

        //    [HttpPost]
        //    public object PostBanner()
        //    {
        //        long totalPrice = 0;
        //        if (!ModelState.IsValid)
        //        {
        //            return BadRequest(ModelState);
        //        }
        //        Banner notice = new Banner();
        //        int _code = 0; 
        //        try
        //        {
        //            string Token = HttpContext.Request?.Headers["Token"];
        //            string DiscountCode = HttpContext.Request?.Headers["DiscountCode"];
        //            var user = _context.Users.Where(p => p.token == Token).FirstOrDefault();
        //            if (user == null)
        //                return new { status = 3, message = "چنین کاربری وجود ندارد." };
        //            //string title = HttpContext.Request?.Form["title"];
        //            //string description = HttpContext.Request?.Form["description"];
        //            //bool isBarber =Convert.ToBoolean(HttpContext.Request?.Form["isBarber"]);
        //            //int cityId =Convert.ToInt32(HttpContext.Request?.Form["cityId"]);
        //            //int provinceId =Convert.ToInt32(HttpContext.Request?.Form["provinceId"]);
        //            //int areaId =Convert.ToInt32(HttpContext.Request?.Form["areaId"]);
        //            if(!string.IsNullOrEmpty(DiscountCode))
        //            {
        //                int n;
        //                if (int.TryParse(DiscountCode, out n))
        //                {           
        //                _code = Convert.ToInt32(DiscountCode);
        //                if (_DiscountCode.IsAlreadyUsed(user.id, _DiscountCode.GetIdByCode(_code))==false && _DiscountCode.CheckCode(_code))
        //                {
        //                        _DiscountCode.AddUserToDiscountCode(user.id, _code);
        //                }
        //                }
        //            }
        //            string data = HttpContext.Request?.Form["data"];

        //            JObject json = JObject.Parse(data);
        //            JObject jalbum = json as JObject;
        //            var movie = "";
        //            Banner noticedata = jalbum.ToObject<Banner>();
        //            string imageUrl = "";
        //            var setting = _context.Settings.FirstOrDefault();

        //            notice.title = noticedata.title;              
        //            notice.createDate = DateTime.Now;
        //            notice.expireDateIsespacial = DateTime.Now;
        //            notice.expireDate = DateTime.Now.AddDays(Convert.ToInt64(setting.countExpireDate));
        //            notice.adminConfirmStatus = EnumStatus.Pending;
        //            notice.isPaid = true; 
        //            notice.Link = noticedata.Link;
        //            notice.userId = user.id;

        //            if (_context.Banner.Count() == 0)
        //                notice.code = "1";
        //            else
        //                notice.code = (Convert.ToInt32(_context.Banner.LastOrDefault().code) + 1).ToString();
        //            var httpRequest = HttpContext.Request;
        //            var hfc = HttpContext.Request.Form.Files;
        //            List<string> images = new List<string>();
        //            for (int i = 0; i < hfc.Count; i++)
        //            {
        //                if (hfc[i].Length > 1024 * 1024 * 10)
        //                {
        //                    return new { status = 1, message = "فایل ارسالی بزرگ تر از حد مجاز می باشد." };
        //                }
        //                else
        //                {
        //                    var namefile = Guid.NewGuid().ToString().Replace('-', '0').Substring(0, 7) + Path.GetExtension(hfc[i].FileName).ToLower();
        //                    var filePath = Path.Combine(environment.WebRootPath, "Banner/", namefile);

        //                    if (hfc[i].Name == "imageUrl")
        //                    {
        //                        imageUrl = "/Banner/" + namefile;
        //                    }


        //                    else if (hfc[i].Name == "movie")
        //                    {
        //                        movie = "/Banner/" + namefile;
        //                    }

        //                    else
        //                    {

        //                        images.Add("/Banner/" + namefile);

        //                    }
        //                    using (var stream = new FileStream(filePath, FileMode.Create))
        //                    {
        //                        hfc[i].CopyTo(stream);
        //                    }
        //                }
        //            }

        //            if(String.IsNullOrEmpty(imageUrl))
        //            {



        //            }

        //            _context.Banner.Add(notice);
        //            foreach (var item in images)
        //            {
        //                _context.BannerImage.Add(new BannerImage
        //                {
        //                    BannerId = notice.Id,
        //                    FileLocation = item
        //                });
        //            }
        ////            var staticPriceItem = _context.StaticPrices.FirstOrDefault(s=>s.code == category.staticregisterPriceId);
        ////            if(staticPriceItem !=null)
        ////{
        ////                totalPrice = staticPriceItem.price;
        ////}

        //            Factor factor = new Factor();
        //            factor.state = State.IsPay;
        //            factor.userId = user.id;
        //            factor.createDatePersian = PersianCalendarDate.PersianCalendarResult(DateTime.Now);
        //            factor.bannerId = notice.Id;
        //            factor.factorKind = FactorKind.Add;
        //            factor.totalPrice = totalPrice;
        //            _context.Factors.Add(factor);
        //            _context.SaveChanges();
        //        }
        //        catch (Exception ex)
        //        {
        //            return new { status = 2, title = "خطا در ثبت بنر", message = "بنر شما ثبت نشد." };

        //        }
        //        return new { noticeId = notice.Id, status = 1, title = "ثبت بنر", message = "بنر شما با موفقیت ثبت گردید." };
        //    }


        [Route("RemoveNotice")]
        public object RemoveNotice(long id)
        {
            Notice noticeitem = _context.Notices.Where(s => s.id == id).FirstOrDefault();
            if (noticeitem == null)
            {
                return new { status = 3, message = "چنین آگهی وجود ندارد." };
            }
            try
            {
                if (noticeitem.image != null)
                {
                    string deletePath = environment.WebRootPath + noticeitem.image;

                    if (System.IO.File.Exists(deletePath))
                    {
                        System.IO.File.Delete(deletePath);
                    }
                }
                if (noticeitem.movie != null)
                {
                    string deletePath = environment.WebRootPath + noticeitem.movie;
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
                var listFactors = _context.Factors.Where(s => s.noticeId == id).ToList();
                var listFavorits = _context.UserFavorites.Where(s => s.noticeId == id).ToList();
                foreach (var itemFav in listFavorits)
                {
                    _context.UserFavorites.Remove(itemFav);
                }
                foreach (var item in listFactors)
                {
                    _context.Factors.Remove(item);
                }
                _context.Notices.Remove(noticeitem);
                _context.SaveChanges();
                return new { status = 1, title = "حذف موفقیت آمیز", message = "آگهی حذف گردید" };

            }
            catch (Exception ex)
            {
                return new { status = 3, message = "امکان حذف آگهی وجود ندارد" };

            }
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
                        item.espacialPrice = categoryitem.staticespacialPriceId == "0" ? 0 : _context.StaticPrices.Where(s => s.code == categoryitem.staticespacialPriceId).FirstOrDefault().price;
                        item.espacialPriceCode = categoryitem.staticespacialPriceId;
                        item.ladderPriceCode = categoryitem.staticladerPriceId;
                        item.registerPriceCode = categoryitem.staticregisterPriceId;
                        item.expirePriceCode = categoryitem.staticexpirePriceId;
                        item.ladderPrice = categoryitem.staticladerPriceId == "0" ? 0 : _context.StaticPrices.Where(s => s.code == categoryitem.staticladerPriceId).FirstOrDefault().price;
                        item.expirePrice = categoryitem.staticexpirePriceId == "0" ? 0 : _context.StaticPrices.Where(s => s.code == categoryitem.staticexpirePriceId).FirstOrDefault().price;
                        item.registerPrice = categoryitem.staticregisterPriceId == "0" ? 0 : _context.StaticPrices.Where(s => s.code == categoryitem.staticregisterPriceId).FirstOrDefault().price;
                        return item;
                    }
                }
            }
            return null;

        }
        private bool IsDrivingPrice(int CatId)
        {
            while (CatId > 0)
            {
                var categoryitem = _context.Categorys.Where(s => s.id == CatId).FirstOrDefault();
                if (categoryitem != null)
                {
                    if (categoryitem.parentCategoryId.HasValue)
                    {
                        CatId = categoryitem.parentCategoryId.Value;
                    }
                    if (CatId == 29)
                    {
                        return true;
                    }
                    if (!categoryitem.parentCategoryId.HasValue)
                    {
                        return false;
                    }
                }
            }
            return false;
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


    }
}