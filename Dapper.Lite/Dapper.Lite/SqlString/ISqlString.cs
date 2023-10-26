using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlTypes;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Dapper.Lite
{
    /// <summary>
    /// 参数化查询SQL字符串接口
    /// </summary>
    public interface ISqlString<T> : ISqlString
    {
        #region Where
        /// <summary>
        /// 追加参数化查询条件SQL
        /// </summary>
        /// <param name="expression">Lambda 表达式</param>
        ISqlString<T> Where(Expression<Func<T, object>> expression);
        #endregion

    }

    /// <summary>
    /// 参数化查询SQL字符串接口
    /// </summary>
    public interface ISqlString
    {
        /// <summary>
        /// 参数化查询的SQL
        /// </summary>
        string SQL { get; }

        /// <summary>
        /// 参数化查询的参数
        /// </summary>
        DbParameter[] Params { get; }

        /// <summary>
        /// 参数化查询的参数
        /// </summary>
        DynamicParameters DynamicParameters { get; }

        /// <summary>
        /// 追加参数化SQL
        /// </summary>
        /// <param name="sql">SQL</param>
        /// <param name="args">参数(支持多个参数或者把多个参数放在一个匿名对象中)</param>
        ISqlString Append(string sql, params object[] args);

        /// <summary>
        /// 追加参数化SQL
        /// </summary>
        /// <param name="condition">当condition等于true时追加SQL，等于false时不追加SQL</param>
        /// <param name="sql">SQL</param>
        /// <param name="args">参数</param>
        ISqlString AppendIf(bool condition, string sql, params object[] args);

        /// <summary>
        /// 返回SQL语句
        /// </summary>
        string ToSql();

        /// <summary>
        /// 创建 in 或 not in SQL
        /// </summary>
        SqlValue ForList(IList list);

        #region 增删改查接口
        /// <summary>
        /// 查询实体
        /// </summary>
        T Query<T>() where T : new();

        /// <summary>
        /// 查询实体
        /// </summary>
        Task<T> QueryAsync<T>() where T : new();

        /// <summary>
        /// 查询列表
        /// </summary>
        List<T> QueryList<T>() where T : new();

        /// <summary>
        /// 查询列表
        /// </summary>
        Task<List<T>> QueryListAsync<T>() where T : new();

        /// <summary>
        /// 分页查询
        /// </summary>
        List<T> QueryPage<T>(string orderby, int pageSize, int currentPage) where T : new();

        /// <summary>
        /// 分页查询
        /// </summary>
        Task<List<T>> QueryPageAsync<T>(string orderby, int pageSize, int currentPage) where T : new();

        /// <summary>
        /// 条件删除
        /// </summary>
        [Obsolete]
        int DeleteByCondition<T>();

        /// <summary>
        /// 条件删除
        /// </summary>
        [Obsolete]
        Task<int> DeleteByConditionAsync<T>();

        /// <summary>
        /// 条件删除
        /// </summary>
        [Obsolete]
        int DeleteByCondition(Type type);

        /// <summary>
        /// 条件删除
        /// </summary>
        [Obsolete]
        Task<int> DeleteByConditionAsync(Type type);

        /// <summary>
        /// 条件删除
        /// </summary>
        int Delete<T>();

        /// <summary>
        /// 条件删除
        /// </summary>
        Task<int> DeleteAsync<T>();

        /// <summary>
        /// 条件删除
        /// </summary>
        int Delete(Type type);

        /// <summary>
        /// 条件删除
        /// </summary>
        Task<int> DeleteAsync(Type type);

        /// <summary>
        /// 执行SQL语句，返回影响的记录数
        /// </summary>
        int Execute();

        /// <summary>
        /// 执行SQL语句，返回影响的记录数
        /// </summary>
        Task<int> ExecuteAsync();

        /// <summary>
        /// 是否存在
        /// </summary>
        bool Exists();

        /// <summary>
        /// 是否存在
        /// </summary>
        Task<bool> ExistsAsync();

        /// <summary>
        /// 查询单个值
        /// </summary>
        object QuerySingle();

        /// <summary>
        /// 查询单个值
        /// </summary>
        T QuerySingle<T>();

        /// <summary>
        /// 查询单个值
        /// </summary>
        Task<object> QuerySingleAsync();

        /// <summary>
        /// 查询单个值
        /// </summary>
        Task<T> QuerySingleAsync<T>();

        /// <summary>
        /// 给定一条查询SQL，返回其查询结果的数量
        /// </summary>
        long QueryCount();

        /// <summary>
        /// 给定一条查询SQL，返回其查询结果的数量
        /// </summary>
        Task<long> QueryCountAsync();

        /// <summary>
        /// 给定一条查询SQL，返回其查询结果的数量
        /// </summary>
        long QueryCount(int pageSize, out long pageCount);

        /// <summary>
        /// 给定一条查询SQL，返回其查询结果的数量
        /// </summary>
        Task<CountResult> QueryCountAsync(int pageSize);
        #endregion

    }
}
