using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dake.DAL;
using Dake.Service.Interface;
using Microsoft.AspNetCore.Mvc;

namespace Dake.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class DiscountCodeController : ControllerBase
    {
        private IDiscountCode _DiscountCode;
        private IStaticPrice _StaticPrice; 
        private readonly Context _context;

        public DiscountCodeController(Context context , IStaticPrice staticPrice , IDiscountCode discountCode)
        {
            _context = context;
            _StaticPrice = staticPrice;
            _DiscountCode = discountCode; 
        }

        [HttpGet("CheckDiscountCode")]
        public object CheckDiscountCode()
        {
			#region Parameters
			int n;
            long n2; 
            int TheDisCountCdoe = 0;
            long DiscountCodePrice = 0;
            long _Price = 0;
            long FinalPrice = 0;
            string FinalPrcieCode = string.Empty; 
            string Price = HttpContext.Request?.Headers["Price"];
            string Token = HttpContext.Request?.Headers["Token"];
            string DiscountCode = HttpContext.Request?.Headers["DiscountCode"];
			#endregion
			#region Main
			var user = _context.Users.Where(p => p.token == Token).FirstOrDefault();
            if (user == null)
            {
                return new { status = 3, message = "چنین کاربری وجود ندارد." };
            }
            if(! int.TryParse(DiscountCode, out n))
            {
                return new { status = 4, message = "کدوارد شده باید عدد باشد" };
            }
            if (!long.TryParse(Price, out n2))
            {
                return new { status = 5, message = "قیمت واردشده باید به عدد باشد" };
            }
            TheDisCountCdoe = Convert.ToInt32(DiscountCode);
            if (!_DiscountCode.CheckCode(TheDisCountCdoe))
            {
                return new { status = 6, message = "کدوارد شده معتبر نمی باشد" };
            }
            if(_DiscountCode.IsAlreadyUsed(user.id , TheDisCountCdoe))
            {
                return new { status = 7, message = "شما قبلا از این کد تخفیف استفاده کرده اید" };
            }
            else
            {
                DiscountCodePrice = _DiscountCode.GetDiscountPrice(TheDisCountCdoe);
                _Price = Convert.ToInt64(Price);
                FinalPrice = _Price - DiscountCodePrice;
                if(FinalPrice < 0)
                {
                    FinalPrice = 0; 
                }
                if(!_StaticPrice.CheckByPrice(FinalPrice) && FinalPrice != 0 )
                {
                    return new { status = 8, message = "امکان استفاده از این کد تخفیف وجود ندارد" };
                }
                return new { status =0,Price = FinalPrice , PriceCode = FinalPrice == 0 ? "0" :  _StaticPrice.GetPriceCodeByPrice(FinalPrice)};
            }
			#endregion
		}
	}
}