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
    public interface ISqlQueryable<T>
    {
        #region 变量
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
        #endregion

        #region LeftJoin
        /// <summary>
        /// 追加 left join SQL
        /// </summary>
        ISqlQueryable<T> LeftJoin<U>(Expression<Func<T, U, object>> expression);
        #endregion

        #region InnerJoin
        /// <summary>
        /// 追加 inner join SQL
        /// </summary>
        ISqlQueryable<T> InnerJoin<U>(Expression<Func<T, U, object>> expression);
        #endregion

        #region Where
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
        #endregion

        #region OrderBy
        /// <summary>
        /// 追加 order by SQL
        /// </summary>
        ISqlQueryable<T> OrderBy(Expression<Func<T, object>> expression);
        #endregion

        #region OrderByDescending
        /// <summary>
        /// 追加 order by SQL
        /// </summary>
        ISqlQueryable<T> OrderByDescending(Expression<Func<T, object>> expression);
        #endregion

        #region 增删改查接口
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
        /// 返回第一行的值，不存在则返回null
        /// </summary>
        T First();

        /// <summary>
        /// 返回第一行的值，不存在则返回null
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
        #endregion

    }
}
