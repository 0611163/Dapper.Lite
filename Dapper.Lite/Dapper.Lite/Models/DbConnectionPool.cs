using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dapper.Lite
{
    /// <summary>
    /// 数据库连接集合
    /// </summary>
    internal class DbConnectionPool
    {
        /// <summary>
        /// 数据库Provider
        /// </summary>
        public IProvider Provider { get; set; }

        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        public string ConnnectionString { get; set; }

        /// <summary>
        /// 数据库连接集合
        /// </summary>
        public ConcurrentQueue<DbConnectionExt> Connections { get; set; }

        /// <summary>
        /// 数据库连接集合 构造函数
        /// </summary>
        public DbConnectionPool(IProvider provider, string connectionString)
        {
            Connections = new ConcurrentQueue<DbConnectionExt>();
            Provider = provider;
            ConnnectionString = connectionString;
        }
    }
}
