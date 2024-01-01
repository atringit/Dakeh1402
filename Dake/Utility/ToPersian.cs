using System.Globalization;
using System;

namespace Dake.Utility
{
	public static class ToPersian
	{
		public static string ToPersians(this DateTime date)
		{
			PersianCalendar persianCalendar = new PersianCalendar();
			try
			{
				return persianCalendar.GetYear(date) + "/" + persianCalendar.GetMonth(date) + "/" +
					   persianCalendar.GetDayOfMonth(date);
			}
			catch (Exception e)
			{
				throw;
			}
		}
	}
}
