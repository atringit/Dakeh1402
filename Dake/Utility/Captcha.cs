using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dake.Utility
{
    public class Captcha
    {
        public string ValueString { get; set; }

        public bool AttemptSucceeded { get; set; }

        public bool AttemptFailed { get; set; }

        public string AttemptFailedMessage { get; set; }
    }

    [Serializable]
    public class CaptchaValue
    {
        public string Value { get; set; }

        public DateTime FirstTimeAttempted { get; set; }

        public DateTime LastTimeAttempted { get; set; }

        public int NumberOfTimesAttempted { get; set; }
    }
}
