﻿using Dapper.Lite;
using System.Configuration;
using System.Threading.Tasks;

namespace OracleTest
{
    public class LiteSqlFactory
    {
        #region 变量
        private static IDapperLite _liteSqlClient = new DapperLite(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString(), new OracleProvider());
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
