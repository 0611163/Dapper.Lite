using Dapper.Lite;
using Oracle.ManagedDataAccess.Client;
using System.Data.Common;

namespace OracleTest
{
    public class OracleProvider : OracleProviderBase, IDbProvider
    {
        #region 创建 DbConnection
        public override DbConnection CreateConnection(string connectionString)
        {
            return new OracleConnection(connectionString);
        }
        #endregion

        #region 生成 DbParameter
        public override DbParameter GetDbParameter(string name, object value)
        {
            return new OracleParameter(name, value);
        }
        #endregion

    }
}
