using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Dapper;

namespace Dapper.Lite
{
    public partial class DbSession : IDbSession
    {
        #region SQL打印
        /// <summary>
        /// SQL打印
        /// </summary>
        public Action<string, DbParameter[]> OnExecuting { get; set; }
        #endregion

        #region  执行简单SQL语句

        #region Exists 是否存在
        /// <summary>
        /// 是否存在
        /// </summary>
        public bool Exists(string sqlString)
        {
            SqlFilter(ref sqlString);
            OnExecuting?.Invoke(sqlString, null);

            object obj = ExecuteScalar(sqlString);

            if (object.Equals(obj, null) || object.Equals(obj, DBNull.Value))
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        #endregion

        #region ExistsAsync 是否存在
        /// <summary>
        /// 是否存在
        /// </summary>
        public async Task<bool> ExistsAsync(string sqlString)
        {
            SqlFilter(ref sqlString);
            OnExecuting?.Invoke(sqlString, null);

            object obj = await ExecuteScalarAsync(sqlString);

            if (object.Equals(obj, null) || object.Equals(obj, DBNull.Value))
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        #endregion


        #region QuerySingle<T> 查询单个值
        /// <summary>
        /// 查询单个值
        /// </summary>
        /// <param name="sqlString">查询语句</param>
        public T QuerySingle<T>(string sqlString)
        {
            SqlFilter(ref sqlString);
            OnExecuting?.Invoke(sqlString, null);

            object obj = ExecuteScalar(sqlString);

            if (object.Equals(obj, null) || object.Equals(obj, DBNull.Value))
            {
                return default(T);
            }
            else
            {
                return (T)Convert.ChangeType(obj, typeof(T));
            }
        }
        #endregion

        #region QuerySingle 查询单个值
        /// <summary>
        /// 查询单个值
        /// </summary>
        /// <param name="sqlString">查询语句</param>
        public object QuerySingle(string sqlString)
        {
            SqlFilter(ref sqlString);
            OnExecuting?.Invoke(sqlString, null);

            object obj = ExecuteScalar(sqlString);

            if (object.Equals(obj, null) || object.Equals(obj, DBNull.Value))
            {
                return null;
            }
            else
            {
                return obj;
            }
        }
        #endregion

        #region QuerySingleAsync<T> 查询单个值
        /// <summary>
        /// 查询单个值
        /// </summary>
        /// <param name="sqlString">查询语句</param>
        public async Task<T> QuerySingleAsync<T>(string sqlString)
        {
            SqlFilter(ref sqlString);
            OnExecuting?.Invoke(sqlString, null);

            object obj = await ExecuteScalarAsync(sqlString);

            if (object.Equals(obj, null) || object.Equals(obj, DBNull.Value))
            {
                return default(T);
            }
            else
            {
                return (T)Convert.ChangeType(obj, typeof(T));
            }
        }
        #endregion

        #region QuerySingleAsync 查询单个值
        /// <summary>
        /// 查询单个值
        /// </summary>
        /// <param name="sqlString">查询语句</param>
        public async Task<object> QuerySingleAsync(string sqlString)
        {
            SqlFilter(ref sqlString);
            OnExecuting?.Invoke(sqlString, null);

            object obj = await ExecuteScalarAsync(sqlString);

            if (object.Equals(obj, null) || object.Equals(obj, DBNull.Value))
            {
                return null;
            }
            else
            {
                return obj;
            }
        }
        #endregion


        #region QueryCount 查询数量
        /// <summary>
        /// 给定一条查询SQL，返回其查询结果的数量
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="pageSize">每页数据条数</param>
        /// <param name="pageCount">总页数</param>
        /// <returns>查询结果的数量</returns>
        public long QueryCount(string sql, int pageSize, out long pageCount)
        {
            if (pageSize <= 0) throw new Exception("pageSize不能小于或等于0");
            long count = QueryCount(sql);
            pageCount = (count - 1) / pageSize + 1;
            return count;
        }

        /// <summary>
        /// 给定一条查询SQL，返回其查询结果的数量
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="pageSize">每页数据条数</param>
        /// <returns>查询结果的数量</returns>
        public async Task<CountResult> QueryCountAsync(string sql, int pageSize)
        {
            if (pageSize <= 0) throw new Exception("pageSize不能小于或等于0");
            long count = await QueryCountAsync(sql);
            long pageCount = (count - 1) / pageSize + 1;
            return new CountResult(count, pageCount);
        }

        /// <summary>
        /// 给定一条查询SQL，返回其查询结果的数量
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <returns>查询结果的数量</returns>
        public long QueryCount(string sql)
        {
            sql = string.Format("select count(*) from ({0}) T", sql);
            return QuerySingle<long>(sql);
        }

        /// <summary>
        /// 给定一条查询SQL，返回其查询结果的数量
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <returns>查询结果的数量</returns>
        public Task<long> QueryCountAsync(string sql)
        {
            sql = string.Format("select count(*) from ({0}) T", sql);
            return QuerySingleAsync<long>(sql);
        }
        #endregion

        #endregion

        #region 执行带参SQL语句

        #region Exists 是否存在
        /// <summary>
        /// 是否存在
        /// </summary>
        public bool Exists(string sqlString, DbParameter[] cmdParms)
        {
            SqlFilter(ref sqlString);
            OnExecuting?.Invoke(sqlString, cmdParms);

            object obj = ExecuteScalar(sqlString, cmdParms);

            if (object.Equals(obj, null) || object.Equals(obj, DBNull.Value))
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        #endregion

        #region ExistsAsync 是否存在
        /// <summary>
        /// 是否存在
        /// </summary>
        public async Task<bool> ExistsAsync(string sqlString, DbParameter[] cmdParms)
        {
            SqlFilter(ref sqlString);
            OnExecuting?.Invoke(sqlString, cmdParms);

            object obj = ExecuteScalarAsync(sqlString, cmdParms);

            if (object.Equals(obj, null) || object.Equals(obj, DBNull.Value))
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        #endregion


        #region QuerySingle<T> 查询单个值
        /// <summary>
        /// 查询单个值
        /// </summary>
        /// <param name="sqlString">查询语句</param>
        /// <param name="cmdParms">参数</param>
        public T QuerySingle<T>(string sqlString, DbParameter[] cmdParms)
        {
            SqlFilter(ref sqlString);
            OnExecuting?.Invoke(sqlString, cmdParms);

            object obj = ExecuteScalar(sqlString, cmdParms);

            if (object.Equals(obj, null) || object.Equals(obj, DBNull.Value))
            {
                return default(T);
            }
            else
            {
                return (T)Convert.ChangeType(obj, typeof(T));
            }
        }
        #endregion

        #region QuerySingle 查询单个值
        /// <summary>
        /// 查询单个值
        /// </summary>
        /// <param name="sqlString">查询语句</param>
        /// <param name="cmdParms">参数</param>
        public object QuerySingle(string sqlString, DbParameter[] cmdParms)
        {
            SqlFilter(ref sqlString);
            OnExecuting?.Invoke(sqlString, cmdParms);

            object obj = ExecuteScalar(sqlString, cmdParms);

            if (object.Equals(obj, null) || object.Equals(obj, DBNull.Value))
            {
                return null;
            }
            else
            {
                return obj;
            }
        }
        #endregion

        #region QuerySingleAsync<T> 查询单个值
        /// <summary>
        /// 查询单个值
        /// </summary>
        /// <param name="sqlString">查询语句</param>
        /// <param name="cmdParms">参数</param>
        public async Task<T> QuerySingleAsync<T>(string sqlString, DbParameter[] cmdParms)
        {
            SqlFilter(ref sqlString);
            OnExecuting?.Invoke(sqlString, cmdParms);

            object obj = await ExecuteScalarAsync(sqlString, cmdParms);

            if (object.Equals(obj, null) || object.Equals(obj, DBNull.Value))
            {
                return default(T);
            }
            else
            {
                return (T)Convert.ChangeType(obj, typeof(T));
            }
        }
        #endregion

        #region QuerySingleAsync 查询单个值
        /// <summary>
        /// 查询单个值
        /// </summary>
        /// <param name="sqlString">查询语句</param>
        /// <param name="cmdParms">参数</param>
        public async Task<object> QuerySingleAsync(string sqlString, DbParameter[] cmdParms)
        {
            SqlFilter(ref sqlString);
            OnExecuting?.Invoke(sqlString, cmdParms);

            object obj = await ExecuteScalarAsync(sqlString, cmdParms);

            if (object.Equals(obj, null) || object.Equals(obj, DBNull.Value))
            {
                return null;
            }
            else
            {
                return obj;
            }
        }
        #endregion


        #region QueryCount 查询数量
        /// <summary>
        /// 给定一条查询SQL，返回其查询结果的数量
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="cmdParms">参数</param>
        /// <param name="pageSize">每页数据条数</param>
        /// <param name="pageCount">总页数</param>
        /// <returns>查询结果的数量</returns>
        public long QueryCount(string sql, DbParameter[] cmdParms, int pageSize, out long pageCount)
        {
            if (pageSize <= 0) throw new Exception("pageSize不能小于或等于0");
            long count = QueryCount(sql, cmdParms);
            pageCount = (count - 1) / pageSize + 1;
            return count;
        }

        /// <summary>
        /// 给定一条查询SQL，返回其查询结果的数量
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="cmdParms">参数</param>
        /// <param name="pageSize">每页数据条数</param>
        /// <returns>查询结果的数量</returns>
        public async Task<CountResult> QueryCountAsync(string sql, DbParameter[] cmdParms, int pageSize)
        {
            if (pageSize <= 0) throw new Exception("pageSize不能小于或等于0");
            long count = await QueryCountAsync(sql, cmdParms);
            long pageCount = (count - 1) / pageSize + 1;
            return new CountResult(count, pageCount);
        }

        /// <summary>
        /// 给定一条查询SQL，返回其查询结果的数量
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="cmdParms">参数</param>
        /// <returns>查询结果的数量</returns>
        public long QueryCount(string sql, DbParameter[] cmdParms)
        {
            sql = string.Format("select count(*) from ({0}) T", sql);

            return QuerySingle<long>(sql, cmdParms);
        }

        /// <summary>
        /// 给定一条查询SQL，返回其查询结果的数量
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="cmdParms">参数</param>
        /// <returns>查询结果的数量</returns>
        public async Task<long> QueryCountAsync(string sql, DbParameter[] cmdParms)
        {
            sql = string.Format("select count(*) from ({0}) T", sql);

            return await QuerySingleAsync<long>(sql, cmdParms);
        }
        #endregion

        #endregion

        #region 传SqlString

        /// <summary>
        /// 执行SQL语句，返回影响的记录数
        /// </summary>
        /// <param name="sql">SqlString</param>
        /// <returns>影响的记录数</returns>
        public int Execute(ISqlString sql)
        {
            return Execute(sql.SQL, sql.Params);
        }

        /// <summary>
        /// 执行SQL语句，返回影响的记录数
        /// </summary>
        /// <param name="sql">SqlString</param>
        /// <returns>影响的记录数</returns>
        public async Task<int> ExecuteAsync(ISqlString sql)
        {
            return await ExecuteAsync(sql.SQL, sql.Params);
        }

        /// <summary>
        /// 是否存在
        /// </summary>
        public bool Exists(ISqlString sql)
        {
            return Exists(sql.SQL, sql.Params);
        }

        /// <summary>
        /// 是否存在
        /// </summary>
        public Task<bool> ExistsAsync(ISqlString sql)
        {
            return ExistsAsync(sql.SQL, sql.Params);
        }

        /// <summary>
        /// 查询单个值
        /// </summary>
        public object QuerySingle(ISqlString sql)
        {
            return QuerySingle(sql.SQL, sql.Params);
        }

        /// <summary>
        /// 查询单个值
        /// </summary>
        public T QuerySingle<T>(ISqlString sql)
        {
            return QuerySingle<T>(sql.SQL, sql.Params);
        }

        /// <summary>
        /// 查询单个值
        /// </summary>
        public Task<object> QuerySingleAsync(ISqlString sql)
        {
            return QuerySingleAsync(sql.SQL, sql.Params);
        }

        /// <summary>
        /// 查询单个值
        /// </summary>
        public Task<T> QuerySingleAsync<T>(ISqlString sql)
        {
            return QuerySingleAsync<T>(sql.SQL, sql.Params);
        }

        /// <summary>
        /// 给定一条查询SQL，返回其查询结果的数量
        /// </summary>
        /// <param name="sql">SqlString</param>
        /// <returns>数量</returns>
        public long QueryCount(ISqlString sql)
        {
            return QueryCount(sql.SQL, sql.Params);
        }

        /// <summary>
        /// 给定一条查询SQL，返回其查询结果的数量
        /// </summary>
        /// <param name="sql">SqlString</param>
        /// <returns>数量</returns>
        public Task<long> QueryCountAsync(ISqlString sql)
        {
            return QueryCountAsync(sql.SQL, sql.Params);
        }

        /// <summary>
        /// 给定一条查询SQL，返回其查询结果的数量
        /// </summary>
        /// <param name="sql">SqlString</param>
        /// <param name="pageSize">每页数据条数</param>
        /// <param name="pageCount">总页数</param>
        /// <returns>查询结果的数量</returns>
        public long QueryCount(ISqlString sql, int pageSize, out long pageCount)
        {
            return QueryCount(sql.SQL, sql.Params, pageSize, out pageCount);
        }

        /// <summary>
        /// 给定一条查询SQL，返回其查询结果的数量
        /// </summary>
        /// <param name="sql">SqlString</param>
        /// <param name="pageSize">每页数据条数</param>
        /// <returns>查询结果的数量</returns>
        public Task<CountResult> QueryCountAsync(ISqlString sql, int pageSize)
        {
            return QueryCountAsync(sql.SQL, sql.Params, pageSize);
        }

        #endregion

        #region ExecuteScalar
        internal object ExecuteScalar(string sqlString, DbParameter[] cmdParms = null)
        {
            var conn = GetConnection(_tran);

            try
            {
                return conn.ExecuteScalar(sqlString, ToDynamicParameters(cmdParms), _tran);
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

        #region ExecuteScalarAsync
        internal async Task<object> ExecuteScalarAsync(string sqlString, DbParameter[] cmdParms = null)
        {
            var conn = GetConnection(_tran);

            try
            {
                return await conn.ExecuteScalarAsync(sqlString, ToDynamicParameters(cmdParms), _tran);
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

        #region Execute
        internal int Execute(string sqlString, DbParameter[] cmdParms = null)
        {
            var conn = GetConnection(_tran);

            try
            {
                return conn.Execute(sqlString, ToDynamicParameters(cmdParms), _tran);
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

        #region ExecuteAsync
        internal async Task<int> ExecuteAsync(string sqlString, DbParameter[] cmdParms = null)
        {
            var conn = GetConnection(_tran);

            try
            {
                return await conn.ExecuteAsync(sqlString, ToDynamicParameters(cmdParms), _tran);
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

    }
}
