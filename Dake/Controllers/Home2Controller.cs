using Dake.DAL;
using Dake.Models;
using Dake.Models.ViewModels;
using Dake.Service.Common;
using Dake.Service.Interface;
using Dake.Utility;
using Dake.ViewModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Text;
using System.Security.Cryptography;
using System.Net;
using System.Net.Http;
using Dake.Controllers.API;
using Dake.Models.ViewModels;
using Dake.Service.Interface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Cryptography.Xml;
using System.Drawing;
using DocumentFormat.OpenXml.Vml;
using DocumentFormat.OpenXml.Office.CustomUI;

namespace Dake.Controllers
{


    public class Home2Controller : Controller
    {
        private readonly Context _context;
        private readonly IHostingEnvironment environment;
        private IDiscountCode _IDiscountCode;

        public static int Progress { get; set; }
        public Home2Controller(Context context, IHostingEnvironment environment, IDiscountCode IdiscountCode)
        {
            this.environment = environment;
            _context = context;
            _IDiscountCode = IdiscountCode;

        }
        public IActionResult Index()
        {
            int cityid = 0;
            ViewData["category"] = _context.Categorys.ToList();
            ViewData["category2"] = _context.Categorys.Where(x => x.parentCategoryId == null).ToList();
            ViewData["city"] = _context.Cities.ToList();
            ViewBag.NoticeCount = _context.Notices.Where(s => s.adminConfirmStatus == EnumStatus.Accept && s.expireDate > DateTime.Now && s.deletedAt == null).Count();
            if (HttpContext.Session.GetString("cityId") != null)
            {
                if (HttpContext.Session.GetString("cityId") != "0")
                {
                    cityid = Convert.ToInt32(HttpContext.Session.GetString("cityId"));
                    ViewBag.CityName = _context.Cities.Where(s => s.id == cityid).FirstOrDefault().name;
                }
            }
            else
            {
                ViewBag.CityName = string.Empty;
            }
            return View();
        }
        public IActionResult AboutUs()
        {
            var data = new AboutUs();
            if (_context.AboutUss.Any())
                data = _context.AboutUss.FirstOrDefault();
            return View(data);
        }
        public IActionResult Contact()
        {
            var data = new ContactUs();
            if (_context.ContactUss.Any())
                data = _context.ContactUss.FirstOrDefault();
            return View(data);
        }
        public IActionResult Rule()
        {
            var data = new Rule();
            if (_context.Rules.Any())
                data = _context.Rules.FirstOrDefault();
            return View(data);
        }
        public IActionResult Stir()
        {
            var data = new Stir();
            if (_context.Stirs.Any())
                data = _context.Stirs.FirstOrDefault();
            return View(data);
        }
        public string GetSubCat(int? catId)
        {
            string sub = "";
            var cats = _context.Categorys.Where(x => x.parentCategoryId == catId || x.id == catId);
            if (cats.Count() > 1)
            {
                foreach (var item in cats)
                {
                    if (catId == item.id)
                        sub += "<li><a id='div-" + item.id + "' onclick='SearchByCat(" + item.parentCategoryId + ")' style='cursor:pointer'>دسته اصلی</a></li>";
                    else
                        sub += "<li><a id='div-" + item.id + "' onclick='SearchByCat(" + item.id + ")' style='cursor:pointer'>" + item.name + "</a></li>";
                }
            }
            return sub;

        }
        
