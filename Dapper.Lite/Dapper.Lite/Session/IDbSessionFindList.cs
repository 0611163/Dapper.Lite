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
        List<T> QueryList<T>(string sql);

        /// <summary>
        /// 查询列表
        /// </summary>
        Task<List<T>> QueryListAsync<T>(string sql);
        #endregion

        #region 查询列表(参数化查询)
        /// <summary>
        /// 查询列表
        /// </summary>
        List<T> QueryList<T>(string sql, DbParameter[] cmdParms);

        /// <summary>
        /// 查询列表
        /// </summary>
        Task<List<T>> QueryListAsync<T>(string sql, DbParameter[] cmdParms);
        #endregion

        #region 查询列表(传SqlString)
        /// <summary>
        /// 查询列表
        /// </summary>
        List<T> QueryList<T>(ISqlString sql);

        /// <summary>
        /// 查询列表
        /// </summary>
        Task<List<T>> QueryListAsync<T>(ISqlString sql);
        #endregion

    }
}
