using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Dapper;

/* ----------------------------------------------------------------------
* 作    者：suxiang
* 创建日期：2016年11月23日
* 更新日期：2023年07月07日
* ---------------------------------------------------------------------- */

namespace Dapper.Lite
{
    /// <summary>
    /// DbSession实例表示与数据库的会话
    /// 使用事务时，DbSession不是线程安全的
    /// DbSession不建议定义成单例模式
    /// </summary>
    public partial class DbSession<TFlag> : DbSession, IDbSession<TFlag>
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public DbSession(string connectionString, IProvider provider, SplitTableMapping splitTableMapping) : base(connectionString, provider, splitTableMapping) { }
    }

    /// <summary>
    /// DbSession实例表示与数据库的会话
    /// 使用事务时，DbSession不是线程安全的
    /// DbSession不建议定义成单例模式
    /// </summary>
    public partial class DbSession : IDbSession
    {
        #region 静态变量
        /// <summary>
        /// SQL过滤正则
        /// </summary>
        private static ConcurrentDictionary<string, Regex> _sqlFilteRegexDict = new ConcurrentDictionary<string, Regex>();

        /// <summary>
        /// 数据库字段名与实体类属性名映射
        /// </summary>
        private static ConcurrentDictionary<Type, bool> _dictForTypeMap = new ConcurrentDictionary<Type, bool>();

        private static object _lockSetTypeMap = new object();
        #endregion

        #region 变量
        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        private string _connectionString;

        /// <summary>
        /// 事务
        /// </summary>
        private DbTransaction _tran;

        /// <summary>
        /// 数据库实现
        /// </summary>
        private IProvider _provider;

        /// <summary>
        /// 分表映射
        /// </summary>
        private SplitTableMapping _splitTableMapping;

        /// <summary>
        /// 事务
        /// </summary>
        public DbTransaction Tran => _tran;

        /// <summary>
        /// 数据库连接
        /// </summary>
        public DbConnection Conn => _tran?.Connection;

        private CommandType _commandType = CommandType.Text;

        private int? _commandTimeout = null; // Dapper参数

        private bool _buffered = true; // Dapper参数

        #endregion

        #region 静态构造函数
        /// <summary>
        /// 静态构造函数
        /// </summary>
        static DbSession()
        {
            _sqlFilteRegexDict.TryAdd("net localgroup ", new Regex("net[\\s]+localgroup[\\s]+", RegexOptions.IgnoreCase));
            _sqlFilteRegexDict.TryAdd("net user ", new Regex("net[\\s]+user[\\s]+", RegexOptions.IgnoreCase));
            _sqlFilteRegexDict.TryAdd("xp_cmdshell ", new Regex("xp_cmdshell[\\s]+", RegexOptions.IgnoreCase));
            _sqlFilteRegexDict.TryAdd("exec ", new Regex("exec[\\s]+", RegexOptions.IgnoreCase));
            _sqlFilteRegexDict.TryAdd("execute ", new Regex("execute[\\s]+", RegexOptions.IgnoreCase));
            _sqlFilteRegexDict.TryAdd("truncate ", new Regex("truncate[\\s]+", RegexOptions.IgnoreCase));
            _sqlFilteRegexDict.TryAdd("drop ", new Regex("drop[\\s]+", RegexOptions.IgnoreCase));
            _sqlFilteRegexDict.TryAdd("restore ", new Regex("restore[\\s]+", RegexOptions.IgnoreCase));
            _sqlFilteRegexDict.TryAdd("create ", new Regex("create[\\s]+", RegexOptions.IgnoreCase));
            _sqlFilteRegexDict.TryAdd("alter ", new Regex("alter[\\s]+", RegexOptions.IgnoreCase));
            _sqlFilteRegexDict.TryAdd("rename ", new Regex("rename[\\s]+", RegexOptions.IgnoreCase));
            _sqlFilteRegexDict.TryAdd("insert ", new Regex("insert[\\s]+", RegexOptions.IgnoreCase));
            _sqlFilteRegexDict.TryAdd("update ", new Regex("update[\\s]+", RegexOptions.IgnoreCase));
            _sqlFilteRegexDict.TryAdd("delete ", new Regex("delete[\\s]+", RegexOptions.IgnoreCase));
            _sqlFilteRegexDict.TryAdd("select ", new Regex("select[\\s]+", RegexOptions.IgnoreCase));
        }
        #endregion

        #region 构造函数
        /// <summary>
        /// 构造函数
        /// </summary>
        public DbSession(string connectionString, IProvider provider, SplitTableMapping splitTableMapping)
        {
            _connectionString = connectionString;
            _provider = provider;
            _splitTableMapping = splitTableMapping ?? new SplitTableMapping();
        }
        #endregion

        #region 创建SqlString对象
        /// <summary>
        /// 创建SqlString对象
        /// </summary>
        public ISqlString Sql(string sql = null, params object[] args)
        {
            return new SqlString(_provider, this, sql, args);
        }

        /// <summary>
        /// 创建SqlString对象
        /// </summary>
        public ISqlString<T> Sql<T>(string sql = null, params object[] args)
        {
            return new SqlString<T>(_provider, this, sql, args);
        }
        #endregion

        #region 创建ISqlQueryable<T>
        /// <summary>
        /// 创建IQueryable
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        public ISqlQueryable<T> Queryable<T>()
        {
            SqlQueryable<T> sqlString = new SqlQueryable<T>(_provider, this, null);
            return sqlString.Queryable();
        }
        #endregion

        #region 查询下一个ID
        /// <summary>
        /// 查询下一个ID
        /// </summary>
        /// <typeparam name="T">实体类</typeparam>
        public int QueryNextId<T>()
        {
            Type type = typeof(T);

            string idName = GetIdName(type, out _);
            string sql = _provider.CreateGetMaxIdSql(GetTableName(_provider, type), _provider.OpenQuote + idName + _provider.CloseQuote);

            object obj = ExecuteScalar(sql);
            if (object.Equals(obj, null) || object.Equals(obj, DBNull.Value))
            {
                return 1;
            }
            else
            {
                return int.Parse(obj.ToString()) + 1;
            }
        }
        #endregion

        #region ForList
        /// <summary>
        /// 创建 in 或 not in SQL
        /// </summary>
        public SqlValue ForList(IList list)
        {
            return _provider.ForList(list);
        }
        #endregion

        #region 获取数据库连接
        /// <summary>
        /// 获取数据库连接
        /// </summary>
        public DbConnection GetConnection(DbTransaction tran = null)
        {
            if (tran != null)
            {
                return tran.Connection;
            }
            else
            {
                return _provider.CreateConnection(_connectionString);
            }
        }

        /// <summary>
        /// 获取数据库连接
        /// </summary>
        public Task<DbConnection> GetConnectionAsync(DbTransaction tran = null)
        {
            if (tran != null)
            {
                return Task.FromResult(tran.Connection);
            }
            else
            {
                return Task.FromResult(_provider.CreateConnection(_connectionString));
            }
        }

        /// <summary>
        /// 获取数据库连接，已经Open
        /// </summary>
        public DbConnection GetOpenedConnection(DbTransaction tran = null)
        {
            if (tran != null)
            {
                return tran.Connection;
            }
            else
            {
                var conn = _provider.CreateConnection(_connectionString);
                if (conn.State == ConnectionState.Closed) conn.Open();
                return conn;
            }
        }

        /// <summary>
        /// 获取数据库连接，已经Open
        /// </summary>
        public async Task<DbConnection> GetOpenedConnectionAsync(DbTransaction tran = null)
        {
            if (tran != null)
            {
                return tran.Connection;
            }
            else
            {
                var conn = _provider.CreateConnection(_connectionString);
                if (conn.State == ConnectionState.Closed) await conn.OpenAsync();
                return conn;
            }
        }
        #endregion

        #region 设置CommandType
        /// <summary>
        /// 设置CommandType
        /// </summary>
        public IDbSession SetCommandType(CommandType commandType)
        {
            _commandType = commandType;
            return this;
        }
        #endregion

        #region Dapper参数设置
        /// <summary>
        /// 设置Dapper参数commandTimeout
        /// </summary>
        public IDbSession SetCommandTimeout(int? commandTimeout)
        {
            _commandTimeout = commandTimeout;
            return this;
        }

        /// <summary>
        /// 设置Dapper参数buffered
        /// </summary>
        public IDbSession SetBuffered(bool buffered)
        {
            _buffered = buffered;
            return this;
        }
        #endregion

    }
}
