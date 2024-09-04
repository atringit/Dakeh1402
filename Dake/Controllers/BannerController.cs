using Dake.DAL;
using Dake.Models;
using Dake.Service;
using Dake.Service.Common;
using Dake.Service.Interface;
using Dake.Utility;
using Dake.ViewModel;
using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Vml;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Dake.Controllers
{
    [Authorize]
    public class BannerController : Controller
    {
        private readonly IBannerSevice _repository;
        private readonly Context _context;
        private IDiscountCode _IDiscountCode;
        private readonly IPaymentService _paymentService;

        public BannerController(IBannerSevice repository, Context context, IDiscountCode IDiscountCode, IPaymentService paymentService)
        {
            _repository = repository;
            _context = context;
            _IDiscountCode = IDiscountCode;
            _paymentService = paymentService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int page , string search)
        {
            return View(await _repository.GetAllData(page , search));
        }

        [HttpGet]
        public async Task<IActionResult> BannerGetById(long id)
        {
            var response = await _repository.GetBannerById(id);

            var result = response.BannerImage.Select(p => new
            {
                p.Id,
                p.Name,
                p.FileLocation,
                p.BannerId
            }).ToList();

            return Json(result);
        }

        [HttpPost]
        public async Task AddOrUpdate(Banner dto, IList<IFormFile> files)
        {
            
            await _repository.AddOrUpdate(dto, files);
           
        }

         [HttpPost]
        public async Task Accepted(Banner dto)
        {
            var curentUser = HttpContext.User.FindFirst(ClaimTypes.MobilePhone).Value; // کاربر جاری
                                                                                       //var userId = _context.Users.Where(w => w.cellphone == curentUser).Select(s => s.id).FirstOrDefault();

            dto.AdminUserAccepted = curentUser;
            await _repository.Accepted(dto);
        }



        public IActionResult CreateBanner()
        {

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddBanner(Banner banner, IList<IFormFile> files)
        {
           
            try
            {
                var banners = _context.Banner.Where(p => p.expireDate >= DateTime.Now && p.adminConfirmStatus == EnumStatus.Accept).Include(p => p.BannerImage).ToList();
                if (banners.Count >= 10)
                {
					return BadRequest("فعلا ظرفیت تکمیل است");
				}
				var setting = _context.Settings.FirstOrDefault();
                banner.expireDate = DateTime.Now.AddDays(Convert.ToInt64(setting.countExpireDate));
				if (files == null || banner.Link == null || banner.title == null)
                {
                    return BadRequest("پر کردن تمامی فیلد ها و ثبت تصویر اجباری است");
                }
                
                var user = _context.Users.FirstOrDefault(x => x.cellphone == User.Identity.Name && x.deleted == null);
                if (user == null)
                {
                    var user2 = _context.Users.FirstOrDefault(x => x.cellphone+x.adminRole == User.Identity.Name && x.deleted == null);
                    banner.user = user2;
                }
                else
                {
                    banner.user = user;
                }
                var bres = await _repository.AddTempBanner(banner, files);

                if (bres.IsSuccess)
                {
                    var bannerPrice = await _context.StaticPrices.Where(w => w.code == "Add_Banner").FirstOrDefaultAsync();

                    int total = (int)bannerPrice.price;

                    if (user != null && user.Invite_Price != 0)
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

                    var bannerId = long.Parse(bres.Data.ToString());

                    Factor factor = new Factor();
                    factor.state = State.NotPay;
                    factor.userId = banner.user.id;
                    factor.createDatePersian = PersianCalendarDate.PersianCalendarResult(DateTime.Now);
                    factor.factorKind = FactorKind.Add;
                    factor.bannerId = bannerId;
                    factor.totalPrice = total;
                    
                    //factor.totalPrice = havediscount ? category.registerPrice - discountprice : category.registerPrice;
                    _context.Factors.Add(factor);
                    //Payment
                    await _context.SaveChangesAsync();

                    if (factor.totalPrice >= 0)
                    {
                        var attempt = new PaymentRequestAttemp
                        {
                            FactorId = factor.id,
                            NoticeId = bannerId,
                            UserId = factor.userId,
                            pursheType = pursheType.RegisterNotice,
                        };

                        await _paymentService.AddPaymentAttempt(attempt);

                        var connectGatewayRequest = new PaymentConnectModel
                        {
                            FactorId = factor.id,
                            Amount = total,
                            ReturnUrl = $"{Request.Scheme}://{Request.Host}/Payments/Banner/{factor.id}",
                            UserMobile = user?.cellphone ?? banner.user.cellphone,
                        };
                        
                        var paymentResponse = await _paymentService.ConnectGateway(connectGatewayRequest);

                        if (paymentResponse.Succeeded)
                        {
                            return Ok(paymentResponse.GatewayUrl);
                        }
                        else
                        {
                            return BadRequest(error: paymentResponse.Error);
                        }
                    }
                    else
                    {
                        await _repository.ConfirmBanner(banner);
                    }
                }
                return Ok();
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
            
        }

        [HttpPost("v2/api/AddBanner")]
        [AllowAnonymous]
        public async Task<IActionResult> AddBannerWeb([FromForm] Banner banner, [FromForm] IList<IFormFile> files)
        {

            try
            {
                string token = HttpContext.Request?.Headers["Token"];

                if (token == null)
                {
                    return Unauthorized();
                }

                var banners = _context.Banner.Where(p => p.expireDate >= DateTime.Now && p.adminConfirmStatus == EnumStatus.Accept).Include(p => p.BannerImage).ToList();
                if (banners.Count >= 10)
                {
                    return BadRequest(new { message = "فعلا ظرفیت تکمیل است" });
                }

                var user = _context.Users.FirstOrDefault(x => x.token == token);

                if (user == null)
                {
                    return Unauthorized();
                }

                banner.user = user;
                var bres = await _repository.AddTempBanner(banner, files);

                if (!bres.IsSuccess)
                {
                    return BadRequest(new { message = "خطا در ثبت بنر" });
                }

                var bannerPrice = await _context.StaticPrices.Where(w => w.code == "Add_Banner").FirstOrDefaultAsync();

                int total = (int)bannerPrice.price;

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

                if (factor.totalPrice >= 0)
                {
                    var attempt = new PaymentRequestAttemp
                    {
                        FactorId = factor.id,
                        NoticeId = bannerId,
                        UserId = factor.userId,
                        pursheType = pursheType.RegisterNotice,
                    };

                    await _paymentService.AddPaymentAttempt(attempt);

                    var connectGatewayRequest = new PaymentConnectModel
                    {
                        FactorId = factor.id,
                        Amount = total,
                        ReturnUrl = $"{Request.Scheme}://{Request.Host}/Payments/BannerWeb/{factor.id}",
                        UserMobile = user?.cellphone ?? banner.user.cellphone,
                    };

                    var paymentResponse = await _paymentService.ConnectGateway(connectGatewayRequest);

                    if (paymentResponse.Succeeded)
                    {
                        return Ok(new { redirectLink = paymentResponse.GatewayUrl });
                    }
                    else
                    {
                        return BadRequest(new { message = paymentResponse.Error });
                    }
                }
                else
                {
                    await _repository.ConfirmBanner(banner);
                }
            }
            catch (Exception ex)
            {
                return UnprocessableEntity(new { message = "اکنون سیستم قادر به پاسخ گویی نمی باشد" });
            }
            return Ok(new { status = 1, message = "آگهی شما با موفقیت ثبت گردید." });

        }

        [HttpDelete]
        public async Task DeleteBanner(long id)
        {
            await _repository.DeleteBanner(id);
        }


        [HttpDelete]
        public async Task DeleteBannerImage(long id)
        {
            await _repository.DeleteBannerImage(id);
        }

    }
}
