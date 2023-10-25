using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Dapper.Lite
{
    /// <summary>
    /// IDbSession接口实例表示与数据库的会话
    /// 一个IDbSession实例对应一个数据库连接，一个IDbSession实例只有一个数据库连接
    /// IDbSession不是线程安全的，不能跨线程使用
    /// </summary>
    public interface IDbSession<TFlag> : IDbSession { }

    /// <summary>
    /// IDbSession接口实例表示与数据库的会话
    /// 一个IDbSession实例对应一个数据库连接，一个IDbSession实例只有一个数据库连接
    /// IDbSession不是线程安全的，不能跨线程使用
    /// </summary>
    public partial interface IDbSession
    {
        #region 创建SqlString对象
        /// <summary>
        /// 创建SqlString对象
        /// </summary>
        ISqlString Sql(string sql = null, params object[] args);
        #endregion

        #region 创建SqlQueryable对象
        /// <summary>
        /// 创建IQueryable
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        ISqlQueryable<T> Queryable<T>() where T : new();
        #endregion

        #region 查询下一个ID
        /// <summary>
        /// 查询下一个ID
        /// </summary>
        /// <typeparam name="T">实体类</typeparam>
        int QueryNextId<T>();
        #endregion

        #region ForList
        /// <summary>
        /// 创建 in 或 not in SQL
        /// </summary>
        SqlValue ForList(IList list);
        #endregion

        #region 获取数据库连接
        /// <summary>
        /// 获取数据库连接
        /// </summary>
        DbConnection GetConnection(DbTransaction tran = null);

        /// <summary>
        /// 获取数据库连接
        /// </summary>
        Task<DbConnection> GetConnectionAsync(DbTransaction tran = null);

        /// <summary>
        /// 获取数据库连接，已经Open
        /// </summary>
        DbConnection GetOpenedConnection(DbTransaction tran = null);

        /// <summary>
        /// 获取数据库连接，已经Open
        /// </summary>
        Task<DbConnection> GetOpenedConnectionAsync(DbTransaction tran = null);
        #endregion

        #region 设置 数据库字段名与实体类属性名映射
        /// <summary>
        /// 设置 数据库字段名与实体类属性名映射
        /// </summary>
        void SetTypeMap(Type type);

        /// <summary>
        /// 设置 数据库字段名与实体类属性名映射
        /// </summary>
        void SetTypeMap<T>();
        #endregion

    }
}
