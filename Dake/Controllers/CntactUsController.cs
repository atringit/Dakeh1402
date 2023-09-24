using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dake.DAL;
using Dake.Models;
using Dake.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PagedList.Core;

namespace Dake.Controllers
{
    [Authorize]

    public class CntactUsController : Controller
    {
        private readonly Context _context;

        public CntactUsController(Context context)
        {
            _context = context;
        }
        [HttpGet]
        public IActionResult Index()
        {
            var model = _context.ContactUss.FirstOrDefault();
            return View(model);
        }
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateOrEdit(ContactUs ContactUs)
        {
            ModelState.Remove("id");
            if (ModelState.IsValid)
            {
                if (ContactUs.id == 0)
                {
                    _context.Add(ContactUs);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    try
                    {
                        _context.Update(ContactUs);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {

                        throw;
                    }
                }
            }
            return RedirectToAction(nameof(Index));


        }
    }
}