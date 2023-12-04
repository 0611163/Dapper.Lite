using System;
using System.Collections;
using System.Data;
using System.Data.Common;
using System.Drawing;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Dapper.Lite
{
    /// <summary>
    /// Dapper.Lite客户端
    /// DapperLite是线程安全的
    /// </summary>
    public class DapperLite<TFlag> : DapperLite, IDapperLite<TFlag>
    {
        #region 构造函数
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="connectionString">数据库连接字符串</param>
        /// <param name="provider">数据库提供者</param>
        public DapperLite(string connectionString, IProvider provider) : base(connectionString, provider) { }
        #endregion

        #region 获取 IDbSession

        /// <summary>
        /// 获取 IDbSession
        /// </summary>
        public new IDbSession<TFlag> GetSession(SplitTableMapping splitTableMapping = null)
        {
            return GetSession<TFlag>(splitTableMapping);
        }
        #endregion

        #region 获取 IDbSession (异步)
        /// <summary>
        /// 获取 IDbSession (异步)
        /// </summary>
        public new Task<IDbSession<TFlag>> GetSessionAsync(SplitTableMapping splitTableMapping = null)
        {
            return GetSessionAsync<TFlag>(splitTableMapping);
        }
        #endregion
    }

    /// <summary>
    /// Dapper.Lite客户端
    /// DapperLite是线程安全的
    /// </summary>
    public class DapperLite : IDapperLite
    {
        #region 变量
        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        private string _connectionString;

        /// <summary>
        /// 数据库提供者
        /// </summary>
        private IProvider _provider;
        #endregion

        #region 构造函数
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="connectionString">数据库连接字符串</param>
        /// <param name="provider">数据库提供者</param>
        public DapperLite(string connectionString, IProvider provider)
        {
            _connectionString = connectionString;
            _provider = provider;

            SqlMapper.AddTypeHandler(typeof(Guid), new GuidTypeHanlder());

            // 预热
            using (var conn = GetConnection())
            {
                conn.Open();
            }
        }
        #endregion

        #region 获取 IDbSession
        /// <summary>
        /// 获取 IDbSession
        /// </summary>
        public IDbSession GetSession(SplitTableMapping splitTableMapping = null)
        {
            DbSession dbSession = new DbSession(_connectionString, _provider, splitTableMapping);

            if (OnExecuting != null)
            {
                dbSession.OnExecuting = OnExecuting;
            }

            return dbSession;
        }

        /// <summary>
        /// 获取 IDbSession
        /// </summary>
        protected IDbSession<TFlag> GetSession<TFlag>(SplitTableMapping splitTableMapping = null)
        {
            DbSession<TFlag> dbSession = new DbSession<TFlag>(_connectionString, _provider, splitTableMapping);

            if (OnExecuting != null)
            {
                dbSession.OnExecuting = OnExecuting;
            }

            return dbSession;
        }
        #endregion

        #region 获取 IDbSession (异步)
        /// <summary>
        /// 获取 IDbSession (异步)
        /// </summary>
        public Task<IDbSession> GetSessionAsync(SplitTableMapping splitTableMapping = null)
        {
            DbSession dbSession = new DbSession(_connectionString, _provider, splitTableMapping);

            if (OnExecuting != null)
            {
                dbSession.OnExecuting = OnExecuting;
            }

            return Task.FromResult(dbSession as IDbSession);
        }

        /// <summary>
        /// 获取 IDbSession (异步)
        /// </summary>
        protected Task<IDbSession<TFlag>> GetSessionAsync<TFlag>(SplitTableMapping splitTableMapping = null)
        {
            DbSession<TFlag> dbSession = new DbSession<TFlag>(_connectionString, _provider, splitTableMapping);

            if (OnExecuting != null)
            {
                dbSession.OnExecuting = OnExecuting;
            }

            return Task.FromResult(dbSession as IDbSession<TFlag>);
        }
        #endregion

        #region 创建SqlString对象
        /// <summary>
        /// 创建SqlString对象
        /// </summary>
        public ISqlString Sql(string sql = null, params object[] args)
        {
            var session = GetSession();
            return session.Sql(sql, args);
        }

        /// <summary>
        /// 创建SqlString对象
        /// </summary>
        public ISqlString<T> Sql<T>(string sql = null, params object[] args)
        {
            var session = GetSession();
            return session.Sql<T>(sql, args);
        }
        #endregion

        #region 创建ISqlQueryable<T>
        /// <summary>
        /// 创建IQueryable
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        public ISqlQueryable<T> Queryable<T>()
        {
            var session = GetSession();
            return session.Queryable<T>();
        }
        #endregion

        #region 查询下一个ID
        /// <summary>
        /// 查询下一个ID
        /// </summary>
        /// <typeparam name="T">实体类</typeparam>
        public int QueryNextId<T>()
        {
            var session = GetSession();
            return session.QueryNextId<T>();
        }
        #endregion

        #region ForList
        /// <summary>
        /// 创建 in 或 not in SQL
        /// </summary>
        public SqlValue ForList(IList list)
        {
            var session = GetSession();
            return session.ForList(list);
        }
        #endregion

        #region 获取数据库连接
        /// <summary>
        /// 获取数据库连接
        /// 如果需要使用数据库事务，请使用IDbSession接口的同名方法
        /// </summary>
        public DbConnection GetConnection()
        {
            var session = GetSession();
            return session.GetConnection(null);
        }

        /// <summary>
        /// 获取数据库连接
        /// 如果需要使用数据库事务，请使用IDbSession接口的同名方法
        /// </summary>
        public Task<DbConnection> GetConnectionAsync()
        {
            var session = GetSession();
            return session.GetConnectionAsync(null);
        }
        #endregion

        #region 设置 数据库字段名与实体类属性名映射
        /// <summary>
        /// 设置 数据库字段名与实体类属性名映射
        /// </summary>
        public void SetTypeMap(Type type)
        {
            var session = GetSession();
            session.SetTypeMap(type);
        }

        /// <summary>
        /// 设置 数据库字段名与实体类属性名映射
        /// </summary>
        public void SetTypeMap<T>()
        {
            var session = GetSession();
            session.SetTypeMap<T>();
        }
        #endregion

        #region SQL打印
        /// <summary>
        /// SQL打印
        /// </summary>
        public Action<string, DbParameter[]> OnExecuting { get; set; }
        #endregion

    }
}
