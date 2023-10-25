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
using Dake.Models.ViewModels;

namespace Dake.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly Context _context;

        public CategoryController(Context context)
        {
            _context = context;

        }
        // GET: api/Categorys
         [HttpGet]
        public object GetCategorys()
        {
            List<CategoryViewModel> categoryViewModels = new List<CategoryViewModel>();
            IQueryable<Category> result = _context.Categorys.OrderByDescending(x=>x.id);
            var res = result.Select(x=>new {x.id,x.name,parentCategoryId=GetAd(x.parentCategoryId),x.expirePrice,x.espacialPrice,x.staticespacialPriceId , x.staticexpirePriceId , x.staticladerPriceId,x.staticregisterPriceId,x.registerPrice,x.image, x.emergencyPrice }).ToList();
            foreach (var item in res)
            {
                categoryViewModels.Add(new CategoryViewModel()
                {
                    id = item.id,
                    name = item.name,
                    parentCategoryId =
                    GetAd(item.parentCategoryId),
                    espacialPriceCode = item.staticespacialPriceId,
                    espacialPrice = item.staticespacialPriceId == "0" ? 0
                    : _context.StaticPrices.Where(s => s.code == item.staticespacialPriceId)
                    .FirstOrDefault().price,
                    expirePrice = item.staticexpirePriceId == "0" ? 0 :
                    _context.StaticPrices.Where(s => s.code == item.staticexpirePriceId).
                    FirstOrDefault().price,
                    expirePriceCode = item.staticexpirePriceId,
                    ladderPrice = item.staticladerPriceId == "0" ? 0 :
                    _context.StaticPrices.Where(s => s.code == item.staticladerPriceId).
                    FirstOrDefault().price,
                    ladderPriceCode = item.staticladerPriceId,
                    registerPrice = item.staticregisterPriceId == "0" ? 0 :
                    _context.StaticPrices.Where(s => s.code == item.staticregisterPriceId).
                    FirstOrDefault().price,
                    registerPriceCode = item.staticregisterPriceId,
                    image = item.image
                });
            }
            return new { data = categoryViewModels };
        }
        private int? GetAd(int? categoryId)
        {
            if (categoryId == null)
                return 0;
            else return categoryId;
           
        }
         [HttpGet("{id}")]

         public object GetCategory(int id)
        {
            if (id == 0)
            {
                return NotFound();
            }

            var Category =  _context.Categorys.Find(id);
            if (Category == null)
            {
                return NotFound();
            }
            return  Category;


        }
        [HttpGet("GetParentCategory/{id}")]
        public IActionResult GetParentCategory(int id)
        {
            var cat = _context.Categorys.Where(p => p.parentCategoryId == id);
            if (cat == null)
            {
                return NotFound();
            }
            return Ok(cat);
            
        }

        
       
    }
}