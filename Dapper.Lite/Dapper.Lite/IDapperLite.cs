using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Dapper.Lite
{
    /// <summary>
    /// Dapper.Lite客户端接口
    /// IDapperLite是线程安全的
    /// 请定义成单例模式
    /// </summary>
    public interface IDapperLite<TFlag> : IDapperLite
    {
        #region 获取 IDbSession
        /// <summary>
        /// 获取 IDbSession
        /// </summary>
        new IDbSession<TFlag> GetSession(SplitTableMapping splitTableMapping = null);
        #endregion

        #region 获取 IDbSession (异步)
        /// <summary>
        /// 获取 IDbSession (异步)
        /// </summary>
        new Task<IDbSession<TFlag>> GetSessionAsync(SplitTableMapping splitTableMapping = null);
        #endregion
    }

    /// <summary>
    /// Dapper.Lite客户端接口
    /// IDapperLite是线程安全的
    /// 请定义成单例模式
    /// </summary>
    public interface IDapperLite
    {
        #region 获取 IDbSession
        /// <summary>
        /// 获取 IDbSession
        /// </summary>
        /// <param name="splitTableMapping">分表映射</param>
        IDbSession GetSession(SplitTableMapping splitTableMapping = null);
        #endregion

        #region 获取 IDbSession (异步)
        /// <summary>
        /// 获取 IDbSession (异步)
        /// </summary>
        /// <param name="splitTableMapping">分表映射</param>
        Task<IDbSession> GetSessionAsync(SplitTableMapping splitTableMapping = null);
        #endregion

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
        DbConnection GetConnection();

        /// <summary>
        /// 获取数据库连接
        /// </summary>
        Task<DbConnection> GetConnectionAsync();
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

        #region SQL打印
        /// <summary>
        /// SQL打印
        /// </summary>
        Action<string, DbParameter[]> OnExecuting { get; set; }
        #endregion

    }
}
