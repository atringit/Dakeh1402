using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Dake.DAL;
using Dake.Models;
using Dake.ViewModel;

namespace Dake.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserInfoController : ControllerBase
    {
        private readonly Context _context;

        public UserInfoController(Context context)
        {
            _context = context;
        }

        // GET: api/Users
        //[HttpGet]
        //public IEnumerable<User> GetUsers()
        //{
        //    return _context.Users;
        //}

        // GET: api/Users/5
        [HttpGet]
        public IActionResult GetUser()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            string Token = HttpContext.Request?.Headers["Token"];
            var user = _context.Users.Where(p => p.token == Token).FirstOrDefault();
            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }
        [HttpPost]
        public object PutUser(UserProfileViewModel UserProfile)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            string token = HttpContext.Request?.Headers["Token"];

            var userEdit = _context.Users.FirstOrDefault(x => x.token == token);
            userEdit.provinceId = UserProfile.provinceId == 0 ? 1112 : UserProfile.provinceId;


            _context.Users.Update(userEdit);
            try
            {
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                if (!UserExists(token))
                {
                    return new { status = 2, title = "خطای ویرایش", message = "کاربر یافت نشد" };
                }
                else
                {
                    return new { status = 3, title = "خطای ویرایش", message = "خطایی رخ داده است." };
                }
            }
             return new { status = 1, title = " ویرایش", message = "ویرایش با موفقیت انجام شد." };
        }
        private bool UserExists(string token)
        {
            return _context.Users.Any(e => e.token == token);
        }

    }
}