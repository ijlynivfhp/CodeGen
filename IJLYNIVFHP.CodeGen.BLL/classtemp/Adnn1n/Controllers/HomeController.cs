using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Niunan.CardShop.Web.Areas.Adnn1n.Controllers
{
    public class HomeController : Controller
    {
        /// <summary>
        /// 后台首页
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
 
            return View();
        }

        public ActionResult Top()
        {
            ViewBag.username = "admin";
            return View();
        }
        public ActionResult Left() { return View(); }
        public ActionResult Welcome() { return View(); }

  /// <summary>生成验证码
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult GenYZM()
        {

              string checkCode = Tool.GenRandomCode("1234567890abcdefgh", 6); // 产生5位随机字符  
            Session["Code"] = checkCode; //将字符串保存到Session中，以便需要时进行验证  
            System.Drawing.Bitmap image = new System.Drawing.Bitmap(70, 20);
            Graphics g = Graphics.FromImage(image);
            try
            {
                //生成随机生成器  
                Random random = new Random();

                //清空图片背景色  
                g.Clear(Color.White);

                // 画图片的背景噪音线  
                int i;
                for (i = 0; i < 25; i++)
                {
                    int x1 = random.Next(image.Width);
                    int x2 = random.Next(image.Width);
                    int y1 = random.Next(image.Height);
                    int y2 = random.Next(image.Height);
                    g.DrawLine(new Pen(Color.Silver), x1, y1, x2, y2);
                }

                Font font = new System.Drawing.Font("Arial", 12, (System.Drawing.FontStyle.Bold));
                System.Drawing.Drawing2D.LinearGradientBrush brush = new System.Drawing.Drawing2D.LinearGradientBrush(new Rectangle(0, 0, image.Width, image.Height), Color.Blue, Color.DarkRed, 1.2F, true);
                g.DrawString(checkCode, font, brush, 2, 2);

                //画图片的前景噪音点  
                g.DrawRectangle(new Pen(Color.Silver), 0, 0, image.Width - 1, image.Height - 1);
                System.IO.MemoryStream ms = new System.IO.MemoryStream();
                image.Save(ms, System.Drawing.Imaging.ImageFormat.Gif);
                Response.ClearContent();
                Response.ContentType = "image/Gif";
                Response.BinaryWrite(ms.ToArray());
                //return File(ms, "image/Gif");
            }
            finally
            {
                g.Dispose();
                image.Dispose();
            }
            return Content("验证码生成错误！");
        }


        /// <summary>ke在线编辑器图片上传
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult KE_Upload()
        {

            Hashtable hash;


            HttpPostedFileBase imgFile = Request.Files["imgFile"];
            if (imgFile != null && !string.IsNullOrEmpty(imgFile.FileName))
            {


                //文件保存目录路径
                String savePath = "/upload/";



                //定义允许上传的文件扩展名
                Hashtable extTable = new Hashtable();
                extTable.Add("image", "gif,jpg,jpeg,png,bmp");

                //最大文件大小,3M以内
                int maxSize = 3000000;

                String dirPath = Server.MapPath(savePath);
                if (!Directory.Exists(dirPath))
                {
                    Directory.CreateDirectory(dirPath);
                }


                String fileName = imgFile.FileName;
                String fileExt = Path.GetExtension(fileName).ToLower();

                if (imgFile.InputStream == null || imgFile.InputStream.Length > maxSize)
                {
                    hash = new Hashtable();
                    hash["error"] = 1;
                    hash["message"] = "上传文件大小超过限制。";
                    return Content(JsonMapper.ToJson(hash));
                }

                if (String.IsNullOrEmpty(fileExt) || Array.IndexOf(((String)extTable["image"]).Split(','), fileExt.Substring(1).ToLower()) == -1)
                {

                    hash = new Hashtable();
                    hash["error"] = 1;
                    hash["message"] = "上传文件扩展名是不允许的扩展名。只允许" + ((String)extTable["image"]) + "格式。";
                    return Content(JsonMapper.ToJson(hash));
                }

                //创建文件夹

                String ymd = DateTime.Now.ToString("yyyyMMdd", DateTimeFormatInfo.InvariantInfo);
                dirPath += ymd + "/";
                if (!Directory.Exists(dirPath))
                {
                    Directory.CreateDirectory(dirPath);
                }

                String newFileName = DateTime.Now.ToString("yyyyMMddHHmmss_ffff", DateTimeFormatInfo.InvariantInfo) + fileExt;
                String filePath = dirPath + newFileName;

                imgFile.SaveAs(filePath);

                #region 给图片加水印
                //string spath = filePath;
                //string waterpath = "";
                //Model.Shuxing sx = new DAL.ShuxingDAL().GetModelByCond("sxname='水印图片'");
                //if (sx != null && !string.IsNullOrEmpty(sx.sxvalue))
                //{
                //    waterpath = Server.MapPath(sx.sxvalue);
                //    string savepath = filePath + "_water.jpg";
                //    Tool.MarkWater(spath, waterpath, savepath);
                //}
                #endregion

                hash = new Hashtable();
                hash["error"] = 0;
                //hash["url"] = fileExt.Contains("gif") ? "/upload/" + ymd + "/" + newFileName : "/upload/" + ymd + "/" + newFileName + "_water.jpg";
                hash["url"] = "/upload/" + ymd + "/" + newFileName;
                return Content(JsonMapper.ToJson(hash));

            }
            else
            {
                hash = new Hashtable();
                hash["error"] = 1;
                hash["message"] = "请选择上传文件.";
                return Content(JsonMapper.ToJson(hash));
            }


        }


        /// <summary>
        /// layui编辑器里的上传图片功能 
        //{
        //  "code": 0 //0表示成功，其它失败
        //  ,"msg": "" //提示信息 //一般上传失败后返回
        //  ,"data": {
        //    "src": "图片路径"
        //    ,"title": "图片名称" //可选
        //  }
        //}
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ImgUpload()
        {
            var imgFile = Request.Files[0];
            if (imgFile != null && !string.IsNullOrEmpty(imgFile.FileName))
            {

                string tempname = "";
                var filename = imgFile.FileName;
                var extname = filename.Substring(filename.LastIndexOf("."), filename.Length - filename.LastIndexOf(".")); //扩展名，如.jpg

                #region 判断后缀
                if (!extname.ToLower().Contains("jpg") && !extname.ToLower().Contains("png") && !extname.ToLower().Contains("gif"))
                {
                    return Json(new { code = 1, msg = "只允许上传jpg,png,gif格式的图片.", });
                }
                #endregion

                #region 判断大小
                long mb = imgFile.ContentLength / 1024 / 1024; // MB
                if (mb > 5)
                {
                    return Json(new { code = 1, msg = "只允许上传小于 5MB 的图片.", });
                }
                #endregion

                var filename1 = System.Guid.NewGuid().ToString().Substring(0, 6) + extname;
                tempname = filename1;
                var path = "/upload/";
                string dir = DateTime.Now.ToString("yyyyMMdd");
                //完整物理路径
                string wuli_path = Server.MapPath($"{path}/{dir}/");
                if (!System.IO.Directory.Exists(wuli_path))
                {
                    System.IO.Directory.CreateDirectory(wuli_path);
                }
                filename = wuli_path + filename1;

                imgFile.SaveAs(filename);

         

                return Json(new { code = 0, msg = "上传成功", data = new { src = $"/upload/{dir}/{filename1}", title = filename1 } });
            }
            return Json(new { code = 1, msg = "上传失败", });
        }

    }
}