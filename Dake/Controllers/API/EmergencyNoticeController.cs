using Dake.DAL;
using Dake.Models;
using Dake.Models.ApiDto;
using Dake.Service.Interface;
using Dake.Utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Dake.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmergencyNoticeController : ControllerBase
    {
        private readonly Context _context;
        private readonly IDiscountCode _DiscountCode;

        public EmergencyNoticeController(Context context, IDiscountCode discountCode)
        {
            _context = context;
            _DiscountCode = discountCode;


        }

        [HttpGet]

        public async Task<IActionResult> EmergencyNotice()
        {
            string Token = HttpContext.Request?.Headers["Token"];
            var user = await _context.Users.Where(p => p.token == Token).FirstOrDefaultAsync();

            var data = await _context.Notices
                .Where(x=> x.isEmergency && x.ExpireDateEmergency> DateTime.Now 
                && x.expireDate >= DateTime.Now && x.adminConfirmStatus == EnumStatus.Accept && x.provinceId == user.provinceId)
                .Include(s => s.category).ToListAsync();

            return new JsonResult(data);
        }

     
        

        [HttpPost]
        public async Task<object> SetEmergencyNotice([FromBody] SetEmergencyNoticeDto model)
        {
            string Token = HttpContext.Request?.Headers["Token"];
            var user = await _context.Users.Where(p => p.token == Token).FirstOrDefaultAsync();

            var data = await _context.Notices.Where(x => x.userId == user.id).ToListAsync();


            var Notice = _context.Notices.Find(model.id);
            int _code = 0;
            long totalPrice = 0;
            long disCountPrice = 0;
            string DiscountCode = HttpContext.Request?.Headers["DiscountCode"];

            if (Notice == null)
                return new { status = 1, message = "چنین درخواستی یافت نشد." };

            if (Notice.isEmergency && Notice.ExpireDateEmergency>= DateTime.Now)
                return new { status = 1, message = "آگهی اضطراری می باشد" };

            var setting = _context.Settings.FirstOrDefault();
            int countExpire = 0;
            if (setting != null)
                countExpire = Convert.ToInt32(setting.countExpireDateEmergency);

            Notice.ExpireDateEmergency = DateTime.Now.AddDays(countExpire);
            Notice.isEmergency = true;

            if (!string.IsNullOrEmpty(DiscountCode))
            {
                int n;
                if (int.TryParse(DiscountCode, out n))
                {
                    _code = Convert.ToInt32(DiscountCode);
                    if (_DiscountCode.IsAlreadyUsed(user.id, _DiscountCode.GetIdByCode(_code)) == false && _DiscountCode.CheckCode(_code))
                    {
                        _DiscountCode.AddUserToDiscountCode(user.id, _code);
                        disCountPrice =  _DiscountCode.GetDiscountPrice(_code);
                    }
                }
            }


            var category = _context.Categorys.Find(Notice.categoryId);
            //var staticPriceItem = _context.StaticPrices.FirstOrDefault(s => s.code == category.emergencyPrice.ToString());
            //if (staticPriceItem != null)
            //{
            //    totalPrice = staticPriceItem.price;
            //}
            totalPrice = category.emergencyPrice - disCountPrice;
            if (totalPrice < 0)
            {
                totalPrice = 0;
            }
            Factor factor = new Factor
            {
                state = State.IsPay,
                userId = user.id,
                createDatePersian = PersianCalendarDate.PersianCalendarResult(DateTime.Now),
                noticeId = model.id,
                factorKind = FactorKind.Emergency,
                totalPrice = totalPrice
            };
            _context.Factors.Add(factor);
            _context.SaveChanges();
            return new { status = 1, title = "ویژه کردن آگهی", message = "اضطراری کردن آگهی با موفقیت انجام شد." };
        }
    }
    
    
}
