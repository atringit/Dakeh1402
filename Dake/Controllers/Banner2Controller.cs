using Dake.Service;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Dake.Controllers
{
    public class Banner2Controller : Controller
    {
        private readonly IBannerSevice _repository;

        public Banner2Controller(IBannerSevice repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int page)
        {
            return View(await _repository.GetAllData(page));
        }
    }
}
