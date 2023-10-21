using Dake.DAL;
using Dake.Models;
using Dake.Service;
using Dake.Service.Common;
using Dake.Service.Interface;
using Dake.Utility;
using DocumentFormat.OpenXml.Drawing;
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
                var bres = _repository.AddBanner(banner, files).Result;

                if (bres.IsSuccess)
                {
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
                        try
                        {
                            PaymentRequestAttemp request = new PaymentRequestAttemp();
                            request.FactorId = factor.id;
                            request.NoticeId = bannerId;
                            request.UserId = user.id;
                            request.pursheType = pursheType.RegisterNotice;

                            _context.Add(request);
                            _context.SaveChanges();


                            

                            var pyment = new Zarinpal.Payment("ceb42ad1-9eb4-47ec-acec-4b45c9135122", total);
                            var res = pyment.PaymentRequest($"پرداخت فاکتور شمارهی {factor.id}", "https://localhost:5001/Payments/Banner/" + factor.id, null, user.cellphone);
                            if (res != null && res.Result != null)
                            {
                                if (res.Result.Status == 100)
                                {
                                    var redi ="https://sandbox.zarinpal.com/pg/StartPay/" + res.Result.Authority;

                                    //var n = _context.Notices.FirstOrDefault(p => p.id == notice.id);
                                    //n.isPaid = true;
                                    //_context.Notices.Update(n);
                                    //_context.SaveChanges();
                                    //if (havediscount)
                                    //{
                                    //    _IDiscountCode.AddUserToDiscountCode(user.id, _code);
                                    //}
                                    //Response.Redirect(string.Format("{0}/Purchase/Index?token={1}", PaymentHelper.PurchasePage, res.Result.Token));
                                    return Ok(redi);
                                }
                                else
                                {
                                    ViewBag.Message = "امکان اتصال به درگاه بانکی وجود ندارد";
                                }
                            }
                        }
                        catch (Exception)
                        {
                            ViewBag.Message = "امکان اتصال به درگاه بانکی وجود ندارد";
                        }
                    }
                }
                return Ok();
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
            
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
