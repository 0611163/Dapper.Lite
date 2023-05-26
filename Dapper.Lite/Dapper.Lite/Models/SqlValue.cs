using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dapper.Lite
{
    /// <summary>
    /// SQL与参数
    /// </summary>
    public class SqlValue
    {
        /// <summary>
        /// SQL片段
        /// </summary>
        public string Sql { get; set; }

        /// <summary>
        /// 参数化查询的参数
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// SQL与参数
        /// </summary>
        /// <param name="value">参数化查询的参数</param>
        public SqlValue(object value)
        {
            Sql = "{0}";
            Value = value;
        }

        /// <summary>
        /// SQL与参数
        /// </summary>
        /// <param name="sql">SQL片段</param>
        /// <param name="value">参数化查询的参数</param>
        public SqlValue(string sql, object value)
        {
            Sql = sql;
            Value = value;
        }
    }
}
