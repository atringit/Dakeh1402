using Dake.DAL;
using Dake.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PagedList.Core;
using System.Linq;
using System.Threading.Tasks;

namespace Dake.Controllers
{
    public class ReportMessageController : Controller
    {
        private readonly Context _context;
        public ReportMessageController(Context context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index(int pageId = 1)
        {
            IQueryable<Message> result = _context.Messages.Where(p=>p.isrep == "YES");
            foreach (var item in result)
            {
                item.sender =await _context.Users.FirstOrDefaultAsync(p=>p.id == item.ssenderId);
            }
            PagedList<Message> res = new PagedList<Message>(result, pageId, 10);
            return View(res);
        }
    }
}
