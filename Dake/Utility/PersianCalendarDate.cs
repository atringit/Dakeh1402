using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Dake.Utility
{
    public static class PersianCalendarDate
    {
        public static string PersianCalendarResult(DateTime? d)
        {
            PersianCalendar pc = new PersianCalendar();
            if (d == null)
                return "";
            else
            return pc.GetYear(d.Value) + "/" + pc.GetMonth(d.Value).ToString().PadLeft(2, '0') + "/" + pc.GetDayOfMonth(d.Value).ToString().PadLeft(2, '0');
        }
        public static string calculatDate(DateTime value)
        {
            DateTime dtNow = DateTime.Now;
            TimeSpan dt = (dtNow - value);
            string Text = "";
            if (dt.Days > 0)
            {
                Text += dt.Days + " روز ";
                Text += " قبل ";
                return Text;
            }
            if (dt.Hours > 0)
            {
                Text += dt.Hours + " ساعت ";
                Text += " قبل ";
                return Text;
            }
            if (dt.Minutes > 0)
            {
                Text += dt.Minutes + " دقیقه ";
                Text += " قبل ";
                return Text;
            }
            Text += " لحظاتی قبل ";
            return Text;
        }

        public static int LeftDayToExpire(DateTime? dateex)
        {
            DateTime dtNow = DateTime.Now;
            if (dateex == null)
            {
                return 0;
            }
            var leftdays = (int)Math.Ceiling((dateex - dtNow).Value.TotalDays);
            return leftdays;
        }

        public static DateTime ConvertShamsiToGareg(string shamsidate)
        {
            PersianCalendar pc = new PersianCalendar();
            var sDateArray = shamsidate.Split("/");
            DateTime dt = new DateTime(int.Parse(sDateArray[0]), int.Parse(sDateArray[1]), int.Parse(sDateArray[2]), pc);
            return dt;
            //Console.WriteLine(dt.ToString(CultureInfo.InvariantCulture));
        }
    }
}
