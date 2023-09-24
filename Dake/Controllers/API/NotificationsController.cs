using Dake.DAL;
using Dake.Models.ApiDto;
using Dake.Service.Interface;
using Microsoft.AspNetCore.Mvc;

namespace Dake.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationsController : ControllerBase
    {
        private readonly Context _context;
        private IInformation _information;

        public NotificationsController(Context context, IInformation information)
        {
            _context = context;
            _information = information;
        }
        [HttpPost("GetInformations")]
        public object GetInformations([FromBody] Pagination dto)
        {
            var data = _information.GetInformations(dto.Page, dto.Pagesize);
            return data;
        }
    }
}
