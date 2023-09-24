using Dake.DAL;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace Dake.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class SettingController : ControllerBase
    {
        private readonly Context _context;

        public SettingController(Context context)
        {
            _context = context;
        }

        [HttpGet]

        public object GetSetting()
        {
            var data = new object();
            if (_context.Settings.Any())
                data = _context.Settings.Select(x => new { wrongWord = wrongWord(x.wrongWord) }).FirstOrDefault();
            return data;

        }
        private List<string> wrongWord(string wrongWord)
        {
            List<string> TagIds = wrongWord.Split('-').ToList();
            return TagIds;
        }
    }
}