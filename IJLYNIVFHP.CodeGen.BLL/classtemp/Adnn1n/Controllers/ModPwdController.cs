using Niunan.CardShop.Util;
using Niunan.CardShop.Web.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Niunan.CardShop.Web.Areas.Adnn1n.Controllers
{
    public class ModPwdController : BaseController
    {
        // GET: Adnn1n/ModPwd
        public ActionResult Index()
        {
            return View();
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Index(string spwd, string npwd, string npwd2) {
            string returnurl = "/Adnn1n/ModPwd/Index";
            if (npwd!=npwd2)
            {
                Tool.AlertAndGo_MVC("二次输入的密码不相同",returnurl);
                return null;
            }
            Model.Admin a = base.GetLoginAdmin();
            if (a==null)
            {
                Tool.AlertAndGo_MVC("请重新登录", returnurl);
                return null;

            }
            spwd = Tool.MD5(a.username + spwd + "cardshop");
            if (spwd!=a.password)
            {
                Tool.AlertAndGo_MVC("原密码不正确", returnurl);
                return null;
            }
            npwd = Tool.MD5(a.username + npwd + "cardshop");
            a.password = npwd;
            new DAL.AdminDAL().UpdateByCond($"password='{a.password}'",$"id={a.id}");
            Tool.AlertAndGo_MVC("密码修改成功", returnurl);
            return null;
        }
    }
}