        public IActionResult GetData(int page, int? catId, string title, string cityId, int? subCatId)
        {


            int _cityId = 0;
            IEnumerable<Notice> notices = null;
            var user = _context.Users.FirstOrDefault(x => x.cellphone == User.Identity.Name && x.deleted == null);
            IQueryable<Notice> resultEspacial = null;
            FirstHomeViewModel firstHomeViewModel = new FirstHomeViewModel();

            if (string.IsNullOrEmpty(HttpContext.Session.GetString("cityId")))
            {
                HttpContext.Session.SetString("cityId", "0");
                _cityId = 0;
            }
            else if (cityId == "0" && HttpContext.Session.GetString("cityId") != null)
            {
                HttpContext.Session.SetString("cityId", "0");
                _cityId = 0;
            }
            else if (!string.IsNullOrEmpty(cityId) && HttpContext.Session.GetString("cityId") != null && cityId != "0")
            {
                HttpContext.Session.SetString("cityId", cityId);
                _cityId = Convert.ToInt32(HttpContext.Session.GetString("cityId"));
            }
            else if (!string.IsNullOrEmpty(HttpContext.Session.GetString("cityId")) &&
            string.IsNullOrEmpty(cityId))
            {
                _cityId = Convert.ToInt32(HttpContext.Session.GetString("cityId"));
            }

            if (user != null)
            {
                if (user.provinceId != null)
                {
                    notices = _context.Notices.Where(x => x.expireDate >= DateTime.Now && x.adminConfirmStatus == EnumStatus.Accept /*&& x.provinceId == user.provinceId*/ && x.deletedAt == null).Include(x => x.category).OrderByDescending(u => u.createDate);
                    resultEspacial = _context.Notices.Where(x => x.expireDate >= DateTime.Now && x.adminConfirmStatus == EnumStatus.Accept && x.isSpecial && x.expireDateIsespacial >= DateTime.Now && x.deletedAt == null /*&& x.provinceId == user.provinceId*/).Include(x => x.category).OrderByDescending(x => x.expireDateIsespacial);
                }
                else
                {
                    notices = _context.Notices.Where(x => x.expireDate >= DateTime.Now && x.adminConfirmStatus == EnumStatus.Accept && x.deletedAt == null).Include(x => x.category).OrderByDescending(u => u.createDate);
                    resultEspacial = _context.Notices.Where(x => x.expireDate >= DateTime.Now && x.adminConfirmStatus == EnumStatus.Accept && x.isSpecial && x.expireDateIsespacial >= DateTime.Now && x.deletedAt == null).Include(x => x.category).OrderByDescending(x => x.expireDateIsespacial);
                }
            }
            else
            {
                notices = _context.Notices.Where(x => x.expireDate >= DateTime.Now && x.adminConfirmStatus == EnumStatus.Accept && x.deletedAt == null).Include(x => x.category).OrderByDescending(u => u.createDate);
                resultEspacial = _context.Notices.Where(x => x.expireDate >= DateTime.Now && x.adminConfirmStatus == EnumStatus.Accept && x.isSpecial && x.expireDateIsespacial >= DateTime.Now && x.deletedAt == null).Include(x => x.category).OrderByDescending(x => x.expireDateIsespacial);
            }
            //resultEspacial = _context.Notices.Where(x => x.isSpecial).OrderByDescending(x => x.expireDateIsespacial);
            if (catId == null)
            {
                notices = notices.ToList();
            }
            else
            {
                List<int> cats = new List<int>();
                List<int> subcats = new List<int>();

                if (_context.Categorys.Any(x => x.parentCategoryId == catId))
                {
                    foreach (var item in _context.Categorys.Where(x => x.parentCategoryId == catId))
                    {
                        cats.Add(item.id);
                    }
                }
                foreach (var item in _context.Categorys.Where(x => cats.Contains((int)x.parentCategoryId)))
                {
                    if (!cats.Contains(item.id))
                        cats.Add(item.id);
                }
                foreach (var item in _context.Categorys.Where(x => cats.Contains((int)x.parentCategoryId)))
                {
                    if (!cats.Contains(item.id))
                        cats.Add(item.id);
                }
                if (subCatId.HasValue)
                {
                    if (_context.Categorys.Any(x => x.id == subCatId))
                    {
                        foreach (var item in _context.Categorys.Where(x => x.id == subCatId))
                        {
                            subcats.Add(item.id);
                        }
                    }
                    foreach (var item in _context.Categorys.Where(x => subcats.Contains((int)x.id)))
                    {
                        if (!subcats.Contains(item.id))
                            subcats.Add(item.id);
                    }
                    foreach (var item in _context.Categorys.Where(x => subcats.Contains((int)x.id)))
                    {
                        if (!subcats.Contains(item.id))
                            subcats.Add(item.id);
                    }
                }

                notices = notices.Where(x => cats.Contains(x.categoryId));
                if (subCatId.HasValue && subcats.Count > 0)
                {
                    notices = notices.Where(x => subcats.Contains(x.category.id));
                }
                resultEspacial = resultEspacial.Where(x => cats.Contains(x.categoryId));
                if (subCatId.HasValue && subcats.Count > 0)
                {
                    resultEspacial = resultEspacial.Where(x => subcats.Contains(x.category.id));
                }
                string sub = "";
                foreach (var item in _context.Categorys.Where(x => x.parentCategoryId == catId))
                {
                    sub += "<li><a onclick='SearchByCat(" + item.id + ")' class='index-link'>" + item.name + "</a></li>";
                }
                firstHomeViewModel.subCat = sub;
            }
            
            if (!string.IsNullOrEmpty(title))
            {
                notices = notices.Where(x => x.title.Contains(title, StringComparison.OrdinalIgnoreCase));

            }
            ViewData["firstEspcial"] = _context.Categorys.Where(x => x.parentCategoryId == null).ToList();
            firstHomeViewModel.notices = _cityId == 0 ? notices.ToList() : notices.Where(s => s.cityId == _cityId).ToList();
            foreach (var item in firstHomeViewModel.notices)
            {
                if (!string.IsNullOrEmpty(item.image) && item.image.Contains("/images/Category/"))
                {
                    item.image = string.Empty;
                }
            }


            firstHomeViewModel.espacialNotices = _cityId == 0
                ? resultEspacial.ToList()
                : resultEspacial.Where(s => s.cityId == _cityId).ToList();

            foreach (var item in firstHomeViewModel.espacialNotices)
            {
                if (!string.IsNullOrEmpty(item.image) && item.image.Contains("/images/Category/"))
                {
                    item.image = string.Empty;
                }
            }
            
            firstHomeViewModel.Banner = _context.Banner.Where(p=> p.expireDate >= DateTime.Now && p.adminConfirmStatus == EnumStatus.Accept).Include(p => p.BannerImage).ToList();

            firstHomeViewModel.Categories = _context.Categorys.ToList();


            firstHomeViewModel.NoticeImage = _context.NoticeImages.ToList();

            var settings = _context.Settings.FirstOrDefault();
            foreach (var item in firstHomeViewModel.notices)
            {
                if (settings?.showPriceForCars == false && IsDrivingPrice(item.categoryId))
                {
                    item.price = 0;
                    item.lastPrice = 0;
                }
            }
            foreach(var item in firstHomeViewModel.notices)
            {
                if (item.image?.Length == 0 && item.movie == null)
                {
                    if(item.category.image == null)
                    {
                        var cat1 =_context.Categorys.FirstOrDefault(p=>p.id == item.category.parentCategoryId);
                        if (cat1.image == null)
                        {
                            if(cat1.parentCategoryId != null)
                            {
                                var cat2 = _context.Categorys.FirstOrDefault(p => p.id == cat1.parentCategoryId);
                                if(cat2.image == null)
                                {
									if (cat2.parentCategoryId != null)
									{
										var cat3 = _context.Categorys.FirstOrDefault(p => p.id == cat2.parentCategoryId);
										if (cat3.image == null)
										{
											if (cat3.parentCategoryId != null)
											{
												var cat4 = _context.Categorys.FirstOrDefault(p => p.id == cat3.parentCategoryId);
												if (cat4.image == null)
												{
													if (cat4.parentCategoryId != null)
													{
														var cat5 = _context.Categorys.FirstOrDefault(p => p.id == cat4.parentCategoryId);
														if (cat5.image == null)
														{
															if (cat5.parentCategoryId != null)
															{
																var cat6 = _context.Categorys.FirstOrDefault(p => p.id == cat5.parentCategoryId);																
															    item.image = cat6.image;
															}
														}
														else
														{
															item.image = cat5.image;
														}
													}
												}
												else
												{
													item.image = cat4.image;
												}

											}
										}
										else
										{
											item.image = cat3.image;
										}

									}
								}
                                else
                                {
									item.image = cat2.image;
								}

							}
                        }
                        else
                        {
							item.image = cat1.image;
						}
                    }
                    else
                    {
                        item.image = item.category.image;
                    }
                    if(item.image == null)
                    {
                        item.image = "/images/empyty.png";
                    }
                }
            }

            return PartialView("_Notice", firstHomeViewModel);
        }
        public IActionResult Profile()
        {
            var user = _context.Users.FirstOrDefault(x => x.cellphone == User.Identity.Name && x.deleted == null);
            if(user == null)
            {
				var user2 = _context.Users.FirstOrDefault(x => x.cellphone+x.adminRole == User.Identity.Name && x.deleted == null);
				ViewData["Discounts"] = _IDiscountCode.GetDiscountCodeForUser(user2.id);
			}
            else
            {
				ViewData["Discounts"] = _IDiscountCode.GetDiscountCodeForUser(user.id);
			}

            ViewData["Cities"] = _context.Cities.OrderBy(u => u.id).ToList();
            ViewData["Categorie"] = _context.Categorys.Where(x => x.parentCategoryId == null).OrderBy(u => u.id).ToList();
            return View();
        }

       


