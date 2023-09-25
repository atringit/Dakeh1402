using Dake.DAL;
using Dake.Models;
using Dake.Service;
using Dake.Service.Common;
using Dake.Service.Interface;
using Dake.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public BannerController(IBannerSevice repository,Context context, IDiscountCode IDiscountCode)
        {
            _repository = repository;
            _context = context;
            _IDiscountCode = IDiscountCode;
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
                var user = _context.Users.FirstOrDefault(x => x.cellphone == User.Identity.Name);
                if (user == null)
                {
                    var user2 = _context.Users.FirstOrDefault(x => x.cellphone+x.adminRole == User.Identity.Name);
                    banner.user = user2;
                }
                else
                {
                    banner.user = user;
                }
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
                                    Response.Redirect(string.Format("{0}/Purchase/Index?token={1}", PaymentHelper.PurchasePage, res.Result.Token));
                                }
                                ViewBag.Message = res.Result.Description;
                                return View("CreateBanner"); ;
                            }
                        }
                        catch (Exception)
                        {
                            ViewBag.Message = "امکان اتصال به درگاه بانکی وجود ندارد";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
            return RedirectToAction("CreateBanner");
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
