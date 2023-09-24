using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dake.DAL;
using Dake.Models;
using Dake.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Dake.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegisterUserController : ControllerBase
    {
        private readonly Context _context;

        public RegisterUserController(Context context)
        {
            _context = context;
        }


        [HttpPost]

        public object RegisterUser(LoginUser user1)
        {
            var user = new User();

            if (_context.Users.Any(p => p.cellphone == user1.cellphone))
            {
                user = _context.Users.FirstOrDefault(p => p.cellphone == user1.cellphone);
                return new { status = 0, title = "ورود", message = "خوش آمدید.", token = user.token };

            }
            user.token = Guid.NewGuid().ToString().Replace('-', '0');
            user.cellphone = user1.cellphone;
            Random random = new Random();
            Role r = _context.Roles.Where(p => p.RoleNameEn == "Member").FirstOrDefault();
            user.role = r;
            _context.Users.Add(user);
            try
            {
                _context.SaveChanges();
                //send sms
            }
            catch (Exception e)
            {
                throw;
            }
            return new { status = 0, title = "ورود", message = "خوش آمدید.", token = user.token };


        }
    }
}