using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Dake.DAL;
using Dake.Models;
using Dake.Models.ViewModels;
using Dake.Service;
using Dake.Service.Interface;
using FirebaseAdmin.Messaging;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PagedList.Core;

namespace Dake.Controllers
{
    [Authorize]

    public class InformationController : Controller
    {
        private readonly Context _context;
        private readonly IPushNotificationService _pushNotificationService;

        public InformationController(Context context, IPushNotificationService pushNotificationService)
        {
            _context = context;
            _pushNotificationService = pushNotificationService;
        }

        [HttpGet]
        public IActionResult Index(int page = 1, bool isregister = false)
        {
            IQueryable<Information> items = _context
                .Informations
                .Include(s => s.InformationMedias).OrderByDescending(x => x.id);
            PagedList<Information> res = new PagedList<Information>(items, page, 10);
            return View(res);
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(Information model, IFormFile[] files)
        {
            _context.Informations.Add(model);
            _context.SaveChanges();
            if (files != null && files.Count() > 0)
            {
                foreach (var item in files)
                {
                    var fileName = Guid.NewGuid().ToString().Replace('-', '0') + Path.GetExtension(item.FileName).ToLower();
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\images\Information\", fileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        item.CopyTo(stream);
                    }
                    _context.InformationMedias.Add(new InformationMedia()
                    {
                        Image = "/images/Information/" + fileName,
                        InformationId = model.id,
                    });
                }
            }
            _context.SaveChanges();

            string host = Request.Host.Host;

            //var _VmPushNotification = new VmPushNotification
            //{
            //    Body = model.description,
            //    Title = model.title,
            //    Url = model.Link == null ? "https://dakeh.net" : model.Link,
            //    // ImgUrl = $"{host}/images/Information/{model.InformationMedias.FirstOrDefault(p=>p.InformationId == model.id).Image}"
            //};
            //await _pushNotificationService.SendNotifToAll(_VmPushNotification);
            
            if (FirebaseApp.DefaultInstance == null)
            {
                FirebaseApp.Create(new AppOptions()
                {
                    Credential = GoogleCredential.FromFile("wwwroot/FireBase/key.json"),
                });
            }
            var message = new FirebaseAdmin.Messaging.Message()
            {
                Notification = new Notification
                {
                    Title = model.title,
                    Body = model.description,
                    //ImageUrl = $"https://{host}/images/Information/{model.InformationMedias.FirstOrDefault(p => p.InformationId == model.id).Image}"
                },
                Topic = "weather"
                //Token = "eSCkI0SBfet6ZP1JUN9UAz:APA91bGswUHU3orqxAD-q7dc0YnXEq5CfrsdlzfXeEOfvRPpBu8HfAAz65f6dk47IvL679uB_0Pqj4h9vvJaAw1ElxcLm5u1uPUjYlsH5ncZ4vCFSp69iB9i6v2qVKYyjl-n1qKkIBJ9"
            };

            // Send the message
            var response = await FirebaseMessaging.DefaultInstance.SendAsync(message);


            return RedirectToAction(nameof(Index));
        }


        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateOrEdit(Information Information)
        {
            ModelState.Remove("id");
            if (ModelState.IsValid)
            {
                var information = _context.Informations.FirstOrDefault();
                if (information != null)
                    _context.Informations.Remove(information);
                _context.Add(Information);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", new { isregister = true });

            }
            return RedirectToAction(nameof(Index));


        }


        [HttpGet]
        public IActionResult Remove(int Id)
        {
            if (Id == 0)
            {
                return NotFound();
            }

            var item = _context.Informations
                .FirstOrDefault(m => m.id == Id);
            if (item == null)
            {
                return NotFound();
            }
            return View(item);

        }

        [HttpPost, ActionName("Remove")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var item = await _context.Informations.Include(s => s.InformationMedias).FirstOrDefaultAsync(s => s.id == id);
                _context.Informations.Remove(item);

                if (item.InformationMedias.Count > 0)
                {
                    foreach (var item2 in item.InformationMedias)
                    {
                        System.IO.File.Delete($"wwwroot/{item2.Image}");
                    }
                }
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                return RedirectToAction("Index", new { @isSuccess = true });
            }
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int Id)
        {
            var item = _context.Informations.Include(s => s.InformationMedias).FirstOrDefault(s => s.id == Id);
            if (item == null)
            {
                return NotFound();
            }
            else
            {
                return View(item);

            }
        }

        [HttpPost]
        public IActionResult Edit(Information model, IFormFile[] files, long[] listPic)
        {
            var infoitem = _context.Informations.FirstOrDefault(s => s.id == model.id);

            if (listPic != null)
            {
                foreach (var item in listPic)
                {
                    var _item = _context.InformationMedias.Where(s => s.Id == item).FirstOrDefault();
                    if (_item != null)
                    {

                        _context.InformationMedias.Remove(_item);
                        System.IO.File.Delete($"wwwroot/{_item.Image}");

                    }
                }
            }
            if (files != null && files.Count() > 0)
            {
                foreach (var item in files)
                {
                    var fileName = Guid.NewGuid().ToString().Replace('-', '0') + Path.GetExtension(item.FileName).ToLower();
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\images\Information\", fileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        item.CopyTo(stream);
                    }
                    _context.InformationMedias.Add(new InformationMedia()
                    {
                        Image = "/images/Information/" + fileName,
                        InformationId = model.id,
                    });
                }
            }
            infoitem.title = model.title;
            infoitem.description = model.description;
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

    }
}