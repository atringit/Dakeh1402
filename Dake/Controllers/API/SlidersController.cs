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
    public class SlidersController : ControllerBase
    {
        private readonly Context _context;
         private ISlider _slider;

        public SlidersController(Context context, ISlider slider)
        {
            _context = context;
            _slider = slider;

        }
        // GET: api/Sliders
         [HttpGet]
        public object GetSliders()
        {
            var data = _slider.GetSliders();
            return data;
        }
        // GET: api/Sliders/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetSlider([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var slider = await _context.Sliders.FindAsync(id);

            if (slider == null)
            {
                return NotFound();
            }

            return Ok(slider);
        }

    }
}