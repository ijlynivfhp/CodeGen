using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IJLYNIVFHP.CodeGen.BLL
{
    /// <summary>
    /// 生成 MVC部分的代码
    /// </summary>
    public class GenCode_MVC
    {
        /// <summary>
        /// 生成list列表页
        /// </summary>
        /// <param name="dbtype">数据库类型</param>
        /// <param name="ns">命名空间</param>
        /// <param name="tabname">表名</param> 
        /// <param name="classname">类名</param> 
        /// <param name="connstr">数据库连接字符串</param>
        /// <returns></returns>
        public string List(string dbtype, string ns, string tabname, string classname, string connstr, int layui_version)
        {
            StringBuilder sb = new StringBuilder();
            if (dbtype.ToLower().Contains("server"))
            {
                #region sql server数据库
                SqlConnection conn = new SqlConnection(connstr);
                conn.Open();
                string sql = "";
                sql += "SELECT a.[name] as '字段名',c.[name] '类型',e.value as '字段说明',sm.text as '默认值',a.isnullable as '是否为空' FROM syscolumns  a  ";
                sql += "left   join    systypes    b   on      a.xusertype=b.xusertype ";
                sql += "left 	join 	systypes 	c 	on  	a.xtype = c.xusertype ";
                sql += "inner   join   sysobjects  d   on      a.id=d.id     and   d.xtype='U' ";
                sql += "left join syscomments sm on a.cdefault=sm.id ";
                sql += "left join sys.extended_properties e on a.id = e.major_id and a.colid = e.minor_id and ";
                sql += "e.name='MS_Description' and e.class_desc='OBJECT_OR_COLUMN'  where d.name='" + tabname + "' ";
                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataReader sdr = cmd.ExecuteReader();
                DataTable dt = new DataTable();
                dt.Load(sdr);
                conn.Close();


                StringBuilder th_html = new StringBuilder(); //表格行
                StringBuilder js_data = new StringBuilder(); //js中的代码
                foreach (DataRow row in dt.Rows)
                {
                    string tmp = string.IsNullOrEmpty(row["字段说明"].ToString()) ? row["字段名"].ToString() : row["字段说明"].ToString();
                    th_html.Append($"<th>{ tmp }</th>\r\n");
                    js_data.Append($"html += '    <td>' + item.{row["字段名"]} + '</td>'; \r\n");
                }

                string tempfile = "list_mvc.txt";
                if (layui_version == 2)
                {
                    tempfile = "list_mvc_layui2.txt";
                }
                StreamReader sr = new StreamReader(System.Environment.CurrentDirectory + @"\classtemp\" + tempfile);
                sb.Append(sr.ReadToEnd());
                sb = sb.Replace("{classname}", classname);
                sb = sb.Replace("{th_html}", th_html.ToString());
                sb = sb.Replace("{js_data}", js_data.ToString());
                #endregion


            }
            else if (dbtype.ToLower().Contains("mysql"))
            {
                #region mysql数据库
                MySqlConnection conn = new MySqlConnection(connstr);
                conn.Open();

                MySqlCommand cmd = new MySqlCommand("use information_schema; select * from columns where table_name='" + tabname + "' and table_schema='" + conn.Database + "'", conn);

                DataTable dt = new DataTable();
                dt.Load(cmd.ExecuteReader());

                var q = from a in dt.AsEnumerable()
                        where a.Field<string>("TABLE_NAME") == tabname
                        orderby a.Field<UInt64>("ORDINAL_POSITION")
                        select new
                        {
                            字段名 = a.Field<string>("COLUMN_NAME"),
                            类型 = a.Field<string>("DATA_TYPE"),
                            字段说明 = a.Field<string>("COLUMN_COMMENT"),
                            默认值 = a.Field<string>("COLUMN_DEFAULT"),
                        };

                StringBuilder th_html = new StringBuilder(); //表格行
                StringBuilder js_data = new StringBuilder(); //js中的代码
                foreach (var row in q)
                {
                    string tmp = string.IsNullOrEmpty(row.字段说明) ? row.字段名 : row.字段说明;
                    th_html.Append($"<th>{tmp}</th>\r\n");
                    js_data.Append($"html += '    <td>' + item.{row.字段名} + '</td>'; \r\n");
                }

                string tempfile = "list_mvc.txt";
                if (layui_version == 2)
                {
                    tempfile = "list_mvc_layui2.txt";
                }
                StreamReader sr = new StreamReader(System.Environment.CurrentDirectory + @"\classtemp\"+tempfile);
                sb.Append(sr.ReadToEnd());
                sb = sb.Replace("{classname}", classname);
                sb = sb.Replace("{th_html}", th_html.ToString());
                sb = sb.Replace("{js_data}", js_data.ToString());
                #endregion
            }
            else
            {
                sb.Append("本数据库末做！");
            }
            return sb.ToString();
        }

        /// <summary>
        /// 生成add页
        /// </summary>
        /// <param name="dbtype">数据库类型</param>
        /// <param name="ns">命名空间</param>
        /// <param name="tabname">表名</param> 
        /// <param name="classname">类名</param> 
        /// <param name="connstr">数据库连接字符串</param>
        /// <returns></returns>
        public string Add(string dbtype, string ns, string tabname, string classname, string connstr, int layui_version, string addlayout = "div")
        {
            StringBuilder sb = new StringBuilder();
            if (dbtype.ToLower().Contains("server"))
            {
                #region sql server
                SqlConnection conn = new SqlConnection(connstr);
                conn.Open();
                string sql = "";
                sql += "SELECT a.[name] as '字段名',c.[name] '类型',e.value as '字段说明',sm.text as '默认值',a.isnullable as '是否为空' FROM syscolumns  a  ";
                sql += "left   join    systypes    b   on      a.xusertype=b.xusertype ";
                sql += "left 	join 	systypes 	c 	on  	a.xtype = c.xusertype ";
                sql += "inner   join   sysobjects  d   on      a.id=d.id     and   d.xtype='U' ";
                sql += "left join syscomments sm on a.cdefault=sm.id ";
                sql += "left join sys.extended_properties e on a.id = e.major_id and a.colid = e.minor_id and ";
                sql += "e.name='MS_Description' and e.class_desc='OBJECT_OR_COLUMN'  where d.name='" + tabname + "' ";
                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataReader sdr = cmd.ExecuteReader();
                DataTable dt = new DataTable();
                dt.Load(sdr);
                conn.Close();

                StringBuilder input_html = new StringBuilder();
                if (addlayout == "div")
                {
                    #region div布局
                    foreach (DataRow row in dt.Rows)
                    {
                        string lable_name = string.IsNullOrEmpty(row["字段说明"].ToString()) ? row["字段名"].ToString() : row["字段说明"].ToString();
                        if (row["字段名"].ToString().ToLower() == "body")
                        {
                            input_html.Append("<div class='layui-form-item'> \r\n");
                            input_html.Append($"            <label class='layui-form-label'>{lable_name}</label> \r\n");
                            input_html.Append("            <div class=\"layui-input-block\"> \r\n");
                            input_html.Append("                @Html.TextAreaFor(a => a.body, new {   @class=\"layui-textarea\",  style=\"height:330px\" })  \r\n");
                            input_html.Append("            </div> \r\n");
                            input_html.Append("        </div> \r\n");
                        }
                        else if (row["字段名"].ToString().ToLower() == "img" || row["字段名"].ToString().ToLower() == "face"|| row["字段名"].ToString().ToLower() == "logo")
                        {
                            if (layui_version == 2)
                            {
                                input_html.Append("<div class='layui-form-item'> \r\n");
                                input_html.Append($"            <label class='layui-form-label'>{lable_name}</label> \r\n");
                                input_html.Append("            <div class='layui-input-block'> \r\n");
                                input_html.Append("               <button type='button' class='layui-btn' id='btnimg'>  <i class='layui-icon'>&#xe67c;</i>上传图片</button> \r\n");
                                input_html.Append("                <div>只能是jpg,png格式的图片，最佳大小：575px * 308px</div> \r\n");
                                input_html.Append("                <img id='img1' style='max-width:120px;' src='@Model.img' /> \r\n");
                                input_html.Append($"                @Html.HiddenFor(a => a.{row["字段名"]}) \r\n");
                                input_html.Append("            </div> \r\n");
                                input_html.Append("        </div> \r\n");
                            }
                            else
                            {
                                input_html.Append("<div class='layui-form-item'> \r\n");
                                input_html.Append($"            <label class='layui-form-label'>{lable_name}</label> \r\n");
                                input_html.Append("            <div class='layui-input-block'> \r\n");
                                input_html.Append("                <input type='file' name='file' class='layui-upload-file'> \r\n");
                                input_html.Append("                <div>只能是jpg,png格式的图片，最佳大小：575px * 308px</div> \r\n");
                                input_html.Append("                <img id='img1' style='max-width:120px;' src='@Model.img' /> \r\n");
                                input_html.Append($"                @Html.HiddenFor(a => a.{row["字段名"]}) \r\n");
                                input_html.Append("            </div> \r\n");
                                input_html.Append("        </div> \r\n");
                            }

                        }
                        else
                        {
                            input_html.Append("        <div class='layui-form-item'> \r\n");
                            input_html.Append($"            <label class='layui-form-label'>{lable_name}</label> \r\n");
                            input_html.Append("            <div class='layui-input-inline'> \r\n");
                            input_html.Append("                @Html.TextBoxFor(a => a." + row["字段名"] + ", new { @class = \"layui-input\",lay_verify = \"required\" }) \r\n");
                            input_html.Append("            </div> \r\n");
                            input_html.Append("            <div class='layui-form-mid layui-word-aux'><!--辅助文字--></div> \r\n");
                            input_html.Append("        </div> \r\n");
                        }
                    }
                    #endregion
                }
                else
                {
                    #region table布局
                    int size = 2;  // 每次取出的元素个数

                    int num = dt.Rows.Count % size == 0 ? dt.Rows.Count / size : dt.Rows.Count / size + 1; // 要循环的次数

                    input_html.Append("<table class='layui-table'>\r\n");
                    for (int i = 0; i < num; i++)
                    {
                        var query = dt.AsEnumerable().Skip(i * size).Take(2);

                        DataRow row = query.ElementAt(0);
                        string label1 = string.IsNullOrEmpty(row["字段说明"].ToString()) ? row["字段名"].ToString() : row["字段说明"].ToString();
                        StringBuilder html1 = new StringBuilder() ;
                        if (row["字段名"].ToString().ToLower() == "body")
                        {
                  
                            html1.Append("                @Html.TextAreaFor(a => a.body, new {   @class=\"layui-textarea\",  style=\"height:330px\" })  \r\n");
                  
                        }
                        else if (row["字段名"].ToString().ToLower() == "img" || row["字段名"].ToString().ToLower() == "face" || row["字段名"].ToString().ToLower() == "logo")
                        {
                            if (layui_version == 2)
                            {

                                html1.Append("               <button type='button' class='layui-btn' id='btnimg'>  <i class='layui-icon'>&#xe67c;</i>上传图片</button> \r\n");
                                html1.Append("                <div>只能是jpg,png格式的图片，最佳大小：575px * 308px</div> \r\n");
                                html1.Append("                <img id='img1' style='max-width:120px;' src='@Model.img' /> \r\n");
                                html1.Append($"                @Html.HiddenFor(a => a.{row["字段名"]}) \r\n");
                 
                            }
                            else
                            {

                                html1.Append("                <input type='file' name='file' class='layui-upload-file'> \r\n");
                                html1.Append("                <div>只能是jpg,png格式的图片，最佳大小：575px * 308px</div> \r\n");
                                html1.Append("                <img id='img1' style='max-width:120px;' src='@Model.img' /> \r\n");
                                html1.Append($"                @Html.HiddenFor(a => a.{row["字段名"]}) \r\n");
                   
                            }

                        }
                        else
                        {

                            html1.Append("                @Html.TextBoxFor(a => a." + row["字段名"] + ", new { @class = \"layui-input\",lay_verify = \"required\" }) \r\n");
                      
                        }

                        string label2 = "";
                        StringBuilder html2 = new StringBuilder();
                        if (query.Count()>1)
                        {
                            DataRow row2 = query.ElementAt(1);
                            label2= string.IsNullOrEmpty(row2["字段说明"].ToString()) ? row2["字段名"].ToString() : row2["字段说明"].ToString();
                            if (row2["字段名"].ToString().ToLower() == "body")
                            {

                                html2.Append("                @Html.TextAreaFor(a => a.body, new {   @class=\"layui-textarea\",  style=\"height:330px\" })  \r\n");

                            }
                            else if (row2["字段名"].ToString().ToLower() == "img")
                            {
                                if (layui_version == 2)
                                {

                                    html2.Append("               <button type='button' class='layui-btn' id='btnimg'>  <i class='layui-icon'>&#xe67c;</i>上传图片</button> \r\n");
                                    html2.Append("                <div>只能是jpg,png格式的图片，最佳大小：575px * 308px</div> \r\n");
                                    html2.Append("                <img id='img1' style='max-width:120px;' src='@Model.img' /> \r\n");
                                    html2.Append("                @Html.HiddenFor(a => a.img) \r\n");

                                }
                                else
                                {

                                    html2.Append("                <input type='file' name='file' class='layui-upload-file'> \r\n");
                                    html2.Append("                <div>只能是jpg,png格式的图片，最佳大小：575px * 308px</div> \r\n");
                                    html2.Append("                <img id='img1' style='max-width:120px;' src='@Model.img' /> \r\n");
                                    html2.Append("                @Html.HiddenFor(a => a.img) \r\n");

                                }

                            }
                            else
                            {

                                html2.Append("                @Html.TextBoxFor(a => a." + row2["字段名"] + ", new { @class = \"layui-input\",lay_verify = \"required\" }) \r\n");

                            }
                        }

                        input_html.Append("<tr>\r\n");
                        input_html.Append($" <th>{label1}</th>\r\n");
                        input_html.Append($" <td>{html1.ToString()}</td>\r\n");
                        input_html.Append($" <th>{label2}</th>\r\n");
                        input_html.Append($" <td>{html2.ToString()}</td>\r\n");
                        input_html.Append("</tr>\r\n");
                    }
                    input_html.Append("</table>\r\n");

                    #endregion
                }


                string tempfile = "add_mvc.txt";
                if (layui_version == 2)
                {
                    tempfile = "add_mvc_layui2.txt";
                }
                StreamReader sr = new StreamReader(System.Environment.CurrentDirectory + @"\classtemp\" + tempfile);
                sb.Append(sr.ReadToEnd());
                sb = sb.Replace("{classname}", classname);
                sb = sb.Replace("{ns}", ns);
                sb = sb.Replace("{input_html}", input_html.ToString());
                #endregion
            }
            else if (dbtype.ToLower().Contains("mysql"))
            {
                #region mysql数据库
                MySqlConnection conn = new MySqlConnection(connstr);
                conn.Open();

                MySqlCommand cmd = new MySqlCommand("use information_schema; select * from columns where table_name='" + tabname + "' and table_schema='" + conn.Database + "'", conn);

                DataTable dt = new DataTable();
                dt.Load(cmd.ExecuteReader());

                var q = from a in dt.AsEnumerable()
                        where a.Field<string>("TABLE_NAME") == tabname
                        orderby a.Field<UInt64>("ORDINAL_POSITION")
                        select new
                        {
                            字段名 = a.Field<string>("COLUMN_NAME"),
                            类型 = a.Field<string>("DATA_TYPE"),
                            字段说明 = a.Field<string>("COLUMN_COMMENT"),
                            默认值 = a.Field<string>("COLUMN_DEFAULT"),
                        };

                StringBuilder input_html = new StringBuilder();
                if (addlayout == "div")
                {
                    #region DIV布局
                    foreach (var row in q)
                    {
                        string lable_name = string.IsNullOrEmpty(row.字段说明) ? row.字段名 : row.字段说明;
                        if (row.字段名.ToString().ToLower() == "body")
                        {
                            input_html.Append("<div class='layui-form-item'> \r\n");
                            input_html.Append($"            <label class='layui-form-label'>{lable_name}</label> \r\n");
                            input_html.Append("            <div class=\"layui-input-block\"> \r\n");
                            input_html.Append("                @Html.TextAreaFor(a => a.body, new {   @class=\"layui-textarea\",  style=\"height:330px\" })  \r\n");
                            input_html.Append("            </div> \r\n");
                            input_html.Append("        </div> \r\n");
                        }
                        else if (row.字段名.ToString().ToLower() == "img")
                        {
                            input_html.Append("<div class='layui-form-item'> \r\n");
                            input_html.Append("            <label class='layui-form-label'>产品图片</label> \r\n");
                            input_html.Append("            <div class='layui-input-block'> \r\n");
                            input_html.Append("                <input type='file' name='file' class='layui-upload-file'> \r\n");
                            input_html.Append("                <div>只能是jpg,png格式的图片，最佳大小：575px * 308px</div> \r\n");
                            input_html.Append("                <img id='img1' style='max-width:120px;' src='@Model.img' /> \r\n");
                            input_html.Append("                @Html.HiddenFor(a => a.img) \r\n");
                            input_html.Append("            </div> \r\n");
                            input_html.Append("        </div> \r\n");
                        }
                        else
                        {
                            input_html.Append("        <div class='layui-form-item'> \r\n");
                            input_html.Append($"            <label class='layui-form-label'>{lable_name}</label> \r\n");
                            input_html.Append("            <div class='layui-input-block'> \r\n");
                            input_html.Append($"                @Html.TextBoxFor(a => a.{row.字段名}, new {{ @class = \"layui-input\",lay_verify = \"required\" }}) \r\n");
                            input_html.Append("            </div> \r\n");
                            input_html.Append("            <div class='layui-form-mid layui-word-aux'><!--辅助文字--></div> \r\n");
                            input_html.Append("        </div> \r\n");
                        }
                    }
                    #endregion
                }
                else
                {
                    #region table布局
                    int size = 2;  // 每次取出的元素个数

                    int num = q.Count() % size == 0 ? q.Count() / size : q.Count() / size + 1; // 要循环的次数

                    input_html.Append("<table class='layui-table'>\r\n");
                    for (int i = 0; i < num; i++)
                    {
                        var query = q.Skip(i * size).Take(2);

                        var row = query.ElementAt(0);
                        string label1 = string.IsNullOrEmpty(row.字段说明) ? row.字段名 : row.字段说明;
                        StringBuilder html1 = new StringBuilder();
                        if (row.字段名.ToLower() == "body")
                        {

                            html1.Append("                @Html.TextAreaFor(a => a.body, new {   @class=\"layui-textarea\",  style=\"height:330px\" })  \r\n");

                        }
                        else if (row.字段名.ToLower() == "img")
                        {
                            if (layui_version == 2)
                            {

                                html1.Append("               <button type='button' class='layui-btn' id='btnimg'>  <i class='layui-icon'>&#xe67c;</i>上传图片</button> \r\n");
                                html1.Append("                <div>只能是jpg,png格式的图片，最佳大小：575px * 308px</div> \r\n");
                                html1.Append("                <img id='img1' style='max-width:120px;' src='@Model.img' /> \r\n");
                                html1.Append("                @Html.HiddenFor(a => a.img) \r\n");

                            }
                            else
                            {

                                html1.Append("                <input type='file' name='file' class='layui-upload-file'> \r\n");
                                html1.Append("                <div>只能是jpg,png格式的图片，最佳大小：575px * 308px</div> \r\n");
                                html1.Append("                <img id='img1' style='max-width:120px;' src='@Model.img' /> \r\n");
                                html1.Append("                @Html.HiddenFor(a => a.img) \r\n");

                            }

                        }
                        else
                        {

                            html1.Append("                @Html.TextBoxFor(a => a." + row.字段名+ ", new { @class = \"layui-input\",lay_verify = \"required\" }) \r\n");

                        }

                        string label2 = "";
                        StringBuilder html2 = new StringBuilder();
                        if (query.Count() > 1)
                        {
                            var row2 = query.ElementAt(1);
                            label2 = string.IsNullOrEmpty(row2.字段说明) ? row2.字段名 : row2.字段说明;
                            if (row2.字段名.ToLower() == "body")
                            {

                                html2.Append("                @Html.TextAreaFor(a => a.body, new {   @class=\"layui-textarea\",  style=\"height:330px\" })  \r\n");

                            }
                            else if (row2.字段名.ToString().ToLower() == "img")
                            {
                                if (layui_version == 2)
                                {

                                    html2.Append("               <button type='button' class='layui-btn' id='btnimg'>  <i class='layui-icon'>&#xe67c;</i>上传图片</button> \r\n");
                                    html2.Append("                <div>只能是jpg,png格式的图片，最佳大小：575px * 308px</div> \r\n");
                                    html2.Append("                <img id='img1' style='max-width:120px;' src='@Model.img' /> \r\n");
                                    html2.Append("                @Html.HiddenFor(a => a.img) \r\n");

                                }
                                else
                                {

                                    html2.Append("                <input type='file' name='file' class='layui-upload-file'> \r\n");
                                    html2.Append("                <div>只能是jpg,png格式的图片，最佳大小：575px * 308px</div> \r\n");
                                    html2.Append("                <img id='img1' style='max-width:120px;' src='@Model.img' /> \r\n");
                                    html2.Append("                @Html.HiddenFor(a => a.img) \r\n");

                                }

                            }
                            else
                            {

                                html2.Append("                @Html.TextBoxFor(a => a." +row2.字段名 + ", new { @class = \"layui-input\",lay_verify = \"required\" }) \r\n");

                            }
                        }

                        input_html.Append("<tr>\r\n");
                        input_html.Append($" <th>{label1}</th>\r\n");
                        input_html.Append($" <td>{html1.ToString()}</td>\r\n");
                        input_html.Append($" <th>{label2}</th>\r\n");
                        input_html.Append($" <td>{html2.ToString()}</td>\r\n");
                        input_html.Append("</tr>\r\n");
                    }
                    input_html.Append("</table>\r\n");

                    #endregion
                }


                string tempfile = "add_mvc.txt";
                if (layui_version == 2)
                {
                    tempfile = "add_mvc_layui2.txt";
                }
                StreamReader sr = new StreamReader(System.Environment.CurrentDirectory + @"\classtemp\"+tempfile);
                sb.Append(sr.ReadToEnd());
                sb = sb.Replace("{classname}", classname);
                sb = sb.Replace("{ns}", ns);
                sb = sb.Replace("{input_html}", input_html.ToString());
                #endregion
            }
            else
            {
                sb.Append("本数据库末做！");
            }
            return sb.ToString();
        }

        /// <summary>
        /// 生成控制器后台代码
        /// </summary>
        /// <returns></returns>
        public string BackCode(string dbtype, string ns, string tabname, string classname, string connstr)
        {
            StringBuilder sb = new StringBuilder();
            if (dbtype.ToLower().Contains("server"))
            {
                #region sql server
                SqlConnection conn = new SqlConnection(connstr);
                conn.Open();
                string sql = "";
                sql += "SELECT a.[name] as '字段名',c.[name] '类型',e.value as '字段说明',sm.text as '默认值',a.isnullable as '是否为空' FROM syscolumns  a  ";
                sql += "left   join    systypes    b   on      a.xusertype=b.xusertype ";
                sql += "left 	join 	systypes 	c 	on  	a.xtype = c.xusertype ";
                sql += "inner   join   sysobjects  d   on      a.id=d.id     and   d.xtype='U' ";
                sql += "left join syscomments sm on a.cdefault=sm.id ";
                sql += "left join sys.extended_properties e on a.id = e.major_id and a.colid = e.minor_id and ";
                sql += "e.name='MS_Description' and e.class_desc='OBJECT_OR_COLUMN'  where d.name='" + tabname + "' ";
                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataReader sdr = cmd.ExecuteReader();
                DataTable dt = new DataTable();
                dt.Load(sdr);
                conn.Close();

                StringBuilder code_data = new StringBuilder();
                foreach (DataRow row in dt.Rows)
                {
                    string lable_name = string.IsNullOrEmpty(row["字段说明"].ToString()) ? row["字段名"].ToString() : row["字段说明"].ToString();
                    code_data.Append($"{row["字段名"]} = item.{row["字段名"]}, \r\n");
                }

                StreamReader sr = new StreamReader(System.Environment.CurrentDirectory + @"\classtemp\backcode_mvc.txt");
                sb.Append(sr.ReadToEnd());
                sb = sb.Replace("{classname}", classname);
                sb = sb.Replace("{ns}", ns);
                sb = sb.Replace("{code_data}", code_data.ToString());
                #endregion
            }
            else if (dbtype.ToLower().Contains("mysql"))
            {
                #region mysql数据库
                MySqlConnection conn = new MySqlConnection(connstr);
                conn.Open();

                MySqlCommand cmd = new MySqlCommand("use information_schema; select * from columns where table_name='" + tabname + "' and table_schema='" + conn.Database + "'", conn);

                DataTable dt = new DataTable();
                dt.Load(cmd.ExecuteReader());

                var q = from a in dt.AsEnumerable()
                        where a.Field<string>("TABLE_NAME") == tabname
                        orderby a.Field<UInt64>("ORDINAL_POSITION")
                        select new
                        {
                            字段名 = a.Field<string>("COLUMN_NAME"),
                            类型 = a.Field<string>("DATA_TYPE"),
                            字段说明 = a.Field<string>("COLUMN_COMMENT"),
                            默认值 = a.Field<string>("COLUMN_DEFAULT"),
                        };

                StringBuilder code_data = new StringBuilder();
                foreach (var row in q)
                {
                    code_data.Append($"{row.字段名} = item.{row.字段名}, \r\n");
                }

                StreamReader sr = new StreamReader(System.Environment.CurrentDirectory + @"\classtemp\backcode_mvc.txt");
                sb.Append(sr.ReadToEnd());
                sb = sb.Replace("{classname}", classname);
                sb = sb.Replace("{ns}", ns);
                sb = sb.Replace("{code_data}", code_data.ToString());
                #endregion
            }
            else
            {
                sb.Append("本数据库末做！");
            }
            return sb.ToString();
        }
    }
}
