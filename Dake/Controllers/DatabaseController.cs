using System;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.Extensions.Configuration;
using Dake.Models.ViewModels;
using System.IO;
using Microsoft.CodeAnalysis.Options;

namespace Dake.Controllers
{
    public class DatabaseController : Controller
    {
        public IConfiguration _configuration;

        public DatabaseController(IConfiguration configuration)
        {
            _configuration = configuration;
        }





        SqlConnection con = new SqlConnection();
        SqlCommand sqlcmd = new SqlCommand();
        SqlDataAdapter da = new SqlDataAdapter();
        DataTable dt = new DataTable();

        public IActionResult Index(DbIndexVM db)
        {

            return View(db);

        }

        //[HttpPost]
        //public IActionResult GetBackUp()
        //{
        //    //IF SQL Server Authentication then Connection String  
        //    //con.ConnectionString = @"Server=MyPC\SqlServer2k8;database=" + YourDBName + ";uid=sa;pwd=password;";  

        //    //IF Window Authentication then Connection String  

        //    var connectionString = _configuration.GetConnectionString("DakeConnection");

        //    var dbName = _configuration.GetValue<string>("ExtraInfo:dbName");

        //    con.ConnectionString = connectionString;
        //    string dbbackNam = DateTime.Now.ToString("ddMMyyyy_HHmmss") + ".Bak'";
        //    string backupDIR = _configuration.GetValue<string>("ExtraInfo:backupDIR");
            

        //    if (!System.IO.Directory.Exists(backupDIR))
        //    {
        //        System.IO.Directory.CreateDirectory(backupDIR);
        //    }
        //    try
        //    {
        //        con.Open();
        //        sqlcmd = new SqlCommand("backup database " + dbName + " to disk='" + "\\" + dbbackNam, con);
        //        sqlcmd.ExecuteNonQuery();
        //        con.Close();

        //        DbIndexVM db = new DbIndexVM
        //        {
        //            IsSuccess = true,
        //            Message = "عملیات با موفقیت انجام شد و بک آپ دیتا بیس در مسیر " + backupDIR + " با نام " + dbbackNam + " ذخیره شد."
        //        };

        //        return RedirectToAction("Index", db);
        //    }
        //    catch (Exception ex)
        //    {
        //        ViewBag.Text = "Error Occured During DB backup process !<br>" + ex.ToString();

        //        DbIndexVM db = new DbIndexVM
        //        {
        //            IsSuccess = true,
        //            Message = ex.Message
        //        };
        //        return RedirectToAction("Index", db);
        //    }
        //}
    }
}
