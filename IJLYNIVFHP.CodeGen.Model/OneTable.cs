using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IJLYNIVFHP.CodeGen.Model
{
    /// <summary>
    /// 一个表
    /// </summary>
    public class OneTable
    {
        /// <summary>
        /// 表名称
        /// </summary>
        public string name { set; get; }
        /// <summary>
        /// 表里所有的字段
        /// </summary>
        public List<Model.OneField> fields { set; get; }
    }
}
