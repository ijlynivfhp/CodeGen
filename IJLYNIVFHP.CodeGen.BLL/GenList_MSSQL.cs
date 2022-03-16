using System;
using System.Data.SqlClient;
using System.Text;

namespace IJLYNIVFHP.CodeGen.BLL
{
    /// <summary>
    /// 生成list页面,del.ashx，add页面相关代码
    /// </summary>
    public class GenList_MSSQL
    {
        public GenList_MSSQL()
        {
        }

        /// <summary>
        /// 生成list前台代码
        /// </summary>
        /// <param name="tabname">表名</param>
        /// <param name="front">表前缀</param>
        /// <param name="connstr">数据库连接字符串</param>
        /// <returns></returns>
        public string List1(string tabname,string front, string connstr)
        {
            StringBuilder sb = new StringBuilder();

            StringBuilder th = new StringBuilder(); //th部分代码
            StringBuilder td = new StringBuilder(); //td部分代码

            SqlConnection conn = new SqlConnection(connstr);
            conn.Open();
            string sql = "";
            sql += "SELECT a.[name] as '字段名',c.[name] '类型',e.value as '字段说明',sm.text as '默认值',a.isnullable as '是否为空' FROM syscolumns  a  ";
            sql += "left   join    systypes    b   on      a.xusertype=b.xusertype ";
            sql += "left 	join 	systypes 	c 	on  	a.xtype = c.xusertype ";
            sql += "inner   join   sysobjects  d   on      a.id=d.id     and   d.xtype='U' ";
            sql += "left join syscomments sm on a.cdefault=sm.id ";
            sql += "left join sys.extended_properties e on a.id = e.major_id and a.colid = e.minor_id and ";
            sql += "e.name='MS_Description' and e.class_desc='OBJECT_OR_COLUMN' where d.name='" + tabname + "' ";
            SqlCommand cmd = new SqlCommand(sql, conn);
            SqlDataReader sdr = cmd.ExecuteReader();
            while (sdr.Read())
            {
                string tmp = sdr["字段说明"].ToString() == "" ? sdr["字段名"].ToString() : sdr["字段说明"].ToString();
                th.Append($"<th>{tmp}</th>\r\n");
                td.Append($"<td> <%#Eval(\"{sdr["字段名"]}\") %> </td> \r\n");
            } 
            conn.Close();

            if (!string.IsNullOrEmpty(front))
            {
                tabname = tabname.Replace(front, "");
            }
           

            sb.Append("<%@ Register Assembly=\"AspNetPager\" Namespace=\"Wuqi.Webdiyer\" TagPrefix=\"webdiyer\" %>\r\n");
            sb.Append("<asp:Content ID=\"Content1\" ContentPlaceHolderID=\"head\" runat=\"server\">\r\n");
            sb.Append("</asp:Content>\r\n");
            sb.Append("<asp:Content ID=\"Content2\" ContentPlaceHolderID=\"Content\" runat=\"server\">\r\n");
            sb.Append("    \r\n");
            sb.Append("    	<div class='place'> \r\n");
            sb.Append("    <span>位置：</span> \r\n");
            sb.Append("    <ul class='placeul'> \r\n");
            sb.Append("    <li><a href='welcome.aspx'>后台</a></li> \r\n");
            sb.Append($"    <li><a href='#'>{tabname}</a></li> \r\n ");
            sb.Append("    </ul> \r\n");
            sb.Append("    </div> \r\n");
            sb.Append("<div class='rightinfo'>\r\n");
            sb.Append("<div class='tools'>\r\n");
            sb.Append("<div class='toolbar'>\r\n");
            
            sb.Append("    <a class=\"layui-btn layui-btn-danger\" href=\"javascript:delmore()\"><i class=\"layui-icon\">&#x1007;</i> 删除所选项</a>\r\n");
            sb.Append($"   <a href=\"{tabname}_add.aspx\" class=\"layui-btn layui-btn-normal\"><i class=\"layui-icon\">&#xe61f;</i> 新 增</a>\r\n");
            sb.Append("</div>\r\n");
            sb.Append("<div class='toolbar2'>\r\n");
            sb.Append("   <div class='layui-input-inline'> \r\n");
            sb.Append("        <asp:TextBox ID='txtkey'  placeholder='查询关键字'   class='layui-input' runat='server'></asp:TextBox> \r\n");
            sb.Append("          </div> \r\n");
            sb.Append("        <asp:LinkButton ID='lbtnSearch' class='layui-btn' onclick='search'  runat='server'><i class='layui-icon'>&#xe615;</i> 搜 索</asp:LinkButton> \r\n");
            sb.Append("</div>\r\n");
            sb.Append("</div>\r\n");
            sb.Append("<table class=\"layui-table\">\r\n");
            sb.Append("    <thead>\r\n");
            sb.Append("        <tr>\r\n");
            sb.Append("            <th>\r\n");
            sb.Append("                <input id=\"chkall\" type=\"checkbox\" class=\"allselect\" /> \r\n");
            sb.Append("            </th>\r\n");
            sb.Append(th);
            sb.Append("            <th></th>\r\n");
            sb.Append("        </tr>\r\n");
            sb.Append("    </thead>\r\n");
            sb.Append("    <tbody>\r\n");
            sb.Append("  \r\n");
            sb.Append("               <asp:Repeater ID=\"rep\" runat=\"server\">\r\n");
            sb.Append("            <ItemTemplate>\r\n");
            sb.Append("                 <tr>\r\n");
            sb.Append("<td><input   type='checkbox' class='chkitem' value='<%#Eval(\"id\") %>' /> </td>\r\n");
            sb.Append(td); 
            sb.Append("            <td>\r\n");
            sb.Append($"                <a href=\"{tabname}_add.aspx?id=<%#Eval(\"id\") %>\"><i class=\"layui-icon\">&#xe642;</i>编辑</a> | \r\n");
            sb.Append("                 <a href=\"javascript:void(0);\" onclick='delone(<%#Eval(\"id\") %>)'><i class=\"layui-icon\">&#x1006;</i>删除</a>\r\n");
            sb.Append("            </td>\r\n");
            sb.Append("        </tr>\r\n");
            sb.Append("            </ItemTemplate>\r\n");
            sb.Append("        </asp:Repeater>\r\n");
            sb.Append(" \r\n");
            sb.Append("    </tbody>\r\n");
            sb.Append("</table> \r\n");
            sb.Append("<div class='pagin'> \r\n");
            sb.Append("        <webdiyer:AspNetPager ID=\"anp\" runat=\"server\" AlwaysShow=\"true\"\r\n");
            sb.Append("        CssClass=\"anpager\" CurrentPageButtonClass=\"cpb\"\r\n");
            sb.Append("        CustomInfoHTML=\"共%RecordCount%条，第%CurrentPageIndex%页/共%PageCount%页\"\r\n");
            sb.Append("        FirstPageText='首页'\r\n");
            sb.Append("        LastPageText='尾页'\r\n");
            sb.Append("        NextPageText=\"下一页\"\r\n");
            sb.Append("        OnPageChanged=\"anpList_PageChanged\"\r\n");
            sb.Append("        PageIndexBoxType=\"DropDownList\"\r\n");
            sb.Append("        PageSize=\"20\"\r\n");
            sb.Append("        PagingButtonSpacing=\"\"\r\n");
            sb.Append("        PrevPageText=\"上一页\"\r\n");
            sb.Append("        ShowCustomInfoSection=\"Left\" ShowMoreButtons=\"False\" ShowPageIndex=\"true\"\r\n");
            sb.Append("        ShowPageIndexBox=\"Always\"\r\n");
            sb.Append("        SubmitButtonText=\"Go\" TextAfterPageIndexBox=\"页\" TextBeforePageIndexBox=\"转到 \">\r\n");
            sb.Append("    </webdiyer:AspNetPager>\r\n");
            sb.Append("</div>\r\n");
            sb.Append("</div>\r\n");
            sb.Append("<script type=\"text/javascript\">\r\n");
            sb.Append("        $(function () {\r\n");
            sb.Append("            $(\"#chkall\").click(function () {\r\n");
            sb.Append("                if ($(this).prop(\"checked\") == true) {\r\n");
            sb.Append("                    $(\".chkitem\").prop(\"checked\", true);\r\n");
            sb.Append("                } else {\r\n");
            sb.Append("                    $(\".chkitem\").prop(\"checked\", false);\r\n");
            sb.Append("                }\r\n");
            sb.Append("            });\r\n");
            sb.Append("        })\r\n");
            sb.Append("        function delmore() {\r\n");
            sb.Append("            if (confirm(\"是否确认删除所选项？\")) {\r\n");
            sb.Append("                var str = \"\";\r\n");
            sb.Append("                $(\".chkitem\").each(function (i) {\r\n");
            sb.Append("                    if ($(this).prop(\"checked\") == true) {\r\n");
            sb.Append("                        str += $(this).val() + \",\";\r\n");
            sb.Append("                    }\r\n");
            sb.Append("                })\r\n");
            sb.Append($"                var url = \"{tabname}_del.ashx?ids=\" + str;\r\n");
            sb.Append("                $.get(url, function (data) {\r\n");
            sb.Append("                    alert(data);\r\n");
            sb.Append("                    location.reload();\r\n");
            sb.Append("                })\r\n");
            sb.Append("            }\r\n");
            sb.Append("           \r\n");
            sb.Append("        }\r\n");
            sb.Append("        function delone(id) {\r\n");
            sb.Append("            if (confirm(\"是否确认删除？\")) {\r\n");
            sb.Append($"                var url = \"{tabname}_del.ashx?ids=\" + id;\r\n");
            sb.Append("                $.get(url, function (data) {\r\n");
            sb.Append("                    alert(data);\r\n");
            sb.Append("                    location.reload();\r\n");
            sb.Append("                })\r\n");
            sb.Append("            }\r\n");
            sb.Append("        }\r\n");
            sb.Append("</script>\r\n");
            sb.Append("</asp:Content>\r\n");

            return sb.ToString();
        }

