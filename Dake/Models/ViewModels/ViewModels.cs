using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dake.Models.ViewModels
{
	    public class ComboBoxViewModel
		{
			public int id { get; set; }
			public string name { get; set; }
		     

		    public bool havenext { get; set; }

		}

	public class CategoryViewModel
	{
		public int id { get; set; }
		public string name { get; set; }

		public int? parentCategoryId { get; set; }
		public long expirePrice { get; set; }
		public string expirePriceCode { get; set; }
		public long espacialPrice { get; set; }
		public string espacialPriceCode { get; set; }
		public long registerPrice { get; set; }
		public string registerPriceCode { get; set; }
		public long ladderPrice { get; set; }
		public string ladderPriceCode { get; set; }

		public string image { get; set; }
	}
	public class CategoryViewModelHelper
	{
	
		public long expirePrice { get; set; }
		public string expirePriceCode { get; set; }
		public long espacialPrice { get; set; }
		public string espacialPriceCode { get; set; }
		public long registerPrice { get; set; }
		public string registerPriceCode { get; set; }
		public long ladderPrice { get; set; }
		public string ladderPriceCode { get; set; }
		public long emergencyPrice { get; set; }
		public string emergencyPriceCode { get; set; }
	}

	public class NoticeViewModelHelper
	{
		public long id { get; set; }
		public string image { get; set; }
		public string description { get; set; }
		public string title { get; set; }
		public EnumStatus adminConfirmStatus { get; set; }
		public long expireDate { get; set; }
		public object dailyVisit { get; set; }
		public bool isExpire { get; set; }
		public long expirePrice { get; set; }


		public bool isEmergency { get; set; }
		public long expireDateEmergency { get; set; }

		public string expirePriceCode { get; set; }
		public long espacialPrice { get; set; }
		public string espacialPriceCode { get; set; }
		public long registerPrice { get; set; }
		public string registerPriceCode { get; set; }
		public long ladderPrice { get; set; }
		public string ladderPriceCode { get; set; }
		public string link { get; set; }
		public int countView { get; set; }
		public bool IsFavorit { get; set; }
		public string areaName { get; set; }
		public string provinceName { get; set; }
		public string cityName { get; set; }
		public string code { get; set; }
		public long createDate { get; set; }
		public string categoryName { get; set; }
		public string user { get; set; }
		public long price { get; set; }
		public string movie { get; set; }
		public bool isSpecial { get; set; }
		public long lastPrice { get; set; }
		public int categoryId { get; set; }

		public int areaId { get; set; }

		public int cityId { get; set; }

		public int provinceId { get; set; }
		public int userId { get; set; }


		public int LeftDayToExpire { get; set; }
		public int LeftDayToExpireEmergency { get; set; }
		public int LeftDayToExpireSpecial { get; set; }
		public long emergencyPrice { get; set; }
		public string emergencyPriceCode { get; set; }

	}

}