        public IActionResult Profile2()
        {
            return View();
        }
        public IActionResult ProfileEdit(int id)
        {
            AddNotice addNotice = new AddNotice();
            var notice = _context.Notices.FirstOrDefault(x => x.id == id);
            addNotice.areaId = notice.areaId;
            addNotice.provinceId = notice.provinceId;
            addNotice.cityId = notice.cityId;
            addNotice.categoryId = notice.categoryId;
            addNotice.description = notice.description;
            addNotice.imageUrl = notice.image;
            addNotice.movieUrl = notice.movie;
            addNotice.price = notice.price.ToString();
            addNotice.lastPrice = notice.lastPrice.ToString();
            addNotice.link = notice.link;
            addNotice.title = notice.title;
            ViewData["Cities"] = _context.Cities.OrderBy(u => u.id).ToList();
            ViewData["Provinces"] = _context.Provinces.Where(x => x.cityId == notice.cityId).OrderBy(u => u.id).ToList();
            ViewData["Areas"] = _context.Areas.Where(x => x.provinceId == notice.provinceId).OrderBy(u => u.id).ToList();
            ViewData["Categorie"] = _context.Categorys.Where(x => x.parentCategoryId != null).OrderBy(u => u.id).ToList();
            var allimages = _context.NoticeImages.Where(x => x.noticeId == id).OrderBy(x => x.id).ToList();
            addNotice.NoticeImages = allimages;
            addNotice.id = id;
            return View(addNotice);
        }
        public string GetProvince(int id)
        {
            var provinces = _context.Provinces.Where(x => x.cityId == id);
            string proContent = "<select class='title form-control selectpicker valid' data-size='5' data-val='true' required  id='provinceId' name='provinceId' aria-describedby='provinceId-error' aria-invalid='false'><option value=''>شهرستان آگهی را انتخاب کنید</option>";
            foreach (var item in provinces)
            {
                proContent += "<option value=" + item.id + ">" + item.name + "</option>";
            }
            //proContent += "</select>" +
            proContent += "</select>";
            //"<span class='text-danger field-validation-valid' data-valmsg-for='provinceId' data-valmsg-replace='true'><span id='provinceId-error' class=''>لطفا شهرستان را وارد کنید</span></span>";
            return proContent;
        }
        public string GetArea(int id)
        {
            var areas = _context.Areas.Where(x => x.provinceId == id);
            string proContent = "<select class='title form-control selectpicker valid' data-size='5' data-val='true' required id='areaId' name='areaId' aria-describedby='areaId-error' aria-invalid='false'><option value=''>محدوده آگهی را انتخاب کنید</option>";
            foreach (var item in areas)
            {
                proContent += "<option value=" + item.id + ">" + item.name + "</option>";
            }
            proContent += "</select>";
            //proContent += "</select>" +
            //    "<span class='text-danger field-validation-valid' data-valmsg-for='areaId' data-valmsg-replace='true'><span id='areaId-error' class=''>لطفا محدوده را وارد کنید</span></span>";
            return proContent;
        }

        [HttpPost]
        public ActionResult Progressing()
        {
            return this.Content(Progress.ToString());
        }

