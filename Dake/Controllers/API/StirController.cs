using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Dake.DAL;
using Dake.Models;
using Dake.Service.Interface;

namespace Dake.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class StirController : ControllerBase
    {
        private readonly Context _context;

        public StirController(Context context)
        {
            _context = context;

        }

        // GET: api/Information
        [HttpGet]

        public object GetStirs()
        {
            var data = new Stir();
            if( _context.Stirs.Any())
              data=  _context.Stirs.FirstOrDefault();
            return data;
        }
    }
}