using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dapper.Lite
{
    /// <summary>
    /// 分表映射
    /// </summary>
    public class SplitTableMapping
    {
        private ConcurrentDictionary<Type, string> _dictForTableName = new ConcurrentDictionary<Type, string>();

        /// <summary>
        /// 分表映射 
        /// </summary>
        /// <param name="entityType">实体类型</param>
        /// <param name="tableName">数据库表名称</param>
        public SplitTableMapping(Type entityType, string tableName)
        {
            AddSplitTable(entityType, tableName);
        }

        /// <summary>
        /// 添加一个分表映射
        /// </summary>
        /// <param name="entityType">实体类型</param>
        /// <param name="tableName">数据库表名称</param>
        public void AddSplitTable(Type entityType, string tableName)
        {
            _dictForTableName.TryAdd(entityType, tableName);
        }

        /// <summary>
        /// 获取数据库表名
        /// </summary>
        /// <param name="entityType">实体类型</param>
        internal string GetTableName(Type entityType)
        {
            string tableName = null;
            _dictForTableName.TryGetValue(entityType, out tableName);
            return tableName;
        }

    }
}
