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

    public class RuleController : Controller
    {
        private readonly Context _context;

        public RuleController(Context context)
        {
            _context = context;
        }
        [HttpGet]
        public IActionResult Index()
        {
            var model = _context.Rules.FirstOrDefault();
            return View(model);
        }
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateOrEdit(Rule Rule)
        {
            ModelState.Remove("id");
            if (ModelState.IsValid)
            {
                if (Rule.id == 0)
                {
                    _context.Add(Rule);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    try
                    {
                        _context.Update(Rule);
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