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
    public class DeleteNoticeImageController : ControllerBase
    {
        private readonly Context _context;
        private INotice _notice;
        private readonly IHostingEnvironment environment;

        public DeleteNoticeImageController(Context context, INotice notice, IHostingEnvironment environment)
        {
            this.environment = environment;
            _context = context;
            _notice = notice;
        }
       
        [HttpPost]
        public object DeleteNoticeImage(DeleteNoticeImage noticeImage)
        {
            var notice = _context.Notices.Find(noticeImage.id);
            if (notice.image == noticeImage.imageName)
            {
                string deletePath = environment.WebRootPath + notice.image;
                if (System.IO.File.Exists(deletePath))
                {
                    System.IO.File.Delete(deletePath);
                }
                notice.image = "";
            }
            else if (notice.movie == noticeImage.imageName)
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
                var allnoticeImages = _context.NoticeImages.Where(x => x.noticeId == noticeImage.id);
                foreach (var item in allnoticeImages)
                {
                    if (item.image == noticeImage.imageName)
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
            return new { status = 1, title = "حذف عکس آگهی", message = "عکس آگهی حذف گردید." };
        }
    }
}