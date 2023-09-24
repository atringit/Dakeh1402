using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dake.Service.Interface;
using Microsoft.AspNetCore.Mvc;
using Dake.Models;
using Microsoft.EntityFrameworkCore;

namespace Dake.Controllers
{
    public class StaticPricesController : Controller
    {
        private IStaticPrice _IStaticPrice;
        public StaticPricesController(IStaticPrice IstaticPrice)
        {
            _IStaticPrice = IstaticPrice; 
        }
        public IActionResult Index(string filterTitle = "")
        {

            if (!string.IsNullOrEmpty(filterTitle))
            {
                return View(_IStaticPrice.GetAll(filterTitle));

            }
            return View(_IStaticPrice.GetAll(string.Empty));
        }
        public  IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var Item = _IStaticPrice.GetById(id.Value) ;
                
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
                _IStaticPrice.Remove(id);
            }
            catch (Exception)
            {
                return RedirectToAction("Index", new { @isSuccess = true });

            }
            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        public IActionResult CreateOrUpdate(StaticPrice model)
        {

            try
            {
            
                if ((model.price == 0 || string.IsNullOrEmpty(model.code)) && model.id == 0)
                {
                   
                        TempData["Error"] = "لطفا قیمت یا کد را وارد کنید";
                        return Json("Error");
                    
                }
                if (_IStaticPrice.CheckCode(model.code , model.price) && model.id == 0)
                {
                    TempData["Error"] = "کد یا قیمت وارد شده تکراری است";
                    return Json("Error");
                }
                _IStaticPrice.AddOrUpdate(model);
            }
            catch (Exception)
            {
            return Json("Error");

            }
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public  IActionResult GetStaticPrice(int id)
        {
            if (id == 0)
            {
                return NotFound();
            }

            var item =_IStaticPrice.GetById(id);
            if (item == null)
            {
                return NotFound();
            }
            return Json(item);

        }

    }
}