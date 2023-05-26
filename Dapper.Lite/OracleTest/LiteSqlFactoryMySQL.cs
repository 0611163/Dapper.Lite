using Dapper.Lite;
using System.Configuration;
using System.Threading.Tasks;

namespace OracleTest
{
    public class LiteSqlFactoryMySQL
    {
        #region 变量
        private static IDapperLiteClient _liteSqlClient = new DapperLiteClient(ConfigurationManager.ConnectionStrings["MySQLConnection"].ToString(), DBType.MySQL, new MySQLProvider());
        #endregion

        #region 获取 IDbSession
        /// <summary>
        /// 获取 IDbSession
        /// </summary>
        public static IDbSession GetSession()
        {
            return _liteSqlClient.GetSession();
        }
        #endregion

        #region 获取 IDbSession (异步)
        /// <summary>
        /// 获取 IDbSession (异步)
        /// </summary>
        public static async Task<IDbSession> GetSessionAsync()
        {
            return await _liteSqlClient.GetSessionAsync();
        }
        #endregion

    }
}
