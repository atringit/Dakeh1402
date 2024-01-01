using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dake.DAL;
using Dake.Models;
using Dake.Models.ViewModels;
using Dake.Service.Interface;
using Dake.ViewModel;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Dake.Controllers
{
    public class Notice2Controller : Controller
    {
        private readonly Context _context;
        private INotice _Notice;
        private readonly IHostingEnvironment environment;

        public Notice2Controller(Context context, INotice Notice, IHostingEnvironment environment)
        {
            this.environment = environment;
            _context = context;
            _Notice = Notice;
        }
        public IActionResult Index(int id)
        {
            var settings = _context.Settings.FirstOrDefault();
            var nuser = User.Identity.Name;
            var muser = _context.Users.FirstOrDefault(p=>p.cellphone == nuser && p.deleted == null);

			DetailNoticeViewModel detailNoticeViewModel = new DetailNoticeViewModel();
            var Notice = _context.Notices.Include(x => x.user).Include(x => x.area).Include(x => x.city).Include(x => x.province).Include(x => x.category).FirstOrDefault(x => x.id == id  && x.deletedAt == null);
            //if (Notice == null)
            if ( Notice == null)
            {
                return RedirectToRoute('/');
            }
            detailNoticeViewModel.notice = Notice;
            detailNoticeViewModel.noticeImages = _context.NoticeImages.Where(x => x.noticeId == id).ToList();
            List<Category> cats = new List<Category>();
            var user = _context.Users.FirstOrDefault(x => x.cellphone == User.Identity.Name && x.deleted == null);
            if (user != null)
            {
                if (detailNoticeViewModel.notice.userId != user.id)
                {
                    if (!_context.VisitNoticeUsers.Any(x => x.noticeId == id && x.userId == user.id))
                    {
                        _context.VisitNoticeUsers.Add(new VisitNoticeUser { noticeId = id, userId = user.id });

                        var VisitNotice = _context.VisitNotices.LastOrDefault(x => x.noticeId == id);

                        if (VisitNotice == null)
                        {
                            Notice.countView = Notice.countView + 1;
                            _context.VisitNotices.Add(new VisitNotice { countView = 1, date = DateTime.Now, noticeId = id });
                        }
                        else
                        {
                            if (DateTime.Compare(VisitNotice.date.Date, DateTime.Now.Date) == 0)
                                VisitNotice.countView++;
                            else
                                _context.VisitNotices.Add(new VisitNotice { countView = 1, date = DateTime.Now, noticeId = id });
                        }
                        
                    }

                }
            }
			if (Notice.countView >= settings.countToSpecialNotice && Notice.isSpecial == false)
			{
				Notice.isSpecial = true;
				Notice.expireDateIsespacial = DateTime.Now.AddDays(Convert.ToInt64(settings.countExpireDateIsespacial));
			}

			_context.SaveChanges();

            int cat = detailNoticeViewModel.notice.categoryId;
            for (int i = 0; i < 4; i++)
            {
                var category = _context.Categorys.FirstOrDefault(x => x.id == cat);
                if (category == null)
                    break;
                cats.Add(category);
                if (category.parentCategoryId != null)
                    cat = (int)category.parentCategoryId;
                else
                    break;
            }
            detailNoticeViewModel.Categorys = cats;
            if (user != null)
            {

                detailNoticeViewModel.userId = user.id;
            }
            if (settings != null && settings.showPriceForCars == false)
            {
                if (IsDrivingPrice(detailNoticeViewModel.notice.categoryId))
                {
                    detailNoticeViewModel.notice.price = 0;
                    detailNoticeViewModel.notice.lastPrice = 0;
                }
            }

            ///

            var _notice = _context.Notices.Single(p => p.id == id);

            var relatednotice = _context.Notices
                .Where(p => p.areaId == _notice.areaId
                            && p.categoryId == _notice.categoryId
                             && p.id!=_notice.id
                             && p.expireDate >= DateTime.Now
                             && p.adminConfirmStatus == EnumStatus.Accept)
                .ToList();


            

            detailNoticeViewModel.Relatednotices = relatednotice;
            if(user != null)
            {
                if (muser.IsBlocked == true)
                {
                    detailNoticeViewModel.notice.user.cellphone = "حساب شما مسدود است و قادر به دیدن شماره تماس اگهی گذار نیستید.";
                }
            }
            //
            return View(detailNoticeViewModel);
        }
        private bool IsDrivingPrice(int CatId)
        {
            while (CatId > 0)
            {
                var categoryitem = _context.Categorys.Where(s => s.id == CatId).FirstOrDefault();
                if (categoryitem != null)
                {
                    if (categoryitem.parentCategoryId.HasValue)
                    {
                        CatId = categoryitem.parentCategoryId.Value;
                    }
                    if (CatId == 29)
                    {
                        return true;
                    }
                    if (!categoryitem.parentCategoryId.HasValue)
                    {
                        return false;
                    }
                }
            }
            return false;
        }
        [HttpGet]
        public IActionResult DeleteNoticeImage(long id, string imageName)
        {
            var notice = _context.Notices.Find(id);
            if (notice.image == imageName)
            {
                string deletePath = environment.WebRootPath + notice.image;
                if (System.IO.File.Exists(deletePath))
                {
                    System.IO.File.Delete(deletePath);
                }
                notice.image = "";
            }
            else if (notice.movie == imageName)
            {
                string deletePath = environment.WebRootPath + notice.movie;
                if (System.IO.File.Exists(deletePath))
                {
                    System.IO.File.Delete(deletePath);
                }
                notice.movie = "";
            }
            else
            {
                var allnoticeImages = _context.NoticeImages.Where(x => x.noticeId == id);
                foreach (var item in allnoticeImages)
                {
                    if (item.image == imageName)
                    {
                        string deletePath = environment.WebRootPath + item.image;
                        if (System.IO.File.Exists(deletePath))
                        {
                            System.IO.File.Delete(deletePath);
                        }

                        _context.NoticeImages.Remove(item);
                    }
                }
            }
            _context.SaveChanges();
            return Ok();
        }


    }
}