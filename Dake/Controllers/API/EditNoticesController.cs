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
    public class EditNoticesController : ControllerBase
    {
        private readonly Context _context;
        private INotice _notice;
        private readonly IHostingEnvironment environment;

        public EditNoticesController(Context context, INotice notice, IHostingEnvironment environment)
        {
            this.environment = environment;
            _context = context;
            _notice = notice;
        }

        [HttpPost("{id}")]
        public object PutNotice([FromRoute] long id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            string Token = HttpContext.Request?.Headers["Token"];
            var user = _context.Users.Where(p => p.token == Token).FirstOrDefault();
            if (user == null)
                return new { status = 3, message = "چنین کاربری وجود ندارد." };
            string data = HttpContext.Request?.Form["data"];
            JObject json = JObject.Parse(data);
            JObject jalbum = json as JObject;
            var movie = "";
            string imageUrl = "";

            Notice Notice = jalbum.ToObject<Notice>();
            var notice = _context.Notices.Find(id);
            var httpRequest = HttpContext.Request;
            var hfc = HttpContext.Request.Form.Files;
            List<string> images = new List<string>();
            for (int i = 0; i < hfc.Count; i++)
            {
                var namefile = Guid.NewGuid().ToString().Replace('-', '0').Substring(0, 7) + Path.GetExtension(hfc[i].FileName).ToLower();
                var filePath = Path.Combine(environment.WebRootPath, "Notice/", namefile);
                if (hfc[i].Name == "imageUrl")
                {
                    imageUrl = "/Notice/" + namefile;
                }
                else if (hfc[i].Name == "movie")
                {
                    movie = "/Notice/" + namefile;
                }
                else
                {

                    images.Add("/Notice/" + namefile);

                }
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    hfc[i].CopyTo(stream);
                }

            }
            if (movie != "")
                notice.movie = movie;
            if (imageUrl != "")
                notice.image = imageUrl;

            if (hfc == null || hfc.Count <= 0)
            {
                notice.image = "/images/nopic.jpg";
            }

            foreach (var item in images)
            {
                _context.NoticeImages.Add(new NoticeImage
                {
                    noticeId = notice.id,
                    image = item
                });
                _context.SaveChanges();

            }



            notice.title = Notice.title;

            notice.description = Notice.description;

            notice.cityId = Notice.cityId;

            notice.provinceId = Notice.provinceId;

            notice.areaId = Notice.areaId;

            notice.categoryId = Notice.categoryId;

            notice.lastPrice = Notice.lastPrice;

            notice.price = Notice.price;

            notice.link = Notice.link;
            notice.adminConfirmStatus = EnumStatus.Pending;

            _context.Notices.Update(notice);
            _context.SaveChanges();
            return new { status = 0, message = "آگهی شما با موفقیت ویرایش گردید." };
        }

    }
}