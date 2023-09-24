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
    public class RuleController : ControllerBase
    {
        private readonly Context _context;

        public RuleController(Context context)
        {
            _context = context;

        }

        // GET: api/Information
        [HttpGet]

        public object GetRules()
        {
           var data = new Rule();
            if( _context.Rules.Any())
              data=  _context.Rules.FirstOrDefault();
            return data;
        }
    }
}