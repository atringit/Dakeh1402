using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dake.DAL;
using Dake.Models;
using Dake.Service.Interface;
using Dake.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using PagedList.Core;

namespace Dake.Controllers
{
    [Produces("application/json")]
     [Route("api/captcha")]
    public class CaptchaController : Controller
{
        //private readonly CaptchaHelper _captchaHelper;

        //public CaptchaController(CaptchaHelper captchaHelper)
        //{
        //    this._captchaHelper = captchaHelper;
        //}

        [HttpGet]
       
    public IActionResult GetCaptchaActionResult(string captchaName)
    {
            CaptchaHelper captchaHelper = new CaptchaHelper();
        var newCaptcha = captchaHelper.CreateNewCaptcha(5);
        var newCaptchaImage = captchaHelper.CreateCaptchaImage(newCaptcha);

        //Return the captcha cookie
        this.SetCaptcha(newCaptcha, captchaName);

        return this.Ok(newCaptchaImage);
    }

    [HttpPost]
    [Route("delete")]
    public IActionResult DeleteCaptchaActionResult(string captchaName)
    {
        this.DeleteCaptcha(captchaName);

        return this.Ok();
    }

    private void SetCaptcha(Captcha captcha, string captchaName)
    {
        //I used Newtonsoft JSON.net to serialize
        this.Response.Cookies.Append(captchaName, JsonConvert.SerializeObject(captcha, Formatting.None, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() }));
    }

    private void DeleteCaptcha(string captchaName)
    {
        this.Response.Cookies.Delete(captchaName);
    }
}

}