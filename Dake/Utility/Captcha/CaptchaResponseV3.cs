using Newtonsoft.Json;
using System;

namespace Jumbula.WebSite.Utilities.Captcha
{
    public class CaptchaResponseV3
    {
        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("score")]
        public double Score { get; set; }

        [JsonProperty("action")]
        public string Action { get; set; }

        [JsonProperty("challenge_ts")]
        public DateTime ChallengeTimeStamp { get; set; }

        [JsonProperty("hostname")]
        public string HostName { get; set; }
    }
}
