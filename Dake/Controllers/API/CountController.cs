using Dake.DAL;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace Dake.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class CountController : ControllerBase
    {
        Context _context;
        public CountController(Context context)
        {
            _context = context;
        }

        [HttpGet("{phone}")]
        public object GetCount([FromRoute] string phone)
        {
            int userId = _context.Users.Single(u => u.cellphone == phone && u.deleted == null).id;
            int notification = _context.Informations.Count();
            int userNotice = _context.Notices.Where(n => n.userId == userId && n.adminConfirmStatus == Models.EnumStatus.Accept&&n.deletedAt == null).Count();
            int userFavorite = _context.UserFavorites.Where(u => u.userId == userId&&u.notice.deletedAt ==  null).Count();
            int message = _context.Messages.Where(m => m.ssenderId == userId).GroupBy(m => m.ItemId).Count();

            return new
            {
                notificationCount = notification.ToString(),
                noticeCount = userNotice.ToString(),
                favoriteCount = userFavorite.ToString(),
                messageCount = message.ToString()
            };
        }
    }
}
