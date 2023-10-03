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
    public class ContactUsController : ControllerBase
    {
        private readonly Context _context;

        public ContactUsController(Context context)
        {
            _context = context;
        }
       
        [HttpGet]

        public object GetContactUss()
        {
            var data = new ContactUs();
            if( _context.ContactUss.Any())
              data=  _context.ContactUss.FirstOrDefault();
            return data;
        }
        [HttpGet("GetVersion")]
        public IActionResult GetVersion()
        {
            var v = _context.ContactUss.FirstOrDefault();
            return Ok(v.androidVersion);
        }
    }
}