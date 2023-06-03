using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Extensions.ObjectPool;

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
        private readonly DefaultObjectPool<DbConnectionExt> _connectionPool;

        private readonly DbConnectionPool _connPool;

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
            _connPool = new DbConnectionPool(provider, connectionString);

            //初始化数据库连接对象池
            var policy = new DbConnectionPooledObjectPolicy(provider, connectionString, this);
            _connectionPool = new DefaultObjectPool<DbConnectionExt>(policy, maxPoolSize);

            List<DbConnectionExt> list = new List<DbConnectionExt>();
            for (int i = 0; i < 5; i++)
            {
                var connExt = _connectionPool.Get();
                connExt.Conn.Open();
                list.Add(connExt);
            }
            foreach (var item in list)
            {
                _connectionPool.Return(item);
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

            return _connectionPool.Get();
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

            await Task.CompletedTask;

            return _connectionPool.Get();
        }
        #endregion

        #region 回收数据库连接
        /// <summary>
        /// 回收数据库连接
        /// </summary>
        public void Release(DbConnectionExt connExt)
        {
            _connectionPool.Return(connExt);
        }
        #endregion

    }
}
