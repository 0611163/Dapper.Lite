using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dapper.Lite
{
    public partial interface IDbSession
    {
        #region 分页查询列表
        /// <summary>
        /// 分页查询列表
        /// </summary>
        List<T> QueryPage<T>(string sql, string orderby, int pageSize, int currentPage) where T : new();

        /// <summary>
        /// 分页查询列表
        /// </summary>
        Task<List<T>> QueryPageAsync<T>(string sql, string orderby, int pageSize, int currentPage) where T : new();
        #endregion

        #region 分页查询列表(参数化查询)
        /// <summary>
        /// 分页查询列表
        /// </summary>
        List<T> QueryPage<T>(string sql, string orderby, int pageSize, int currentPage, DbParameter[] cmdParms) where T : new();

        /// <summary>
        /// 分页查询列表
        /// </summary>
        Task<List<T>> QueryPageAsync<T>(string sql, string orderby, int pageSize, int currentPage, DbParameter[] cmdParms) where T : new();
        #endregion

        #region 分页查询列表(传SqlString)
        /// <summary>
        /// 分页查询列表
        /// </summary>
        List<T> QueryPage<T>(ISqlString sql, string orderby, int pageSize, int currentPage) where T : new();

        /// <summary>
        /// 分页查询列表
        /// </summary>
        Task<List<T>> QueryPageAsync<T>(ISqlString sql, string orderby, int pageSize, int currentPage) where T : new();
        #endregion

    }
}
