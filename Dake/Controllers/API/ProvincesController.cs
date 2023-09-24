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
    public class ProvincesController : ControllerBase
    {
        private readonly Context _context;

        private ICity _City;

        public ProvincesController(Context context, ICity City)
        {
            _context = context;
            _City = City;

        }

        // GET: api/AllPrices
        [HttpGet("{cityId}")]
        public object GetProvinces([FromRoute]int cityId)
        {
            var data = _City.GetProvinces(cityId);
            return data;
        }

       
        
    }
}