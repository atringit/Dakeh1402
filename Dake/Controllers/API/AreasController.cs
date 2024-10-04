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
    public class AreasController : ControllerBase
    {
        private readonly Context _context;

        private ICityService _City;

        public AreasController(Context context, ICityService City)
        {
            _context = context;
            _City = City;

        }

        // GET: api/AllPrices
        [HttpGet("{provinceId}")]
        public object GetAreas([FromRoute]int provinceId)
        {
            var data = _City.GetAreas(provinceId);
            return data;
        }

        // GET: api/AllPrices/5
        
    }
}