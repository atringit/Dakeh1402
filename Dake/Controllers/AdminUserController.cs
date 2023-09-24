using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Dake.DAL;
using Dake.Utility;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Dake.Service.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using Dake.Models;
using Microsoft.AspNetCore.Identity;
using Dake.ViewModel;
using Dake.Models.ApiDto;

namespace Dake.Controllers
{
    public class AdminUserController : Controller
    {
        private readonly Context _context;
        private IUser _user;
        private INotice _Notice;

        //private readonly UserManager<User> _userManager;
        public AdminUserController(Context context, IUser user,INotice notice)
        //,UserManager<User> userManager)
        {
            //_userManager = userManager;
            _context = context;
            _user = user;
            _Notice = notice;
        }
        //       [HttpGet]
        //public async Task<int> GetCurrentUserId()
        //{
        //	User usr = await GetCurrentUserAsync();
        //	return usr.id;
        //}

        //   private Task<User> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);
        public IActionResult Index(int page = 1, string filtercellphone = "")
        {
            var model = _user.GetAdminUsers(page, filtercellphone);
            return View(model);
        }
        //[Route("User/Login")]

        public ActionResult Login()
        {
            return View();
        }
        //[Route("get-captcha-image")]
        //public IActionResult GetCaptchaImage()
        //{
        //    int width = 100;
        //    int height = 36;
        //    var captchaCode = Captcha.GenerateCaptchaCode();
        //    var result = Captcha.GenerateCaptchaImage(width, height, captchaCode);
        //    HttpContext.Session.SetString("CaptchaCode", result.CaptchaCode);
        //    Stream s = new MemoryStream(result.CaptchaByteData);
        //    return new FileStreamResult(s, "image/png");
        //}

        public int Acsepted(string cellphone)
        {
            if (cellphone == null)
            {
                return 0;
            }
            var notice =  _context.Notices.Where(p => p.adminConfirmStatus == EnumStatus.Accept && p.AdminUserAccepted == cellphone).Count();
            return notice;
        }


        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            AdminUserViewModel adminUserViewModel = new AdminUserViewModel();
            adminUserViewModel.cellphone = user.cellphone;
            adminUserViewModel.passwordShow = user.passwordShow;
            adminUserViewModel.roleId = user.roleId;
            adminUserViewModel.oTPDate = user.oTPDate;
            adminUserViewModel.id = user.id;
            adminUserViewModel.adminRoleEdit = user.adminRole;

            ViewData["citys"] = new SelectList(_context.Cities, "id", "name");
            //ViewData["roles"] = new SelectList(_context.Roles, "id", "name");


            var userCitys = _context.AdminsInCities.Where(w => w.user.cellphone == user.cellphone).Select(s => s.cityId).ToList();
            adminUserViewModel.adminCitysEdit = string.Join(",", userCitys);

            return View(adminUserViewModel);
        }

        public IActionResult AdminUser()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AdminUser(UserAdmin userAdmin)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var user = _context.Users.Include(x => x.role).FirstOrDefault(x => x.role.RoleNameEn == "Admin" && x.passwordShow == userAdmin.passwordShow);
                    if (user == null)
                    {
                        ModelState.AddModelError("passwordShow", "رمز عبور فعلی نا معتبر است");
                        return View();
                    }
                    else
                    {
                        user.passwordShow = userAdmin.newPasswordShow;
                        user.password = BCrypt.Net.BCrypt.HashPassword(userAdmin.newPasswordShow, BCrypt.Net.BCrypt.GenerateSalt());
                        await _context.SaveChangesAsync();
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    throw;
                }
                ViewData["passwordChanged"] = "رمز عبور تغییر یافت";
                return View();
            }
            return View();
        }


       

        [HttpPost]
        public IActionResult InActive(int id)
        {
            try
            {
                var user = _context.Users.IgnoreQueryFilters().FirstOrDefault(x => x.id == id);
                //if (user.isActive == true)
                //    user.isActive = false;
                //else
                //    user.isActive = true;

                _context.SaveChanges();
                return Json("Done");
            }
            catch (Exception ex)
            {
                return Json("Fail");
            }
        }
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }
        [HttpPost]
        public IActionResult Delete(int id)
        {
            try
            {
                var user = _context.Users.FirstOrDefault(x => x.id == id);
                    user.deleted = "delete";
                _context.Update(user);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));

            }
            catch (Exception ex)
            {
                return RedirectToAction(nameof(Index));
            }
        }

        public IActionResult Create()
        {
            ViewData["citys"] = new SelectList(_context.Cities, "id", "name");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(AdminUserViewModel adminUserViewModel)
        {
            if (_context.Users.Any(x => x.cellphone == adminUserViewModel.cellphone && adminUserViewModel.deleted == null))
            {
                ModelState.AddModelError("ReapeteMobile", "شماره همراه تکراری است");

                return View(adminUserViewModel);
            }
            var user = new User();
            user.cellphone = adminUserViewModel.cellphone;
            user.oTPDate = DateTime.Now;
            user.passwordShow = adminUserViewModel.passwordShow;
            user.isCodeConfirmed = true;
            string roles = "";
            foreach (var item in adminUserViewModel.adminRole)
            {
                roles += item + ",";
            }
            roles=roles.Remove(roles.Length - 1);
            user.adminRole = roles;
            user.password = BCrypt.Net.BCrypt.HashPassword(adminUserViewModel.passwordShow, BCrypt.Net.BCrypt.GenerateSalt());
            Role r = _context.Roles.Where(p => p.RoleNameEn == "Admin").FirstOrDefault();
            user.role = r;
            _context.Users.Add(user);
           var res = _context.SaveChanges();

            ///تخصیص شهرها برای کاربر
            if(res > 0)
            {
                var userId = _context.Users.Where(w=> w.cellphone == adminUserViewModel.cellphone).Select(s=> s.id).FirstOrDefault();

                foreach (var cityId in adminUserViewModel.adminCitys)
                {
                    _context.AdminsInCities.Add(new AdminsInCity { cityId = cityId, userid = userId });
                }

                _context.SaveChanges();
            }
            return RedirectToAction(nameof(Index));
        }


        [HttpGet]
        public IActionResult GetNoticeCount(string fromd, string tod)
        {
            var curentUser = HttpContext.User.FindFirst(ClaimTypes.MobilePhone).Value; // کاربر جاری

            var cnt = _Notice.GetAdminAcceptedNoticeCount(fromd, tod, curentUser);

            return Json(cnt);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(AdminUserViewModel adminUserViewModel)
        {
            
            var user = _context.Users.Find(adminUserViewModel.id);
            user.passwordShow = adminUserViewModel.passwordShow;
            string roles = "";
            foreach (var item in adminUserViewModel.adminRole)
            {
                roles += item + ",";
            }
            roles=roles.Remove(roles.Length - 1);
            user.adminRole = roles;
            user.password = BCrypt.Net.BCrypt.HashPassword(adminUserViewModel.passwordShow, BCrypt.Net.BCrypt.GenerateSalt());

            ///تخصیص شهرها برای کاربر
           // if (res > 0)
            {
             //   var userId = _context.Users.Where(w => w.cellphone == adminUserViewModel.cellphone).Select(s => s.id).FirstOrDefault();

                foreach (var cityId in adminUserViewModel.adminCitys)
                {
                    _context.AdminsInCities.Add(new AdminsInCity { cityId = cityId, userid = user.id });
                }

                _context.SaveChanges();
            }

            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

    }
}