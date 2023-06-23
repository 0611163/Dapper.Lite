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

        private ConcurrentDictionary<Type, string> _dictForSchema = new ConcurrentDictionary<Type, string>();

        /// <summary>
        /// 分表映射
        /// </summary>
        public SplitTableMapping() { }

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
        /// <param name="schema">数据库Schema</param>
        public void AddSplitTable(Type entityType, string tableName, string schema = null)
        {
            _dictForTableName.TryAdd(entityType, tableName);
            _dictForSchema.TryAdd(entityType, schema);
        }

        /// <summary>
        /// 是否存在映射
        /// </summary>
        internal bool Exists(Type entityType)
        {
            return _dictForTableName.ContainsKey(entityType);
        }

        /// <summary>
        /// 获取数据库表名
        /// </summary>
        /// <param name="entityType">实体类型</param>
        internal string GetTableName(Type entityType)
        {
            string tableName;
            _dictForTableName.TryGetValue(entityType, out tableName);
            return tableName;
        }

        /// <summary>
        /// 获取数据库Schema
        /// </summary>
        /// <param name="entityType">实体类型</param>
        internal string GetSchema(Type entityType)
        {
            string schema;
            _dictForSchema.TryGetValue(entityType, out schema);
            return schema;
        }

    }
}
