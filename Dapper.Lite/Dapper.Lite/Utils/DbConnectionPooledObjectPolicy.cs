using Microsoft.Extensions.ObjectPool;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dapper.Lite
{
    internal class DbConnectionPooledObjectPolicy : IPooledObjectPolicy<DbConnectionExt>
    {
        /// <summary>
        /// 数据库提供者
        /// </summary>
        private readonly IProvider _provider;

        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        private readonly string _connectionString;

        private readonly DbConnectionFactory _dbConnectionFactory;

        public DbConnectionPooledObjectPolicy(IProvider provider, string connectionString, DbConnectionFactory dbConnectionFactory)
        {
            _provider = provider;
            _connectionString = connectionString;
            _dbConnectionFactory = dbConnectionFactory;
        }

        public DbConnectionExt Create()
        {
            DbConnection conn = _provider.CreateConnection(_connectionString);
            DbConnectionExt connExt = new DbConnectionExt(conn, _provider, _connectionString, _dbConnectionFactory);
            return connExt;
        }

        public bool Return(DbConnectionExt obj)
        {
            return true;
        }
    }
}
