using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IJLYNIVFHP.CodeGen.Model
{
    /// <summary>
    /// 数据库中一个字段的内容
    /// </summary>
    public class OneField
    {
        /// <summary>
        /// 字段名
        /// </summary>
        public string name { set; get; }
        /// <summary>
        /// 数据库中的类型
        /// </summary>
        public string dbtype { set; get; }
        /// <summary>
        /// 对应C#中的类型
        /// </summary>
        public string csharptype { set; get; }
        /// <summary>
        /// 长度
        /// </summary>
        public int length { set; get; }
        /// <summary>
        /// 小数点长度
        /// </summary>
        public int xscale { set; get; }
        /// <summary>
        /// 是否可空
        /// </summary>
        public bool isnull { set; get; }
        /// <summary>
        /// 默认值 
        /// </summary>
        public string defaultvalue { set; get; }
        /// <summary>
        /// 字段说明
        /// </summary>
        public string remark { set; get; }
        /// <summary>
        /// 是否主键
        /// </summary>
        public bool isprimarykey { set; get; }
    }
}
