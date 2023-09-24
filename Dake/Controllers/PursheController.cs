using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Dake.DAL;
using Dake.Models;
using Dake.Utility;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Dake.Controllers
{
    public class PursheController : Controller
    {
        private readonly Context _context;


        public PursheController(Context context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Index(PaymentRequest request)
        {
            if (!ModelState.IsValid)
                return View(request);
            try
            {
                var res = PaymentHelper.SendRequest(1, 20000, "http://dakeh.net/TestPurshe/Verify");
                if (res != null && res.Result != null)
                {
                    if (res.Result.ResCode == "1029")
                    {
                    Response.Redirect(string.Format("{0}/Purchase/Index?token={1}", PaymentHelper.PurchasePage, res.Result.Token));
                    }
                    ViewBag.Message = res.Result.Description;
                    return View(); ;
                }
            }
            catch (Exception ex)
            {
             ViewBag.Message = ex.ToString();
            }
            return View();
        }
        [HttpPost]
        public ActionResult Verify(PurchaseResult result)
        {
            return View(result);
        }
        [HttpPost]
        public ActionResult VerifyRequest(PurchaseResult result)
        {
            try
            {
                var dataBytes = Encoding.UTF8.GetBytes(result.Token);
                var symmetric = SymmetricAlgorithm.Create("TripleDes");
                symmetric.Mode = CipherMode.ECB;
                symmetric.Padding = PaddingMode.PKCS7;
                var encryptor = symmetric.CreateEncryptor(Convert.FromBase64String("azuXOm5p545cHNfwfTgrxWUoE4HHBwBQ"), new byte[8]);
                var signedData = Convert.ToBase64String(encryptor.TransformFinalBlock(dataBytes, 0, dataBytes.Length));
                var data = new{token = result.Token,SignData = signedData};
                var ipgUri = string.Format("{0}/api/v0/Advice/Verify","https://sadad.shaparak.ir");
                var res = CallApi<VerifyResultData>(ipgUri, data);
                if (res != null && res.Result != null)
                {
                    if (res.Result.ResCode == "0")
                    {
                        result.VerifyResultData = res.Result;
                        res.Result.Succeed = true;
                        //ChangeNoticePay
                        int _orderid = int.Parse(result.OrderId);
                        PaymentRequestAttemp payment = _context.PaymentRequestAttemps.Where(s => s.Id == _orderid).FirstOrDefault();
                        if(payment != null)
                        {
                            if(payment.pursheType == pursheType.RegisterNotice)
                            {
                            Notice noticeitem = _context.Notices.Where(s => s.id == payment.NoticeId).FirstOrDefault();
                            if (noticeitem != null)
                            {
                                noticeitem.isPaid = true;
                               TempData["PursheResult"] = "کاربر گرامی ، عملیات پرداخت موفقیت آمیز بود ، پس از برسی ادمین ، آگهی شما قابل مشاهده می باشد.";
                                }
                            }
                            else if (payment.pursheType == pursheType.Ladders)
                            {
                                Notice noticeitem = _context.Notices.Where(s => s.id == payment.NoticeId).FirstOrDefault();
                                if (noticeitem != null)
                                {
                                    noticeitem.createDate = DateTime.Now;
                                    TempData["PursheResult"] = $"کاربرگرامی ، عملیات نردبان کردن آگهی {noticeitem.title} با موفقیت انجام شد";
                                }
                            }
                            else if(payment.pursheType == pursheType.Extend)
                            {
                                Notice noticeitem = _context.Notices.Where(s => s.id == payment.NoticeId).FirstOrDefault();
                                if (noticeitem != null)
                                {
                                    var setting = _context.Settings.FirstOrDefault();
                                    noticeitem.expireDate = noticeitem.expireDate.AddDays(Convert.ToInt64(setting.countExpireDate));
                                    var daysofextend = setting.countExpireDate == null ? 0 : setting.countExpireDate;
                                    TempData["PursheResult"] = $"کاربر گرامی ، عملیات تمدید آگهی {noticeitem.title} به مدت {daysofextend} روز با موفقیت انجام شد";
                                }
                            }
                            else if(payment.pursheType == pursheType.Special)
                            {
                                Notice noticeitem = _context.Notices.Where(s => s.id == payment.NoticeId).FirstOrDefault();
                                if (noticeitem != null)
                                {
                                    noticeitem.isSpecial = true;
                                    TempData["PursheResult"] = $"کاربر گرامی ، اکنون آگهی {noticeitem.title} جزو آگهی های ویژه است ";

                                }

                            }
                            _context.SaveChanges();
                        }

   
                        return RedirectToAction("../Home2/Profile2");
                    }
                    ViewBag.Message = res.Result.Description;
                    TempData["ErrorPursheResult"] = "متاسفانه عملیات پرداخت موفقیت آمیز نبود";

                    return View("../Home2/Profile2");
                }
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.ToString();
            }
            return View("Verify", result);
        }
        public static async Task<T> CallApi<T>(string apiUrl, object value)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(apiUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                var w = client.PostAsJsonAsync(apiUrl, value);
                w.Wait();
                HttpResponseMessage response = w.Result;
                if (response.IsSuccessStatusCode)
                {
                    var result = response.Content.ReadAsAsync<T>();
                    result.Wait();
                    return result.Result;
                }
                return default(T);
            }
        }
    }
}