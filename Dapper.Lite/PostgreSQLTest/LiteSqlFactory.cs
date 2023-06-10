using Dapper.Lite;
using Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PostgreSQLTest
{
    public class LiteSqlFactory
    {
        #region 变量
        private static IDapperLiteClient _liteSqlClient = new DapperLiteClient(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString(), new PostgreSQLProvider());
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
