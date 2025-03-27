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
        public async Task<Result<List<CategoryViewModel>>> GetCategorys()
        {
            var staticPrices = await _context
                .StaticPrices
                .ToListAsync();

            var query = _context
                .Categorys
                .Where(w => w.parentCategoryId != null)
                .Select(s => new CategoryViewModel
                {
                    id = s.id,
                    name = s.name,
                    parentCategoryId = s.parentCategoryId,
                    espacialPriceCode = s.staticespacialPriceId,
                    espacialPrice = s.staticespacialPriceId == "0"
                    ? 0
                    : staticPrices.Find(x => x.code == s.staticespacialPriceId).price,
                    expirePrice = s.staticexpirePriceId == "0"
                    ? 0
                    : staticPrices.Find(x => x.code == s.staticexpirePriceId).price,
                    expirePriceCode = s.staticexpirePriceId,
                    ladderPrice = s.staticladerPriceId == "0"
                    ? 0
                    : staticPrices.Find(x => x.code == s.staticladerPriceId).price,
                    ladderPriceCode = s.staticladerPriceId,
                    registerPrice = s.staticregisterPriceId == "0"
                    ? 0
                    : staticPrices.Find(x => x.code == s.staticregisterPriceId).price,
                    registerPriceCode = s.staticregisterPriceId,
                    image = s.image
                });

            var data = await query.ToListAsync();

            return new Result<List<CategoryViewModel>>(
                isSuccess: true,
                data: data);
        }

        [HttpGet("GetParentCategories")]
        public async Task<Result<List<CategoryViewModel>>> GetParentCategories()
        {
            var staticPrices = await _context
                .StaticPrices
                .ToListAsync();

            var query = _context
                .Categorys
                .Where(w => w.parentCategoryId == null)
                .Select(s => new CategoryViewModel
                {
                    id = s.id,
                    name = s.name,
                    parentCategoryId = 0,
                    espacialPriceCode = s.staticespacialPriceId,
                    espacialPrice = s.staticespacialPriceId == "0"
                    ? 0
                    : staticPrices.Find(x => x.code == s.staticespacialPriceId).price,
                    expirePrice = s.staticexpirePriceId == "0"
                    ? 0
                    : staticPrices.Find(x => x.code == s.staticexpirePriceId).price,
                    expirePriceCode = s.staticexpirePriceId,
                    ladderPrice = s.staticladerPriceId == "0"
                    ? 0
                    : staticPrices.Find(x => x.code == s.staticladerPriceId).price,
                    ladderPriceCode = s.staticladerPriceId,
                    registerPrice = s.staticregisterPriceId == "0"
                    ? 0
                    : staticPrices.Find(x => x.code == s.staticregisterPriceId).price,
                    registerPriceCode = s.staticregisterPriceId,
                    image = s.image
                });

            var data = await query.ToListAsync();

            return new Result<List<CategoryViewModel>>(
                isSuccess: true,
                data: data);
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