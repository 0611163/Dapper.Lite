using Dapper.Lite;
using Models;
using System.Configuration;
using System.Reflection;
using System.Threading.Tasks;

namespace DAL
{
    public class DapperLiteFactory
    {
        #region 变量
        private static IDapperLite _db = new DapperLite(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString(), new MySQLProvider());

        public static IDapperLite Db => _db;
        #endregion

        #region 获取 IDbSession
        /// <summary>
        /// 获取 IDbSession
        /// </summary>
        /// <param name="splitTableMapping">分表映射</param>
        public static IDbSession GetSession(SplitTableMapping splitTableMapping = null)
        {
            return _db.GetSession(splitTableMapping);
        }
        #endregion

        #region 获取 IDbSession (异步)
        /// <summary>
        /// 获取 IDbSession (异步)
        /// </summary>
        /// <param name="splitTableMapping">分表映射</param>
        public static async Task<IDbSession> GetSessionAsync(SplitTableMapping splitTableMapping = null)
        {
            return await _db.GetSessionAsync(splitTableMapping);
        }
        #endregion

    }
}
