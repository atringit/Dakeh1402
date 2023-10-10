using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dake.DAL;
using Dake.Models;
using Dake.Service.Interface;
using Microsoft.AspNetCore.Mvc;

namespace Dake.Controllers
{
    public class DiscountCodeController : Controller
    {
        private IDiscountCode _IDiscountCode;
        private readonly Context _context;

        public DiscountCodeController(IDiscountCode IdiscountCode , Context context)
        {
            _IDiscountCode = IdiscountCode;
            _context = context; 
        }

        public IActionResult Index()
        {
            return View(_IDiscountCode.GetAll());
        }
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var Item = _IDiscountCode.GetById(id.Value);

            if (Item == null)
            {
                return NotFound();
            }
            return View(Item);
        }
        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            try
            {
                _IDiscountCode.Remove(id);
            }
            catch (Exception)
            {
                return RedirectToAction("Index", new { @isSuccess = true });

            }
            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        public IActionResult CreateOrUpdate(DiscountCode model)
        {
            try
            {
                if (model.price == 0 || model.code == 0  || model.count == 0   && model.id == 0)
                {

                    TempData["Error"] = "لطفا قیمت یا کد را وارد کنید";
                    return Json("Error");
                }
                if (_IDiscountCode.CheckCode(model.code) && model.id == 0)
                {
                    TempData["Error"] = "کد  وارد شده تکراری است";
                    return Json("Error");
                }
                _IDiscountCode.AddOrUpdate(model);
            }
            catch (Exception)
            {
                return Json("Error");

            }
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public IActionResult GetDiscountCode(int id)
        {
            if (id == 0)
            {
                return NotFound();
            }

            var item = _IDiscountCode.GetById(id);
            if (item == null)
            {
                return NotFound();
            }
            return Json(item);
        }

        [HttpGet]
        public JsonResult Validity(string code , string categoryId)
        {
            var user = _context.Users.FirstOrDefault(x => x.cellphone == User.Identity.Name && x.deleted == null);
            long totalprice = 0; 
            int n;
            if (string.IsNullOrEmpty(code))
            {
                return Json(new { success = false });
            }
            if (! int.TryParse(code, out n))
            {
                return Json(new { success = false });
            }
            int _code = Convert.ToInt32(code);
            if (_IDiscountCode.IsAlreadyUsed(user.id, _IDiscountCode.GetIdByCode(_code)))
            {
                return Json(new { success = false , alreadyused = true });

            }
            if (_IDiscountCode.CheckCode(_code))
            {
                if (!string.IsNullOrEmpty(categoryId))
                {
                    int _catId = Convert.ToInt32(categoryId);
                    var catitem = _context.Categorys.FirstOrDefault(s=>s.id == _catId);
                    totalprice = catitem.registerPrice - _IDiscountCode.GetDiscountPrice(_code); 
                    if(totalprice < 0)
                    {
                        totalprice = 0;
                    }

                }


                return Json(new { success = true , price = _IDiscountCode.GetDiscountPrice(_code) , total = totalprice });
            }
            else
            {
                return Json(new { success = false });
            }
        }
    }
}