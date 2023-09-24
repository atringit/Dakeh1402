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
    public class DeleteNoticeController : ControllerBase
    {
        private readonly Context _context;
        private INotice _notice;
        private readonly IHostingEnvironment environment;

        public DeleteNoticeController(Context context, INotice notice, IHostingEnvironment environment)
        {
            this.environment = environment;
            _context = context;
            _notice = notice;
        }
       
        [HttpPost]
        public object DeleteNotice(long id)
        {
           var Notice =  _context.Notices.Find(id);
            if (Notice.image != null)
            {

                string deletePath = environment.WebRootPath + Notice.image;

                if (System.IO.File.Exists(deletePath))
                {
                    System.IO.File.Delete(deletePath);
                }
            }
            if (Notice.movie != null)
            {

                string deletePath = environment.WebRootPath + Notice.movie;

                if (System.IO.File.Exists(deletePath))
                {
                    System.IO.File.Delete(deletePath);
                }
            }
            var noticeImage = _context.NoticeImages.Where(x => x.noticeId == id);
            foreach (var item in noticeImage)
            {
                string deletePath = environment.WebRootPath + item.image;
                if (System.IO.File.Exists(deletePath))
                {
                    System.IO.File.Delete(deletePath);
                }
                _context.NoticeImages.Remove(item);
            }
            var factor = _context.Factors.Where(x => x.noticeId == id);
            foreach (var item in factor)
            {
                _context.Factors.Remove(item);
            }
            var userFavorite = _context.UserFavorites.Where(x => x.noticeId == id);
            foreach (var item in userFavorite)
            {
                _context.UserFavorites.Remove(item);
            }
            _context.Notices.Remove(Notice);
            _context.SaveChanges();
            return new { status = 1, title = "حذف آگهی", message = " آگهی حذف گردید." };
        }
    }
}