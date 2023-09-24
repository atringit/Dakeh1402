using Dake.DAL;
using Dake.Service.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dake.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class JWTController : ControllerBase
    {
        private readonly Context _context;
        private readonly IDiscountCode _DiscountCode;

        public class JwtModel
        {
            public long TotalPrice { get; set; }
            public string Sku { get; set; }
        }

        [HttpPost]
        public async Task<string> GetToken([FromBody] JwtModel model)
        {
            string Token = HttpContext.Request?.Headers["Token"];
            var user = await _context.Users.Where(p => p.token == Token).FirstOrDefaultAsync();

            long disCountPrice = 0;
            string DiscountCode = HttpContext.Request?.Headers["DiscountCode"];


            if (!string.IsNullOrEmpty(DiscountCode))
            {
                var _code = Convert.ToInt32(DiscountCode);
                if (_DiscountCode.IsAlreadyUsed(user.id, _DiscountCode.GetIdByCode(_code)) == false && _DiscountCode.CheckCode(_code))
                {
                    _DiscountCode.AddUserToDiscountCode(user.id, _code);
                    disCountPrice = _DiscountCode.GetDiscountPrice(_code);
                }

            }


            string key = "5M3QGjnhVL1GU_pX1-boqRoKF9gWdSR4EMGYSqz_lIc";

            // Create Security key  using private key above:
            // not that latest version of JWT using Microsoft namespace instead of System
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));

            // Also note that securityKey length should be >256b
            // so you have to make sure that your private key has a proper length
            //
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha512);

            //  Finally create a Token
            var header = new JwtHeader(credentials);

            //Some PayLoad that contain information about the  customer
            var r = new Random();
            string timeStamp = GetTimestamp(DateTime.Now);

            var payload = new JwtPayload
           {
               { "price", model.TotalPrice },
               { "package_name", "com.dakeh.app"},
               { "sku", model.Sku },
               { "exp",  timeStamp },
               { "nonce", r.Next() }// not req
           };

            //
            var secToken = new JwtSecurityToken(header, payload);
            var handler = new JwtSecurityTokenHandler();

            // Token to String so you can use it in your client
            var tokenString = handler.WriteToken(secToken);



            // And finally when  you received token from client
            // you can  either validate it or try to  read
            //  var token = handler.ReadJwtToken(tokenString);

            return tokenString;
        }

        private static string GetTimestamp(DateTime value)
        {
            return value.ToString("yyyyMMddHHmmssffff");
        }
    }
}
