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
using Dake.Models.ApiDto;

namespace Dake.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class InformationController : ControllerBase
    {
        private readonly Context _context;
        private IInformation _information;

        public InformationController(Context context, IInformation information)
        {
            _context = context;
            _information = information;

        }

        // GET: api/Information
        [HttpGet("{page}/{pagesize}")]

        public object GetInformations([FromRoute]int page, [FromRoute] int pagesize)
        {
            var data = _information.GetInformations(page, pagesize);
            return data;
        }

        [HttpGet("GetAllInformations/{page}/{pagesize}")]

        public object GetAllInformations([FromRoute] int page, [FromRoute] int pagesize)
        {
            var data = _information.GetInformations(page, pagesize);
            return data;
        }

        // GET: api/Information/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetInformation([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var information = await _context.Informations.FindAsync(id);

            if (information == null)
            {
                return NotFound();
            }

            return Ok(information);
        }
    }
}