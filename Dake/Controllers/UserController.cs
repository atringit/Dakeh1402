using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Dake.DAL;
using Dake.Utility;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Dake.Service.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using Dake.Models;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SmsIrRestfulNetCore;
using ClosedXML.Excel;
using Dake.Service.Common;
using Jumbula.WebSite.Utilities.Captcha;
using DocumentFormat.OpenXml.Bibliography;
using System.Data;

namespace Dake.Controllers
{
    public class UserController : Controller
    {
        private readonly Context _context;
        private IUser _user;
        private ICity _city;

        public UserController(Context context, IUser user, ICity city)
        {
            _context = context;
            _user = user;
            _city = city;
        }

        #region
        public IActionResult Index(int page = 1, string filtercellphone = "" , int CityId = 0)
        {
            var model = _user.GetUsers(page, filtercellphone, CityId);
            ViewData["Cities"] = _context.Cities.ToList();
            return View(model);
        }
        //[Route("User/Login")]
        public ActionResult Login(string Message)
        {
            ViewBag.Error = Message;
            return View();
        }


        [HttpPost]

        public ActionResult SignIn(LoginUserAdmin user)
        {
            //var responseCaptcha = Request.Form["g-recaptcha-response"];

            //CaptchaResult captchaResult = GoogleCaptcha.ValidateCaptcha(responseCaptcha, CaptchaConstant.SecretKey);
            //if (!captchaResult.Status)
            //{
            //    return RedirectToAction("Login", new { Message = "Captcha Error" });
            //}

            if (user.password == "" || user.password == null || user.cellphone == "" || user.cellphone == null)
            {
                ModelState.Clear();
                ModelState.AddModelError("", "شماره همراه یا رمز عبور صحیح نیست");
                return View("Login");
            }
            var u = _context.Users.Include(x => x.role).Where(p => p.cellphone == user.cellphone && p.role.RoleNameEn == "Admin" && p.deleted == null).FirstOrDefault();
            if (u == null)
            {
                ModelState.Clear();
                ModelState.AddModelError("", "شماره همراه یا رمز عبور صحیح نیست");
                return View("Login");
            }
            if (!BCrypt.Net.BCrypt.Verify(user.password, u.password))
            {
                ModelState.Clear();
                ModelState.AddModelError("", "نام کاربری یا رمز عبور صحیح نیست");
                return View("Login");
            }

            var claims = new List<Claim>()
                    {
                        new Claim(ClaimTypes.NameIdentifier,user.cellphone.ToString()+u.adminRole),
                        new Claim(ClaimTypes.Name,user.cellphone+u.adminRole),
                        new Claim(ClaimTypes.MobilePhone,user.cellphone),
                        //new Claim("UserId",_user.sele)
                    };
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            var properties = new AuthenticationProperties
            {
                IsPersistent = user.RememberMe
            };
            HttpContext.SignInAsync(principal, properties);
            return RedirectToAction("Index", "Home");



        }

        private void SetCaptcha(Captcha captcha, string captchaName)
        {
            this.Response.Cookies.Append(captchaName, JsonConvert.SerializeObject(captcha, Formatting.None, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() }));
        }

