using System;
using System.Collections;
using System.Data.Common;
using System.Drawing;
using System.Linq.Expressions;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;

namespace Dapper.Lite
{
    /// <summary>
    /// Dapper.Lite客户端
    /// DapperLiteClient是线程安全的
    /// </summary>
    public class DapperLiteClient : IDapperLiteClient
    {
        #region 变量
        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        private string _connectionString;

        /// <summary>
        /// 数据库类型
        /// </summary>
        private DBType? _dbType;

        /// <summary>
        /// 数据库提供者类型
        /// </summary>
        private Type _providerType;

        /// <summary>
        /// 主键自增全局配置
        /// </summary>
        private bool _autoIncrement;

        /// <summary>
        /// 数据库连接池
        /// </summary>
        private DbConnectionFactory _connFactory;
        #endregion

        #region 构造函数
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="connectionString">数据库连接字符串</param>
        /// <param name="dbType">数据库类型</param>
        /// <param name="provider">数据库Provider</param>
        /// <param name="maxPoolSize">数据库最大连接池大小，null值表示不设置上限</param>
        /// <param name="autoIncrement">主键自增全局配置(如果实体类或实体类的主键添加了AutoIncrementAttribute特性则不使用全局配置)</param>
        public DapperLiteClient(string connectionString, DBType dbType, IProvider provider, int? maxPoolSize = 100, bool autoIncrement = false)
        {
            _connectionString = connectionString;
            _dbType = dbType;
            _autoIncrement = autoIncrement;

            ProviderFactory.RegisterDBProvider(dbType, provider);
            _connFactory = new DbConnectionFactory(provider, connectionString, maxPoolSize);

            SqlMapper.AddTypeHandler(typeof(Guid), new GuidTypeHanlder());
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="connectionString">数据库连接字符串</param>
        /// <param name="providerType">数据库提供者类型</param>
        /// <param name="provider">数据库Provider</param>
        /// <param name="maxPoolSize">数据库最大连接池大小，null值表示不设置上限</param>
        /// <param name="autoIncrement">主键自增全局配置(如果实体类或实体类的主键添加了AutoIncrementAttribute特性则不使用全局配置)</param>
        public DapperLiteClient(string connectionString, Type providerType, IProvider provider, int? maxPoolSize = 100, bool autoIncrement = false)
        {
            _connectionString = connectionString;
            _providerType = providerType;
            _autoIncrement = autoIncrement;

            ProviderFactory.RegisterDBProvider(providerType, provider);
            _connFactory = new DbConnectionFactory(provider, connectionString, maxPoolSize);

            SqlMapper.AddTypeHandler(typeof(Guid), new GuidTypeHanlder());
        }
        #endregion

        #region 获取 IDbSession
        /// <summary>
        /// 获取 IDbSession
        /// </summary>
        public IDbSession GetSession(SplitTableMapping splitTableMapping = null)
        {
            DbSession dbSession;

            if (_dbType != null)
            {
                dbSession = new DbSession(_connectionString, _dbType.Value, splitTableMapping, _connFactory, _autoIncrement);
            }
            else
            {
                dbSession = new DbSession(_connectionString, _providerType, splitTableMapping, _connFactory, _autoIncrement);
            }

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
            DbSession dbSession;

            if (_dbType != null)
            {
                dbSession = new DbSession(_connectionString, _dbType.Value, splitTableMapping, _connFactory, _autoIncrement);
            }
            else
            {
                dbSession = new DbSession(_connectionString, _providerType, splitTableMapping, _connFactory, _autoIncrement);
            }

            if (OnExecuting != null)
            {
                dbSession.OnExecuting = OnExecuting;
            }

            return Task.FromResult(dbSession as IDbSession);
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
        public ISqlQueryable<T> Sql<T>(string sql = null, params object[] args) where T : new()
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
        /// <param name="alias">别名，默认值t</param>
        public ISqlQueryable<T> Queryable<T>(string alias = null) where T : new()
        {
            var session = GetSession();
            return session.Queryable<T>(alias);
        }

        /// <summary>
        /// 创建IQueryable
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="expression">返回匿名对象的表达式</param>
        public ISqlQueryable<T> Queryable<T>(Expression<Func<T, object>> expression) where T : new()
        {
            var session = GetSession();
            return session.Queryable<T>(expression);
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

        #region 从连接池池获取连接
        /// <summary>
        /// 从连接池池获取连接
        /// 如果需要使用数据库事务，请使用IDbSession接口的同名方法
        /// </summary>
        public DbConnectionExt GetConnection()
        {
            var session = GetSession();
            return session.GetConnection(null);
        }

        /// <summary>
        /// 从连接池池获取连接
        /// 如果需要使用数据库事务，请使用IDbSession接口的同名方法
        /// </summary>
        public Task<DbConnectionExt> GetConnectionAsync()
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
