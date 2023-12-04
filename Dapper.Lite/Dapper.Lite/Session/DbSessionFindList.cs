using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace Dapper.Lite
{
    public partial class DbSession : IDbSession
    {
        #region QueryList<T> 查询列表
        /// <summary>
        /// 查询列表
        /// </summary>
        public List<T> QueryList<T>(string sql)
        {
            SqlFilter(ref sql);
            SetTypeMap<T>();
            OnExecuting?.Invoke(sql, null);

            var conn = GetConnection(_tran);

            try
            {
                return conn.Query<T>(sql, null, _tran).ToList();
            }
            finally
            {
                if (_tran == null)
                {
                    if (conn.State != ConnectionState.Closed) conn.Close();
                }
            }
        }
        #endregion

        #region QueryListAsync<T> 查询列表
        /// <summary>
        /// 查询列表
        /// </summary>
        public async Task<List<T>> QueryListAsync<T>(string sql)
        {
            SqlFilter(ref sql);
            SetTypeMap<T>();
            OnExecuting?.Invoke(sql, null);

            var conn = GetConnection(_tran);

            try
            {
                return (await conn.QueryAsync<T>(sql, null, _tran)).ToList();
            }
            finally
            {
                if (_tran == null)
                {
                    if (conn.State != ConnectionState.Closed) conn.Close();
                }
            }
        }
        #endregion


        #region QueryList<T> 查询列表(参数化查询)
        /// <summary>
        /// 查询列表
        /// </summary>
        public List<T> QueryList<T>(string sql, DbParameter[] cmdParms)
        {
            SetTypeMap<T>();
            OnExecuting?.Invoke(sql, cmdParms);

            var conn = GetConnection(_tran);

            try
            {
                return conn.Query<T>(sql, ToDynamicParameters(cmdParms), _tran).ToList();
            }
            finally
            {
                if (_tran == null)
                {
                    if (conn.State != ConnectionState.Closed) conn.Close();
                }
            }
        }
        #endregion

        #region QueryListAsync<T> 查询列表(参数化查询)
        /// <summary>
        /// 查询列表
        /// </summary>
        public async Task<List<T>> QueryListAsync<T>(string sql, DbParameter[] cmdParms)
        {
            SetTypeMap<T>();
            OnExecuting?.Invoke(sql, cmdParms);

            var conn = GetConnection(_tran);

            try
            {
                return (await conn.QueryAsync<T>(sql, ToDynamicParameters(cmdParms), _tran)).ToList();
            }
            finally
            {
                if (_tran == null)
                {
                    if (conn.State != ConnectionState.Closed) conn.Close();
                }
            }
        }
        #endregion

        #region 查询列表(传SqlString)
        /// <summary>
        /// 查询列表
        /// </summary>
        public List<T> QueryList<T>(ISqlString sql)
        {
            return QueryList<T>(sql.SQL, sql.Params);
        }

        /// <summary>
        /// 查询列表
        /// </summary>
        public Task<List<T>> QueryListAsync<T>(ISqlString sql)
        {
            return QueryListAsync<T>(sql.SQL, sql.Params);
        }
        #endregion

    }
}
