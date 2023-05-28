﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dapper.Lite
{
    /// <summary>
    /// 数据库连接扩展
    /// </summary>
    public class DbConnectionExt : IDisposable
    {
        /// <summary>
        /// 数据库提供者
        /// </summary>
        internal IProvider Provider { get; set; }

        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        internal string ConnectionString { get; set; }

        /// <summary>
        /// 数据库事务
        /// </summary>
        public DbTransactionExt Tran { get; set; }

        /// <summary>
        /// 数据库连接
        /// </summary>
        public DbConnection Conn { get; set; }

        /// <summary>
        /// 数据库连接工厂
        /// </summary>
        internal DbConnectionFactory ConnFactory { get; set; }

        /// <summary>
        /// 数据库连接扩展
        /// </summary>
        public DbConnectionExt(DbConnection conn, IProvider provider, string connectionString, DbConnectionFactory connFactory)
        {
            Conn = conn;
            Provider = provider;
            ConnectionString = connectionString;
            ConnFactory = connFactory;
        }

        /// <summary>
        /// 资源释放
        /// </summary>
        public void Dispose()
        {
            if (Tran == null)
            {
                ConnFactory.Release(this);
            }
        }
    }
}
