using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dapper.Lite
{
    public partial class DbSession : IDbSession
    {
        /// <summary>
        /// 设置表名映射
        /// </summary>
        public void SetTableNameMap<T>(string tableName, string schema = null)
        {
            _splitTableMapping.AddSplitTable(typeof(T), tableName, schema);
        }

        /// <summary>
        /// 设置表名映射
        /// </summary>
        public void SetTableNameMap(Type type, string tableName, string schema = null)
        {
            _splitTableMapping.AddSplitTable(type, tableName, schema);
        }
    }
}
