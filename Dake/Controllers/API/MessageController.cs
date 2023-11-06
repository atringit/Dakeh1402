using Dake.DAL;
using Dake.Models;
using Dake.Models.ApiDto;
using Dake.Models.ViewModels;
using Dake.Service;
using Dake.Service.Interface;
using Dake.Utility;
using Dake.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Internal;
using SmsIrRestfulNetCore;
using FirebaseAdmin.Messaging;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;

namespace Dake.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        private IMessage _message;
        private readonly Context _context;
        private readonly IPushNotificationService _pushNotificationService;

        public MessageController(IMessage message, Context context, IPushNotificationService pushNotificationService)
        {
            _message = message;
            _context = context;
            _pushNotificationService = pushNotificationService;
        }
        
        [HttpPost("AddMessage")]
        public async Task<IActionResult> AddMessage([FromBody] AddMessageDto dto)
        {
            
            var user = _context.Users.FirstOrDefault(u => u.cellphone == dto.phone && u.deleted == null);
            var receiver = _context.Users.FirstOrDefault(u => u.cellphone == dto.receiverPhone && u.deleted == null);

            Models.Message message = new Models.Message()
            {
                text = dto.text,
                ItemId = dto.itemId,
                MessageType =dto.messageType == 0 ? MessageType.Notice : MessageType.CrashReport,
                rreceiverId = receiver.id,
                ssenderId = user.id,
                date = DateTime.Now
            };

            if(user.IsBlocked != true)
            {
				_context.Messages.Add(message);
				_context.SaveChanges();
			}
            else
            {
                return new JsonResult("شما قادر به ارسال پیام نمیباشید");

			}

            if (dto.messageType == 0 )
            {
                var notice = _context.Notices.Where(p => p.id == dto.itemId).FirstOrDefault();
                if (receiver.id == notice.userId)
                {
                    
                   // var _VmPushNotification = new VmPushNotification
                    //{
                      //  Body = $"{user.cellphone} بر روی آگهی شما پیام گذاشت",
                    //    Title = notice.title,
                        //Url = "https://dakeh.net",
                      //  UserId = notice.userId
                    //};
                   // await _pushNotificationService.SendNotifToSpecialUser(_VmPushNotification);
                    
                    if (FirebaseApp.DefaultInstance == null)
                    {
                        FirebaseApp.Create(new AppOptions()
                        {
                            Credential = GoogleCredential.FromFile("./../Dake/wwwroot/FireBase/key.json"),
                        });
                    }
                    var userr = _context.Users.FirstOrDefault(p => p.id == notice.userId);
                    var messagee = new FirebaseAdmin.Messaging.Message()
                    {
                        Notification = new Notification
                        {
                            Title = notice.title,
                            Body = $"{user.cellphone} بر روی آگهی شما پیام گذاشت",


                        },
                        Token = userr.PushNotifToken
                        
                    };

                    // Send the message
                    var response = await FirebaseMessaging.DefaultInstance.SendAsync(messagee);
                }
                else if (user.id == notice.userId)
                {
                    //var _VmPushNotification = new VmPushNotification
                    //{
                    //    Body = $"{user.cellphone} پاسخ شما را داد",
                    //    Title = notice.title,
                    //    Url = "https://dakeh.net",
                    //    UserId = receiver.id
                    //};
                    //await _pushNotificationService.SendNotifToSpecialUser(_VmPushNotification);
                    if (FirebaseApp.DefaultInstance == null)
                    {
                        FirebaseApp.Create(new AppOptions()
                        {
                            Credential = GoogleCredential.FromFile("./../Dake/wwwroot/FireBase/key.json"),
                        });
                    }
                    
                    var messagee = new FirebaseAdmin.Messaging.Message()
                    {
                        Notification = new Notification
                        {
                            Title = notice.title,
                            Body = $"{user.cellphone} پاسخ شما را داد",


                        },
                        Token = receiver.PushNotifToken

                    };

                    // Send the message
                    var response = await FirebaseMessaging.DefaultInstance.SendAsync(messagee);
                }
            }


            return new JsonResult(message);
        }
        
        [HttpPost("DeleteAllNoticeMessageWithSpecifyUser")]
        public async Task<object> DeleteAllNoticeMessageWithSpecifyUser([FromBody] DeleteMessagDto model)
        {
            List<Models.Message> messages;
            if (_context.Users.Any(u => u.cellphone == model.phone))
            {
                int userId = _context.Users.Single(u => u.cellphone == model.phone).id;
            }
            else
            {
                return new { status = false, message = $"User with phone number {model.phone} does not exist" };
            }
            if (_context.Messages.Any(p => p.MessageType == MessageType.Notice
                            && p.ItemId == model.itemId && p.ssenderId == model.SenderUserId))
            {
                messages = _context.Messages.Where(p => p.MessageType == MessageType.Notice
                            && p.ItemId == model.itemId && p.ssenderId == model.SenderUserId).ToList();
            }
            else
            {
                return new { status = false, message = "Info not found" };
            }
            _context.Messages.RemoveRange(messages);
            await _context.SaveChangesAsync();
            return new { status = true, message = "Success"};
        }

        [HttpPost("DeleteAllCrashReportMessageWithSpecifyUser")]
        public async Task<object> DeleteAllCrashReportMessageWithSpecifyUser([FromBody] DeleteMessagDto model)
        {
            List<Models.Message> messages;
            if (_context.Users.Any(u => u.cellphone == model.phone))
            {
                int userId = _context.Users.Single(u => u.cellphone == model.phone).id;
            }
            else
            {
                return new { status = false, message = $"User with phone number {model.phone} does not exist" };

            }
            if (_context.Messages.Any(p => p.MessageType == MessageType.CrashReport
                            && p.ItemId == model.itemId && p.ssenderId == model.SenderUserId))
            {
                messages = _context.Messages.Where(p => p.MessageType == MessageType.CrashReport
                            && p.ItemId == model.itemId && p.ssenderId == model.SenderUserId).ToList();
            }
            else
            {
                return new { status = false, message = "Info not found" };
            }
            _context.Messages.RemoveRange(messages);
            await _context.SaveChangesAsync();
            return new { status = true, message = "Success" };
        }

        
        [HttpGet("GetNoticeMessage/{noticeId}")]
        public IActionResult GetNoticeMessage([FromRoute] int noticeId)
        {
            string Token 
                = HttpContext.Request?.Headers["Token"];
            var user = _context.Users.Where(p => p.token == Token).FirstOrDefault();
            var result = _context.Messages
                .Where(n => n.ItemId == noticeId).Where(n => n.ssenderId == user.id || n.rreceiverId == user.id)
                .Select(n => new GeNoticeMessageDTO
                {
                    Date = n.date.ToPersianDateForMessage(),
                    senderId = n.ssenderId,
                    userCellPhone = user.cellphone,
                    senderPhone = n.sender.cellphone,
                    id = n.id,
                    recieverPhone = _context.Users.FirstOrDefault(p=>p.id == n.rreceiverId).cellphone,
                    itemId = n.ItemId,
                    //shouldGetPhone = n.receiver.cellphone == user.cellphone ? n.sender.cellphone : n.receiver.cellphone,
                    //shouldGetId = n.receiver.cellphone == user.cellphone ? n.sender.id : n.receiver.id,
                    recieverId = n.rreceiverId,
                    text = n.text,
                })//unique by user id and last record of each user
                .Distinct((p, p1) => p.recieverId != p1.recieverId && p.senderId != p1.senderId).Distinct((p, p1) => p.recieverId != p1.senderId).GroupBy(n => n.senderId)
                .Select(n => n.OrderByDescending(m => m.id).FirstOrDefault())
                .ToList();

            if (result == null)
            {
                return NotFound();
            }

            return Ok(new { data = result });
        }


        [HttpGet("GetNoticeMessageByUserId/{noticeId}/{userId}")]
        public dynamic GetNoticeMessageByUserId([FromRoute] int noticeId, [FromRoute] int userId)
        {
            
            string Token 
                = HttpContext.Request?.Headers["Token"];
            var user = _context.Users.Where(p => p.token == Token).FirstOrDefault();
            
            var result = _context.Messages
                .Where(n => n.ItemId == noticeId).Where(n => 
                    (n.ssenderId == userId && n.rreceiverId == user.id) ||
                    (n.ssenderId == user.id && n.rreceiverId == userId))
                .Select(n => new
                {
                    noticeId = n.ItemId,
                    userId,
                    userId2 = user.id,
                    senderId = n.ssenderId,
                    text = n.text,
                    userPhone = n.sender.cellphone,
                })
                .ToList();

            if (result == null)
            {
                return NotFound();
            }

            return Ok(new { data = result });
        }


        [HttpGet("GetUserNoticeMessage/{userId}")]
        public object GetUserNoticeMessage([FromRoute] int senderId)
        {
            var result = _context.Messages
                .Where(n => n.ssenderId == senderId)
                .Select(n => new
                {
                    ItemId = n.ItemId,
                    n.MessageType,
                    senderId = n.ssenderId,
                    text = n.text,
                    userPhone = n.sender.cellphone,
                }).ToList();

            if (result == null)
            {
                return new { status = false, message = "Info not found" };
            }

            return result;
        }

        [HttpGet("GetCrashReportMessage/{itemId}")]
        public object GetCrashReportMessage([FromRoute] int itemId)
        {
            var result = _context.Messages
                .Where(n => n.MessageType == MessageType.CrashReport
                            && n.ItemId == itemId)
                .Select(n => new
                {
                    ItemId = n.ItemId,
                    n.MessageType,
                    senderId = n.ssenderId,
                    text = n.text,
                    userPhone = n.sender.cellphone,
                })
                .ToList();

            if (result == null)
            {
                return new { status = false, message = "Info not found" };
            }

            return result;
        }

        [HttpGet("GetAllMessageUser/{phone}")]
        public object GetAllMessageUser([FromRoute] string phone)
        {
            //2207
            int userId = _context.Users.Single(u => u.cellphone == phone && u.deleted == null).id;
            var result = _context.Messages
                .Where(m => m.ssenderId == userId || m.Notice.userId == userId)
                .Select(s => new
                {
                    id = s.id,
                    text = s.text,
                    senderId = s.ssenderId,
                    ItemId = s.ItemId,
                    s.MessageType,
                    date = s.date,
                    noticeTitle = s.MessageType == MessageType.Notice ? _context.Notices.FirstOrDefault(n => n.id == s.ItemId).title : "",
                    crashReport = s.MessageType == MessageType.CrashReport ? _context.ReportNotices.FirstOrDefault(x => x.id == s.ItemId).message : ""
                })
                .OrderByDescending(s => s.date)
                .GroupBy(m => m.ItemId)
                .ToList();

            if (result == null)
            {
                return new { status = false, message = "Info not found" };
            }
            return result;
        }

        [HttpGet("{senderId}")]
        public object GetAllMessageSendedUser([FromRoute] int senderId)
        {
            var result = _context.Messages
                .Where(m => m.ssenderId == senderId);

            return result;
        }

        [HttpPost("GetNoticeMessageUser/{noticeId}/{senderId}")]
        public object GetNoticeMessageUser([FromBody] GetNoticeMessageUserDto dto)
        {
            var result = _context.Messages
                .Where(m => m.MessageType == MessageType.Notice && m.ItemId == dto.noticeId && m.ssenderId == dto.senderId).ToList();


            return result;
        }

        [HttpPost("GetCrashReportMessageUser/{noticeId}/{senderId}")]
        public object GetCrashReportMessageUser([FromBody] GetCrashReportMessageUser dto)
        {
            var result = _context.Messages
                .Where(m => m.MessageType == MessageType.CrashReport && m.ItemId == dto.reportId && m.ssenderId == dto.senderId).ToList();


            return result;
        }
        [HttpPost("RepMessgae/{id}")]
        public async Task<IActionResult> RepMessgae(int id)
        {
            string Token
                = HttpContext.Request?.Headers["Token"];
            var user = _context.Users.Where(p => p.token == Token).FirstOrDefault();
            IQueryable<Models.Message> m = _context.Messages;
            var message = m.FirstOrDefault(p=> p.id == id);
            if (message.rreceiverId != user.id)
            {
                return BadRequest();
            }
            message.isrep = "YES";
            _context.Messages.Update(message);
            await _context.SaveChangesAsync();
            return Ok("گذارش ثبت شد");
        }
    }

}
