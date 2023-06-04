using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Dapper.Lite
{
    /// <summary>
    /// 数据库连接工厂
    /// </summary>
    public class DbConnectionFactory
    {
        /// <summary>
        /// 数据库连接池
        /// </summary>
        private readonly DbConnectionPool _connectionPool;

        /// <summary>
        /// 连接池最大数量
        /// </summary>
        private readonly int _maxPoolSize;

        /// <summary>
        /// 数据库提供者
        /// </summary>
        private readonly IProvider _provider;

        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        private readonly string _connectionString;

        #region 构造函数
        /// <summary>
        /// 数据库连接工厂构造函数
        /// </summary>
        public DbConnectionFactory(IProvider provider, string connectionString, int maxPoolSize)
        {
            _provider = provider;
            _connectionString = connectionString;
            _maxPoolSize = maxPoolSize;
            _connectionPool = new DbConnectionPool(provider, connectionString);

            //初始化数据库连接对象池
            for (int i = 0; i < maxPoolSize; i++)
            {
                DbConnection conn = _provider.CreateConnection(_connectionString);
                DbConnectionExt connExt = new DbConnectionExt(conn, provider, connectionString, this);
                _connectionPool.Connections.Enqueue(connExt);
                if (i < 5 && conn.State == ConnectionState.Closed) conn.Open();
            }
        }
        #endregion

        #region GetConnection 从数据库连接池获取一个数据库连接
        /// <summary>
        /// 从数据库连接池获取一个数据库连接
        /// </summary>
        public DbConnectionExt GetConnection(DbTransactionExt _tran)
        {
            if (_tran != null)
            {
                return _tran.ConnEx;
            }

            DbConnectionExt connExt;
            SpinWait spinWait = new SpinWait();
            while (!_connectionPool.Connections.TryDequeue(out connExt))
            {
                spinWait.SpinOnce();
            }

            return connExt;
        }
        #endregion

        #region GetConnectionAsync 从数据库连接池获取一个数据库连接
        /// <summary>
        /// 从数据库连接池获取一个数据库连接
        /// </summary>
        public async Task<DbConnectionExt> GetConnectionAsync(DbTransactionExt _tran)
        {
            if (_tran != null)
            {
                return _tran.ConnEx;
            }

            DbConnectionExt connExt;
            SpinWait spinWait = new SpinWait();
            while (!_connectionPool.Connections.TryDequeue(out connExt))
            {
                spinWait.SpinOnce();
            }

            await Task.CompletedTask;

            return connExt;
        }
        #endregion

        #region 回收数据库连接
        /// <summary>
        /// 回收数据库连接
        /// </summary>
        public void Release(DbConnectionExt connExt)
        {
            _connectionPool.Connections.Enqueue(connExt);
        }
        #endregion

    }
}
