using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Dake.DAL;
using Dake.Models;
using Dake.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using ClosedXML.Excel;
using System.IO;
using System.Data;
using DocumentFormat.OpenXml.Office.CustomUI;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Bibliography;

namespace Dake.Controllers
{
	[Authorize]

	public class FactorController : Controller
	{
		private readonly Context _context;
		private IFactor _factor;

		public FactorController(Context context, IFactor factor)
		{
			_context = context;
			_factor = factor;
		}

		// GET: Information
		public IActionResult Index(int page = 1, string filterTitle = "")
		{
			var model = _factor.GetFactors(page);
			return View(model);
		}
		public IActionResult HistoryItems(int factorId)
		{
			var model = _context.FactorItems.Include(x => x.Product).Include(x => x.Factor).Where(x => x.FactorId == factorId).ToList();

			return View(model);
		}
		public IActionResult AdvisorDetail(int factorId)
		{
			var model = _context.Factors.Include(x => x.notice).FirstOrDefault(x => x.id == factorId);
			return View(model);
		}
		private DataTable GetEmpData()
		{
			var Factorlist = _context.Factors.Include(x => x.notice).Include(x => x.user).OrderByDescending(x => x.id).ToList();
			DataTable data = new DataTable();
			data.TableName = "EmpData";
			data.Columns.Add("کد", typeof(string));
			data.Columns.Add("شماره همراه", typeof(string));
			data.Columns.Add("آگهی", typeof(string));
			data.Columns.Add("قیمت کل", typeof(string));
			data.Columns.Add("تاریخ", typeof(string));
			data.Columns.Add("نوع", typeof(string));
			
			if (Factorlist.Count() > 0)
			{
				int index = 0;
				Factorlist.ForEach(Item =>
				{
					var title = "";
					var price = "";
					var created = "";
					var type = "";
					if (Factorlist[index].notice != null)
					{
						if (Factorlist[index].notice.title != null)
							title = Factorlist[index].notice.title;

						if (Factorlist[index].notice.price != null)
							price = Factorlist[index].notice.price.ToString();
						if(Factorlist[index].notice.createDate != null)
							created = Utility.PersianCalendarDate.PersianCalendarResult(Factorlist[index].notice.createDate);
					}
					if(Factorlist[index].bannerId == null)
					{
						type = "عادی";
					}
					else
					{
						type = "بنری";
					}
					
					data.Rows.Add(Factorlist[index].id, Factorlist[index].user.cellphone, title, price, created, type);
					index++;
				});
			}
			return data;
		}
		[HttpPost]
		public IActionResult Print()
		{
			var emp = GetEmpData();
			using(XLWorkbook wb= new XLWorkbook())
			{
				wb.AddWorksheet(emp, "employee Records");
				using(MemoryStream ms = new MemoryStream())
				{
					wb.SaveAs(ms);
					return File(ms.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" , "Sample.xlsx");
				}
			}
			
			//string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
			//string fileName = "authors.xlsx";
			//var Factorlist = _context.Factors.Include(s => s.user).Include(s => s.notice).ToList();
			//var builder = new System.Text.StringBuilder();
			//using (var workbook = new XLWorkbook())
			//{
			//	IXLWorksheet worksheet =
			//	workbook.Worksheets.Add("Authors");
			//	worksheet.Cell(1, 1).Value = "کد";
			//	worksheet.Cell(1, 2).Value = "شماره همراه";
			//	worksheet.Cell(1, 3).Value = "آگهی";
			//	worksheet.Cell(1, 4).Value = "قیمت کل";
			//	worksheet.Cell(1, 5).Value = "تاریخ";
			//	worksheet.Cell(1, 6).Value = "نوع";

			//	for (int index = 1; index <= Factorlist.Count; index++)
			//	{
			//		worksheet.Cell(index + 1, 1).Value =
			//		Factorlist[index - 1].id;
			//		worksheet.Cell(index + 1, 2).Value =
			//		Factorlist[index - 1].user.cellphone;
			//		worksheet.Cell(index + 1, 3).Value =
			//		Factorlist[index - 1].notice.title;
			//		worksheet.Cell(index + 1, 4).Value =
			//		Factorlist[index - 1].notice.price;
			//		worksheet.Cell(index + 1, 5).Value = Utility.PersianCalendarDate.PersianCalendarResult(Factorlist[index - 1].notice.createDate);
			//		worksheet.Cell(index + 1, 6).Value = Factorlist[index - 1].factorKind == FactorKind.Add ? "جدید" : Factorlist[index - 1].factorKind == FactorKind.Extend ? "تمدید" : Factorlist[index - 1].factorKind == FactorKind.Ladder ? "نردبان" : Factorlist[index - 1].factorKind == FactorKind.Special ? "ویژه" : "نا مشخص";
			
			//	}
			//	using (var stream = new MemoryStream())
			//	{
			//		workbook.SaveAs(stream);
			//		var content = stream.ToArray();
			//		return File(content, contentType, fileName);
			//	}


			
		}
	}
}
