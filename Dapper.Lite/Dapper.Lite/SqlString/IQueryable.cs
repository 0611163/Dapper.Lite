using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Dapper.Lite
{
    /// <summary>
    /// 查询接口
    /// </summary>
    public interface ISqlQueryable<T> where T : new()
    {
        /// <summary>
        /// 追加参数化SQL
        /// </summary>
        /// <param name="condition">当condition等于true时追加SQL，等于false时不追加SQL</param>
        /// <param name="expression">Lambda 表达式</param>
        ISqlQueryable<T> WhereIf(bool condition, Expression<Func<T, object>> expression);

        /// <summary>
        /// 追加参数化SQL
        /// </summary>
        /// <param name="condition">当condition等于true时追加SQL，等于false时不追加SQL</param>
        /// <param name="expression">Lambda 表达式</param>
        ISqlQueryable<T> WhereIf<U>(bool condition, Expression<Func<U, object>> expression);

        /// <summary>
        /// 追加参数化SQL
        /// </summary>
        /// <param name="expression">Lambda 表达式</param>
        ISqlQueryable<T> Where(Expression<Func<T, object>> expression);

        /// <summary>
        /// 追加参数化SQL
        /// </summary>
        /// <param name="expression">Lambda 表达式</param>
        ISqlQueryable<T> Where<U>(Expression<Func<U, object>> expression);

        /// <summary>
        /// 追加参数化SQL
        /// </summary>
        /// <param name="expression">Lambda 表达式</param>
        ISqlQueryable<T> Where<U>(Expression<Func<T, U, object>> expression);

        /// <summary>
        /// 追加参数化SQL
        /// </summary>
        /// <param name="expression">Lambda 表达式</param>
        ISqlQueryable<T> Where<U, D>(Expression<Func<T, U, D, object>> expression);

        /// <summary>
        /// 追加 order by SQL
        /// </summary>
        ISqlQueryable<T> OrderBy(Expression<Func<T, object>> expression);

        /// <summary>
        /// 追加 order by SQL
        /// </summary>
        ISqlQueryable<T> OrderByDescending(Expression<Func<T, object>> expression);

        /// <summary>
        /// 追加 left join SQL
        /// </summary>
        ISqlQueryable<T> LeftJoin<U>(Expression<Func<T, U, object>> expression);

        /// <summary>
        /// 追加 inner join SQL
        /// </summary>
        ISqlQueryable<T> InnerJoin<U>(Expression<Func<T, U, object>> expression);

        /// <summary>
        /// 追加 right join SQL
        /// </summary>
        ISqlQueryable<T> RightJoin<U>(Expression<Func<T, U, object>> expression);

        /// <summary>
        /// Where 连表
        /// </summary>
        ISqlQueryable<T> WhereJoin<U>(Expression<Func<T, U, object>> expression);

        /// <summary>
        /// 追加 select SQL
        /// </summary>
        /// <param name="subSql">子SQL</param>
        /// <param name="alias">表别名，默认值t</param>
        ISqlQueryable<T> Select(ISqlQueryable<T> subSql, string alias = null);

        /// <summary>
        /// 追加 select SQL
        /// </summary>
        /// <param name="sql">SQL，插入到子SQL的前面，或者插入到{0}的位置</param>
        /// <param name="subSql">子SQL</param>
        /// <param name="alias">表别名，默认值t</param>
        ISqlQueryable<T> Select(string sql, ISqlQueryable<T> subSql = null, string alias = null);

        /// <summary>
        /// 追加 select SQL
        /// </summary>
        /// <param name="subSql">子SQL</param>
        /// <param name="alias">表别名，默认值t</param>
        ISqlQueryable<T> Select<U>(ISqlQueryable<U> subSql, string alias = null) where U : new();

        /// <summary>
        /// 追加 select SQL
        /// </summary>
        /// <param name="sql">SQL，插入到子SQL的前面，或者插入到{0}的位置</param>
        /// <param name="subSql">子SQL</param>
        /// <param name="alias">表别名，默认值t</param>
        ISqlQueryable<T> Select<U>(string sql, ISqlQueryable<U> subSql = null, string alias = null) where U : new();

        /// <summary>
        /// 追加 select SQL
        /// </summary>
        /// <param name="expression">返回匿名对象的表达式</param>
        ISqlQueryable<T> Select(Expression<Func<T, object>> expression);

        /// <summary>
        /// 追加 select SQL
        /// </summary>
        ISqlQueryable<T> Select<U>(Expression<Func<U, object>> expression, Expression<Func<T, object>> expression2);

        /// <summary>
        /// 执行查询
        /// </summary>
        List<T> ToList();

        /// <summary>
        /// 执行查询
        /// </summary>
        Task<List<T>> ToListAsync();

        /// <summary>
        /// 执行查询
        /// </summary>
        List<T> ToPageList(int page, int pageSize);

        /// <summary>
        /// 执行查询
        /// </summary>
        Task<List<T>> ToPageListAsync(int page, int pageSize);

        /// <summary>
        /// 返回数量
        /// </summary>
        long Count();

        /// <summary>
        /// 返回数量
        /// </summary>
        Task<long> CountAsync();

        /// <summary>
        /// 返回数量
        /// </summary>
        T First();

        /// <summary>
        /// 返回数量
        /// </summary>
        Task<T> FirstAsync();

        /// <summary>
        /// 是否存在
        /// </summary>
        bool Exists();

        /// <summary>
        /// 返回数量
        /// </summary>
        Task<bool> ExistsAsync();

        /// <summary>
        /// 删除
        /// </summary>
        int Delete();

        /// <summary>
        /// 删除
        /// </summary>
        Task<int> DeleteAsync();

        #region 部分ISqlString接口
        /// <summary>
        /// 转换为ISqlString
        /// </summary>
        ISqlString AsSqlString();

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
        ISqlQueryable<T> Append(string sql, params object[] args);

        /// <summary>
        /// 追加参数化SQL
        /// </summary>
        ///  <param name="condition">当condition等于true时追加SQL，等于false时不追加SQL</param>
        /// <param name="sql">SQL</param>
        /// <param name="args">参数(支持多个参数或者把多个参数放在一个匿名对象中)</param>
        ISqlQueryable<T> AppendIf(bool condition, string sql, params object[] args);

        /// <summary>
        /// 追加参数化SQL
        /// </summary>
        /// <param name="condition">当condition等于true时追加SQL，等于false时不追加SQL</param>
        /// <param name="sql">SQL</param>
        /// <param name="argsFunc">参数</param>
        ISqlQueryable<T> AppendIf(bool condition, string sql, params Func<object>[] argsFunc);

        /// <summary>
        /// 追加参数化SQL
        /// </summary>
        /// <param name="sql">SQL，插入到子SQL的前面，或者插入到{0}的位置</param>
        /// <param name="subSql">子SQL</param>
        ISqlQueryable<T> AppendSubSql(string sql, ISqlString subSql);

        /// <summary>
        /// 追加参数化SQL
        /// </summary>
        /// <param name="sql">SQL，插入到子SQL的前面，或者插入到{0}的位置</param>
        /// <param name="subSql">子SQL</param>
        ISqlQueryable<T> AppendSubSql(string sql, ISqlQueryable<T> subSql);

        /// <summary>
        /// 追加参数化SQL
        /// </summary>
        /// <param name="sql">SQL</param>
        /// <param name="args">参数(支持多个参数或者把多个参数放在一个匿名对象中)</param>
        ISqlQueryable<T> LeftJoin(string sql, params object[] args);

        /// <summary>
        /// 追加参数化SQL
        /// </summary>
        /// <param name="sql">SQL</param>
        /// <param name="args">参数(支持多个参数或者把多个参数放在一个匿名对象中)</param>
        ISqlQueryable<T> InnerJoin(string sql, params object[] args);

        /// <summary>
        /// 追加参数化SQL
        /// </summary>
        /// <param name="sql">SQL</param>
        /// <param name="args">参数(支持多个参数或者把多个参数放在一个匿名对象中)</param>
        ISqlQueryable<T> RightJoin(string sql, params object[] args);

        /// <summary>
        /// 追加参数化SQL
        /// </summary>
        /// <param name="sql">SQL</param>
        /// <param name="args">参数(支持多个参数或者把多个参数放在一个匿名对象中)</param>
        ISqlQueryable<T> Where(string sql, params object[] args);

        /// <summary>
        /// 追加参数化SQL
        /// </summary>
        /// <param name="condition">当condition等于true时追加SQL，等于false时不追加SQL</param>
        /// <param name="sql">SQL</param>
        /// <param name="args">参数(支持多个参数或者把多个参数放在一个匿名对象中)</param>
        ISqlQueryable<T> WhereIf(bool condition, string sql, params object[] args);

        /// <summary>
        /// 追加参数化SQL
        /// </summary>
        /// <param name="sql">SQL</param>
        /// <param name="args">参数(支持多个参数或者把多个参数放在一个匿名对象中)</param>
        ISqlQueryable<T> Having(string sql, params object[] args);

        /// <summary>
        /// 追加参数化SQL
        /// </summary>
        /// <param name="sql">SQL</param>
        /// <param name="args">参数(支持多个参数或者把多个参数放在一个匿名对象中)</param>
        ISqlQueryable<T> GroupBy(string sql, params object[] args);

        /// <summary>
        /// 追加参数化SQL
        /// </summary>
        /// <param name="sql">SQL</param>
        /// <param name="args">参数(支持多个参数或者把多个参数放在一个匿名对象中)</param>
        ISqlQueryable<T> OrderBy(string sql, params object[] args);

        /// <summary>
        /// 封装 StringBuilder AppendFormat 追加非参数化SQL
        /// </summary>
        /// <param name="sql">SQL</param>
        /// <param name="args">参数</param>
        ISqlQueryable<T> AppendFormat(string sql, params object[] args);

        /// <summary>
        /// 返回SQL语句
        /// </summary>
        string ToSql();

        /// <summary>
        /// 创建 in 或 not in SQL
        /// </summary>
        SqlValue ForList(IList list);
        #endregion

    }
}
