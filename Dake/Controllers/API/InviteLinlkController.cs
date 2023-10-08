using Dake.DAL;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace Dake.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class InviteLinlkController:ControllerBase
    {
        private readonly Context _context;
        public InviteLinlkController(Context context)
        {
            _context = context;
        }
        [HttpGet("GetInviteLink/{invitelink}")]
        public IActionResult GetInviteLink([FromRoute] string invitelink)
        {
            var user = _context.Users.Where(p=>p.deleted == null).FirstOrDefault(p=>p.Invite_Link == invitelink);
            if (user == null)
            {
                return NotFound("لینک اشتباه است");
            }
            else
            {
                user.Invite_Price += 1000;
                user.Invite_Link = null;
                _context.Users.Update(user);
                _context.SaveChanges();
                return Redirect("https://cafebazaar.ir/app/com.dakeh.app/");
            }
        }
        [HttpGet("GetUserPrice")]
        public IActionResult GetUserPrice()
        {
            string usertoken = HttpContext.Request?.Headers["Token"];
            var user = _context.Users.FirstOrDefault(u => u.token == usertoken);
            if (user == null)
            {

                return NotFound();
            }
            else
            {
                var userprice = user.Invite_Price;
                return Ok(userprice);
            }
        }
        [HttpGet("GetUserPriceWeb")]
        public IActionResult GetUserPriceWeb()
        {
            var usercell = User.Identity.Name;
            var user = _context.Users.FirstOrDefault(p=>p.cellphone == usercell && p.deleted == null);
            if (user == null)
            {

                return NotFound();
            }
            else
            {
                var userprice = user.Invite_Price;
                return Ok(userprice);
            }
        }
        [HttpPost]
        public IActionResult GenrateInvite()
        {
            Random random = new Random();
            string usertoken = HttpContext.Request?.Headers["Token"];
            var user = _context.Users.FirstOrDefault(p=>p.token == usertoken);
            if (user == null)
            {
                return NotFound();
            }
            else
            {
                user.Invite_Link = $"{random.Next(1000, 9999)}{user.cellphone}{random.Next(1000, 9999)}";
                _context.SaveChanges();
                return Ok(user.Invite_Link);
            }
        }
        [HttpPost("GenrateInviteForWeb")]
        public IActionResult GenrateInviteForWeb()
        {
            Random random = new Random();
            var usercell = User.Identity.Name;
            var user = _context.Users.FirstOrDefault(p=>p.cellphone == usercell && p.deleted == null);
            if (user == null)
            {
                return NotFound();
            }
            else
            {
                user.Invite_Link = $"{random.Next(1000, 9999)}{user.cellphone}{random.Next(1000, 9999)}";
                _context.SaveChanges();
                return Ok(user.Invite_Link);
            }
        }
        [HttpGet("GetInviteLinkForUser")]
        public IActionResult GetInviteLinkForUser()
        {
            string usertoken = HttpContext.Request?.Headers["Token"];
            var user = _context.Users.FirstOrDefault(u => u.token == usertoken);
            if (user == null)
            {

                return NotFound();
            }
            else
            {
                var userlink = user.Invite_Link;
                return Ok(userlink);
            }
        }
        [HttpGet("GetInviteLinkForUserweb")]
        public IActionResult GetInviteLinkForUserweb()
        {
            
            var usercell = User.Identity.Name;
            var user = _context.Users.FirstOrDefault(p=>p.cellphone == usercell && p.deleted == null);
            if (user == null)
            {

                return NotFound();
            }
            else
            {
                var userlink = user.Invite_Link;
                return Ok(userlink);
            }
        }
    }
}
