using Dake.DAL;
using Dake.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Linq;

namespace Dake.Controllers
{
    public class AppManagerController: Controller
    {
        private readonly Context _context;
        public AppManagerController(Context context)
        {
            _context = context;
        }
        [HttpPost]
        public IActionResult AddDakehApp([FromForm]IFormFile File)
        {
            if (File == null)
            {
                return NotFound();
            }
            else
            {
                var namefile = "dakeh" + System.IO.Path.GetExtension(File.FileName).ToLower();
                string e = System.IO.Path.GetExtension(namefile);

                if (e == ".apk")
                {
                    //var filepach = $"Apps/DakehApp";
                    string filePath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Apps", "DakehApp", namefile);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        File.CopyToAsync(stream);
                    }
                    return Ok();
                }
                else
                {
                    return BadRequest();
                }

            }
        }
        [HttpPost]
        public IActionResult AddFreevideocompressor([FromForm] IFormFile File)
        {
            if (File == null)
            {
                return NotFound();
            }
            else
            {
                var namefile = "Freevideocompressor-[dakeh.net]" + System.IO.Path.GetExtension(File.FileName).ToLower();
                string e = System.IO.Path.GetExtension(namefile);

                if (e == ".zip")
                {
                    //var filepach = $"Apps/DakehApp";
                    string filePath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Apps", "Freevideocompressor", namefile);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        File.CopyToAsync(stream);
                    }
                    return Ok();
                }
                else
                {
                    return BadRequest();
                }

            }
        }
        [HttpPost]
        public IActionResult AddFreevideocompressorapp([FromForm] IFormFile File)
        {
            if (File == null)
            {
                return NotFound();
            }
            else
            {
                var namefile = "app1" + System.IO.Path.GetExtension(File.FileName).ToLower();
                string e = System.IO.Path.GetExtension(namefile);

                if (e == ".apk")
                {
                    //var filepach = $"Apps/DakehApp";
                    string filePath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Apps", "Freevideocompressor", namefile);
                    
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        File.CopyToAsync(stream);
                    }
                    return Ok();
                }
                else
                {
                    return BadRequest();
                }

            }
        }
        [HttpPost]
		public IActionResult AddAppBanner([FromForm] IFormFile File)
		{
			if (File == null)
			{
				return NotFound();
			}
			else
			{
				var namefile = "BannerApp" + System.IO.Path.GetExtension(File.FileName).ToLower();
				string e = System.IO.Path.GetExtension(namefile);

				if (e == ".apk")
				{
					//var filepach = $"Apps/DakehApp";
					string filePath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Apps", "Banner", namefile);

					using (var stream = new FileStream(filePath, FileMode.Create))
					{
						File.CopyToAsync(stream);
					}
					return Ok();
				}
				else
				{
					return BadRequest();
				}

			}
		}

	}
}
