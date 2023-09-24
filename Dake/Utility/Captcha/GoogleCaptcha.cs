using Newtonsoft.Json;
using System.Net;

namespace Jumbula.WebSite.Utilities.Captcha
{
    public class GoogleCaptcha
    {
        public static CaptchaResult ValidateCaptcha(string response, string secret)
        {
            var client = new WebClient();
            var reply = client.DownloadString($"{CaptchaConstant.RecaptchaUrl}?secret={secret}&response={response}");

            var captchaResponse = JsonConvert.DeserializeObject<CaptchaResponse>(reply);

            //when response is false check for the error message
            if (!captchaResponse.Success)
            {
                if (captchaResponse.ErrorCodes.Count <= 0) return new CaptchaResult { Status = false, Message = "Validation error." };
                var error = captchaResponse.ErrorCodes[0].ToLower();
                string errorMessage;

                switch (error)
                {
                    case CaptchaConstant.MissingInputSecret:
                        errorMessage = "The secret parameter is missing.";
                        break;
                    case CaptchaConstant.InvalidInputSecret:
                        errorMessage = "The secret parameter is invalid or malformed.";
                        break;
                    case CaptchaConstant.MissingInputResponse:
                        errorMessage = "The CAPTCHA response is missing.";
                        break;
                    case CaptchaConstant.InvalidInputResponse:
                        errorMessage = "The response parameter is invalid or malformed.";
                        break;
                    default:
                        errorMessage = "Error occured. Please try again";
                        break;
                }

                return new CaptchaResult { Status = false, Message = errorMessage };
            }
            else
            {
                return new CaptchaResult { Status = true, Message = "Validation success." };
            }
        }

        public static CaptchaResponseV3 ValidateCaptchaV3(string token, string secretKey)
        {
            var reply = new WebClient().DownloadString($"{CaptchaConstant.RecaptchaUrl}?secret={secretKey}&response={token}");

            var captchaResponse = JsonConvert.DeserializeObject<CaptchaResponseV3>(reply);

            return captchaResponse;
        }


    }
}
