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
using SmsIrRestfulNetCore;

namespace Dake.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegisterController : ControllerBase
    {
        private readonly Context _context;

        public RegisterController(Context context)
        {
            _context = context;
        }
        [HttpPost]

        public object Register(LoginUser user1)
        {

            //send sms

            User user;
            Random random = new Random();
            if (_context.Users.IgnoreQueryFilters().Where(p=>p.deleted == null).Any(p => p.cellphone == user1.cellphone ))
            {
                user = _context.Users.IgnoreQueryFilters().Where(p=>p.deleted == null).FirstOrDefault(p => p.cellphone == user1.cellphone);
                user.PushNotifToken = user1.PushNotifToken;

                user.code = random.Next(1000, 9999).ToString();
                user.oTPDate = DateTime.Now;
                try
                {
                    _context.SaveChanges();
                    SmsIrRestfulNetCore.Token tokenInstance = new SmsIrRestfulNetCore.Token();
                    //var token = tokenInstance.GetToken("dd7d9dcdb87acc4bcf71a1aa", "&konkour!!");
                    var token = tokenInstance.GetToken("3ca938cd34f116822bad458f", "Atrin2020");
                    var ultraFastSend = new UltraFastSend()
                    {
                        Mobile = Convert.ToInt64(user1.cellphone),
                        //TemplateId = 16765,
                        //TemplateId = 23305,
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
                        return new { status = 4, title = "خطای ارسال پیامک", message = "پیامک ارسال نشد." };
                    }
                }
                catch (Exception e)
                {
                    return new { status = 2, title = "خطا در ارسال کد تایید", message = "کد تایید شما ارسال نشد" };
                }
                return new { status = 1, title = "کد تایید", message = "کد تایید شما ارسال شد" };
            }
            user = new User
            {
                token = Guid.NewGuid().ToString().Replace('-', '0'),
                cellphone = user1.cellphone,
                code = random.Next(1000, 9999).ToString(),
                oTPDate = DateTime.Now,
                PushNotifToken = user1.PushNotifToken,
                Invite_Link = $"{random.Next(1000, 9999)}{user1.cellphone}{random.Next(1000, 9999)}",
                Invite_Price = 0
        };

            Role r = _context.Roles.Where(p => p.RoleNameEn == "Member").FirstOrDefault();
            user.role = r;
            _context.Users.Add(user);
            try
            {
                _context.SaveChanges();
                //send sms
                SmsIrRestfulNetCore.Token tokenInstance = new SmsIrRestfulNetCore.Token();
                //var token = tokenInstance.GetToken("dd7d9dcdb87acc4bcf71a1aa", "&konkour!!");
                var token = tokenInstance.GetToken("3ca938cd34f116822bad458f", "Atrin2020");
                var ultraFastSend = new UltraFastSend()
                {
                    Mobile = Convert.ToInt64(user1.cellphone),
                    TemplateId = 34584,
                    //TemplateId = 16765,
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
                    return new { status = 4, title = "خطای ارسال پیامک", message = "پیامک ارسال نشد." };
                }
            }
            catch (Exception e)
            {
                return new { status = 2, title = "خطا در ارسال کد تایید", message = "کد تایید شما ارسال نشد" };
            }
            return new { status = 1, title = "کد تایید", message = "کد تایید شما ارسال شد" };
        }


    }
}