        /// <summary>
        /// 生成list后台代码
        /// </summary>
        /// <param name="classname">类名，无DAL后缀</param>
        /// <returns></returns>
        public string List2(string classname)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($"        DAL.{classname}DAL dal = new DAL.{classname}DAL();\r\n");
            sb.Append("        protected void Page_Load(object sender, EventArgs e)\r\n");
            sb.Append("        {\r\n");
            sb.Append("            if (!IsPostBack)\r\n");
            sb.Append("            {\r\n");
            sb.Append("                anp.RecordCount = dal.CalcCount(GetCond());\r\n");
            sb.Append("                BindRep();\r\n");
            sb.Append("            }\r\n");
            sb.Append("        }\r\n");

            sb.Append("        private void BindRep()\r\n");
            sb.Append("        {\r\n");
            sb.Append("            rep.DataSource = dal.GetListArray(\"*\", \"id\", \"desc\", anp.PageSize, anp.CurrentPageIndex, GetCond());\r\n");
            sb.Append("            rep.DataBind();\r\n");
            sb.Append("        }\r\n");

            sb.Append("        private string GetCond()\r\n");
            sb.Append("        {\r\n");
            sb.Append("            string cond = \"\";\r\n");
            sb.Append("            return cond;\r\n");
            sb.Append("        }\r\n");

