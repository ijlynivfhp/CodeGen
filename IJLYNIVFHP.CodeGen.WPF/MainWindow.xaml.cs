using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;
using IJLYNIVFHP.CodeGen.BLL;
using System.Data;

namespace IJLYNIVFHP.CodeGen.WPF
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();


            #region 窗口加载完后显示存在XML文件中的值
            XDocument xdoc = XDocument.Load(System.Environment.CurrentDirectory + "\\appconfig.xml");

            var ad = from a in xdoc.Descendants("Config")
                     select new
                     {
                         dbtype = a.Element("dbtype").Value,
                         connstr = a.Element("connstr").Value,
                         genway = a.Element("genway").Value,
                         ns = a.Element("namespace").Value,
                         front = a.Element("front").Value,
                         output = a.Element("output").Value
                     };

            string tmp = ad.ElementAt(0).dbtype;
            int x = 0;
            switch (tmp)
            {
                case "SQL Server":x = 0;break;
                case "MySQL": x = 1;break;
                case "Access": x = 2;break;
                case "SQLite": x = 3;break;
                default:
                    break;
            }
            ddldb.SelectedIndex = x;
            txtconnstr.Text = ad.ElementAt(0).connstr;
            if (ad.ElementAt(0).genway == "0")
            {
                radmicro.IsChecked = true;
            }
            else if (ad.ElementAt(0).genway == "1")
            {
                radIJLYNIVFHP.IsChecked = true;
            }
            else
            {
                raddapper.IsChecked = true;
            }
            txtmingming.Text = ad.ElementAt(0).ns;
            txtqianzui.Text = ad.ElementAt(0).front;
            txtmulu.Text = ad.ElementAt(0).output;
            #endregion
        }

        //选择数据库不同，预览框显示不同连接字符串
        private void ddldb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.RemovedItems.Count == 1)
            {
                //不是第一次加载
                if (lsbleft != null)
                {
                    lsbleft.Items.Clear();
                }
                if (lsbright != null)
                {
                    lsbright.Items.Clear();
                }

                ComboBoxItem sel = ddldb.SelectedItem as ComboBoxItem;
                string tmp = "SQL Server";
                if (sel != null && sel.Content != null)
                {
                    tmp = sel.Content.ToString();
                }
                switch (tmp)
                {
                    case "SQL Server":

                        txtyulan.Text = @"示例连接字符串: server=192.168.0.135;uid=sa;pwd=Famous901;database=LaborSys;pooling=true;min pool size=5;max pool size=100;";
                        break;
                    case "MySQL":

                        txtyulan.Text = @"示例连接字符串: server=localhost;database=test;uid=root;pwd=123456;charset=utf8";
                        break;
                    case "Access":

                        txtyulan.Text = @"示例连接字符串: Provider=Microsoft.Jet.OLEDB.4.0;Data Source=|DataDirectory|data.mdb";
                        break;
                    case "SQLite":

                        txtyulan.Text = @"示例连接字符串: Data Source=d:/data/fdm.sqlite";
                        break;
                    default:
                        break;
                }
            }

        }
        List<string> listleft = new List<string>();
        List<string> listright = new List<string>();

        //点击连接数据库的按钮，显示数据库中所有的表到左边listbox中
        private void btnconn_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                lsbleft.Items.Clear();
                lsbright.Items.Clear();
                listleft.Clear();
                listright.Clear();
                string connstr = txtconnstr.Text.Trim();

                string tmp = (ddldb.SelectedItem as ComboBoxItem).Content.ToString();

                //sqlite
                if (tmp == "SQLite")
                {
                    SQLiteConnection conn = new SQLiteConnection(connstr);
                    conn.Open();
                    string sql = "select name from sqlite_master where type='table'";
                    SQLiteCommand cmd = new SQLiteCommand(sql, conn);
                    SQLiteDataReader sdr = cmd.ExecuteReader();
                    while (sdr.Read())
                    {
                        listleft.Add(sdr[0].ToString());
                    }




                    conn.Close();
                }

                //MySQL
                if (tmp == "MySQL")
                {
                    MySqlConnection conn = new MySqlConnection(connstr);
                    conn.Open();
                    string sql = "show tables";
                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    MySqlDataReader mdr = cmd.ExecuteReader();
                    while (mdr.Read())
                    {
                        listleft.Add(mdr[0].ToString());
                    }

                    conn.Close();
                }

                //Access
                if (tmp == "Access")
                {
                    OleDbConnection conn = new OleDbConnection(connstr);
                    conn.Open();
                    string sql = "SELECT MSysObjects.Name FROM MsysObjects WHERE (Left([Name],1)<>\"~\") AND (Left$([Name],4) <> \"Msys\") AND (MSysObjects.Type)=1 ORDER BY MSysObjects.Name;";
                    OleDbCommand cmd = new OleDbCommand(sql, conn);
                    OleDbDataReader odr = cmd.ExecuteReader();
                    while (odr.Read())
                    {
                        listleft.Add(odr["name"].ToString());
                    }

                    conn.Close();
                }

                //SQL Server
                if (tmp == "SQL Server")
                {
                    SqlConnection conn = new SqlConnection(connstr);
                    conn.Open();
                    string sql = "SELECT name FROM sysobjects WHERE xtype = 'U' AND OBJECTPROPERTY (id, 'IsMSShipped') = 0  order by name ";
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    SqlDataReader sdr = cmd.ExecuteReader();
                    while (sdr.Read())
                    {
                        listleft.Add(sdr["name"].ToString());
                    }

                    conn.Close();
                }

                foreach (var item in listleft)
                {
                    lsbleft.Items.Add(item);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        //左边→右边
        private void btnleft2right_Click(object sender, RoutedEventArgs e)
        {
            foreach (string item in lsbleft.SelectedItems)
            {
                listright.Add(item);
                listleft.Remove(item);
            }

            listleft.Sort();
            listright.Sort();

            lsbright.Items.Clear();
            foreach (var item in listright)
            {
                lsbright.Items.Add(item);
            }
            lsbleft.Items.Clear();
            foreach (var item in listleft)
            {
                lsbleft.Items.Add(item);
            }
        }
        //左边→右边 全部
        private void btnleft2right_all_Click(object sender, RoutedEventArgs e)
        {
            foreach (string item in lsbleft.Items)
            {
                listright.Add(item);
                listleft.Remove(item);
            }

            listleft.Sort();
            listright.Sort();

            lsbright.Items.Clear();
            foreach (var item in listright)
            {
                lsbright.Items.Add(item);
            }
            lsbleft.Items.Clear();

        }
        //右边→左边
        private void btnright2left_Click(object sender, RoutedEventArgs e)
        {
            foreach (string item in lsbright.SelectedItems)
            {
                listright.Remove(item);
                listleft.Add(item);
            }

            listleft.Sort();
            listright.Sort();

            lsbright.Items.Clear();
            foreach (var item in listright)
            {
                lsbright.Items.Add(item);
            }
            lsbleft.Items.Clear();
            foreach (var item in listleft)
            {
                lsbleft.Items.Add(item);
            }
        }
        //右边→ 左边 全部
        private void btnright2left_all_Click(object sender, RoutedEventArgs e)
        {
            foreach (string item in lsbright.Items)
            {
                listright.Remove(item);
                listleft.Add(item);
            }

            listleft.Sort();
            listright.Sort();

            lsbright.Items.Clear();

            lsbleft.Items.Clear();
            foreach (var item in listleft)
            {
                lsbleft.Items.Add(item);
            }
        }
        //DAL预览按钮
        private void btndalyulan_Click(object sender, RoutedEventArgs e)
        {
            if (lsbright.Items.Count == 0)
            {
                MessageBox.Show("请选择操作表.");
                return;
            }

            string tmp = (ddldb.SelectedItem as ComboBoxItem).Content.ToString();


            string tabname = lsbright.Items[0].ToString();
            string ns = txtmingming.Text.Trim();
            if (string.IsNullOrEmpty(ns))
            {
                MessageBox.Show("请输入命名空间.");
                return;
            }
            string front = txtqianzui.Text.Trim();
            string classname = tabname;
            if (front.Length > 0)
            {
                classname = tabname.Replace(front, "");
            }
            classname = classname.Substring(0, 1).ToUpper() + classname.Substring(1);
            string connstr = txtconnstr.Text.Trim();

            Model.OneTable onetable = BLL.Tools.GetOneTable(tmp, connstr, tabname);

            if (radmicro.IsChecked.Value)
            {
                //基于微软企业库生成DAL
                txtyulan.Text = GenDAL_MSSQL_One.GenAllCode(ns, tabname, classname, connstr);
            }
            else if (radIJLYNIVFHP.IsChecked.Value)
            {
                //基于IJLYNIVFHP数据库操作类生成DAL
                if (tmp == "SQLite")
                {
                    txtyulan.Text = GenDAL_SQLite_IJLYNIVFHP.GenAllCode(ns, tabname, classname, connstr);
                }
                if (tmp == "MySQL")
                {
                    txtyulan.Text = GenDAL_MySQL_IJLYNIVFHP.GenAllCode(ns, tabname, classname, connstr);
                }
                if (tmp == "Access")
                {
                    txtyulan.Text = GenDAL_Access_IJLYNIVFHP.GenAllCode(ns, tabname, classname, connstr);
                }
                if (tmp == "SQL Server")
                {
                    txtyulan.Text = GenDAL_MSSQL_IJLYNIVFHP.GenAllCode(ns, tabname, classname, connstr);
                }
            }
            else
            {
                //基于dapper
                if (tmp == "SQL Server")
                {
                    txtyulan.Text = BLL.GenDAL_MSSQL_IJLYNIVFHP.GenAllCode(ns, tabname, classname, connstr, "MSSQLDALTemp_dapper.txt");
                }
                else
                {
                    txtyulan.Text = GenDAL_MySQL_IJLYNIVFHP.GenAllCode(ns, tabname, classname, connstr, "MySQLDALTemp_dapper.txt");
                }
            }
        }
        //model预览按钮
        private void btnmodelyulan_Click(object sender, RoutedEventArgs e)
        {
            if (lsbright.Items.Count == 0)
            {
                MessageBox.Show("请选择操作表.");
                return;
            }

            string tmp = (ddldb.SelectedItem as ComboBoxItem).Content.ToString();

            string tabname = lsbright.Items[0].ToString();
            string ns = txtmingming.Text.Trim();
            if (string.IsNullOrEmpty(ns))
            {
                MessageBox.Show("请输入命名空间.");
                return;
            }
            string front = txtqianzui.Text.Trim();
            string classname = tabname;
            if (front.Length > 0)
            {
                classname = tabname.Replace(front, "");
            }
            classname = classname.Substring(0, 1).ToUpper() + classname.Substring(1);
            string connstr = txtconnstr.Text.Trim();

            if (tmp == "SQLite")
            {
                txtyulan.Text = GenModel_SQLite.GenAllCode(ns, tabname, classname, connstr);
            }


            if (tmp == "MySQL")
            {
                txtyulan.Text = GenModel_MySQL.GenAllCode(ns, tabname, classname, connstr);
            }

            if (tmp == "Access")
            {
                txtyulan.Text = GenModel_Access.GenAllCode(ns, tabname, classname, connstr);
            }

            if (tmp == "SQL Server")
            {
                txtyulan.Text = GenModel_MSSQL.GenAllCode(ns, tabname, classname, connstr);
            }
        }
        //生成数据库文档代码
        private void btngenwendang_Click(object sender, RoutedEventArgs e)
        {
            if (lsbright.Items.Count == 0)
            {
                MessageBox.Show("请选择操作表.");
                return;
            }

            List<string> list_right = new List<string>(); //要生成数据库文档的表名集合
            string connstr = txtconnstr.Text.Trim(); // 数据库连接字符串

            foreach (string tabname in lsbright.Items)
            {
                list_right.Add(tabname);
            }

            string tmp = (ddldb.SelectedItem as ComboBoxItem).Content.ToString(); //数据库类型


            string output = txtmulu.Text.Trim();
            if (string.IsNullOrEmpty(output))
            {
                MessageBox.Show("请输入代码生成目录路径！");
                return;
            }
            if (!Directory.Exists(output))
            {
                Directory.CreateDirectory(output);
            }

            string filepath = output + Tools.GetDBName(tmp, connstr) + "数据库文档.html";
            StreamWriter sw = new StreamWriter(filepath, false, Encoding.UTF8);
            sw.Write(DBDocGen.GenDoc(tmp, connstr, list_right));
            sw.Flush();
            sw.Close();
            sw.Dispose();


            MessageBox.Show("文档生成功.");
            txtyulan.Text = "文档已生成到:" + output;
            System.Diagnostics.Process.Start(output);
        }
        //生成代码按钮
        private void btngencode_Click(object sender, RoutedEventArgs e)
        {
            try
            {


                if (lsbright.Items.Count == 0)
                {
                    MessageBox.Show("请选择操作表.");
                    return;
                }

                string tmp = (ddldb.SelectedItem as ComboBoxItem).Content.ToString();


                string output = txtmulu.Text.Trim();
                if (string.IsNullOrEmpty(output))
                {
                    MessageBox.Show("请输入代码生成目录！");
                    return;
                }
                if (!Directory.Exists(output))
                {
                    Directory.CreateDirectory(output);
                }

                #region Model
                string output_model = "";
                if (output.LastIndexOf("\\") >= 0)
                {
                    output_model = output + "Model\\";
                }
                else
                {
                    output_model = output + "\\Model\\";
                }
                if (!Directory.Exists(output_model))
                {
                    Directory.CreateDirectory(output_model);
                }

                string ns = txtmingming.Text.Trim();
                if (string.IsNullOrEmpty(ns))
                {
                    MessageBox.Show("请输入命名空间.");
                    return;
                }
                string front = txtqianzui.Text.Trim();
                string classname = "";
                string connstr = txtconnstr.Text.Trim();

                foreach (string tabname in lsbright.Items)
                {
                    classname = tabname;
                    if (front.Length > 0)
                    {
                        classname = tabname.Replace(front, "");
                    }
                    classname = classname.Substring(0, 1).ToUpper() + classname.Substring(1);

                    string filepath = output_model + classname + ".cs";
                    StreamWriter sw = new StreamWriter(filepath, false);
                    if (tmp == "Sqlite")
                    {
                        sw.Write(GenModel_SQLite.GenAllCode(ns, tabname, classname, connstr));
                    }
                    if (tmp == "MySQL")
                    {
                        sw.Write(GenModel_MySQL.GenAllCode(ns, tabname, classname, connstr));
                    }
                    if (tmp == "SQL Server")
                    {
                        sw.Write(GenModel_MSSQL.GenAllCode(ns, tabname, classname, connstr));
                    }
                    else if (tmp == "Access")
                    {
                        sw.Write(GenModel_Access.GenAllCode(ns, tabname, classname, connstr));
                    }
                    sw.Flush();
                    sw.Close();
                    sw.Dispose();
                }
                #endregion

                #region DAL
                string output_dal = "";
                if (output.LastIndexOf("\\") >= 0)
                {
                    output_dal = output + "DAL\\";
                }
                else
                {
                    output_dal = output + "\\DAL\\";
                }
                if (!Directory.Exists(output_dal))
                {
                    Directory.CreateDirectory(output_dal);
                }

                foreach (string tabname in lsbright.Items)
                {
                    classname = tabname;

                    if (front.Length > 0)
                    {
                        classname = tabname.Replace(front, "");
                    }
                    classname = classname.Substring(0, 1).ToUpper() + classname.Substring(1);

                    string filepath = output_dal + classname + "DAL.cs";
                    StreamWriter sw = new StreamWriter(filepath, false);
                    if (radmicro.IsChecked.Value)
                    {
                        //基于微软企业库
                        sw.Write(GenDAL_MSSQL_One.GenAllCode(ns, tabname, classname, connstr));
                    }
                    else if (radIJLYNIVFHP.IsChecked.Value)
                    {
                        //基于IJLYNIVFHP数据库操作类
                        if (tmp == "Sqlite")
                        {
                            sw.Write(GenDAL_SQLite_IJLYNIVFHP.GenAllCode(ns, tabname, classname, connstr));
                        }
                        if (tmp == "MySQL")
                        {
                            sw.Write(GenDAL_MySQL_IJLYNIVFHP.GenAllCode(ns, tabname, classname, connstr));
                        }
                        if (tmp == "SQL Server")
                        {
                            sw.Write(GenDAL_MSSQL_IJLYNIVFHP.GenAllCode(ns, tabname, classname, connstr));
                        }
                        else if (tmp == "Access")
                        {
                            sw.Write(GenDAL_Access_IJLYNIVFHP.GenAllCode(ns, tabname, classname, connstr));
                        }
                    }
                    else
                    {
                        //基于Dapper
                        if (tmp == "SQL Server")
                        {
                            sw.Write(GenDAL_MSSQL_IJLYNIVFHP.GenAllCode(ns, tabname, classname, connstr, "MSSQLDALTemp_dapper.txt"));
                        }
                        else if (tmp == "MySQL")
                        {
                            sw.Write(GenDAL_MySQL_IJLYNIVFHP.GenAllCode(ns, tabname, classname, connstr, "MySQLDALTemp_dapper.txt"));
                        }
                    }
                    sw.Flush();
                    sw.Close();
                    sw.Dispose();
                }
                #endregion

                #region BLL
                string output_bll = "";
                if (output.LastIndexOf("\\") >= 0)
                {
                    output_bll = output + "BLL\\";
                }
                else
                {
                    output_bll = output + "\\BLL\\";
                }
                if (!Directory.Exists(output_bll))
                {
                    Directory.CreateDirectory(output_bll);
                }

                foreach (string tabname in lsbright.Items)
                {
                    classname = tabname;

                    if (front.Length > 0)
                    {
                        classname = tabname.Replace(front, "");
                    }
                    classname = classname.Substring(0, 1).ToUpper() + classname.Substring(1);

                    string filepath = output_bll + classname + "BLL.cs";
                    StreamWriter sw = new StreamWriter(filepath, false);
                    if (radmicro.IsChecked.Value)
                    {
                        //基于微软企业库
                    }
                    else if (radIJLYNIVFHP.IsChecked.Value)
                    {
                        //基于IJLYNIVFHP数据库操作类
                        if (tmp == "Sqlite")
                        {

                        }
                        if (tmp == "MySQL")
                        {

                        }
                        if (tmp == "SQL Server")
                        {
                            sw.Write(GenBLL_MSSQL_IJLYNIVFHP.GenAllCode(ns, tabname, classname, connstr));
                        }
                        else if (tmp == "Access")
                        {

                        }
                    }
                    else
                    {
                        //基于dapper

                    }
                    sw.Flush();
                    sw.Close();
                    sw.Dispose();
                }
                #endregion

                #region MSSQLHelper
                if (radIJLYNIVFHP.IsChecked.Value)
                {
                    if (tmp == "Sqlite")
                    {
                        string filepath = output_dal + "SQLiteHelper.cs";
                        StreamWriter sw = new StreamWriter(filepath, false);
                        sw.Write(GenDAL_SQLite_IJLYNIVFHP.GenSQLiteHelper(ns, connstr));
                        sw.Flush();
                        sw.Close();
                        sw.Dispose();
                    }
                    if (tmp == "SQL Server")
                    {

                        string filepath = output_dal + "MSSQLHelper.cs";
                        StreamWriter sw = new StreamWriter(filepath, false);
                        sw.Write(GenDAL_MSSQL_IJLYNIVFHP.GenMSSQLHelper(ns, connstr));
                        sw.Flush();
                        sw.Close();
                        sw.Dispose();



                    }
                    else if (tmp == "Access")
                    {
                        string filepath = output_dal + "AccessHelper.cs";
                        StreamWriter sw = new StreamWriter(filepath, false);
                        sw.Write(GenDAL_Access_IJLYNIVFHP.GenAccessHelper(ns, connstr));
                        sw.Flush();
                        sw.Close();
                        sw.Dispose();
                    }
                    else if (tmp == "MySQL")
                    {
                        string filepath = output_dal + "MySQLHelper.cs";
                        StreamWriter sw = new StreamWriter(filepath, false);
                        sw.Write(GenDAL_MySQL_IJLYNIVFHP.GenMySQLHelper(ns, connstr));
                        sw.Flush();
                        sw.Close();
                        sw.Dispose();
                    }
                }
                else if (raddapper.IsChecked.Value)
                {
                    string filepath = output_dal + "ConnectionFactory.cs";
                    StreamWriter sw = new StreamWriter(filepath, false);
                    sw.Write(GenDAL_MSSQL_IJLYNIVFHP.GenMSSQLHelper(ns, connstr, "MSSQLHelperTemp_dapper.txt"));
                    sw.Flush();
                    sw.Close();
                    sw.Dispose();
                }
                #endregion

                MessageBox.Show("代码生成功.");
                txtyulan.Text = "代码已生成到:" + output;
                System.Diagnostics.Process.Start(output);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"出错：{ex.Message}");
            }
        }
        //生成list页面前台代码
        private void btnlist1_Click(object sender, RoutedEventArgs e)
        {
            if (lsbright.Items.Count == 0)
            {
                MessageBox.Show("请选择需要操作的表！");
                return;
            }
            string tabname = lsbright.Items[0].ToString();
            string front = txtqianzui.Text.Trim();
            string connstr = txtconnstr.Text.Trim(); // 数据库连接字符串
            txtyulan.Text = new GenList_MSSQL().List1(tabname, front, connstr);
        }
        //生成 list页面后台代码
        private void btnlist2_Click(object sender, RoutedEventArgs e)
        {
            if (lsbright.Items.Count == 0)
            {
                MessageBox.Show("请选择需要操作的表！");
                return;
            }
            string tabname = lsbright.Items[0].ToString();
            string front = txtqianzui.Text.Trim();
            string classname = tabname;  //类名

            if (front.Length > 0)
            {
                classname = tabname.Replace(front, "");
            }
            classname = classname.Substring(0, 1).ToUpper() + classname.Substring(1);
            txtyulan.Text = new GenList_MSSQL().List2(classname);
        }
        //生成del.ashx代码
        private void btndelashx_Click(object sender, RoutedEventArgs e)
        {
            if (lsbright.Items.Count == 0)
            {
                MessageBox.Show("请选择需要操作的表！");
                return;
            }
            string tabname = lsbright.Items[0].ToString();
            string front = txtqianzui.Text.Trim();
            string classname = tabname;  //类名

            if (front.Length > 0)
            {
                classname = tabname.Replace(front, "");
            }
            classname = classname.Substring(0, 1).ToUpper() + classname.Substring(1);
            txtyulan.Text = new GenList_MSSQL().Del(classname);
        }
        //生成add页面前台代码
        private void btnadd1_Click(object sender, RoutedEventArgs e)
        {
            if (lsbright.Items.Count == 0)
            {
                MessageBox.Show("请选择需要操作的表！");
                return;
            }
            string tabname = lsbright.Items[0].ToString();
            string front = txtqianzui.Text.Trim();
            string connstr = txtconnstr.Text.Trim(); // 数据库连接字符串
            txtyulan.Text = new GenList_MSSQL().Add1(tabname, front, connstr);
        }
        //生成 add页面后台代码
        private void btnadd2_Click(object sender, RoutedEventArgs e)
        {
            if (lsbright.Items.Count == 0)
            {
                MessageBox.Show("请选择需要操作的表！");
                return;
            }
            string tabname = lsbright.Items[0].ToString();
            string front = txtqianzui.Text.Trim();
            string connstr = txtconnstr.Text.Trim(); // 数据库连接字符串
            txtyulan.Text = new BLL.GenList_MSSQL().Add2(tabname, front, connstr);
        }
        //关闭窗体前保存相关信息
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                string dbtype = (ddldb.SelectedItem as ComboBoxItem).Content.ToString();
                string connstr = txtconnstr.Text.Trim();
                string genway = radmicro.IsChecked.Value ? "0" : radIJLYNIVFHP.IsChecked.Value ? "1" : "2";
                string ns = txtmingming.Text.Trim();
                string front = txtqianzui.Text.Trim();
                string output = txtmulu.Text.Trim();


                XElement owner = new XElement("Config", new XElement[]{
                        new XElement("dbtype",dbtype),
                        new XElement("connstr",connstr),
                        new XElement("genway",genway),
                        new XElement("namespace",ns),
                        new XElement("front",front),
                        new XElement("output",output),
                   });
                owner.Save(System.Environment.CurrentDirectory + "\\appconfig.xml");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        //生成MVC——LIST前台CSHTML代码
        private void btnlist_mvc_Click(object sender, RoutedEventArgs e)
        {
            if (lsbright.Items.Count == 0)
            {
                MessageBox.Show("请选择操作表.");
                return;
            }

            string dbtype = (ddldb.SelectedItem as ComboBoxItem).Content.ToString();


            string tabname = lsbright.Items[0].ToString();
            string ns = txtmingming.Text.Trim();
            if (string.IsNullOrEmpty(ns))
            {
                MessageBox.Show("请输入命名空间.");
                return;
            }
            string front = txtqianzui.Text.Trim();
            string classname = tabname;
            if (front.Length > 0)
            {
                classname = tabname.Replace(front, "");
            }
            classname = classname.Substring(0, 1).ToUpper() + classname.Substring(1);
            string connstr = txtconnstr.Text.Trim();

            int layui_version = (bool)layui1.IsChecked ? 1 : 2;

            txtyulan.Text = new BLL.GenCode_MVC().List(dbtype, ns, tabname, classname, connstr, layui_version);

        }

        /// <summary>
        /// 生成add_mvc页面CSHTML代码
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnadd_mvc_Click(object sender, RoutedEventArgs e)
        {
            if (lsbright.Items.Count == 0)
            {
                MessageBox.Show("请选择操作表.");
                return;
            }

            string dbtype = (ddldb.SelectedItem as ComboBoxItem).Content.ToString();


            string tabname = lsbright.Items[0].ToString();
            string ns = txtmingming.Text.Trim();
            if (string.IsNullOrEmpty(ns))
            {
                MessageBox.Show("请输入命名空间.");
                return;
            }
            string front = txtqianzui.Text.Trim();
            string classname = tabname;
            if (front.Length > 0)
            {
                classname = tabname.Replace(front, "");
            }
            classname = classname.Substring(0, 1).ToUpper() + classname.Substring(1);
            string connstr = txtconnstr.Text.Trim();

 

            int layui_version = (bool)layui1.IsChecked ? 1 : 2;

            string addlayout = (bool)div.IsChecked ? "div" : "table";
      

            txtyulan.Text = new BLL.GenCode_MVC().Add(dbtype, ns, tabname, classname, connstr,layui_version,addlayout);
        }
        //生成 MVC后台控制器代码
        private void btncode_mvc_Click(object sender, RoutedEventArgs e)
        {
            if (lsbright.Items.Count == 0)
            {
                MessageBox.Show("请选择操作表.");
                return;
            }

            string dbtype = (ddldb.SelectedItem as ComboBoxItem).Content.ToString();


            string tabname = lsbright.Items[0].ToString();
            string ns = txtmingming.Text.Trim();
            if (string.IsNullOrEmpty(ns))
            {
                MessageBox.Show("请输入命名空间.");
                return;
            }
            string front = txtqianzui.Text.Trim();
            string classname = tabname;
            if (front.Length > 0)
            {
                classname = tabname.Replace(front, "");
            }
            classname = classname.Substring(0, 1).ToUpper() + classname.Substring(1);
            string connstr = txtconnstr.Text.Trim();

            txtyulan.Text = new BLL.GenCode_MVC().BackCode(dbtype, ns, tabname, classname, connstr);
        }

        //生成数据库的insert语句
        private void btngeninsertsql_Click(object sender, RoutedEventArgs e)
        {
            if (lsbright.Items.Count == 0)
            {
                MessageBox.Show("请选择操作表.");
                return;
            }

            List<string> list_right = new List<string>(); //要生成数据库文档的表名集合
            string connstr = txtconnstr.Text.Trim(); // 数据库连接字符串

            foreach (string tabname in lsbright.Items)
            {
                list_right.Add(tabname);
            }

            string dbtype = (ddldb.SelectedItem as ComboBoxItem).Content.ToString(); //数据库类型

            StringBuilder sb = new StringBuilder();
            sb.Append("--SET IDENTITY_INSERT [dbo].[admin] ON \r\n");
            sb.Append("--SET IDENTITY_INSERT [dbo].[admin] OFF \r\n\r\n");

            if (dbtype.ToLower().Contains("server"))
            {
                foreach (var tablename in list_right)
                {
                    Model.OneTable onetable = Tools.GetOneTable(dbtype, connstr, tablename);

                    string files_str = String.Join(",", onetable.fields.Where(o=>o.isprimarykey==false).Select(a => a.name).ToArray()); //如id,name,age...最后是没有,的

                    List<string> list_value = new List<string>();
                    #region 拼接value
                    DataTable dt_value = Tools.GetData(dbtype, connstr, tablename);
                    foreach (DataRow row in dt_value.Rows)
                    {
                        list_value.Clear();
                        foreach (var onefiled in onetable.fields)
                        {
                            if (onefiled.dbtype == "datetime")
                            {
                                list_value.Add("'" + Convert.ToDateTime(row[onefiled.name]).ToString("yyyy-MM-dd HH:mm:ss") + "'");
                            }
                            else
                            {
                                list_value.Add("'" + row[onefiled.name].ToString() + "'");
                            }
                         
                        }
                        string value_str = string.Join(",", list_value.ToArray());
                        string sql = $"insert into [{tablename}]({files_str}) values({value_str})";
                        sb.Append(sql + "\r\n\r\n");
                    }
                    #endregion
                
                }

            }
            else
            {
                sb.Append("该数据库末做。");
            }


            txtyulan.Text = sb.ToString();

        }

        //生成MVA后台页面框架代码
        private void btn_gen_admin_Click(object sender, RoutedEventArgs e)
        {
            string output = txtmulu.Text.Trim();
            if (string.IsNullOrEmpty(output))
            {
                MessageBox.Show("请输入生成目录路径！");
                return;
            }
            output += @"\Adnn1n\";
            if (!Directory.Exists(output))
            {
                Directory.CreateDirectory(output);
            }

            string source_path = System.Environment.CurrentDirectory + @"\classtemp\Adnn1n\"; //原文件目录
            string target_path = output; //要复制到哪里的目录

            DirectoryCopy(source_path, target_path, true);

            MessageBox.Show("生成成功，注：不要直接复制，里面的命名空间什么的都没有改，用记事本打开了有选择性的复制！");
            System.Diagnostics.Process.Start(output);
        }

        //复制目录 https://msdn.microsoft.com/zh-cn/library/bb762914(v=vs.110).aspx
        private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            DirectoryInfo[] dirs = dir.GetDirectories();
            // If the destination directory doesn't exist, create it.
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = System.IO.Path.Combine(destDirName, file.Name);
                if(!File.Exists(temppath))
                {
                    file.CopyTo(temppath, false);
                }
                
            }

            // If copying subdirectories, copy them and their contents to new location.
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = System.IO.Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                }
            }
        }
    }
}
