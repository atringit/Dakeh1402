using Dake.DAL;
using Dake.Models;
using Dake.Service.Interface;
using PagedList.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Dake.Service
{
    public class sliderService : ISlider
    {
        private Context _context;
        public sliderService(Context context)
        {
            _context = context;
        }
        public object GetSliders()
        {
            IQueryable<Slider> result = _context.Sliders;
            List<Slider> res = result.OrderByDescending(u => u.id).ToList();
            return new { data = res };
        }
        public PagedList<Slider> GetSliders(int page = 1, string filterLink = "")
        {
            IQueryable<Slider> result = _context.Sliders.OrderByDescending(x => x.id);
            if (!string.IsNullOrEmpty(filterLink))
            {
                result = result.Where(u => u.link.Contains(filterLink));
            }
            PagedList<Slider> res = new PagedList<Slider>(result, page, 20);
            return res;
        }

        public int AddSliderFromAdmin(Slider Slider)
        {
            #region Save Image
            if (Slider.imageUrl != null)
            {
                string imagePath = "";
                Slider.image = Guid.NewGuid().ToString().Replace("-", "") + Path.GetExtension(Slider.imageUrl.FileName);
                imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/Slider", Slider.image);
                using (var stream = new FileStream(imagePath, FileMode.Create))
                {
                    Slider.imageUrl.CopyTo(stream);
                }
                Slider.image = "/images/Slider/" + Slider.image;
            }

           
            #endregion

            return AddSlider(Slider);
        }
        public int AddSlider(Slider Slider)
        {
            _context.Sliders.Add(Slider);
            _context.SaveChanges();
            return Slider.id;
        }
        public void EditSlider(Slider Slider)
        {
            Slider _Slider = _context.Sliders.Find(Slider.id);
            _Slider.link = Slider.link;

            if (Slider.imageUrl != null)
            {
                string deletePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/Slider/", _Slider.image);
                if (File.Exists(deletePath))
                {
                    File.Delete(deletePath);
                }
                string imagePath = "";
                _Slider.image = Guid.NewGuid().ToString().Replace("-", "") + Path.GetExtension(Slider.imageUrl.FileName);
                imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/Slider", _Slider.image);
                using (var stream = new FileStream(imagePath, FileMode.Create))
                {
                    Slider.imageUrl.CopyTo(stream);
                }
                _Slider.image = "/images/Slider/" + _Slider.image;

            }
            
            _context.Sliders.Update(_Slider);
            _context.SaveChanges();
        }

    }
}
