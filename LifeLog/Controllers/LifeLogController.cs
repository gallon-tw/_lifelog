using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LifeLog.Models;
using System.Data;
using System.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;


namespace LifeLog.Controllers
{
    public class LifeLogController : Controller
    {
        private MydbContext mydb = new MydbContext();
        string SessionKeyAdmin = "admin";

        [HttpGet]
        public IActionResult Index(string search_key,string shift_date)
        {
            string sql_string = "select a.*,b.file_name FROM [dbo].[lifelog] a LEFT JOIN [dbo].[weather_file_map] b on a.weather_code = b.weather_code  where 1=1 ";
            if (search_key != null) sql_string += " and (a.title like '%"+ search_key + "%' or a.content like '%" + search_key + "%') ";
            if (shift_date != null) sql_string += " and a.shift_date = CONVERT(DATE, '" + shift_date + "') ";
            sql_string += " order by a.created_at desc ";
            DataTable dt = mydb.find_by_sql(sql_string);
            
            if (HttpContext.Session.GetString(SessionKeyAdmin) == null) { HttpContext.Session.SetString(SessionKeyAdmin,"N"); }

            ViewBag.data = dt;
            ViewBag.admin = HttpContext.Session.GetString(SessionKeyAdmin);
            return View(); 
        }
      

        public IActionResult Create()
        {
            ViewBag.admin = HttpContext.Session.GetString(SessionKeyAdmin);
            return View();
        }
        [HttpPost]
        public IActionResult New(IFormCollection post)
        {
            string sql_string = "INSERT INTO [dbo].[lifelog]([id] ,[content] ,[title] ,[shift_date],[weather_code] ,[weather_text],[created_at],[updated_at],[location]) ";
            sql_string += "VALUES( next value for dbo.seq_lifelog , '" + post["content"] + "','" + post["title"] + "',CONVERT(DATE, '"+ post["shift_date"] + "'),'" + post["weather_code"] + "','" + post["weather_text"] + "', ";
            sql_string += " CONVERT(DATETIME, '" + post["shift_date"] + "'), CONVERT(DATETIME, '" + post["shift_date"] + "') , '" + post["location"] + "' ) ";

            mydb.update_by_sql(sql_string);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Edit(int? id)
        {
            DataTable dt = mydb.find_by_sql("select a.*,b.file_name FROM [dbo].[lifelog] a LEFT JOIN [dbo].[weather_file_map] b on a.weather_code = b.weather_code where a.id = " + id.ToString());
            ViewBag.data = dt;
            ViewBag.admin = HttpContext.Session.GetString(SessionKeyAdmin);
            return View();
        }

        [HttpPost]
        public IActionResult Update(IFormCollection post)
        {
            string sql_string = "UPDATE [dbo].[lifelog] SET title = '"+ post["title"] + "' , content = '"+ post["content"] + "' ";
            sql_string += " ,updated_at = CONVERT(DATETIME, '" + post["shift_date"] + "') ";
            sql_string += " where id = "+ post["id"] + " ";

            mydb.update_by_sql(sql_string);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Delete(int? id)
        {
            mydb.update_by_sql("DELETE FROM [dbo].[lifelog] where id = " + id);
            return RedirectToAction("Index");
        }

        public IActionResult Read(int? id)
        {

            DataTable dt = mydb.find_by_sql("select a.*,b.file_name FROM [dbo].[lifelog] a LEFT JOIN [dbo].[weather_file_map] b on a.weather_code = b.weather_code  where a.id = "+id+"");
      
            ViewBag.data = dt;
            ViewBag.admin = HttpContext.Session.GetString(SessionKeyAdmin);
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.SetString(SessionKeyAdmin, "N");
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult CheckUser(IFormCollection post)
        {
           
            DataTable dt = mydb.find_by_sql("select * FROM [dbo].[user] where user_name = '" + post["user_name"]+"' and  password = '"+ post["password"] + "'");
            if (dt.Rows.Count > 0) {
                HttpContext.Session.SetString(SessionKeyAdmin, "Y");
                return RedirectToAction("Index");
            }
            else
                return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult GetWeatherFileName(int? code)
        {
            DataTable dt = mydb.find_by_sql("SELECT [weather_code],[file_name] FROM [dbo].[weather_file_map] WHERE weather_code = " + code);
            return Json(new { code = dt.Rows[0]["weather_code"], file_name = dt.Rows[0]["file_name"] });
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


    }
}
