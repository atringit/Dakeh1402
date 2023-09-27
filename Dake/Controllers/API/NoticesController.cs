using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Dake.DAL;
using Dake.Models;
using Dake.Models.ViewModels;
using Dake.Service.Interface;
using Dake.Utility;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Dake.ViewModel;
using Dake.Service.Common;
using System.Drawing;
using DocumentFormat.OpenXml.Presentation;

namespace Dake.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class NoticesController : ControllerBase
    {
        private readonly Context _context;
        private INotice _notice;
        private readonly IHostingEnvironment environment;
        private IDiscountCode _DiscountCode;


        public NoticesController(Context context, INotice notice, IHostingEnvironment environment, IDiscountCode discountCode)
        {
            this.environment = environment;
            _context = context;
            _notice = notice;
            _DiscountCode = discountCode;
        }
        [HttpGet("{page}/{pagesize}")]
        public async Task<object> GetNotices([FromRoute] int page, [FromRoute] int pagesize)
        {
            string Token = HttpContext.Request?.Headers["Token"];
            var notices = _context.Notices.Where(p => p.isEmergency && p.ExpireDateEmergency < DateTime.Now && p.deletedAt == null).ToList();
            foreach (var notice in notices)
            {
                notice.isEmergency = false;
                await _context.SaveChangesAsync();
            }
            var data = _notice.GetNotices(Token, page, pagesize);
            //var _data = _context.Notices.Skip(page).Take(pagesize).ToList();

            return data;
        }
        [HttpPost("{id}")]
        public object ExtendedNotice([FromRoute] long id)
        {
            var Notice = _context.Notices.Find(id);
            long totalPrice = 0;
            if (Notice == null)
                return new { status = 1, message = "چنین درخواستی یافت نشد." };

            if (Notice.expireDate >= DateTime.Now)
                return new { status = 1, message = "آگهی منقضی نشده است و نمیتوان تمدید کرد" };

            var setting = _context.Settings.FirstOrDefault();
            Notice.expireDate = DateTime.Now.AddDays(Convert.ToInt64(setting.countExpireDate));
            var user = _context.Users.Find(Notice.userId);

            var category = _context.Categorys.Find(Notice.categoryId);
            totalPrice = category.expirePrice;
            if (totalPrice < 0)
            {
                totalPrice = 0;
            }

            //var staticPriceItem = _context.StaticPrices.FirstOrDefault(s => s.code == category.staticexpirePriceId);
            //if (staticPriceItem != null)
            //{
            //    totalPrice = staticPriceItem.price;
            //}
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

        [HttpGet("{id}")]
        public async Task<object> GetNotice([FromRoute] int id)
        {
            var notices = _context.Notices.Where(p => p.isEmergency && p.ExpireDateEmergency < DateTime.Now && p.deletedAt == null).ToList();
            foreach (var notice in notices)
            {
                notice.isEmergency = false;
                await _context.SaveChangesAsync();
            }

        //public object GetNotice([FromRoute] long id)
        //{
            var settings = _context.Settings.FirstOrDefault();

            NoticeViewModelHelper noticeViewModels = new NoticeViewModelHelper();

            string Token = HttpContext.Request?.Headers["Token"];
            var user = _context.Users.Where(p => p.token == Token).FirstOrDefault();
            if (user == null)
                return new { status = 3, message = "چنین کاربری وجود ندارد." };
            var Notice = _context.Notices.Where(n => n.id == id && n.deletedAt == null).FirstOrDefault();
            if (Notice.userId != user.id)
            {
                if (!_context.VisitNoticeUsers.Any(x => x.noticeId == id && x.userId == user.id))
                {
                    _context.VisitNoticeUsers.Add(new VisitNoticeUser { noticeId = id, userId = user.id });
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




            var noticeitem = _context.Notices.Select(p => new
            {
                p.id,
                p.link,
                p.countView,
                IsFavorit,
                areaName = p.area.name,
                provinceName = p.province.name,
                cityName = p.city.name,
                p.code,
                createDate = DateToUnix(p.createDate),
                p.description,
                categoryName = p.category.name,
                user = p.user.cellphone,
                userId = p.user.id,
                p.title,
                p.price,
                p.movie,
                p.image,
                p.isSpecial,
                p.lastPrice,
                dailyVisit = DailyVisit(p.id),
                p.categoryId,
                p.areaId,
                p.cityId,
                p.provinceId,
                p.isEmergency,
                p.expireDate,
                p.expireDateIsespacial,
                p.ExpireDateEmergency
            }).FirstOrDefault(x => x.id == id);



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
                noticeViewModels.emergencyPrice = PriceItem.emergencyPrice;
                noticeViewModels.emergencyPriceCode = PriceItem.emergencyPriceCode;
                noticeViewModels.expirePrice = PriceItem.expirePrice;
                noticeViewModels.expirePriceCode = PriceItem.expirePriceCode;
                noticeViewModels.ladderPrice = PriceItem.ladderPrice;
                noticeViewModels.ladderPriceCode = PriceItem.ladderPriceCode;
                noticeViewModels.registerPrice = PriceItem.registerPrice;
                noticeViewModels.registerPriceCode = PriceItem.registerPriceCode;
                noticeViewModels.userId = noticeitem.userId;

                noticeViewModels.LeftDayToExpire = PersianCalendarDate.LeftDayToExpire(noticeitem.expireDate);
                noticeViewModels.LeftDayToExpireEmergency = PersianCalendarDate.LeftDayToExpire(noticeitem.ExpireDateEmergency);
                noticeViewModels.LeftDayToExpireSpecial = PersianCalendarDate.LeftDayToExpire(noticeitem.expireDateIsespacial);

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
        private string GetPathAndFilename(string filename)
        {
            string path = System.IO.Path.Combine(environment.WebRootPath, "Notice/", filename);
            if (!System.IO.File.Exists(path))
                Directory.CreateDirectory(path);
            return path;
        }
        private async Task ProgressMethod(IFormFile file, string namefile)
        {
            long totalBytes = file.Length;
            long totalReadBytes = 0;
            byte[] buffer = new byte[16 * 1024];
            int readBytes;
            using (FileStream output = System.IO.File.Create(this.GetPathAndFilename(namefile)))
            {
                using (Stream input = file.OpenReadStream())
                {
                    while ((readBytes = input.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        await output.WriteAsync(buffer, 0, readBytes);
                        totalReadBytes += readBytes;
                        int Progress = (int)((float)totalReadBytes / (float)totalBytes * 100.0);
                        await Task.Delay(10);
                    }

                }

            }
        }
        [HttpPost("PostNotice")]
        public async  Task<IActionResult> PostNotice([FromForm] AddNotice addNotice, List<IFormFile> image)
        {
            var test = _context.StaticPrices;
            int Progress = 0;
            PaymentRequest _paymentRequest = new PaymentRequest();
            long discountprice = 0;
            bool havediscount = false;
            int _code = 0;
            try
            {
                int number;
                string Token = HttpContext.Request?.Headers["Token"];
                var setting = _context.Settings.FirstOrDefault();
                var category = _context.Categorys.Find(addNotice.categoryId);
                var user = _context.Users.FirstOrDefault(x => x.token == Token);
                


                if (user == null)
                {
                    return BadRequest("لطفا از حساب کاربری خود خارج ، و مجددا وارد شوید");
                }

                //////
                if (user.IsBlocked)
                {
                    return BadRequest("شما در لیست سیاه قرار دارید و مجاز به ثبت آگهی نمی باشید");
                }

                var _price1 = addNotice.price.Replace(",", "");
                var _price2 = addNotice.lastPrice.Replace(",", "");
                //if (!int.TryParse(_price1, out number) || !int.TryParse(_price2, out number))
                //{
                //    TempData["PursheResult"] = "قیمت را به عدد وارد کنید";
                //    return View("Profile2");
                //}
                //discount
                if (!string.IsNullOrEmpty(addNotice.discountcode) && category.registerPrice > 1000)
                {
                    int n;
                    if (!int.TryParse(addNotice.discountcode, out n))
                    {
                        return NotFound("کد وارد شده معتبر نیست");
                    }
                    _code = Convert.ToInt32(addNotice.discountcode);

                    if (!_DiscountCode.CheckCode(_code))
                    {
                        return NotFound("کد وارد شده معتبر نیست");

                    }
                    if (_DiscountCode.IsAlreadyUsed(user.id, _DiscountCode.GetIdByCode(_code)))
                    {
                        return NotFound("این کد قبلا توسط شما استفاده شده است");

                    }
                    else
                    {
                        discountprice = _DiscountCode.GetDiscountPrice(_code);
                        havediscount = true;
                    }
                }

                bool checkWrongWords()
                {
                    if (setting.wrongWord.Contains(addNotice.title) || setting.wrongWord.Contains(addNotice.description))
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }

                if (checkWrongWords() == false)
                {
                    return BadRequest("لطفا در توضیحات و عنوان از کلمات مناسب استفاده نمایید.");
                }

                var notice = new Notice();
                notice.title = addNotice.title;
                notice.lastPrice = Convert.ToInt64(addNotice.lastPrice.Replace(",", ""));
                notice.areaId = addNotice.areaId;
                notice.cityId = addNotice.cityId;
                notice.provinceId = addNotice.provinceId;
                notice.price = Convert.ToInt64(addNotice.price.Replace(",", ""));
                notice.categoryId = addNotice.categoryId;
                notice.description = addNotice.description;
                notice.createDate = DateTime.Now;
                notice.expireDateIsespacial = DateTime.Now;
                notice.expireDate = DateTime.Now.AddDays(Convert.ToInt64(setting.countExpireDate));
                notice.adminConfirmStatus = EnumStatus.Pending;
                notice.link = addNotice.link;
                notice.userId = user.id;
                if (_context.Notices.Count() == 0)
                    notice.code = "1";
                else
                    notice.code = (Convert.ToInt32(_context.Notices.LastOrDefault().code) + 1).ToString();
                List<string> images = new List<string>();

                if (image != null && image.Count > 0)
                {
                    foreach (var file in image)
                    {
                        string text = "Dakeh.Net";
                        string files = System.IO.Path.GetFileNameWithoutExtension(file.FileName) + ".png";
                        var namefile = Guid.NewGuid().ToString().Replace('-', '0').Substring(0, 7) + System.IO.Path.GetExtension(file.FileName).ToLower();
                        string filePath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Notice", namefile);



                        //var filePath = System.IO.Path.Combine(environment.WebRootPath, "Notice/", namefile);
                        string e = System.IO.Path.GetExtension(namefile);

                        if (e == ".jpg" || e == ".webp" || e == ".jpeg" || e == ".png" || e == ".gif")
                        {
                            images.Add("/Notice/" + namefile);

                            if (String.IsNullOrEmpty(notice.image))
                            {
                                notice.image = "/Notice/" + namefile;
                                using (var stream = new FileStream(filePath, FileMode.Create))
                                {

                                    await file.CopyToAsync(stream);
                                }
                                await ProgressMethod(file, namefile);
                                using (Image originalImage = Image.FromStream(file.OpenReadStream()))
                                {

                                    using (Bitmap oBitmap = new Bitmap(originalImage))
                                    {
                                        using (Graphics g = Graphics.FromImage(oBitmap))
                                        {

                                            Brush oBrush = new SolidBrush(Color.Green);
                                            System.Drawing.Font oFont = new System.Drawing.Font("ARial", 25, FontStyle.Bold, GraphicsUnit.Pixel);
                                            SizeF osizef = new SizeF();
                                            osizef = g.MeasureString(text, oFont);
                                            Point oPoint = new Point(oBitmap.Width - ((int)osizef.Width + 10), oBitmap.Height - ((int)osizef.Height + 10));
                                            g.DrawString(text, oFont, oBrush, oPoint);
                                        }
                                        oBitmap.Save(filePath, System.Drawing.Imaging.ImageFormat.Jpeg);
                                    }

                                }
                            }
                            else
                            {
                                using (var stream = new FileStream(filePath, FileMode.Create))
                                {

                                    await file.CopyToAsync(stream);
                                }
                                await ProgressMethod(file, namefile);
                                using (Image originalImage = Image.FromStream(file.OpenReadStream()))
                                {

                                    using (Bitmap oBitmap = new Bitmap(originalImage))
                                    {
                                        using (Graphics g = Graphics.FromImage(oBitmap))
                                        {

                                            Brush oBrush = new SolidBrush(Color.Green);
                                            System.Drawing.Font oFont = new System.Drawing.Font("ARial", 25, FontStyle.Bold, GraphicsUnit.Pixel);
                                            SizeF osizef = new SizeF();
                                            osizef = g.MeasureString(text, oFont);
                                            Point oPoint = new Point(oBitmap.Width - ((int)osizef.Width + 10), oBitmap.Height - ((int)osizef.Height + 10));
                                            g.DrawString(text, oFont, oBrush, oPoint);
                                        }
                                        oBitmap.Save(filePath, System.Drawing.Imaging.ImageFormat.Jpeg);
                                    }

                                }

                            }
                        }
                        else if (e == ".mp4")
                        {
                            if (String.IsNullOrEmpty(notice.movie))
                            {
                                notice.movie = "/Notice/" + namefile;
                                using (var stream = new FileStream(filePath, FileMode.Create))
                                {
                                    await file.CopyToAsync(stream);
                                }
                                await ProgressMethod(file, namefile);

                                //if (String.IsNullOrEmpty(notice.image))
                                //{
                                //    GetBitMap.GetThumbnail(filePath.Replace('/', '\\'), System.IO.Directory.GetCurrentDirectory() +  "\\wwwroot\\Notice\\" + namefile);
                                //    //notice.image = "/Notice/" + namefile.Replace(".mp4", ".jpg");
                                //}
                            }
                        }

                    }

                }

                if (string.IsNullOrEmpty(notice.image))
                {
                    notice.image = "";
                }

                _context.Notices.Add(notice);


                ////تایید خودکار آگهی //////////////////////
                if (setting.AutoAccept)
                {
                    notice.adminConfirmStatus = EnumStatus.Accept;

                    CommonService.SendSMS_Accept(user.cellphone, notice.title);
                }



                await _context.SaveChangesAsync();


                foreach (var item in images)
                {
                    _context.NoticeImages.Add(new NoticeImage
                    {
                        noticeId = notice.id,
                        image = item
                    });
                    await _context.SaveChangesAsync();
                }
                var totalp = 0;
                List<Models.Category> cats = new List<Models.Category>();
                int cat = notice.categoryId;
                for (int i = 0; i < 10; i++)
                {
                    var categorys = _context.Categorys.FirstOrDefault(x => x.id == cat);
                    if (categorys == null)
                        break;
                    cats.Add(categorys);
                    if (categorys.parentCategoryId != null)
                        cat = (int)categorys.parentCategoryId;
                    else
                        break;
                }
                foreach (var item in cats)
                {
                    if (item.registerPrice > 0)
                    {
                        totalp = (int)item.registerPrice;
                    }
                }
                Factor factor = new Factor();
                factor.state = State.IsPay;
                factor.userId = user.id;
                factor.createDatePersian = PersianCalendarDate.PersianCalendarResult(DateTime.Now);
                factor.noticeId = notice.id;
                factor.factorKind = FactorKind.Add;
                factor.totalPrice = havediscount ? totalp - discountprice : totalp;
                _context.Factors.Add(factor);
                //Payment
                await _context.SaveChangesAsync();

                if (factor.totalPrice >= 10000)
                {
                    try
                    {
                        PaymentRequestAttemp request = new PaymentRequestAttemp();
                        request.FactorId = factor.id;
                        request.NoticeId = notice.id;
                        request.UserId = user.id;
                        request.pursheType = pursheType.RegisterNotice;
                        _context.Add(request);
                        _context.SaveChanges();

                        var res = PaymentHelper.SendRequest(request.Id, havediscount ? totalp - discountprice : totalp, "http://dakeh.net/Purshe/VerifyRequest");
                        if (res != null && res.Result != null)
                        {
                            if (res.Result.ResCode == "0")
                            {
                                if (havediscount)
                                {
                                    _DiscountCode.AddUserToDiscountCode(user.id, _code);
                                }
                                Response.Redirect(string.Format("{0}/Purchase/Index?token={1}", PaymentHelper.PurchasePage, res.Result.Token));
                            }
                            return Ok(res.Result.Description);
                        }
                    }
                    catch (Exception)
                    {
                        return Ok( new { status = 2, title = "ثبت آگهی", message = "امکان اتصال به درگاه بانکی وجود ندارد." });
                    }
                }

            }
            catch (Exception ex)
            {
                return Ok( new { status = 2, title = "خطا در ثبت آگهی", message = ex.Message + " " + ex.StackTrace + " " + ex.InnerException?.Message + " " + ex.InnerException?.InnerException?.Message });
            }
            return Ok( new { status = 1, title = "ثبت آگهی", message = "آگهی شما با موفقیت ثبت گردید." });
        }

        [Route("RemoveNotice")]
        public object RemoveNotice(long id)
        {
            Notice noticeitem = _context.Notices.Where(s => s.id == id).FirstOrDefault();
            if (noticeitem == null || noticeitem.deletedAt != null)
            {
                return new { status = 3, message = "چنین آگهی وجود ندارد." };
            }
            try
            {
                
                //change deleted at to current date
                noticeitem.deletedAt = DateTime.Now;
                _context.SaveChanges();

                // _context.Notices.Remove(noticeitem);
                
                /*if (noticeitem.image != null)
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
                _context.SaveChanges();*/
                return new { status = 1, title = "حذف موفقیت آمیز", message = "آگهی حذف گردید" };

            }
            catch (Exception ex)
            {
                return new { status = 3, message = "امکان حذف آگهی وجود ندارد" + ex.Message + ' ' + ex.StackTrace + ' ' + ex.Data + ' ' + ex.Source + ' ' + ex.InnerException?.Message + ' ' + ex.InnerException?.InnerException?.Message };
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
                        item.espacialPrice = categoryitem.staticespacialPriceId == "0" ? 0 : _context.StaticPrices.Where(s => s.code == categoryitem.staticespacialPriceId).FirstOrDefault()?.price ?? 0;
                        item.espacialPriceCode = categoryitem.staticespacialPriceId;
                        item.ladderPriceCode = categoryitem.staticladerPriceId;
                        item.emergencyPriceCode = categoryitem.staticemergencyPriceId;
                        item.registerPriceCode = categoryitem.staticregisterPriceId;
                        item.expirePriceCode = categoryitem.staticexpirePriceId;
                        item.emergencyPrice = categoryitem.staticemergencyPriceId == null || categoryitem.staticemergencyPriceId == "0" ? 0 : _context.StaticPrices.Where(s => s.code == categoryitem.staticemergencyPriceId).FirstOrDefault()?.price ?? 0;
                        item.ladderPrice = categoryitem.staticladerPriceId == "0" ? 0 : _context.StaticPrices.Where(s => s.code == categoryitem.staticladerPriceId).FirstOrDefault()?.price ?? 0;
                        item.expirePrice = categoryitem.staticexpirePriceId == "0" ? 0 : _context.StaticPrices.Where(s => s.code == categoryitem.staticexpirePriceId).FirstOrDefault()?.price ?? 0;
                        item.registerPrice = categoryitem.staticregisterPriceId == "0" ? 0 : _context.StaticPrices.Where(s => s.code == categoryitem.staticregisterPriceId).FirstOrDefault()?.price ?? 0;
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