            sb.Append("        protected void anpList_PageChanged(object sender, EventArgs e)\r\n");
            sb.Append("        {\r\n");
            sb.Append("            anp.RecordCount = dal.CalcCount(GetCond());\r\n");
            sb.Append("            BindRep();\r\n");
            sb.Append("        }\r\n");

            sb.Append("        protected void search(object sender, EventArgs e) \r\n");
            sb.Append("        { \r\n");
            sb.Append("            anp.RecordCount = dal.CalcCount(GetCond()); \r\n");
            sb.Append("            BindRep(); \r\n");
            sb.Append("        } \r\n");
            return sb.ToString();
        }

        /// <summary>
        /// 生成del.ashx的相关代码
        /// </summary>
        /// <param name="classname">类名，无DAL后缀 </param>
        /// <returns></returns>
        public string Del(string classname)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("        public void ProcessRequest(HttpContext context)\r\n");
            sb.Append("        {\r\n");
            sb.Append("            context.Response.ContentType = \"text/plain\";\r\n");
            sb.Append("            string ids = context.Request.QueryString[\"ids\"];\r\n");
            sb.Append("            int success = 0;\r\n");
            sb.Append("            string[] ss = ids.Split(',');\r\n");
            sb.Append($"            DAL.{classname}DAL dal = new DAL.{classname}DAL();\r\n");
            sb.Append("            foreach (var item in ss)\r\n");
            sb.Append("            {\r\n");
            sb.Append("                int x;\r\n");
            sb.Append("                if (int.TryParse(item, out x))\r\n");
            sb.Append("                {\r\n");
            sb.Append("                    dal.Delete(x);\r\n");
            sb.Append("                    success++;\r\n");
            sb.Append("                }\r\n");
            sb.Append("            }\r\n");
            sb.Append("            context.Response.Write(\"成功删除\" + success + \"条记录！\");\r\n");
            sb.Append("        }\r\n");
            return sb.ToString();
        }

        /// <summary>
        /// 生成前台add页面代码
        /// </summary>
        /// <param name="tabname">表名</param>
        /// <param name="front">前缀</param>
        /// <param name="connstr">连接字符串</param>
        /// <returns></returns>
        public string Add1(string tabname, string front, string connstr)
        {
            StringBuilder sb = new StringBuilder();

            StringBuilder tr = new StringBuilder(); //tr部分代码 

            SqlConnection conn = new SqlConnection(connstr);
            conn.Open();
            string sql = "";
            sql += "SELECT a.[name] as '字段名',c.[name] '类型',e.value as '字段说明',sm.text as '默认值',a.isnullable as '是否为空' FROM syscolumns  a  ";
            sql += "left   join    systypes    b   on      a.xusertype=b.xusertype ";
            sql += "left 	join 	systypes 	c 	on  	a.xtype = c.xusertype ";
            sql += "inner   join   sysobjects  d   on      a.id=d.id     and   d.xtype='U' ";
            sql += "left join syscomments sm on a.cdefault=sm.id ";
            sql += "left join sys.extended_properties e on a.id = e.major_id and a.colid = e.minor_id and ";
            sql += "e.name='MS_Description' and e.class_desc='OBJECT_OR_COLUMN' where d.name='" + tabname + "' ";
            SqlCommand cmd = new SqlCommand(sql, conn);
            SqlDataReader sdr = cmd.ExecuteReader();
            int c = 0;
            while (sdr.Read())
            {
                if (sdr["字段名"].ToString().ToLower() == "id" || sdr["字段名"].ToString().ToLower() == "createdate")
                {
                    continue;
                }
                string style = c == 0 ? "style='width:120px'" : "";
                string tmp = sdr["字段说明"].ToString() == "" ? sdr["字段名"].ToString() : sdr["字段说明"].ToString();


                tr.Append("  <div class='layui-form-item'> \r\n");
                tr.Append($"    <label class='layui-form-label'>{tmp}</label> \r\n");
                tr.Append("    <div class='layui-input-inline'> \r\n");
                tr.Append($"                <asp:TextBox ID='txt{sdr["字段名"]}' runat='server' CssClass='layui-input'></asp:TextBox>\r\n");
                tr.Append("    </div> \r\n");
                tr.Append("    <div class='layui-form-mid layui-word-aux'><!--辅助文字--></div> \r\n");
                tr.Append("  </div> \r\n");
                c++;
            }
            conn.Close();

            if (!string.IsNullOrEmpty(front))
            {
                tabname = tabname.Replace(front, "");
            }

            sb.Append("<asp:Content ID='Content1' ContentPlaceHolderID='head' runat='server'>\r\n");
            sb.Append("</asp:Content>\r\n");
            sb.Append("<asp:Content ID='Content2' ContentPlaceHolderID='Content' runat='server'>\r\n");
            sb.Append("    	<div class='place'> \r\n");
            sb.Append("    <span>位置：</span> \r\n");
            sb.Append("    <ul class='placeul'> \r\n");
            sb.Append("    <li><a href='welcome.aspx'>后台</a></li> \r\n");
            sb.Append($"    <li><a href='#'>{tabname}</a></li> \r\n ");
            sb.Append("    </ul> \r\n");
            sb.Append("    </div> \r\n");
            sb.Append($"<div class='formtitle'><span><asp:Literal ID='lith1' Text='新增{tabname}' runat='server'></asp:Literal></span></div>\r\n");
 
            sb.Append("    <div class='layui-form'>\r\n");
            sb.Append(tr);
            sb.Append("  <div class='layui-form-item'> \r\n");
            sb.Append("    <div class='layui-input-block'> \r\n");
            sb.Append("                <asp:Button ID='btnAdd' OnClick='btnAdd_Click' class='layui-btn' runat='server' Text='新 增' /> \r\n");
        
            sb.Append("      <button type='reset' class='layui-btn layui-btn-primary'>重置</button> \r\n");
            sb.Append("    </div> \r\n");
            sb.Append("  </div> \r\n"); 
            sb.Append("    </div>\r\n");
            sb.Append("</asp:Content>\r\n");

            return sb.ToString();
        }

        /// <summary>
        /// 生成后台代码
        /// </summary>
        /// <param name="tabname">表名</param>
        /// <param name="front">前缀</param>
        /// <param name="connstr">数据库连接字符串</param>
        /// <returns></returns>
        public string Add2(string tabname, string front, string connstr)
        {
            StringBuilder sb = new StringBuilder();
            string classname = tabname;  //类名

            if (front.Length > 0)
            {
                classname = tabname.Replace(front, "");
            }
            classname = classname.Substring(0, 1).ToUpper() + classname.Substring(1);
            StringBuilder txtset = new StringBuilder(); //设置txt的值
            StringBuilder gettxt = new StringBuilder(); //获取Txt的值
            StringBuilder updateparam = new StringBuilder(); //更新部分代码
            StringBuilder addparam = new StringBuilder(); //添加部分代码

            SqlConnection conn = new SqlConnection(connstr);
            conn.Open();
            string sql = "";
            sql += "SELECT a.[name] as '字段名',c.[name] '类型',e.value as '字段说明',sm.text as '默认值',a.isnullable as '是否为空' FROM syscolumns  a  ";
            sql += "left   join    systypes    b   on      a.xusertype=b.xusertype ";
            sql += "left 	join 	systypes 	c 	on  	a.xtype = c.xusertype ";
            sql += "inner   join   sysobjects  d   on      a.id=d.id     and   d.xtype='U' ";
            sql += "left join syscomments sm on a.cdefault=sm.id ";
            sql += "left join sys.extended_properties e on a.id = e.major_id and a.colid = e.minor_id and ";
            sql += "e.name='MS_Description' and e.class_desc='OBJECT_OR_COLUMN' where d.name='" + tabname + "' ";
            SqlCommand cmd = new SqlCommand(sql, conn);
            SqlDataReader sdr = cmd.ExecuteReader();
            while (sdr.Read())
            {
                if (sdr["字段名"].ToString().ToLower() == "id" || sdr["字段名"].ToString().ToLower()=="createdate")
                {
                    continue;
                }
                string lx_tmp = Tools.DbTypeToCSharpType(sdr["类型"].ToString(), "0");
                switch (lx_tmp)
                {
                    case "string":
                        txtset.Append($"txt{sdr["字段名"]}.Text = m.{sdr["字段名"]};\r\n");
                        gettxt.Append($"string {sdr["字段名"]} = txt{sdr["字段名"]}.Text.Trim();\r\n");
                        break;
                    default:
                        txtset.Append($"txt{sdr["字段名"]}.Text = m.{sdr["字段名"]}.ToString();\r\n");
                        gettxt.Append($"{lx_tmp} {sdr["字段名"]} = {lx_tmp}.Parse(txt{sdr["字段名"]}.Text.Trim());\r\n");
                        break;
                }

               
                
                updateparam.Append($"m.{sdr["字段名"]} = {sdr["字段名"]};\r\n");
                addparam.Append($"{sdr["字段名"]} = {sdr["字段名"]},\r\n");
            }
            conn.Close();

            tabname = string.IsNullOrEmpty(front) ? tabname : tabname.Replace(front, "");

            sb.Append($"        DAL.{classname}DAL dal = new DAL.{classname}DAL();\r\n");
            sb.Append("        protected void Page_Load(object sender, EventArgs e)\r\n");
            sb.Append("        {\r\n");
            sb.Append("            if (!IsPostBack)\r\n");
            sb.Append("            {\r\n"); 
            sb.Append("                string id = Request.QueryString[\"id\"];\r\n");
            sb.Append("                int x;\r\n");
            sb.Append("                if (int.TryParse(id, out x))\r\n");
            sb.Append("                {\r\n");
            sb.Append($"                    Model.{classname} m = dal.GetModel(x);\r\n");
            sb.Append("                    if (m != null)\r\n");
            sb.Append("                    {\r\n");
            sb.Append(txtset); 
            sb.Append("                        btnAdd.Text = \"编 辑\";\r\n");
            sb.Append($"                        lith1.Text = \"编辑{tabname}\";\r\n");
            sb.Append("                    }\r\n");
            sb.Append("                }\r\n");
            sb.Append("            }\r\n");
            sb.Append("        }\r\n");

            sb.Append("        protected void btnAdd_Click(object sender, EventArgs e)\r\n");
            sb.Append("        {\r\n");
            sb.Append(gettxt); 

            sb.Append("            string id = Request.QueryString[\"id\"];\r\n");
            sb.Append("            int x;\r\n");
            sb.Append("            if (int.TryParse(id, out x))\r\n");
            sb.Append("            {\r\n");
            sb.Append($"                Model.{classname} m = dal.GetModel(x);\r\n");
            sb.Append("                if (m != null)\r\n");
            sb.Append("                {\r\n");
            sb.Append(updateparam); 
            sb.Append("                    dal.Update(m);\r\n");
            sb.Append($"                    Tool.AlertAndGo(\"编辑成功！\", \"{tabname}.aspx\", this.Page);\r\n");
            sb.Append("                    return;\r\n");
            sb.Append("                }\r\n");
            sb.Append("            }\r\n"); 
            sb.Append($"            dal.Add(new Model.{classname}()\r\n");
            sb.Append("            {\r\n");
            sb.Append(addparam); 
            sb.Append("            });\r\n");
            sb.Append($"            Tool.AlertAndGo(\"新增成功！\", \"{tabname}.aspx\", this.Page);\r\n");
            sb.Append("        }\r\n");


            return sb.ToString();
        }
    }
}