        private void DeleteCaptcha(string captchaName)
        {
            this.Response.Cookies.Delete(captchaName);
        }
        #region Logout
        [Route("Logout")]
        public IActionResult Logout()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Redirect("/User/Login");
        }

        #endregion

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        public IActionResult AdminUser()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AdminUser(UserAdmin userAdmin)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var user = _context.Users.Include(x => x.role).FirstOrDefault(x => x.role.RoleNameEn == "Admin" && x.passwordShow == userAdmin.passwordShow);
                    if (user == null)
                    {
                        ModelState.AddModelError("passwordShow", "رمز عبور فعلی نا معتبر است");
                        return View();
                    }
                    else
                    {
                        user.passwordShow = userAdmin.newPasswordShow;
                        user.password = BCrypt.Net.BCrypt.HashPassword(userAdmin.newPasswordShow, BCrypt.Net.BCrypt.GenerateSalt());
                        await _context.SaveChangesAsync();
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    throw;
                }
                ViewData["passwordChanged"] = "رمز عبور تغییر یافت";
                return View();
            }
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, User user)
        {
            if (id != user.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {

                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {

                    throw;

                }
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        [HttpPost]
        public IActionResult InActive(int id)
        {
            try
            {
                var user = _context.Users.IgnoreQueryFilters().FirstOrDefault(x => x.id == id);
                //if (user.isActive == true)
                //    user.isActive = false;
                //else
                //    user.isActive = true;

                _context.SaveChanges();
                return Json("Done");
            }
            catch (Exception ex)
            {
                return Json("Fail");
            }
        }

        [HttpPost]
        public IActionResult Blocked(int id)
        {
            try
            {
                var user = _context.Users.IgnoreQueryFilters().FirstOrDefault(x => x.id == id);
                if (user.IsBlocked == true)
                {
                    user.IsBlocked = false;
                    CommonService.SendSMS(user.cellphone, 78619);

                }
                else
                {
                    user.IsBlocked = true;
                    CommonService.SendSMS(user.cellphone, 78618);
                }
                _context.SaveChanges();
                return Json("Done");
            }
            catch (Exception ex)
            {
                return Json("Fail");
            }
        }



        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = _context.Users.Where(p=>p.isCodeConfirmed == true || p.isCodeConfirmed == false).FirstOrDefault(p=>p.id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }
        [HttpPost]
        public IActionResult Delete(int id)
        {
            try
            {
                var user = _context.Users.FirstOrDefault(x => x.id == id);
                user.deleted = "delete";
                _context.Users.Update(user);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));

            }
            catch (Exception ex)
            {
                return RedirectToAction(nameof(Index));
            }
        }



        public IActionResult UserNotice(int id)
        {
            var userItem = _context.Users.IgnoreQueryFilters().Where(s => s.id == id).FirstOrDefault();
            if (userItem != null)
            {
                ViewBag.UserName = userItem.cellphone;
                var NoticeItems = _context.Notices.Include(s => s.category).Include(s => s.city).Where(s => s.userId == userItem.id).ToList();
                return View(NoticeItems);
            }
            else
            {
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        public JsonResult SendSms(string txtsms, int UserId)
        {
            var userItem = _context.Users.IgnoreQueryFilters().FirstOrDefault(s => s.id == UserId);
            if (userItem != null)
            {
                SmsIrRestfulNetCore.Token tokenInstance2 = new SmsIrRestfulNetCore.Token();
                var token2 = tokenInstance2.GetToken("3ca938cd34f116822bad458f", "Atrin2020");
                var ultraFastSend2 = new UltraFastSend()
                {
                    Mobile = Convert.ToInt64(userItem.cellphone),
                    TemplateId = 78617,
                    ParameterArray = new List<UltraFastParameters>()
                      {
                    new UltraFastParameters()
                          {
                          Parameter = "Message" , ParameterValue =txtsms

                           }
                         }.ToArray()
                };

                UltraFastSendRespone ultraFastSendRespone2 = new UltraFast().Send(token2, ultraFastSend2);
            }
            return Json("ok");
        }

        [HttpPost]
        public IActionResult Print(int CityId)
        {


            string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            string fileName = "authors.xlsx";
            var builder = new System.Text.StringBuilder();
            var Userlist = _context.Users.Include(s => s.province).ThenInclude(s => s.city).IgnoreQueryFilters().ToList();
            using (var workbook = new XLWorkbook())
            {
                IXLWorksheet worksheet =
                workbook.Worksheets.Add("Authors");
                worksheet.Cell(1, 1).Value = "شماره همراه";
                worksheet.Cell(1, 2).Value = "شهر";

                for (int index = 1; index <= Userlist.Count; index++)
                {
                    worksheet.Cell(index + 1, 1).Value =
                    Userlist[index - 1].cellphone;
                    if(Userlist[index - 1].province != null)
                    {
                        if(Userlist[index - 1].province.city != null)
                        {
							worksheet.Cell(index + 1, 2).Value =
						    Userlist[index - 1].province.city.name;
						}
						
					}
                    

                }
                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content, contentType, fileName);
                }
            }
        }
        #endregion


        public IActionResult ChangePassword(string Message)
        {
            ViewBag.Error = Message;
            return View();

        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePassModel model)
        {
            if (string.IsNullOrWhiteSpace(model.OldPassword) || string.IsNullOrWhiteSpace(model.NewPassword))
            {
                ModelState.Clear();
                return RedirectToAction("ChangePassword", new { Message = "اطلاعات نمیتواند خالی باشد" });
            }
            var user = _context.Users.Include(x => x.role).FirstOrDefault(x => x.role.RoleNameEn == "Admin" && x.passwordShow == model.OldPassword);

            if (user == null)
            {
                ModelState.Clear();
                return RedirectToAction("ChangePassword", new { Message = "اطلاعات صحیح نیست" });

            }
            user.passwordShow = model.NewPassword;
            user.password = BCrypt.Net.BCrypt.HashPassword(model.NewPassword, BCrypt.Net.BCrypt.GenerateSalt());
            await _context.SaveChangesAsync();
            return View();

        }
        public class ChangePassModel
        {
            public string OldPassword { get; set; }
            public string NewPassword { get; set; }
        }
        [HttpPost("SetUserPrice/{id}/{price}")]
        public IActionResult SetUserPrice(int id,int price)
        {
            var user = _context.Users.Find(id);
            if (user == null)
            {
                return NotFound();
            }
            user.Invite_Price = price;
            _context.Users.Update(user);
            _context.SaveChanges();
            return Ok();
        }
    }
}