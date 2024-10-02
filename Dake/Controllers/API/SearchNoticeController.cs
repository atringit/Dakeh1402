using Dake.DAL;
using Dake.Models;
using Dake.Service.Interface;
using Dake.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dake.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class SearchNoticeController : ControllerBase
    {
        private readonly Context _context;
        private readonly ICategoryImageService _categoryImageService;

        public SearchNoticeController(Context context, ICategoryImageService categoryImageService)
        {
            _context = context;
            _categoryImageService = categoryImageService;
        }

        [HttpPost]
        public async Task<object> GetNotices(ProductSearch2 searchNotice)
        {
            var notices = _context.Notices.Where(p => p.isEmergency && p.ExpireDateEmergency < DateTime.Now).ToList();
            foreach (var notice in notices)
            {
                notice.isEmergency = false;
                await _context.SaveChangesAsync();
            }

            string Token = HttpContext.Request?.Headers["Token"];
            var user = _context.Users.Where(p => p.token == Token).FirstOrDefault();
            if (user == null)
                return new { status = 3, message = "چنین کاربری وجود ندارد." };
            var result = _context.Notices.Include(s => s.area).Include(s => s.category).Include(s => s.province).Include(s => s.city).Where(x => x.expireDate >= DateTime.Now && x.adminConfirmStatus == EnumStatus.Accept).ToList();
            //if (user.provinceId != null)
            //{
            //    result = result.Where(x =>  x.provinceId == user.provinceId);
            //}
            int page = 1;
            int pageSize = 10;
            page = searchNotice.page;
            page = page > 0 ? page : 1;
            pageSize = pageSize > 0 ? pageSize : 10;
            if (!string.IsNullOrEmpty(searchNotice.title))
                result = result.Where(x => x.title.Contains(searchNotice.title) || (x.description != null && x.description.Contains(searchNotice.title))).ToList();

            if (searchNotice.areaId != 0 && searchNotice.areaId != null)
                result = result.Where(x => x.areaId == searchNotice.areaId).ToList();
            if (searchNotice.categoryId != 0 && searchNotice.categoryId != null)
            {
                var categoryId = searchNotice.categoryId;
                //get all subcategory by using GetSubCategories method
                List<Category> subCategories = this.GetSubCategories(categoryId);
                //add category to subcategory
                subCategories.Add(_context.Categorys.Where(x => x.id == categoryId).FirstOrDefault());
                //get all notices by using subcategory
                result = result.Where(x => subCategories.Any(y => y.id == x.categoryId)).ToList();
            }
            if (searchNotice.cityId != 0 && searchNotice.cityId != null)
                result = result.Where(x => x.cityId == searchNotice.cityId).ToList();
            if (searchNotice.provinceId != 0 && searchNotice.provinceId != null)
                result = result.Where(x => x.provinceId == searchNotice.provinceId).ToList();
            //result where deletedAt is null and adminConfirmStatus is accept
            result = result.Where(x => x.deletedAt == null && x.adminConfirmStatus == EnumStatus.Accept).ToList();
            //special notices
            var specialNotices = result.Where(x => x.isSpecial && x.expireDateIsespacial >= DateTime.Now).ToList();

            foreach (var item in result)
            {
                if (string.IsNullOrEmpty(item.image))
                {
                    item.image = await _categoryImageService.GetCategoryImageAsync(item.categoryId);
                }
            }
            foreach (var item in specialNotices)
            {
                if (string.IsNullOrEmpty(item.image))
                {
                    item.image = await _categoryImageService.GetCategoryImageAsync(item.categoryId);
                }
            }

            const int pagesize = 10;
            // int skip = (page - 1) * pagesize;
            var res = result.Skip((page - 1) * pagesize).Take(pagesize).Select(x => new
            {
                x.id,
                x.title,
                x.description,
                x.image,
                x.category.name,
                x.categoryId,
                x.isEmergency,
                x.price,
                x.lastPrice,
                x.movie,
            }).ToList();

            var resEspacial = specialNotices.Skip((page - 1) * pagesize).Take(pagesize).Select(x => new
            {
                x.id,
                x.title,
                x.description,
                x.image,
                x.category.name,
                x.movie,
                x.isSpecial,
                x.price,
                x.lastPrice,
            }).ToList();

            var banners = _context.Banner.Where(p => p.expireDate >= DateTime.Now && p.adminConfirmStatus == EnumStatus.Accept).Include(p => p.BannerImage).ToList();

            return new { data = res, resEspacial, banners, totalCount = result.Count() };
        }

        private List<Category> GetSubCategories(int? categoryId)
        {
            var subCategories = new List<Category>();
            if (_context.Categorys.Any(x => x.parentCategoryId == categoryId))
            {
                foreach (var item in _context.Categorys.Where(x => x.parentCategoryId == categoryId).ToList())
                {
                    subCategories.Add(item);
                    subCategories.AddRange(GetSubCategories(item.id));
                }
            }
            return subCategories;
        }
    }
}