using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace Dapper.Lite
{
    public partial class DbSession : IDbSession
    {
        #region QueryPage<T> 分页查询列表
        /// <summary>
        /// 分页查询列表
        /// </summary>
        public List<T> QueryPage<T>(string sql, string orderby, int pageSize, int currentPage) where T : new()
        {
            sql = _provider.CreatePageSql(sql, orderby, pageSize, currentPage);

            return QueryList<T>(sql);
        }
        #endregion

        #region QueryPageAsync<T> 分页查询列表
        /// <summary>
        /// 分页查询列表
        /// </summary>
        public async Task<List<T>> QueryPageAsync<T>(string sql, string orderby, int pageSize, int currentPage) where T : new()
        {
            sql = _provider.CreatePageSql(sql, orderby, pageSize, currentPage);

            return await QueryListAsync<T>(sql);
        }
        #endregion

        #region QueryPage<T> 分页查询列表(参数化查询)
        /// <summary>
        /// 分页查询列表
        /// </summary>
        public List<T> QueryPage<T>(string sql, string orderby, int pageSize, int currentPage, DbParameter[] cmdParms) where T : new()
        {
            sql = _provider.CreatePageSql(sql, orderby, pageSize, currentPage);

            return QueryList<T>(sql, cmdParms);
        }

        /// <summary>
        /// 分页查询列表
        /// </summary>
        public async Task<List<T>> QueryPageAsync<T>(string sql, string orderby, int pageSize, int currentPage, DbParameter[] cmdParms) where T : new()
        {
            sql = _provider.CreatePageSql(sql, orderby, pageSize, currentPage);

            return await QueryListAsync<T>(sql, cmdParms);
        }
        #endregion

        #region 分页查询列表(传SqlString)
        /// <summary>
        /// 分页查询列表
        /// </summary>
        public List<T> QueryPage<T>(ISqlString sql, string orderby, int pageSize, int currentPage) where T : new()
        {
            return QueryPage<T>(sql.SQL, orderby, pageSize, currentPage, sql.Params);
        }

        /// <summary>
        /// 分页查询列表
        /// </summary>
        public Task<List<T>> QueryPageAsync<T>(ISqlString sql, string orderby, int pageSize, int currentPage) where T : new()
        {
            return QueryPageAsync<T>(sql.SQL, orderby, pageSize, currentPage, sql.Params);
        }
        #endregion

    }
}