        [HttpPost]
        public async Task<IActionResult> AddNotice(AddNotice addNotice, List<IFormFile> image)
        {
            if (!ModelState.IsValid)
            {
                var user = _context.Users.FirstOrDefault(x => x.cellphone == User.Identity.Name && x.deleted == null);
                if (user == null)
                {
                    var user2 = _context.Users.FirstOrDefault(x => x.cellphone + x.adminRole == User.Identity.Name && x.deleted == null);
                    ViewData["Discounts"] = _IDiscountCode.GetDiscountCodeForUser(user2.id);
                }
                else
                {
                    ViewData["Discounts"] = _IDiscountCode.GetDiscountCodeForUser(user.id);
                }

                ViewData["Cities"] = _context.Cities.OrderBy(u => u.id).ToList();
                ViewData["Categorie"] = _context.Categorys.Where(x => x.parentCategoryId == null).OrderBy(u => u.id).ToList();

                return View(nameof(Profile), addNotice);
            }
            var test = _context.StaticPrices;
            Progress = 0;
            PaymentRequest _paymentRequest = new PaymentRequest();
            int discountprice = 0;
            bool havediscount = false;
            int _code = 0;
            try
            {
                int number;
                var setting = _context.Settings.FirstOrDefault();
                var category = _context.Categorys.Find(addNotice.categoryId);
                var user1 = _context.Users.FirstOrDefault(x => x.cellphone == User.Identity.Name && x.deleted == null);
                User user = new User();
                if(user1 != null)
                {
                     user = user1;
                }
                else
                {
                     user = _context.Users.FirstOrDefault(x => x.cellphone + x.adminRole == User.Identity.Name && x.deleted == null);
                }
                

                if (user == null)
                {
                    TempData["PursheResult"] = "لطفا از حساب کاربری خود خارج ، و مجددا وارد شوید";
                    return View("Profile2");
                }

                //////
                if (user.IsBlocked)
                {
                    TempData["DiscountErrorMessage"] = "شما در لیست سیاه قرار دارید و مجاز به ثبت آگهی نمی باشید. ";
                    return RedirectToAction(nameof(Profile));
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
                        TempData["DiscountErrorMessage"] = "کد وارد شده معتبر نیست";
                        return RedirectToAction(nameof(Profile));
                    }
                    _code = Convert.ToInt32(addNotice.discountcode);

                    if (!_IDiscountCode.CheckCode(_code))
                    {
                        TempData["DiscountErrorMessage"] = "کد وارد شده معتبر نیست";
                        return RedirectToAction(nameof(Profile));

                    }
                    if (_IDiscountCode.IsAlreadyUsed(user.id, _IDiscountCode.GetIdByCode(_code)))
                    {
                        TempData["DiscountErrorMessage"] = "این کد قبلا توسط شما استفاده شده است ";
                        return RedirectToAction(nameof(Profile));

                    }
                    else
                    {
                        discountprice = (int)_IDiscountCode.GetDiscountPrice(_code);
                        havediscount = true;
                    }
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
                                            Font oFont = new Font("ARial", 25, FontStyle.Bold, GraphicsUnit.Pixel);
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
											Font oFont = new Font("ARial", 25, FontStyle.Bold, GraphicsUnit.Pixel);
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
                int total = 10000;
                if (user.Invite_Price != 0)
                {
                    if (user.Invite_Price > total)
                    {
                        int in_price = user.Invite_Price - total;
                        total = 0;
                        user.Invite_Price = in_price;
                        _context.Users.Update(user);
                        _context.SaveChanges();
                    }
                    else
                    {
                        total = total - user.Invite_Price;
                        user.Invite_Price = 0;
                        _context.Users.Update(user);
                        _context.SaveChanges();
                    }
                }
                Factor factor = new Factor();
                factor.state = State.NotPay;
                factor.userId = user.id;
                factor.createDatePersian = PersianCalendarDate.PersianCalendarResult(DateTime.Now);
                factor.noticeId = notice.id;
                factor.factorKind = FactorKind.Add;
                factor.totalPrice = total;
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

                        //var res = PaymentHelper.SendRequest(request.Id, havediscount ? totalp - discountprice : totalp, "http://dakeh.net/Purshe/VerifyRequest");
                        int totall = havediscount ? totalp - discountprice : totalp;
                        
                        var pyment = new Zarinpal.Payment("ceb42ad1-9eb4-47ec-acec-4b45c9135122", total);
                        var res = pyment.PaymentRequest($"پرداخت فاکتور شمارهی {factor.id}", "https://dakeh.net/Payments/Index/" + factor.id, null , user.cellphone);
                        if (res != null && res.Result != null)
                        {   
                            if (res.Result.Status == 100)
                            {
                                return Redirect("https://zarinpal.com/pg/StartPay/" + res.Result.Authority);
                                //var n = _context.Notices.FirstOrDefault(p => p.id == notice.id);
                                //n.isPaid = true;
                                //_context.Notices.Update(n);
                                //_context.SaveChanges();
                                //if (havediscount)
                                //{
                                //    _IDiscountCode.AddUserToDiscountCode(user.id, _code);
                                //}
                                //Response.Redirect(string.Format("{0}/Purchase/Index?token={1}", PaymentHelper.PurchasePage, res.Result.Token));
                            }
                            else
                            {
                                ViewBag.Message = "امکان اتصال به درگاه بانکی وجود ندارد";
                            }
                            
                            return View("Profile2"); ;
                        }
                    }
                    catch (Exception)
                    {
                        ViewBag.Message = "امکان اتصال به درگاه بانکی وجود ندارد";
                    }
                }

            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
            return RedirectToAction("Profile2");
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
                        Progress = (int)((float)totalReadBytes / (float)totalBytes * 100.0);
                        await Task.Delay(10);
                    }

                }

            }
        }


        private string GetPathAndFilename(string filename)
        {
            string path = System.IO.Path.Combine(environment.WebRootPath, "Notice/", filename);
            if (!System.IO.File.Exists(path))
                Directory.CreateDirectory(path);
            return path;
        }
        [HttpPost]
        public IActionResult EditNotice(AddNotice addNotice)
        {
            try
            {
                var setting = _context.Settings.FirstOrDefault();
                var category = _context.Categorys.Find(addNotice.categoryId);

                var notice = _context.Notices.Find(addNotice.id);
                notice.title = addNotice.title;
                notice.lastPrice = Convert.ToInt64(addNotice.lastPrice.Replace(",", ""));
                notice.areaId = addNotice.areaId;
                notice.cityId = addNotice.cityId;
                notice.provinceId = addNotice.provinceId;
                notice.price = Convert.ToInt64(addNotice.price.Replace(",", ""));
                notice.categoryId = addNotice.categoryId;
                notice.description = addNotice.description;
                notice.link = addNotice.link;
                var allimages = _context.NoticeImages.Where(x => x.noticeId == addNotice.id).OrderBy(x => x.id).ToList();

                List<string> images = new List<string>();
                var notimage = addNotice.image;
                var noticecount = 0;
                if (notimage != null)
                {
                    noticecount = addNotice.image.Count();
                }


                for (int i = 0; i < noticecount; i++)
                {

                    var namefile = Guid.NewGuid().ToString().Replace('-', '0').Substring(0, 7) + System.IO.Path.GetExtension(addNotice.image[i].FileName).ToLower();
                    var filePath = System.IO.Path.Combine(environment.WebRootPath, "Notice/", namefile);

                    string e = System.IO.Path.GetExtension(namefile);
                    if (e == ".mp4")
                    {
                        notice.movie = "/Notice/" + namefile;
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            addNotice.image[i].CopyTo(stream);
                        }
                    }
                    else
                    {
                        if (String.IsNullOrEmpty(notice.image))
                        {
                            notice.image = "/Notice/" + namefile;
                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                addNotice.image[i].CopyTo(stream);
                            }
                            images.Add("/Notice/" + namefile);
                            string text = "Dakeh.Net";
                            using (Image originalImage = Image.FromStream(addNotice.image[i].OpenReadStream()))
                            {

                                using (Bitmap oBitmap = new Bitmap(originalImage))
                                {
                                    using (Graphics g = Graphics.FromImage(oBitmap))
                                    {

                                        Brush oBrush = new SolidBrush(Color.Green);
                                        Font oFont = new Font("ARial", 25, FontStyle.Bold, GraphicsUnit.Pixel);
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
                                addNotice.image[i].CopyTo(stream);
                            }
                            images.Add("/Notice/" + namefile);
                            string text = "Dakeh.Net";
                            using (Image originalImage = Image.FromStream(addNotice.image[i].OpenReadStream()))
                            {

                                using (Bitmap oBitmap = new Bitmap(originalImage))
                                {
                                    using (Graphics g = Graphics.FromImage(oBitmap))
                                    {

                                        Brush oBrush = new SolidBrush(Color.Green);
                                        Font oFont = new Font("ARial", 25, FontStyle.Bold, GraphicsUnit.Pixel);
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

                }
                //if (addNotice.image != null)
                //{
                //var namefile = Guid.NewGuid().ToString().Replace('-', '0').Substring(0, 7) + Path.GetExtension(addNotice.image.FileName).ToLower();
                //var filePath = Path.Combine(environment.WebRootPath, "Notice/", namefile);
                //string deletePath = environment.WebRootPath + notice.image;
                //if (System.IO.File.Exists(deletePath))
                //{
                //    System.IO.File.Delete(deletePath);
                //}
                //notice.image = "/Notice/" + namefile;
                //using (var stream = new FileStream(filePath, FileMode.Create))
                //{
                //    addNotice.image.CopyTo(stream);
                //}
                //}
                //if (addNotice.image_1 != null)
                //{
                //    var namefile = Guid.NewGuid().ToString().Replace('-', '0').Substring(0, 7) + Path.GetExtension(addNotice.image_1.FileName).ToLower();
                //    var filePath = Path.Combine(environment.WebRootPath, "Notice/", namefile);
                //    string deletePath = environment.WebRootPath + allimages.FirstOrDefault()?.image;
                //    _context.NoticeImages.Remove(allimages.FirstOrDefault());
                //    if (System.IO.File.Exists(deletePath))
                //    {
                //        System.IO.File.Delete(deletePath);
                //    }
                //    using (var stream = new FileStream(filePath, FileMode.Create))
                //    {
                //        addNotice.image_1.CopyTo(stream);
                //    }
                //    images.Add("/Notice/" + namefile);
                //}
                //if (addNotice.image_2 != null)
                //{
                //    var namefile = Guid.NewGuid().ToString().Replace('-', '0').Substring(0, 7) + Path.GetExtension(addNotice.image_2.FileName).ToLower();
                //    var filePath = Path.Combine(environment.WebRootPath, "Notice/", namefile);
                //    string deletePath = environment.WebRootPath + allimages.Skip(1)?.FirstOrDefault()?.image;
                //    _context.NoticeImages.Remove(allimages.Skip(1)?.FirstOrDefault());

                //    if (System.IO.File.Exists(deletePath))
                //    {
                //        System.IO.File.Delete(deletePath);
                //    }
                //    using (var stream = new FileStream(filePath, FileMode.Create))
                //    {
                //        addNotice.image_2.CopyTo(stream);
                //    }
                //    images.Add("/Notice/" + namefile);
                //}
                //if (addNotice.image_3 != null)
                //{
                //    var namefile = Guid.NewGuid().ToString().Replace('-', '0').Substring(0, 7) + Path.GetExtension(addNotice.image_3.FileName).ToLower();
                //    var filePath = Path.Combine(environment.WebRootPath, "Notice/", namefile);
                //    string deletePath = environment.WebRootPath + allimages.Skip(2)?.FirstOrDefault()?.image;
                //    _context.NoticeImages.Remove(allimages.Skip(2)?.FirstOrDefault());

                //    if (System.IO.File.Exists(deletePath))
                //    {
                //        System.IO.File.Delete(deletePath);
                //    }
                //    using (var stream = new FileStream(filePath, FileMode.Create))
                //    {
                //        addNotice.image_3.CopyTo(stream);
                //    }
                //    images.Add("/Notice/" + namefile);
                //}
                //if (addNotice.image_4 != null)
                //{
                //    var namefile = Guid.NewGuid().ToString().Replace('-', '0').Substring(0, 7) + Path.GetExtension(addNotice.image_4.FileName).ToLower();
                //    var filePath = Path.Combine(environment.WebRootPath, "Notice/", namefile);
                //    string deletePath = environment.WebRootPath + allimages.Skip(3)?.FirstOrDefault()?.image;
                //    _context.NoticeImages.Remove(allimages.Skip(3)?.FirstOrDefault());

                //    if (System.IO.File.Exists(deletePath))
                //    {
                //        System.IO.File.Delete(deletePath);
                //    }
                //    using (var stream = new FileStream(filePath, FileMode.Create))
                //    {
                //        addNotice.image_4.CopyTo(stream);
                //    }
                //    images.Add("/Notice/" + namefile);
                //}
                //if (addNotice.image_5 != null)
                //{
                //    var namefile = Guid.NewGuid().ToString().Replace('-', '0').Substring(0, 7) + Path.GetExtension(addNotice.image_5.FileName).ToLower();
                //    var filePath = Path.Combine(environment.WebRootPath, "Notice/", namefile);
                //    string deletePath = environment.WebRootPath + allimages.Skip(4)?.FirstOrDefault()?.image;
                //    _context.NoticeImages.Remove(allimages.Skip(4)?.FirstOrDefault());

                //    if (System.IO.File.Exists(deletePath))
                //    {
                //        System.IO.File.Delete(deletePath);
                //    }
                //    using (var stream = new FileStream(filePath, FileMode.Create))
                //    {
                //        addNotice.image_5.CopyTo(stream);
                //    }
                //    images.Add("/Notice/" + namefile);
                //}
                //if (addNotice.image_6 != null)
                //{
                //    var namefile = Guid.NewGuid().ToString().Replace('-', '0').Substring(0, 7) + Path.GetExtension(addNotice.image_6.FileName).ToLower();
                //    var filePath = Path.Combine(environment.WebRootPath, "Notice/", namefile);
                //    string deletePath = environment.WebRootPath + allimages.Skip(5)?.FirstOrDefault()?.image;
                //    _context.NoticeImages.Remove(allimages.Skip(5)?.FirstOrDefault());

                //    if (System.IO.File.Exists(deletePath))
                //    {
                //        System.IO.File.Delete(deletePath);
                //    }
                //    using (var stream = new FileStream(filePath, FileMode.Create))
                //    {
                //        addNotice.image_6.CopyTo(stream);
                //    }
                //    images.Add("/Notice/" + namefile);
                //}
                //if (addNotice.image_7 != null)
                //{
                //    var namefile = Guid.NewGuid().ToString().Replace('-', '0').Substring(0, 7) + Path.GetExtension(addNotice.image_7.FileName).ToLower();
                //    var filePath = Path.Combine(environment.WebRootPath, "Notice/", namefile);
                //    string deletePath = environment.WebRootPath + allimages.Skip(6)?.FirstOrDefault()?.image;
                //    _context.NoticeImages.Remove(allimages.Skip(6)?.FirstOrDefault());

                //    if (System.IO.File.Exists(deletePath))
                //    {
                //        System.IO.File.Delete(deletePath);
                //    }
                //    using (var stream = new FileStream(filePath, FileMode.Create))
                //    {
                //        addNotice.image_7.CopyTo(stream);
                //    }
                //    images.Add("/Notice/" + namefile);
                //}
                //if (addNotice.image_8 != null)
                //{
                //    var namefile = Guid.NewGuid().ToString().Replace('-', '0').Substring(0, 7) + Path.GetExtension(addNotice.image_8.FileName).ToLower();
                //    var filePath = Path.Combine(environment.WebRootPath, "Notice/", namefile);
                //    string deletePath = environment.WebRootPath + allimages.Skip(7)?.FirstOrDefault()?.image;
                //    _context.NoticeImages.Remove(allimages.Skip(7)?.FirstOrDefault());

                //    if (System.IO.File.Exists(deletePath))
                //    {
                //        System.IO.File.Delete(deletePath);
                //    }
                //    using (var stream = new FileStream(filePath, FileMode.Create))
                //    {
                //        addNotice.image_8.CopyTo(stream);
                //    }
                //    images.Add("/Notice/" + namefile);
                //}
                //if (addNotice.image_9 != null)
                //{
                //    var namefile = Guid.NewGuid().ToString().Replace('-', '0').Substring(0, 7) + Path.GetExtension(addNotice.image_9.FileName).ToLower();
                //    var filePath = Path.Combine(environment.WebRootPath, "Notice/", namefile);
                //    string deletePath = environment.WebRootPath + allimages.Skip(8)?.FirstOrDefault()?.image;
                //    _context.NoticeImages.Remove(allimages.Skip(8)?.FirstOrDefault());

                //    if (System.IO.File.Exists(deletePath))
                //    {
                //        System.IO.File.Delete(deletePath);
                //    }
                //    using (var stream = new FileStream(filePath, FileMode.Create))
                //    {
                //        addNotice.image_9.CopyTo(stream);
                //    }
                //    images.Add("/Notice/" + namefile);
                //}
                //if (addNotice.movie != null)
                //{
                //    var namefile = Guid.NewGuid().ToString().Replace('-', '0').Substring(0, 7) + Path.GetExtension(addNotice.movie.FileName).ToLower();
                //    var filePath = Path.Combine(environment.WebRootPath, "Notice/", namefile);

                //    notice.movie = "/Notice/" + namefile;
                //    using (var stream = new FileStream(filePath, FileMode.Create))
                //    {
                //        addNotice.movie.CopyTo(stream);
                //    }
                //}
                notice.adminConfirmStatus = EnumStatus.Pending;

                _context.Notices.Update(notice);
                foreach (var item in images)
                {
                    _context.NoticeImages.Add(new NoticeImage
                    {
                        noticeId = notice.id,
                        image = item
                    });
                }

                _context.SaveChanges();
            }
            catch (Exception ex)
            {

            }

            return RedirectToAction("Profile2");

        }
        public IActionResult GetMyNotice(int page)
        {
            var user1 = _context.Users.FirstOrDefault(x => x.cellphone == User.Identity.Name && x.deleted == null);
            User user = new User();
            if (user1 != null)
            {
                user = user1;
            }
            else
            {
                user = _context.Users.FirstOrDefault(x => x.cellphone + x.adminRole == User.Identity.Name && x.deleted == null);
            }
            IQueryable<Notice> result = _context.Notices.Where(x => x.userId == user.id && x.deletedAt == null);
            //foreach (var item in result)
            //{
            //    if (item.image == "")
            //    {
            //        item.image = "/images/empyty.png";
            //    }
            //}
            int skip = (page - 1) * 8;

            if (user != null)
            {

                List<Notice> res = result.OrderByDescending(u => u.createDate).Skip(skip).ToList();
                return PartialView("_MyNotice", res);
            }
            else
            {
                return RedirectToAction(nameof(Logout));
            }
        }
        public IActionResult GetMyFavorite(int page)
        {
            var user1 = _context.Users.FirstOrDefault(x => x.cellphone == User.Identity.Name && x.deleted == null);
            User user = new User();
            if (user1 != null)
            {
                user = user1;
            }
            else
            {
                user = _context.Users.FirstOrDefault(x => x.cellphone + x.adminRole == User.Identity.Name && x.deleted == null);
            }
            IQueryable<UserFavorite> result = _context.UserFavorites.Include(x => x.notice).Where(x => x.userId == user.id && x.notice.deletedAt == null);
            foreach (var item in result)
            {
                if (item.notice.image == "")
                {
                    item.notice.image = "/images/empyty.png";
                }
            }
            int skip = (page - 1) * 8;
            if (user != null)
            {
                List<UserFavorite> res = result.OrderByDescending(u => u.id).Skip(skip).Take(8).ToList();
                return PartialView("_MyFavoriteNotice", res);
            }
            else
            {
                return RedirectToAction(nameof(Logout));

            }

        }
        public IActionResult GetMyBanner(int page)
        {
            var user1 = _context.Users.FirstOrDefault(x => x.cellphone == User.Identity.Name && x.deleted == null);
            User user = new User();
            if (user1 != null)
            {
                user = user1;
            }
            else
            {
                user = _context.Users.FirstOrDefault(x => x.cellphone + x.adminRole == User.Identity.Name && x.deleted == null);
            }
            List<Banner> result = _context.Banner.Where(x => x.userId == user.id).Include(p=>p.BannerImage).ToList();
            
            int skip = (page - 1) * 8;
            if (user != null)
            {
                return PartialView("_MyBannner", result);
            }
            else
            {
                return RedirectToAction(nameof(Logout));

            }

        }
        [HttpPost]
        public IActionResult AddToFavorite(int id)
        {
            var user = _context.Users.FirstOrDefault(x => x.cellphone == User.Identity.Name && x.deleted == null);
            if (user == null)
            {
                return Json("UserNull");

            }
            if (_context.UserFavorites.Any(x => x.noticeId == id && x.userId == user.id))
            {
                return Json("warning");

            }
            else
            {
                var userFavorite = new UserFavorite();
                userFavorite.noticeId = id;
                userFavorite.userId = user.id;
                _context.UserFavorites.Add(userFavorite);
                _context.SaveChanges();
            }
            return Json("Success");
        }
        [HttpPost]
        public IActionResult AddToDestroy(int id, string message)
        {
            var user = _context.Users.FirstOrDefault(x => x.cellphone == User.Identity.Name && x.deleted == null);
            if (_context.ReportNotices.Any(x => x.noticeId == id && x.userId == user.id))
            {
                return Json("warning");

            }
            else
            {
                var ReportNotice = new ReportNotice();
                ReportNotice.noticeId = id;
                ReportNotice.userId = user.id;
                ReportNotice.message = message;
                _context.ReportNotices.Add(ReportNotice);
                _context.SaveChanges();
            }
            return Json("Success");
        }
        [HttpPost]
        public IActionResult LadderNotice(long laddernoticeid)
        {
            try
            {
                Factor factor = new Factor();
                var Notice = _context.Notices.Find(laddernoticeid);
                var user = _context.Users.Find(Notice.userId);
                var category = GetParent(Notice.categoryId);

                if (Notice.expireDate < DateTime.Now)
                {
                    TempData["ErrorPursheResult"] = "متاسفانه آگهی منقضی شده است ";

                    return View("Profile2");
                }
                if (category.laderPrice >= 10000)
                {
                    factor.state = State.IsPay;
                    factor.userId = user.id;
                    factor.createDatePersian = PersianCalendarDate.PersianCalendarResult(DateTime.Now);
                    factor.noticeId = Notice.id;
                    factor.factorKind = FactorKind.Ladder;
                    factor.totalPrice = category.laderPrice;
                    _context.Factors.Add(factor);
                    _context.SaveChanges();
                    PaymentRequestAttemp request = new PaymentRequestAttemp();
                    request.FactorId = factor.id;
                    request.NoticeId = Notice.id;
                    request.UserId = user.id;
                    request.pursheType = pursheType.Ladders;
                    _context.Add(request);
                    _context.SaveChanges();
                    var res = PaymentHelper.SendRequest(request.Id, category.laderPrice, "http://dakeh.net/Purshe/VerifyRequest");
                    if (res != null && res.Result != null)
                    {
                        if (res.Result.ResCode == "0")
                        {
                            Response.Redirect(string.Format("{0}/Purchase/Index?token={1}", PaymentHelper.PurchasePage, res.Result.Token));
                        }
                        ViewBag.Message = res.Result.Description;
                        return View("Profile2");
                    }
                }
                else
                {
                    Notice.createDate = DateTime.Now;
                    factor.state = State.IsPay;
                    factor.userId = user.id;
                    factor.createDatePersian = PersianCalendarDate.PersianCalendarResult(DateTime.Now);
                    factor.noticeId = Notice.id;
                    factor.factorKind = FactorKind.Ladder;
                    factor.totalPrice = category.laderPrice;
                    _context.Factors.Add(factor);
                    _context.SaveChanges();
                    TempData["PursheResult"] = $"کاربرگرامی ، عملیات نردبان کردن آگهی {Notice.title} با موفقیت انجام شد";
                    return RedirectToAction("Profile2");
                }
                return View("Profile2");
            }
            catch (Exception)
            {
                return Content("اکنون سیستم قادر به پاسخ گویی نمی باشد");
            }
        }

        [HttpPost]
        public async Task<dynamic> EmergencyNotice(long EmergencyNoticeid)
        {
            Factor factor = new Factor();
            var Notice = await _context.Notices.FindAsync(EmergencyNoticeid);

            if (Notice.expireDate < DateTime.Now)
            {
                TempData["ErrorPursheResult"] = "متاسفانه آگهی منقضی شده است ";

                return View("Profile2");

            }
            if (Notice.ExpireDateEmergency >= DateTime.Now && Notice.isEmergency)
            {
                TempData["ErrorPursheResult"] = " آگهی شما اظطراری می باشد ";

                return View("Profile2");
            }

            var setting = await _context.Settings.FirstOrDefaultAsync();
            var category = GetParent(Notice.categoryId);
            var user = await _context.Users.FindAsync(Notice.userId);
            if (category.emergencyPrice >= 10000)
            {
                factor.state = State.IsPay;
                factor.userId = user.id;
                factor.createDatePersian = PersianCalendarDate.PersianCalendarResult(DateTime.Now);
                factor.noticeId = Notice.id;
                factor.factorKind = FactorKind.Emergency;
                factor.totalPrice = category.emergencyPrice;
                _context.Factors.Add(factor);
                await _context.SaveChangesAsync();

                PaymentRequestAttemp request = new PaymentRequestAttemp();
                request.FactorId = factor.id;
                request.NoticeId = Notice.id;
                request.UserId = user.id;
                request.pursheType = pursheType.emergency;
                _context.Add(request);
                await _context.SaveChangesAsync();

                var res = PaymentHelper.SendRequest(request.Id, category.emergencyPrice, "http://dakeh.net/Purshe/VerifyRequest");
                if (res != null && res.Result != null)
                {
                    if (res.Result.ResCode == "0")
                    {
                        Response.Redirect(string.Format("{0}/Purchase/Index?token={1}", PaymentHelper.PurchasePage, res.Result.Token));
                    }
                    ViewBag.Message = res.Result.Description;
                    return View("Profile2");

                }
                return RedirectToAction("Profile2");
            }
            else
            {
                Notice.isEmergency = true;
                Notice.ExpireDateEmergency = DateTime.Now.AddDays(Convert.ToInt64(setting.countExpireDateIsespacial));
                factor.state = State.IsPay;
                factor.userId = user.id;
                factor.createDatePersian = PersianCalendarDate.PersianCalendarResult(DateTime.Now);
                factor.noticeId = Notice.id;
                factor.factorKind = FactorKind.Emergency;
                factor.totalPrice = category.emergencyPrice;
                _context.Factors.Add(factor);
                await _context.SaveChangesAsync();

                TempData["PursheResult"] = $"کاربر گرامی ، اکنون آگهی {Notice.title} جزو آگهی های اضطراری است ";
                return RedirectToAction("Profile2");
            }

        }

        [HttpPost]
        public IActionResult ExtendedNotice(long Extendnoticeid)
        {
            try
            {
                Factor factor = new Factor();
                var Notice = _context.Notices.Find(Extendnoticeid);


                if (Notice.expireDate >= DateTime.Now)
                {
                    TempData["ErrorPursheResult"] = " آگهی منقضی نشده است و نمیتوان تمدید کرد ";

                    return View("Profile2");
                }

                var setting = _context.Settings.FirstOrDefault();
                var category = GetParent(Notice.categoryId);
                var user = _context.Users.Find(Notice.userId);
                if (category.expirePrice >= 10000)
                {
                    factor.state = State.IsPay;
                    factor.userId = user.id;
                    factor.createDatePersian = PersianCalendarDate.PersianCalendarResult(DateTime.Now);
                    factor.noticeId = Notice.id;
                    factor.factorKind = FactorKind.Extend;
                    factor.totalPrice = category.expirePrice;
                    _context.Factors.Add(factor);
                    _context.SaveChanges();
                    PaymentRequestAttemp request = new PaymentRequestAttemp();
                    request.FactorId = factor.id;
                    request.NoticeId = Notice.id;
                    request.UserId = user.id;
                    request.pursheType = pursheType.Extend;
                    _context.Add(request);
                    _context.SaveChanges();
                    var res = PaymentHelper.SendRequest(request.Id, category.expirePrice, "http://dakeh.net/Purshe/VerifyRequest");
                    if (res != null && res.Result != null)
                    {
                        if (res.Result.ResCode == "0")
                        {
                            Response.Redirect(string.Format("{0}/Purchase/Index?token={1}", PaymentHelper.PurchasePage, res.Result.Token));
                        }
                        ViewBag.Message = res.Result.Description;
                        return View("Profile2");

                    }
                    return RedirectToAction("Profile2");
                }
                else
                {
                    Notice.expireDate = DateTime.Now.AddDays(Convert.ToInt64(setting.countExpireDate));
                    factor.state = State.IsPay;
                    factor.userId = user.id;
                    factor.createDatePersian = PersianCalendarDate.PersianCalendarResult(DateTime.Now);
                    factor.noticeId = Extendnoticeid;
                    factor.factorKind = FactorKind.Extend;
                    factor.totalPrice = category.expirePrice;
                    _context.Factors.Add(factor);
                    _context.SaveChanges();
                    var daysofextend = setting.countExpireDate == null ? 0 : setting.countExpireDate;
                    TempData["PursheResult"] = $"کاربر گرامی ، عملیات تمدید آگهی {Notice.title} به مدت {daysofextend} روز با موفقیت انجام شد";
                    return RedirectToAction("Profile2");
                }
            }
            catch (Exception)
            {
                return Content("اکنون سیستم قادر به پاسخ گویی نمی باشد");
            }
        }
        
        
        private Category GetParent(int CatId)
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
                        return categoryitem;
                    }
                }
            }
            return null;

        }
        
        [HttpGet]
        public object GetInfoOfProfile()
        {
            var user = _context.Users.FirstOrDefault(x => x.cellphone == User.Identity.Name && x.deleted == null);
            var provinces = _context.Provinces.OrderBy(x => x.name);
            string proContent = "<select class='title form-control selectpicker valid' data-size='5' data-val='true'  id='provinceId2' name='provinceId2' ><option value=''>همه شهرستان‌ها</option>";
            foreach (var item in provinces)
            {
                if (user != null)
                {

                    if (item.id == user.provinceId)
                        proContent += "<option selected value=" + item.id + ">" + item.name + "</option>";
                    else
                        proContent += "<option  value=" + item.id + ">" + item.name + "</option>";
                }
            }
            proContent += "</select>";

            bool isBlocked = false;
            if (user != null)
                 isBlocked = user.IsBlocked;

            var res = new { proContent, isBlocked  };
            return res;
        }

        [HttpGet]
        public object GetInfoOfFooter()
        {
            var aboutus= _context.AboutUss.ToList().FirstOrDefault();
            var contacts = _context.ContactUss.ToList().FirstOrDefault();
            //var footer = _context.Footers.ToList();

            var footer = new { contacts, aboutus?.description };
            return footer;
        }


        public IActionResult Logout()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Redirect("/Home2/Index");
        }
        [HttpPost]
        public IActionResult ChangeProOfUser(int id)
        {
            var user = _context.Users.FirstOrDefault(x => x.cellphone == User.Identity.Name && x.deleted == null);
            if (id > 0)
            {
                user.provinceId = id;
                _context.SaveChanges();
                return Json("Success");
            }
            else
            {
                return Json("Fail");

            }
        }
        
        [HttpPost]
        public IActionResult SpecialNotice(long Specialdnoticeid)
        {
            try
            {
                Factor factor = new Factor();
                var Notice = _context.Notices.Find(Specialdnoticeid);

                if (Notice.expireDate < DateTime.Now)
                {
                    TempData["ErrorPursheResult"] = "متاسفانه آگهی منقضی شده است ";

                    return View("Profile2");

                }

                if (Notice.expireDateIsespacial>=DateTime.Now && Notice.isSpecial)
                {
                    TempData["ErrorPursheResult"] = " آگهی شما ویژه می باشد ";

                    return View("Profile2");
                }

                var setting = _context.Settings.FirstOrDefault();
                var category = GetParent(Notice.categoryId);
                var user = _context.Users.Find(Notice.userId);

                if (category.espacialPrice >= 10000)
                {
                    factor.state = State.IsPay;
                    factor.userId = user.id;
                    factor.createDatePersian = PersianCalendarDate.PersianCalendarResult(DateTime.Now);
                    factor.noticeId = Notice.id;
                    factor.factorKind = FactorKind.Special;
                    factor.totalPrice = category.espacialPrice;
                    _context.Factors.Add(factor);
                    _context.SaveChanges();
                    PaymentRequestAttemp request = new PaymentRequestAttemp();
                    request.FactorId = factor.id;
                    request.NoticeId = Notice.id;
                    request.UserId = user.id;
                    request.pursheType = pursheType.Special;
                    _context.Add(request);
                    _context.SaveChanges();
                    var res = PaymentHelper.SendRequest(request.Id, category.espacialPrice, "http://dakeh.net/Purshe/VerifyRequest");
                    if (res != null && res.Result != null)
                    {
                        if (res.Result.ResCode == "0")
                        {
                            Response.Redirect(string.Format("{0}/Purchase/Index?token={1}", PaymentHelper.PurchasePage, res.Result.Token));
                        }
                        ViewBag.Message = res.Result.Description;
                        return View("Profile2");

                    }
                    return RedirectToAction("Profile2");
                }
                else
                {
                    Notice.isSpecial = true;
                    Notice.expireDateIsespacial = DateTime.Now.AddDays(Convert.ToInt64(setting.countExpireDateIsespacial));
                    factor.state = State.IsPay;
                    factor.userId = user.id;
                    factor.createDatePersian = PersianCalendarDate.PersianCalendarResult(DateTime.Now);
                    factor.noticeId = Notice.id;
                    factor.factorKind = FactorKind.Special;
                    factor.totalPrice = category.espacialPrice;
                    _context.Factors.Add(factor);
                    _context.SaveChanges();
                    var daysofextend = setting.countExpireDate == null ? 0 : setting.countExpireDate;
                    TempData["PursheResult"] = $"کاربر گرامی ، اکنون آگهی {Notice.title} جزو آگهی های ویژه است ";
                    return RedirectToAction("Profile2");
                }
            }
            catch (Exception)
            {
                return Content("اکنون سیستم قادر به پاسخ گویی نمی باشد");

            }
        }
        
        public IActionResult removeBanner(int Id)
        {
            var user1 = _context.Users.FirstOrDefault(x => x.cellphone == User.Identity.Name && x.deleted == null);
            User user = new User();
            if (user1 != null)
            {
                user = user1;
            }
            else
            {
                user = _context.Users.FirstOrDefault(x => x.cellphone + x.adminRole == User.Identity.Name && x.deleted == null);
            }
            var banner = _context.Banner.FirstOrDefault(p=>p.Id == Id);
            if(banner.userId == user.id)
            {
               
                var factor = _context.Factors.FirstOrDefault(p => p.bannerId == banner.Id);
                var factoritem = _context.FactorItems.FirstOrDefault(p => p.FactorId == factor.id);
                if(factoritem != null)
                {
                    _context.FactorItems.Remove(factoritem);
                    _context.SaveChanges();
                }
                if(factor != null)
                {
                    _context.Factors.Remove(factor);
                    _context.SaveChanges();
                }
                _context.Banner.Remove(banner);
                _context.SaveChanges();
                return Json("Success");
            }
            else
            {
                return RedirectToAction(nameof(Logout));
            }
        }


        [HttpPost]
        public IActionResult RemoveNotice(int Id)
        {
            if (Id == 0)
            {
                TempData["RemoveError"] = "این آگهی قابل حذف نیست";
                return Json("warning");
            }
            Notice noticeitem = _context.Notices.Where(s => s.id == Id).FirstOrDefault();

            if (noticeitem == null)
            {
                TempData["RemoveError"] = "این آگهی قابل حذف نیست";
                return Json("warning");

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
                var noticeImage = _context.NoticeImages.Where(x => x.noticeId == Id);
                foreach (var item in noticeImage)
                {
                    string deletePath = environment.WebRootPath + item.image;
                    if (System.IO.File.Exists(deletePath))
                    {
                        System.IO.File.Delete(deletePath);
                    }
                    _context.NoticeImages.Remove(item);
                }
                var listFactors = _context.Factors.Where(s => s.noticeId == Id).ToList();
                var listFavorits = _context.UserFavorites.Where(s => s.noticeId == Id).ToList();
                foreach (var itemFav in listFavorits)
                {
                    _context.UserFavorites.Remove(itemFav);
                }
                foreach (var item in listFactors)
                {
                    _context.Factors.Remove(item);
                }
                var visitnotice = _context.VisitNotices.Where(p => p.noticeId == noticeitem.id).ToList();
                _context.VisitNotices.RemoveRange(visitnotice);
                _context.SaveChanges();


                var visitnoticeuser = _context.VisitNoticeUsers.Where(p => p.noticeId == noticeitem.id).ToList();
                _context.VisitNoticeUsers.RemoveRange(visitnoticeuser);
                _context.SaveChanges();

                _context.Notices.Remove(noticeitem);
                _context.SaveChanges();
                return Json("Success");
            }
            catch (Exception ex)
            {
                TempData["RemoveError"] = "این آگهی قابل حذف نیست";
                return Json("warning");
            }

        }


        [HttpGet]
        public async Task<JsonResult> GetChildsCategories(int Id)
        {
            List<ComboBoxViewModel> items = new List<ComboBoxViewModel>();
            try
            {
                var categoryitems = await _context.Categorys.Where(s => s.parentCategoryId == Id).ToListAsync();
                if (categoryitems.Count() > 0)
                {
                    foreach (var item in categoryitems)
                    {
                        items.Add(new ComboBoxViewModel() { id = item.id, name = item.name });

                    }

                    return Json(new { success = true, list = items.ToList(), isfinal = false, parentid = Id });

                }
                else
                {
                    var catitem = await _context.Categorys.Where(s => s.id == Id).FirstOrDefaultAsync();
                    if (catitem != null)
                    {
                        items.Add(new ComboBoxViewModel() { id = catitem.id, name = catitem.name });

                    }
                    return Json(new { success = true, list = items.ToList(), isfinal = true, parentid = Id });

                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, responseText = "Faild To Get  Data" });
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetBackCategories(int Id)
        {
            bool isfinal = true;
            List<ComboBoxViewModel> items = new List<ComboBoxViewModel>();
            try
            {
                var categoriitem = await _context.Categorys.Where(s => s.id == Id).FirstOrDefaultAsync();
                if (categoriitem != null)
                {
                    var previouscategores = await _context.Categorys.Where(s => s.parentCategoryId == categoriitem.parentCategoryId).ToListAsync();


                    if (previouscategores.Count() > 0)
                    {
                        foreach (var item in previouscategores)
                        {
                            items.Add(new ComboBoxViewModel() { id = item.id, name = item.name, havenext = _context.Categorys.Any(s => s.parentCategoryId == item.id) });

                        }

                        foreach (var item2 in previouscategores)
                        {
                            if (item2.parentCategoryId != null)
                            {
                                isfinal = false;
                                break;
                            }

                        }


                        return Json(new { success = true, list = items.ToList(), isfinal = isfinal, parentid = categoriitem.parentCategoryId });
                    }

                }
                return Json(new { success = false, responseText = "Faild To Get  Data" });

            }
            catch (Exception)
            {
                return Json(new { success = false, responseText = "Faild To Get  Data" });
            }
        }

        [HttpGet]
        public async Task<JsonResult> MobileGetChildsCategories(int Id)
        {
            List<ComboBoxViewModel> items = new List<ComboBoxViewModel>();
            try
            {
                var categoryitems = await _context.Categorys.Where(s => s.parentCategoryId == Id).ToListAsync();
                if (categoryitems.Count() > 0)
                {
                    foreach (var item in categoryitems)
                    {
                        items.Add(new ComboBoxViewModel() { id = item.id, name = item.name, havenext = _context.Categorys.Any(s => s.parentCategoryId == item.id) });

                    }

                    return Json(new { success = true, list = items.ToList(), isfinal = false, parentid = Id });

                }
                else
                {
                    var catitem = await _context.Categorys.Where(s => s.id == Id).FirstOrDefaultAsync();
                    if (catitem != null)
                    {
                        items.Add(new ComboBoxViewModel() { id = catitem.id, name = catitem.name });

                    }
                    return Json(new { success = true, list = items.ToList(), isfinal = true, parentid = Id });

                }

            }
            catch (Exception ex)
            {
                return Json(new { success = false, responseText = "Faild To Get  Data" });
            }
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

        public async Task<JsonResult> GetSubCategorys(int Id)
        {
            List<ComboBoxViewModel> items = new List<ComboBoxViewModel>();
            try
            {
                var categoryitems = await _context.Categorys.Where(s => s.parentCategoryId == Id).ToListAsync();

                foreach (var item in categoryitems)
                {
                    items.Add(new ComboBoxViewModel() { id = item.id, name = item.name });

                }
                return Json(new { success = true, list = items.ToList() });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, responseText = "Faild To Get  Data" });
            }
        }
    }
}
