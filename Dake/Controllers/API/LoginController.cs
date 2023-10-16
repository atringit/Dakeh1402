using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dake.DAL;
using Dake.Utility;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Dake.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly Context _context;

        public LoginController(Context context)
        {
            _context = context;
        }
        [HttpPost]

        public object Login(RegisterUser user)
        {
            var data = _context.Users.IgnoreQueryFilters().Where(p => p.role.RoleNameEn == "Member"&&p.deleted == null).FirstOrDefault(p => p.cellphone == user.cellphone);
            if (data == null)
            {
                return new { status = 2, title = "خطای ورود", message = "چنین کاربری وجود ندارد." };
            }
            if (DateTime.Compare(data.oTPDate, DateTime.Now.AddMinutes(5))>0)
                return new { status = 3, title = "خطای ورود", message = "زمان ارسال کد تایید شما گذشته است." };
            if (data.code != user.code)
                return new { status = 4, title = "خطای ورود", message = "کد تایید شما نامعتبر است." };
            data.isCodeConfirmed = true;
            try
            {
                _context.SaveChanges();
            }
            catch(Exception ex)
            {
                return new { status = 5, title = "خطای ورود", message = "خطایی رخ داده است." };

            }
            return new { status = 1, title = "ورود", message = "خوش آمدید.", token = data.token,cityName=CityName(data.provinceId),provinceId=NullToInt(data.provinceId) };

        }
        private string CityName(int? provinceId)
        {
            if (provinceId == null)
                return "";
            else
            {
                var province = _context.Provinces.Find(provinceId);
                return province.name;
            }

        }
        private int NullToInt(int? provinceId)
        {
            if (provinceId == null)
                return 0;
            else
                return Convert.ToInt32(provinceId);
        }
        [HttpPost("LogOut")]
        public IActionResult LogOut()
        {
            string Token = HttpContext.Request?.Headers["Token"];
            var user = _context.Users.FirstOrDefault(p=>p.token == Token);
            user.token = null;
            _context.Users.Update(user);
            _context.SaveChanges();
            return Ok("LogOut Succsed");
        }
    }
}