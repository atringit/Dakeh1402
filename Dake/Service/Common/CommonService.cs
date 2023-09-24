using SmsIrRestfulNetCore;
using System;
using System.Collections.Generic;

namespace Dake.Service.Common
{
    public static class CommonService
    {

        /// <summary>
        /// ارسال پیامک 
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static bool SendSMS(string phoneNumber, int TemplateId)
        {
            if (phoneNumber != null)
            {
                SmsIrRestfulNetCore.Token tokenInstance2 = new SmsIrRestfulNetCore.Token();
                //var token2 = tokenInstance2.GetToken("907587e455e6dab6987387e4", "Qw78!@147Qe&");
                var token2 = tokenInstance2.GetToken("3ca938cd34f116822bad458f", "Atrin2020");

                var ultraFastSend2 = new UltraFastSend()
                {
                    Mobile = Convert.ToInt64(phoneNumber),
                    TemplateId = TemplateId,
                    ParameterArray = new List<UltraFastParameters>()
                      {
                    new UltraFastParameters()
                          {
                          Parameter = "Phonenumber" , ParameterValue =phoneNumber

                           }
                         }.ToArray()
                };

                UltraFastSendRespone ultraFastSendRespone2 = new UltraFast().Send(token2, ultraFastSend2);
                return ultraFastSendRespone2.IsSuccessful;
            }
            return false;
        }


        /// <summary>
        /// ارسال پیامک تایید آگهی
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <param name="noticeTitle">عنوان آگهی</param>
        /// <returns></returns>
        public static bool SendSMS_Accept(string phoneNumber, string noticeTitle)
        {
            if (phoneNumber != null)
            {
                try
                {
                    SmsIrRestfulNetCore.Token tokenInstance = new SmsIrRestfulNetCore.Token();
                    var token = tokenInstance.GetToken("3ca938cd34f116822bad458f", "Atrin2020");
                    var ultraFastSend = new UltraFastSend()
                    {
                        Mobile = Convert.ToInt64(phoneNumber),
                        TemplateId = 60218,
                        ParameterArray = new List<UltraFastParameters>()
                      {
                    new UltraFastParameters()
                          {
                          Parameter = "Title" , ParameterValue = noticeTitle

                           }
                         }.ToArray()

                    };

                    UltraFastSendRespone ultraFastSendRespone = new UltraFast().Send(token, ultraFastSend);

                    return ultraFastSendRespone.IsSuccessful;

                }
                catch (Exception e)
                {
                    return false;

                }
            }
            return false;
        }

        /// <summary>
        /// ارسال پیامک عدم تایید
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <param name="message"></param>
        /// <returns></returns>
          public static bool SendSMS_NoAccept(string phoneNumber, string noticeTitle, string description)
        {
            if (phoneNumber != null)
            {
                try
                {
                    SmsIrRestfulNetCore.Token tokenInstance = new SmsIrRestfulNetCore.Token();
                    var token = tokenInstance.GetToken("3ca938cd34f116822bad458f", "Atrin2020");
                    var ultraFastSend = new UltraFastSend()
                    {
                        Mobile = Convert.ToInt64(phoneNumber),
                        TemplateId = 60219,
                        ParameterArray = new List<UltraFastParameters>()
                      {
                    new UltraFastParameters()
                          {
                          Parameter = "Title" , ParameterValue = noticeTitle

                           },
                     new UltraFastParameters()
                          {
                          Parameter = "Description" , ParameterValue = description

                           },

                         }.ToArray()

                    };

                    UltraFastSendRespone ultraFastSendRespone = new UltraFast().Send(token, ultraFastSend);

                    return ultraFastSendRespone.IsSuccessful;

                }
                catch (Exception e)
                {
                    return false;

                }
            }
            return false;
        }


    }
}
