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
        #region SQL打印
        /// <summary>
        /// SQL打印
        /// </summary>
        Action<string, DbParameter[]> OnExecuting { get; set; }
        #endregion

        #region 执行简单SQL语句

        /// <summary>
        /// 是否存在
        /// </summary>
        bool Exists(string sqlString);

        /// <summary>
        /// 是否存在
        /// </summary>
        Task<bool> ExistsAsync(string sqlString);

        /// <summary>
        /// 查询单个值
        /// </summary>
        object QuerySingle(string sqlString);

        /// <summary>
        /// 查询单个值
        /// </summary>
        T QuerySingle<T>(string sqlString);

        /// <summary>
        /// 查询单个值
        /// </summary>
        Task<object> QuerySingleAsync(string sqlString);

        /// <summary>
        /// 查询单个值
        /// </summary>
        Task<T> QuerySingleAsync<T>(string sqlString);

        /// <summary>
        /// 给定一条查询SQL，返回其查询结果的数量
        /// </summary>
        /// <param name="sqlString">SQL语句</param>
        /// <returns>查询结果的数量</returns>
        long QueryCount(string sqlString);

        /// <summary>
        /// 给定一条查询SQL，返回其查询结果的数量
        /// </summary>
        /// <param name="sqlString">SQL语句</param>
        /// <returns>查询结果的数量</returns>
        Task<long> QueryCountAsync(string sqlString);

        /// <summary>
        /// 给定一条查询SQL，返回其查询结果的数量
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="pageSize">每页数据条数</param>
        /// <param name="pageCount">总页数</param>
        /// <returns>查询结果的数量</returns>
        long QueryCount(string sql, int pageSize, out long pageCount);

        /// <summary>
        /// 给定一条查询SQL，返回其查询结果的数量
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="pageSize">每页数据条数</param>
        /// <returns>查询结果的数量</returns>
        Task<CountResult> QueryCountAsync(string sql, int pageSize);

        #endregion

        #region 执行带参数的SQL语句

        /// <summary>
        /// 是否存在
        /// </summary>
        bool Exists(string sqlString, DbParameter[] cmdParms);

        /// <summary>
        /// 是否存在
        /// </summary>
        Task<bool> ExistsAsync(string sqlString, DbParameter[] cmdParms);

        /// <summary>
        /// 查询单个值
        /// </summary>
        object QuerySingle(string sqlString, DbParameter[] cmdParms);

        /// <summary>
        /// 查询单个值
        /// </summary>
        T QuerySingle<T>(string sqlString, DbParameter[] cmdParms);

        /// <summary>
        /// 查询单个值
        /// </summary>
        Task<object> QuerySingleAsync(string sqlString, DbParameter[] cmdParms);

        /// <summary>
        /// 查询单个值
        /// </summary>
        Task<T> QuerySingleAsync<T>(string sqlString, DbParameter[] cmdParms);

        /// <summary>
        /// 给定一条查询SQL，返回其查询结果的数量
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="cmdParms">参数</param>
        /// <returns>数量</returns>
        long QueryCount(string sql, DbParameter[] cmdParms);

        /// <summary>
        /// 给定一条查询SQL，返回其查询结果的数量
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="cmdParms">参数</param>
        /// <returns>数量</returns>
        Task<long> QueryCountAsync(string sql, DbParameter[] cmdParms);

        /// <summary>
        /// 给定一条查询SQL，返回其查询结果的数量
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="cmdParms">参数</param>
        /// <param name="pageSize">每页数据条数</param>
        /// <param name="pageCount">总页数</param>
        /// <returns>查询结果的数量</returns>
        long QueryCount(string sql, DbParameter[] cmdParms, int pageSize, out long pageCount);

        /// <summary>
        /// 给定一条查询SQL，返回其查询结果的数量
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="cmdParms">参数</param>
        /// <param name="pageSize">每页数据条数</param>
        /// <returns>查询结果的数量</returns>
        Task<CountResult> QueryCountAsync(string sql, DbParameter[] cmdParms, int pageSize);

        #endregion

        #region 查询并返回DataTable
        /// <summary>
        /// 执行查询语句，返回DbDataReader
        /// </summary>
        DataTable QueryDataTable(string sqlString);

        /// <summary>
        /// 执行查询语句，返回DbDataReader
        /// </summary>
        Task<DataTable> QueryDataTableAsync(string sqlString);

        /// <summary>
        /// 执行查询语句，返回DbDataReader
        /// </summary>
        /// <param name="sqlString">查询语句</param>
        ///  <param name="cmdParms">参数</param>
        /// <returns>IDataReader</returns>
        DataTable QueryDataTable(string sqlString, DbParameter[] cmdParms);

        /// <summary>
        /// 执行查询语句，返回DbDataReader
        /// </summary>
        /// <param name="sqlString">查询语句</param>
        ///  <param name="cmdParms">参数</param>
        /// <returns>IDataReader</returns>
        Task<DataTable> QueryDataTableAsync(string sqlString, DbParameter[] cmdParms);

        /// <summary>
        /// 分页查询，返回DataTable
        /// </summary>
        DataTable QueryPage(string sql, string orderby, int pageSize, int currentPage);

        /// <summary>
        /// 分页查询，返回DataTable
        /// </summary>
        Task<DataTable> QueryPageAsync(string sql, string orderby, int pageSize, int currentPage);

        /// <summary>
        /// 分页查询，返回DataTable
        /// </summary>
        DataTable QueryPage(string sql, string orderby, int pageSize, int currentPage, DbParameter[] cmdParms);

        /// <summary>
        /// 分页查询，返回DataTable
        /// </summary>
        Task<DataTable> QueryPageAsync(string sql, string orderby, int pageSize, int currentPage, DbParameter[] cmdParms);
        #endregion

        #region 传SqlString

        /// <summary>
        /// 执行SQL语句，返回影响的记录数
        /// </summary>
        /// <param name="sql">SqlString</param>
        /// <returns>影响的记录数</returns>
        int Execute(ISqlString sql);

        /// <summary>
        /// 执行SQL语句，返回影响的记录数
        /// </summary>
        /// <param name="sql">SqlString</param>
        /// <returns>影响的记录数</returns>
        Task<int> ExecuteAsync(ISqlString sql);

        /// <summary>
        /// 是否存在
        /// </summary>
        bool Exists(ISqlString sql);

        /// <summary>
        /// 是否存在
        /// </summary>
        Task<bool> ExistsAsync(ISqlString sql);

        /// <summary>
        /// 查询单个值
        /// </summary>
        object QuerySingle(ISqlString sql);

        /// <summary>
        /// 查询单个值
        /// </summary>
        T QuerySingle<T>(ISqlString sql);

        /// <summary>
        /// 查询单个值
        /// </summary>
        Task<object> QuerySingleAsync(ISqlString sql);

        /// <summary>
        /// 查询单个值
        /// </summary>
        Task<T> QuerySingleAsync<T>(ISqlString sql);

        /// <summary>
        /// 给定一条查询SQL，返回其查询结果的数量
        /// </summary>
        /// <param name="sql">SqlString</param>
        /// <returns>数量</returns>
        long QueryCount(ISqlString sql);

        /// <summary>
        /// 给定一条查询SQL，返回其查询结果的数量
        /// </summary>
        /// <param name="sql">SqlString</param>
        /// <returns>数量</returns>
        Task<long> QueryCountAsync(ISqlString sql);

        /// <summary>
        /// 给定一条查询SQL，返回其查询结果的数量
        /// </summary>
        /// <param name="sql">SqlString</param>
        /// <param name="pageSize">每页数据条数</param>
        /// <param name="pageCount">总页数</param>
        /// <returns>查询结果的数量</returns>
        long QueryCount(ISqlString sql, int pageSize, out long pageCount);

        /// <summary>
        /// 给定一条查询SQL，返回其查询结果的数量
        /// </summary>
        /// <param name="sql">SqlString</param>
        /// <param name="pageSize">每页数据条数</param>
        /// <returns>查询结果的数量</returns>
        Task<CountResult> QueryCountAsync(ISqlString sql, int pageSize);

        #endregion

    }
}
