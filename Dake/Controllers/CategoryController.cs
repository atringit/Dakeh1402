using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Dake.DAL;
using Dake.Models;
using Dake.Service.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PagedList.Core;

namespace Dake.Controllers
{
    public class CategoryController : Controller
    {
        private Icategory _category;
        private readonly Context _context;
        private IStaticPrice _staticPrice; 
        public CategoryController(Icategory category,IStaticPrice staticPrice, Context context)
        {
            _context = context;
            _category = category;
            _staticPrice = staticPrice;
        }
        [HttpGet]
        public IActionResult Index(int page = 1, string filterTitle = "", bool isSuccess = false)
        {
            var model = _category.GetCategories(page, filterTitle);
            ViewData["StaticPrice"] = _staticPrice.GetAll(string.Empty);
            if (isSuccess)
                ViewBag.success = "شما قادر به حذف نمی باشید چون آگهی با این رکورد ثبت شده یا دارای زیر دسته است";
            return View(model);
        }
        [HttpGet]
        public IActionResult SubCategory(int id, int page = 1)
        {
            IQueryable<Category> result = _context.Categorys.Where(x => x.parentCategoryId == id);
            var cat = _context.Categorys.FirstOrDefault(x => x.id == id);
            ViewData["parentCategoryId"] = id;
            ViewData["parentCategoryName"] = cat.name;
            PagedList<Category> res = new PagedList<Category>(result, page, 10);
            return View(res);
        }
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateOrEdit(Category Category)
        {
            ModelState.Remove("id");
            ModelState.Remove("image");

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (Category.id == 0)
            {
                if (Category.imageUrl != null)
                {

                    string imagePath = "";
                    Category.image = Guid.NewGuid().ToString().Replace("-", "") + Path.GetExtension(Category.imageUrl.FileName);
                    imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/Category", Category.image);
                    using (var stream = new FileStream(imagePath, FileMode.Create))
                    {
                        Category.imageUrl.CopyTo(stream);
                    }
                    Category.image = "/images/Category/" + Category.image;

                }
                if (string.IsNullOrEmpty(Category.staticespacialPriceId))
                {
                    Category.staticespacialPriceId = "0";
                }
                if (string.IsNullOrEmpty(Category.staticexpirePriceId))
                {
                    Category.staticexpirePriceId = "0";
                }
                if (string.IsNullOrEmpty(Category.staticemergencyPriceId))
                {
                    Category.staticemergencyPriceId = "0";
                }

                if (string.IsNullOrEmpty(Category.staticladerPriceId))
                {
                    Category.staticladerPriceId = "0";
                }
                if (string.IsNullOrEmpty(Category.staticregisterPriceId))
                {
                    Category.staticregisterPriceId = "0";
                }

                _context.Add(Category);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            else
            {
                try
                {
                    if (Category.imageUrl != null)
                    {
                        string deletePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/Category/", Category.image);
                        if (System.IO.File.Exists(deletePath))
                        {
                            System.IO.File.Delete(deletePath);
                        }
                        string imagePath = "";
                        Category.image = Guid.NewGuid().ToString().Replace("-", "") + Path.GetExtension(Category.imageUrl.FileName);
                        imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/Category", Category.image);
                        using (var stream = new FileStream(imagePath, FileMode.Create))
                        {
                            Category.imageUrl.CopyTo(stream);
                        }
                        Category.image = "/images/Category/" + Category.image;

                    }
                    if (string.IsNullOrEmpty(Category.staticespacialPriceId))
                    {
                        Category.staticespacialPriceId = "0";
                    }

                    if (string.IsNullOrEmpty(Category.staticexpirePriceId))
                    {
                        Category.staticexpirePriceId = "0";
                    }
                    if (string.IsNullOrEmpty(Category.staticemergencyPriceId))
                    {
                        Category.staticemergencyPriceId = "0";
                    }
                    if (string.IsNullOrEmpty(Category.staticladerPriceId))
                    {
                        Category.staticladerPriceId = "0";
                    }
                    if (string.IsNullOrEmpty(Category.staticregisterPriceId))
                    {
                        Category.staticregisterPriceId = "0";
                    }
                    _context.Update(Category);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoryExists(Category.id))
                    {
                        return Json("Error");
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            return Json("Done");

        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateOrEditSub([Bind("id,name,parentCategoryId")] Category Category)
        {
            ModelState.Remove("id");
            var parent = _context.Categorys.FirstOrDefault(x => x.id == Category.parentCategoryId);
            Category.registerPrice = parent.registerPrice;
            Category.expirePrice = parent.expirePrice;
            Category.espacialPrice = parent.espacialPrice;
            Category.staticregisterPriceId = parent.staticregisterPriceId;
            Category.staticladerPriceId = parent.staticladerPriceId;
            Category.staticexpirePriceId = parent.staticexpirePriceId;
            Category.staticespacialPriceId = parent.staticespacialPriceId;
            Category.emergencyPrice = parent.emergencyPrice;
            if (ModelState.IsValid)
            {
                if (Category.id == 0)
                {
                    _context.Add(Category);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("SubCategory", new { @id = parent.id });
                }
                else
                {
                    try
                    {
                        _context.Update(Category);
                        await _context.SaveChangesAsync();
                        return RedirectToAction("SubCategory", new { @id = parent.id });

                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!CategoryExists(Category.id))
                        {
                            return Json("Error");
                        }
                        else
                        {
                            throw;
                        }
                    }
                }
            }
            return Json("Done");

        }





        [HttpPost]
        // [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,name,registerPrice,expirePrice,ladderPrice")] Category Category)
        {
            if (id != Category.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(Category);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoryExists(Category.id))
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
            return View(Category);
        }

        public async Task<IActionResult> GetCategory(int id)
        {
            if (id == 0)
            {
                return NotFound();
            }

            var Category = await _context.Categorys.FindAsync(id);
            if (Category == null)
            {
                return NotFound();
            }
            return Json(Category);

        }
        private bool CategoryExists(int id)
        {
            return _context.Categorys.Any(e => e.id == id);
        }
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categorys
                .FirstOrDefaultAsync(m => m.id == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // POST: Sliders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var Category = await _context.Categorys.Where(s=>s.id == id).FirstOrDefaultAsync();
                _context.Categorys.Remove(Category);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", new { @isSuccess = true });

            }
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> DeleteSub(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categorys
                .FirstOrDefaultAsync(m => m.id == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // POST: Sliders/Delete/5
        [HttpPost, ActionName("DeleteSub")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteSubConfirmed(int id)
        {
                var Category = await _context.Categorys.FindAsync(id);

            try
            {
                _context.Categorys.Remove(Category);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return RedirectToAction("SubCategory", new { @id=Category.parentCategoryId,@isSuccess = true });

            }
            return RedirectToAction("SubCategory", new { @id=Category.parentCategoryId });

        }




        public async Task<IActionResult> RemoveRegisterPrice()
		{
		
            var list = await _context.Categorys.ToListAsync();
            list.ForEach(a => a.staticregisterPriceId = "0");
            list.ForEach(a => a.registerPrice = 0);

            await _context.SaveChangesAsync();
            TempData["Done"] = "عملیات انجام شد!";
            return RedirectToAction(nameof(Index));
        }


    }
}