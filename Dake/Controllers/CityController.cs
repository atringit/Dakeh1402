using System;
using System.Collections.Generic;
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
    public class CityController : Controller
    {
         private ICity _City;
        private readonly Context _context;

        public CityController(ICity City, Context context)
        {
            _context = context;
            _City = City;
        }
        [HttpGet]
        public IActionResult Index(int page = 1, string filterTitle = "", bool isSuccess = false)
        {
            var model = _City.GetCities(page, filterTitle);
            if (isSuccess)
                ViewBag.success = "شما قادر به حذف نمی باشید چون آگهی با این رکورد ثبت شده";
            return View(model);
        }
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateOrEdit([Bind("id,name")] City city)
        {
            ModelState.Remove("id");
            if (ModelState.IsValid)
            {
                if (city.id == 0)
                {
                    _context.Add(city);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    try
                    {
                        _context.Update(city);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!CityExists(city.id))
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
        private bool CityExists(int id)
        {
            return _context.Cities.Any(e => e.id == id);
        }
        private bool ProvinceExists(int id)
        {
            return _context.Provinces.Any(e => e.id == id);
        }
        private bool AreaExists(int id)
        {
            return _context.Areas.Any(e => e.id == id);
        }

        public async Task<IActionResult> GetCity(int id)
        {
            if (id == 0)
            {
                return NotFound();
            }

            var city = await _context.Cities.FindAsync(id);
            if (city == null)
            {
                return NotFound();
            }
            return Json(city);

        }
        [HttpGet]
        public IActionResult Province(int id, int page = 1, bool isSuccess = false)
        {
            IQueryable<Province> result = _context.Provinces.Where(x => x.cityId == id);
            ViewData["parentCityId"] = id;
            ViewData["parentCityName"] = _context.Cities.FirstOrDefault(x => x.id == id).name;
            if (isSuccess)
                ViewBag.success = "شما قادر به حذف نمی باشید چون آگهی با این رکورد ثبت شده";
            PagedList<Province> res = new PagedList<Province>(result, page, 10);
            return View(res);
        }
        public async Task<IActionResult> GetProvince(int id)
        {
            if (id == 0)
            {
                return NotFound();
            }

            var Province = await _context.Provinces.FindAsync(id);
            if (Province == null)
            {
                return NotFound();
            }
            return Json(Province);

        }
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateOrEditProvince([Bind("id,name,cityId")] Province province)
        {
            ModelState.Remove("id");
            if (ModelState.IsValid)
            {
                if (province.id == 0)
                {
                    _context.Add(province);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    try
                    {
                        _context.Update(province);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!ProvinceExists(province.id))
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

        [HttpGet]
        public IActionResult Area(int id, int page = 1, bool isSuccess = false)
        {
            IQueryable<Area> result = _context.Areas.Where(x => x.provinceId == id);
            if (isSuccess)
                ViewBag.success = "شما قادر به حذف نمی باشید چون آگهی با این رکورد ثبت شده";
            ViewData["parentProvinceId"] = id;
            ViewData["parentCityId"] = _context.Provinces.FirstOrDefault(x => x.id == id).cityId;
            ViewData["parentProvinceName"] = _context.Provinces.FirstOrDefault(x => x.id == id).name;
            PagedList<Area> res = new PagedList<Area>(result, page, 10);
            return View(res);
        }
        public async Task<IActionResult> GetArea(int id)
        {
            if (id == 0)
            {
                return NotFound();
            }

            var Area = await _context.Areas.FindAsync(id);
            if (Area == null)
            {
                return NotFound();
            }
            return Json(Area);

        }
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateOrEditArea([Bind("id,name,provinceId")] Area Area)
        {
            ModelState.Remove("id");
            if (ModelState.IsValid)
            {
                if (Area.id == 0)
                {
                    _context.Add(Area);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    try
                    {
                        _context.Update(Area);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!AreaExists(Area.id))
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

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var city = await _context.Cities
                .FirstOrDefaultAsync(m => m.id == id);
            if (city == null)
            {
                return NotFound();
            }

            return View(city);
        }

        // POST: Sliders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var city = await _context.Cities.FindAsync(id);

                var province = _context.Provinces.Where(x => x.cityId == id);
                List<int> allProvinceId = new List<int>();
                foreach (var item in province)
                {
                    allProvinceId.Add(item.id);
                    _context.Provinces.Remove(item);
                }
                foreach (var item in allProvinceId)
                {
                    var areas = _context.Areas.Where(x => x.provinceId == item);
                    foreach (var item2 in areas)
                    {
                        _context.Areas.Remove(item2);
                    }
                }
                _context.Cities.Remove(city);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", new { @isSuccess = true });

            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> DeleteProvince(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var Province = await _context.Provinces
                .FirstOrDefaultAsync(m => m.id == id);
            if (Province == null)
            {
                return NotFound();
            }

            return View(Province);
        }

        // POST: Sliders/Delete/5
        [HttpPost, ActionName("DeleteProvince")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmedProvince(int id)
        {
            var province = await _context.Provinces.FindAsync(id);
            int cityId = province.cityId;
            try
            {

                var areas = _context.Areas.Where(x => x.provinceId == id);
                foreach (var item in areas)
                {
                    _context.Areas.Remove(item);
                }

                _context.Provinces.Remove(province);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return RedirectToAction("Province", new { @id = cityId, @isSuccess = true });

            }
            return RedirectToAction("Province", new { @id = cityId }); ;
        }

        public async Task<IActionResult> DeleteArea(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var Area = await _context.Areas
                .FirstOrDefaultAsync(m => m.id == id);
            if (Area == null)
            {
                return NotFound();
            }

            return View(Area);
        }

        // POST: Sliders/Delete/5
        [HttpPost, ActionName("DeleteArea")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmedArea(int id)
        {
            var Area = await _context.Areas.FindAsync(id);
            var provinceId = Area.provinceId;
            try
            {
                _context.Areas.Remove(Area);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return RedirectToAction("Area", new { @id = provinceId, @isSuccess = true });

            }
            return RedirectToAction("Area", new { @id = provinceId });

        }
    }
}