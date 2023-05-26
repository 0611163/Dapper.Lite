using Dapper.Lite;
using Npgsql;
using System.Data.Common;

namespace PostgreSQLTest
{
    public class PostgreSQLProvider : PostgreSQLProviderBase, IDbProvider
    {
        #region 创建 DbConnection
        public override DbConnection CreateConnection(string connectionString)
        {
            return new NpgsqlConnection(connectionString);
        }
        #endregion

        #region 生成 DbParameter
        public override DbParameter GetDbParameter(string name, object value)
        {
            return new NpgsqlParameter(name, value);
        }
        #endregion

    }
}
