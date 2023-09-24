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
    public class SearchNoticeBasedCityController : ControllerBase
    {
        private readonly Context _context;
        private INotice _notice;
        private readonly IHostingEnvironment environment;

        public SearchNoticeBasedCityController(Context context, INotice notice, IHostingEnvironment environment)
        {
            this.environment = environment;
            _context = context;
            _notice = notice;
        }
       [HttpPost]
        public object GetNotices(ProductSearch2 searchNotice)
        {
           var result = _context.Notices.Include(s=>s.category).Include(s=>s.city).Include(s=>s.area).Include(s=>s.province).Where(x => x.expireDate >= DateTime.Now  && x.adminConfirmStatus==EnumStatus.Accept).ToList();
           int page = 1;
            int pageSize = 10;
            page = searchNotice.page;
            page = page > 0 ? page : 1;
            pageSize = pageSize > 0 ? pageSize : 10;
            string Token = HttpContext.Request?.Headers["Token"];
            var user = _context.Users.Where(p => p.token == Token).FirstOrDefault();
            if (!String.IsNullOrEmpty(searchNotice.title))
                result = result.Where(x => x.title.Contains(searchNotice.title) || x.description.Contains(searchNotice.title)).ToList();
            //if (user.provinceId!=null)
            //    result = result.Where(x => x.provinceId == user.provinceId);
            foreach (var item in result)
            {
                if (string.IsNullOrEmpty(item.image) == false && item.image.Contains("/images/Category/"))
                {
                    item.image = getCategoryImage(item.categoryId);
                }
            }
            var res = result.OrderByDescending(x => x.expireDate).Skip(10 * (page - 1)).Take(10).Select(x => new { x.id, x.image, x.title, x.description, areaName = x.area.name, cityName = x.city.name, provinceName = x.province.name , x.categoryId}).ToList();

            return new { data = res, totalCount = result.Count() };
        }

        private string getCategoryImage(int catId)
        {
            var categoryItem = _context.Categorys.FirstOrDefault(s => s.id == catId);
            if (!categoryItem.parentCategoryId.HasValue)
            {
                return categoryItem.image;
            }
            var categoryItem2 = _context.Categorys.FirstOrDefault(s => s.id == categoryItem.parentCategoryId);
            if (!categoryItem2.parentCategoryId.HasValue)
            {
                return categoryItem2.image;
            }
            var categoryItem3 = _context.Categorys.FirstOrDefault(s => s.id == categoryItem2.parentCategoryId);
            if (!categoryItem3.parentCategoryId.HasValue)
            {
                return categoryItem3.image;
            }
            var categoryItem4 = _context.Categorys.FirstOrDefault(s => s.id == categoryItem3.parentCategoryId);
            if (!categoryItem4.parentCategoryId.HasValue)
            {
                return categoryItem4.image;
            }
            var categoryItem5 = _context.Categorys.FirstOrDefault(s => s.id == categoryItem4.parentCategoryId);
            if (!categoryItem5.parentCategoryId.HasValue)
            {
                return categoryItem5.image;
            }
            var categoryItem6 = _context.Categorys.FirstOrDefault(s => s.id == categoryItem5.parentCategoryId);
            if (!categoryItem6.parentCategoryId.HasValue)
            {
                return categoryItem6.image;
            }
            var categoryItem7 = _context.Categorys.FirstOrDefault(s => s.id == categoryItem6.parentCategoryId);
            if (!categoryItem7.parentCategoryId.HasValue)
            {
                return categoryItem7.image;
            }
            var categoryItem8 = _context.Categorys.FirstOrDefault(s => s.id == categoryItem7.parentCategoryId);
            if (!categoryItem8.parentCategoryId.HasValue)
            {
                return categoryItem8.image;
            }
            var categoryItem9 = _context.Categorys.FirstOrDefault(s => s.id == categoryItem8.parentCategoryId);
            if (!categoryItem9.parentCategoryId.HasValue)
            {
                return categoryItem9.image;
            }
            var categoryItem10 = _context.Categorys.FirstOrDefault(s => s.id == categoryItem9.parentCategoryId);
            if (!categoryItem10.parentCategoryId.HasValue)
            {
                return categoryItem10.image;
            }
            return string.Empty;
        }



    }
}