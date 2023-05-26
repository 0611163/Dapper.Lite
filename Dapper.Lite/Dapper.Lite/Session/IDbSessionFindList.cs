using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dapper.Lite
{
    public partial interface IDbSession
    {
        #region 查询列表
        /// <summary>
        /// 查询列表
        /// </summary>
        List<T> QueryList<T>(string sql) where T : new();

        /// <summary>
        /// 查询列表
        /// </summary>
        Task<List<T>> QueryListAsync<T>(string sql) where T : new();
        #endregion

        #region 查询列表(参数化查询)
        /// <summary>
        /// 查询列表
        /// </summary>
        List<T> QueryList<T>(string sql, DbParameter[] cmdParms) where T : new();

        /// <summary>
        /// 查询列表
        /// </summary>
        Task<List<T>> QueryListAsync<T>(string sql, DbParameter[] cmdParms) where T : new();
        #endregion

        #region 查询列表(传SqlString)
        /// <summary>
        /// 查询列表
        /// </summary>
        List<T> QueryList<T>(ISqlString sql) where T : new();

        /// <summary>
        /// 查询列表
        /// </summary>
        Task<List<T>> QueryListAsync<T>(ISqlString sql) where T : new();
        #endregion

    }
}
