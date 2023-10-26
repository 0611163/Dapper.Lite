using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dapper.Lite
{
    /// <summary>
    /// 表达式树解析，返回值
    /// </summary>
    internal class ExpValue
    {
        /// <summary>
        /// Type = SqlAndDbParameter
        /// </summary>
        public List<DbParameter> DbParameters { get; set; }

        /// <summary>
        /// Type = SqlAndDbParameter
        /// </summary>
        public String Sql { get; set; }

        /// <summary>
        /// 表达式树解析，返回值
        /// </summary>
        public ExpValue()
        {
            DbParameters = new List<DbParameter>();
        }
    }
}
