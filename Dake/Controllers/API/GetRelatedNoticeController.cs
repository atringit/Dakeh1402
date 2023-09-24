using Dake.DAL;
using Dake.Models;
using Dake.Service.Interface;
using Dake.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dake.Controllers.API
{
	[Route("api/[controller]")]
	[ApiController]
	public class GetRelatedNoticeController : ControllerBase
	{
        private readonly Context _context;
        private INotice _notice;
        public GetRelatedNoticeController(Context context, INotice notice)
        {
            _context = context;
            _notice = notice;
        }

        [HttpGet("{id}")]
        public object GetRelatedNotice([FromRoute] int id)
        {
            string Token = HttpContext.Request?.Headers["token"];
            List<NoticeShortViewModel> noticeShortViewModels = new List<NoticeShortViewModel>();
            var user = _context.Users.Where(p => p.token == Token).FirstOrDefault();
            var noticeItem = _context.Notices.Where(s => s.id == id).FirstOrDefault();
            if (user == null)
            {
                return new { status = 2, title = "خطا", message = "کاربر مورد نظر یافت نشد" };

            }
            if (noticeItem != null)
            {
                var Items = _context.Notices.Where(p => p.areaId == noticeItem.areaId
                                                        && p.categoryId == noticeItem.categoryId
                                                        && p.id!=noticeItem.id
                                                        && p.expireDate >= DateTime.Now
                                                        && p.adminConfirmStatus == EnumStatus.Accept);
                foreach (var item in Items)
                {
                    noticeShortViewModels.Add(new NoticeShortViewModel() { id = item.id, title = item.title, url = item.image ,  categoryId= item.categoryId});
                }

				foreach (var item in noticeShortViewModels)
				{
                    if (string.IsNullOrEmpty(item.url) == false && item.url.Contains("/images/Category/"))
                    {
                        item.url = getCategoryImage(item.categoryId);
                    }
                }

                return new { data = noticeShortViewModels };
  
            }
            else
            {
                return new { status = 4, title = "خطا", message = "اگهی مورد نظر یافت نشد" };

            }
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
