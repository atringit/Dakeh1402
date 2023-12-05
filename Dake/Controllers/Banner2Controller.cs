using Dake.DAL;
using Dake.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Dake.Controllers
{
    public class Banner2Controller : Controller
    {
        private readonly Context _context;
        private readonly IBannerSevice _repository;

        public Banner2Controller(IBannerSevice repository,Context context)
        {
            _repository = repository;
            _context = context;
        }
        [HttpPost]
        public async Task<IActionResult> RemoveBanner(long id)
        {
            await _repository.DeleteBanner(id);
            return Ok("succsed");
        }
        [HttpGet]
        public async Task<IActionResult> Index(int page)
        {
            return View(await _repository.GetAllData(page));
        }
        [HttpPost("VeiwBanner")]
        public async Task<IActionResult> VeiwBanner(int id)
        {
            var banner =await _context.Banner.FirstOrDefaultAsync(p => p.Id == id);
            banner.countView = banner.countView + 1;
            _context.Banner.Update(banner);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
