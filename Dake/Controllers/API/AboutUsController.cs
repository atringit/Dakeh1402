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
using Microsoft.AspNetCore.Cors;

namespace Dake.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class AboutUsController : ControllerBase
    {
        private readonly Context _context;

        public AboutUsController(Context context)
        {
            _context = context;

        }


        // GET: api/Information
        //[EnableCors("AllowOrigin")]
        [HttpGet]

        public object GetAboutUss()
        {
            var data = new AboutUs();
            if( _context.AboutUss.Any())
              data=  _context.AboutUss.FirstOrDefault();
            return data;
        }
    }
}