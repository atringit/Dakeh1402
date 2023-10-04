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

namespace Dake.Controllers
{
    public class User2Controller : Controller
    {
        private readonly Context _context;
        private IUser _user;
        //private readonly UserManager<User> _userManager;
        public User2Controller(Context context, IUser user)
        //,UserManager<User> userManager)
        {
            //_userManager = userManager;
            _context = context;
            _user = user;
        }
        [HttpPost]
        public ActionResult Register(LoginUser user1)
        {
            var user = new User();
            Random random = new Random();
            if (_context.Users.IgnoreQueryFilters().Where(p=> p.deleted == null).Any(p => p.cellphone == user1.cellphone))
            {
                user = _context.Users.IgnoreQueryFilters().Where(p=> p.deleted == null).FirstOrDefault(p => p.cellphone == user1.cellphone);
                user.code = random.Next(1000, 9999).ToString();
                user.oTPDate = DateTime.Now;
                try
                {
                    _context.SaveChanges();
                    SmsIrRestfulNetCore.Token tokenInstance = new SmsIrRestfulNetCore.Token();
                    var token = tokenInstance.GetToken("3ca938cd34f116822bad458f", "Atrin2020");
                    var ultraFastSend = new UltraFastSend()
                    {
                        Mobile = Convert.ToInt64(user1.cellphone),
                        //TemplateId = 16765,
                        TemplateId = 34584,
                        ParameterArray = new List<UltraFastParameters>()
                      {
                    new UltraFastParameters()
                          {
                          Parameter = "VerificationCode" , ParameterValue = user.code

                           }
                         }.ToArray()

                    };

                    UltraFastSendRespone ultraFastSendRespone = new UltraFast().Send(token, ultraFastSend);

                    if (!ultraFastSendRespone.IsSuccessful)
                    {
                        return Json("FailTosendSMS");
                    }
                }
                catch (Exception e)
                {
                    return Json("FailTosendSMS");

                }
                return Json("success");

            }
            user.token = Guid.NewGuid().ToString().Replace('-', '0');
            user.cellphone = user1.cellphone;
            user.code = random.Next(1000, 9999).ToString();
            user.oTPDate = DateTime.Now;
            Role r = _context.Roles.Where(p => p.RoleNameEn == "Member").FirstOrDefault();
            user.role = r;
            SmsIrRestfulNetCore.Token tokenInstance2 = new SmsIrRestfulNetCore.Token();
            var token2 = tokenInstance2.GetToken("3ca938cd34f116822bad458f", "Atrin2020");
            var ultraFastSend2 = new UltraFastSend()
            {
                Mobile = Convert.ToInt64(user1.cellphone),
                //TemplateId = 16765,
                TemplateId = 34584,
                ParameterArray = new List<UltraFastParameters>()
                      {
                    new UltraFastParameters()
                          {
                          Parameter = "VerificationCode" , ParameterValue = user.code

                           }
                         }.ToArray()

            };

            UltraFastSendRespone ultraFastSendRespone2 = new UltraFast().Send(token2, ultraFastSend2);

            if (!ultraFastSendRespone2.IsSuccessful)
            {
                return Json("FailTosendSMS");
            }
            user.Invite_Link = $"{random.Next(1000, 9999)}{user1.cellphone}{random.Next(1000, 9999)}";
            user.Invite_Price = 0;
            _context.Users.Add(user);
            _context.SaveChanges();

            return Json("success");
        }
        [HttpPost]
        public ActionResult Login(RegisterUser user)
        {
            var data = _context.Users.IgnoreQueryFilters().Where(p => p.role.RoleNameEn == "Member" && p.deleted == null).FirstOrDefault(p => p.cellphone == user.cellphone);
            if (data == null)
            {
              return Json("FailToLogin");
            }
           
            if (data.code != user.code)
              return Json("FailCode");
                
            data.isCodeConfirmed = true;
            try
            {
                _context.SaveChanges();
            }
            catch(Exception ex)
            {
              return Json("FailToLogin");

            }
             var claims = new List<Claim>()
                    {
                        new Claim(ClaimTypes.NameIdentifier,user.cellphone.ToString()),
                        new Claim(ClaimTypes.Name,user.cellphone)
                    };
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            var properties = new AuthenticationProperties
            {
                IsPersistent = true
            };
            HttpContext.SignInAsync(principal, properties);
            return Json("success");

        }


   
    }
}