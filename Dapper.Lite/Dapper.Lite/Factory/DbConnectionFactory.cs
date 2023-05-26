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
        /// 数据库连接超时释放时间
        /// </summary>
        private readonly int _timeout = 10;

        /// <summary>
        /// 连接池最小数量
        /// </summary>
        private readonly int _minPoolSize = 10;

        /// <summary>
        /// 定时器
        /// </summary>
        private readonly Timer _timer;

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
            _connectionPool = new DbConnectionPool(provider, connectionString);

            //初始化数据库连接对象池
            for (int i = 0; i < maxPoolSize; i++)
            {
                DbConnection conn = _provider.CreateConnection(_connectionString);
                DbConnectionExt connExt = new DbConnectionExt(conn, provider, connectionString, this);
                if (i < _minPoolSize)
                {
                    conn.Open();
                }
                _connectionPool.Connections.Enqueue(connExt);
            }

            #region 定时释放数据库连接
            _timer = new Timer(obj =>
            {
                try
                {
                    for (int i = 0; i < 10; i++)
                    {
                        if (_connectionPool.Connections.TryDequeue(out DbConnectionExt connExt))
                        {
                            //数据库连接过期重新创建
                            if (DateTime.Now.Subtract(connExt.UpdateTime).TotalSeconds > _timeout)
                            {
                                connExt.Conn.Close();
                                DbConnection conn = _provider.CreateConnection(_connectionString);
                                connExt = new DbConnectionExt(conn, _provider, _connectionString, this);
                            }

                            _connectionPool.Connections.Enqueue(connExt);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }, null, 1000, 1000);
            #endregion

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

            SpinWait spinWait = new SpinWait();
            DbConnectionExt connExt;
            while (!_connectionPool.Connections.TryDequeue(out connExt))
            {
                spinWait.SpinOnce();
            }
            if (connExt.Conn.State != ConnectionState.Open)
            {
                connExt.Conn.Open();
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

            SpinWait spinWait = new SpinWait();
            DbConnectionExt connExt;
            while (!_connectionPool.Connections.TryDequeue(out connExt))
            {
                spinWait.SpinOnce();
            }
            if (connExt.Conn.State != ConnectionState.Open)
            {
                await connExt.Conn.OpenAsync();
            }
            return connExt;
        }
        #endregion

        #region 回收数据库连接
        /// <summary>
        /// 回收数据库连接
        /// </summary>
        public void Release(DbConnectionExt connExt)
        {
            //数据库连接过期重新创建
            if (DateTime.Now.Subtract(connExt.UpdateTime).TotalSeconds > _timeout)
            {
                connExt.Conn.Close();
                DbConnection conn = _provider.CreateConnection(_connectionString);
                connExt = new DbConnectionExt(conn, _provider, _connectionString, this);
            }

            _connectionPool.Connections.Enqueue(connExt);
        }
        #endregion

    }
}
