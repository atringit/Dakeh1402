using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Dake.DAL;
using Dake.Models;
using Dake.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace Dake.Controllers
{
    [Authorize]

    public class SlidersController : Controller
    {
        private readonly Context _context;
        private ISlider _slider;
        private readonly IHostingEnvironment environment;

        public SlidersController(Context context,ISlider slider, IHostingEnvironment environment)
        {
            this.environment = environment;
            _context = context;
            _slider = slider;
        }

        // GET: Sliders
        public IActionResult Index(int page = 1, string filterLink = "")
        {
            var model = _slider.GetSliders(page, filterLink);
            return View(model);
        }

        // GET: Sliders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var slider = await _context.Sliders
                .FirstOrDefaultAsync(m => m.id == id);
            if (slider == null)
            {
                return NotFound();
            }

            return View(slider);
        }

        // GET: Sliders/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Sliders/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("id,image,movie,link,imageUrl,movieUrl")] Slider slider)
        {
            if (ModelState.IsValid)
            {
              int productId = _slider.AddSliderFromAdmin(slider);

                return RedirectToAction(nameof(Index));
            }
            return View(slider);
        }

        // GET: Sliders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var slider = await _context.Sliders.FindAsync(id);
            if (slider == null)
            {
                return NotFound();
            }
            return View(slider);
        }

        // POST: Sliders/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,image,movie,link,imageUrl,movieUrl")] Slider slider)
        {
            if (id != slider.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                 _slider.EditSlider(slider);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SliderExists(slider.id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(slider);
        }

        // GET: Sliders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var slider = await _context.Sliders
                .FirstOrDefaultAsync(m => m.id == id);
            if (slider == null)
            {
                return NotFound();
            }

            return View(slider);
        }

        // POST: Sliders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var slider = await _context.Sliders.FindAsync(id);
             if (slider.image != null)
            {
               
                    string deletePath = environment.WebRootPath+ slider.image;
                    
                    if (System.IO.File.Exists(deletePath))
                    {
                        System.IO.File.Delete(deletePath);
                    }
            }
            
            _context.Sliders.Remove(slider);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SliderExists(int id)
        {
            return _context.Sliders.Any(e => e.id == id);
        }
    }
}
