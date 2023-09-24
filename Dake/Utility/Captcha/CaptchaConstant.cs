namespace Jumbula.WebSite.Utilities.Captcha
{
    public class CaptchaConstant
    {
        public static string SecretKey { get; set; }
        public const string MissingInputSecret = "missing-input-secret";
        public const string InvalidInputSecret = "invalid-input-secret";
        public const string MissingInputResponse = "missing-input-response";
        public const string InvalidInputResponse = "invalid-input-response";
        public const string RecaptchaUrl = "https://www.google.com/recaptcha/api/siteverify";
    }
}
