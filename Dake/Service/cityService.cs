using Dake.DAL;
using Dake.Models;
using Dake.Service.Interface;
using Microsoft.EntityFrameworkCore;
using PagedList.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dake.Service
{
    public class CityService : ICityService
    {
        private readonly Context _context;
        private const int ALL_Cities_ID = 33;

        public CityService(Context context)
        {
            _context = context;
        }

        public async Task<object> GetCitiesAsync()
        {
            IQueryable<City> result = _context.Cities.OrderBy(u => u.id != ALL_Cities_ID);
            List<City> res = await result.ToListAsync();
            return new { data = res, totalCount = result.Count() };
        }
         public object GetProvinces(int cityId)
        {
            IQueryable<Province> result = _context.Provinces;
            object res = result.Where(x=>x.cityId==cityId).OrderBy(u => u.id).Select( x=>new {x.id,x.name }).ToList();
            return new { data = res, totalCount = result.Count() };
        }
        public object GetAreas(int provinceId)
        {
            IQueryable<Area> result = _context.Areas;
            object res = result.Where(x=>x.provinceId==provinceId).OrderBy(u => u.id).Select( x=>new {x.id,x.name }).ToList();
            return new { data = res, totalCount = result.Count() };
        }

        public PagedList<City> GetCities(int pageId = 1, string filterName = "")
        {
            IQueryable<City> result = _context.Cities.OrderByDescending(x => x.id);
            
            if(!string.IsNullOrEmpty(filterName))
            {
                result = result.Where(x => x.name.Contains(filterName));
            }

            return new PagedList<City>(result, pageId, 10);
        }
    }
}
