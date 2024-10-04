using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dake.DAL;
using Dake.Service.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Dake.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class CitiesController : ControllerBase
    {
        private readonly Context _context;

        private ICityService _cityService;

        public CitiesController(Context context, ICityService cityService)
        {
            _context = context;
            _cityService = cityService;

        }

        // GET: api/AllPrices
        [HttpGet("GetCities")]
        public async Task<IActionResult> GetCities()
        {
            var data = await _cityService.GetCitiesAsync();
            return Ok(data);
        }
		[HttpGet("GetProvinceForAndroaid/{id}")]
		public IActionResult GetProvinceForAndroaid(int id)
		{
			var provinces = _context.Provinces.Where(x => x.cityId == id);
			return Ok(provinces);
		}

		// GET: api/AllPrices/5

	}
}