using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Dake.DAL;
using Dake.Models;
using Dake.Service.Interface;
using Newtonsoft.Json.Linq;
using Dake.Utility;
using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace Dake.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class LadderNoticeController : ControllerBase
    {
        private readonly Context _context;
        private IDiscountCode _DiscountCode;

        public LadderNoticeController(Context context, IDiscountCode discountCode)
        {
            _context = context;
            _DiscountCode = discountCode;

        }

        [HttpPost("{id}")]
        public object LadderNotice([FromRoute]long id)
        {
            var Notice = _context.Notices.Find(id);
            long totalPrice = 0;
            string DiscountCode = HttpContext.Request?.Headers["DiscountCode"];
            int _code = 0;
            long disCountPrice = 0;

            if (Notice==null)
            return new { status = 1, message = "چنین درخواستی یافت نشد." };
            Notice.createDate = DateTime.Now;
            var user = _context.Users.Find(Notice.userId);
            if (!string.IsNullOrEmpty(DiscountCode))
            {
                int n;
                if (int.TryParse(DiscountCode, out n))
                {
                    _code = Convert.ToInt32(DiscountCode);
                    if (_DiscountCode.IsAlreadyUsed(user.id, _DiscountCode.GetIdByCode(_code)) == false && _DiscountCode.CheckCode(_code))
                    {
                        _DiscountCode.AddUserToDiscountCode(user.id, _code);
                        disCountPrice = _DiscountCode.GetDiscountPrice(_code);

                    }
                }
            }
            var category = _context.Categorys.Find(Notice.categoryId);
            totalPrice = category.laderPrice - disCountPrice;
            if (totalPrice < 0)
            {
                totalPrice = 0;
            }

            Factor factor = new Factor
            {
                state = State.IsPay,
                userId = user.id,
                createDatePersian = PersianCalendarDate.PersianCalendarResult(DateTime.Now),
                noticeId = id,
                factorKind = FactorKind.Ladder,
                totalPrice = totalPrice
            };
            _context.Factors.Add(factor);
            _context.SaveChanges();
            return new { status = 1,title="نردبان آگهی", message = "نردبان آگهی با موفقیت انجام شد." };

        }
       
    }
}