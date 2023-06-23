using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dapper.Lite
{
    public partial interface IDbSession
    {
        /// <summary>
        /// 设置表名映射
        /// </summary>
        void SetTableNameMap<T>(string tableName, string schema = null);

        /// <summary>
        /// 设置表名映射
        /// </summary>
        void SetTableNameMap(Type type, string tableName, string schema = null);
    }
}
