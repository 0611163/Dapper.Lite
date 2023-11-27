﻿using Dapper.Lite;
using MySqlConnector;
using System.Data.Common;

namespace Provider
{
    public class MySQLProvider : MySQLProviderBase, IDbProvider
    {
        #region 创建 DbConnection
        public override DbConnection CreateConnection(string connectionString)
        {
            return new MySqlConnection(connectionString);
        }
        #endregion

        #region 生成 DbParameter
        public override DbParameter GetDbParameter(string name, object value)
        {
            return new MySqlParameter(name, value);
        }
        #endregion

    }
}
