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
* 更新日期：2023年05月25日
* ---------------------------------------------------------------------- */

namespace Dapper.Lite
{
    /// <summary>
    /// DbSession
    /// 一个DbSession实例对应一个数据库连接，一个DbSession实例只有一个数据库连接
    /// DbSession不是线程安全的，不能跨线程使用
    /// </summary>
    public partial class DbSession : IDbSession
    {
        #region 静态变量
        /// <summary>
        /// SQL过滤正则
        /// </summary>
        private static Dictionary<string, Regex> _sqlFilteRegexDict = new Dictionary<string, Regex>();

        /// <summary>
        /// 数据库字段名与实体类属性名映射
        /// </summary>
        private static ConcurrentDictionary<Type, bool> _dictForTypeMap = new ConcurrentDictionary<Type, bool>();
        #endregion

        #region 变量
        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        private string _connectionString;

        /// <summary>
        /// 事务
        /// </summary>
        private DbTransactionExt _tran;

        /// <summary>
        /// 数据库连接
        /// </summary>
        private DbConnectionExt _conn;

        /// <summary>
        /// 数据库实现
        /// </summary>
        private IProvider _provider;

        /// <summary>
        /// 数据库自增(全局设置)
        /// </summary>
        private bool _autoIncrement;

        /// <summary>
        /// 分表映射
        /// </summary>
        private SplitTableMapping _splitTableMapping;

        /// <summary>
        /// 数据库连接池
        /// </summary>
        private DbConnectionFactory _connFactory;
        #endregion

        #region 静态构造函数
        /// <summary>
        /// 静态构造函数
        /// </summary>
        static DbSession()
        {
            _sqlFilteRegexDict.Add("net localgroup ", new Regex("net[\\s]+localgroup[\\s]+", RegexOptions.IgnoreCase));
            _sqlFilteRegexDict.Add("net user ", new Regex("net[\\s]+user[\\s]+", RegexOptions.IgnoreCase));
            _sqlFilteRegexDict.Add("xp_cmdshell ", new Regex("xp_cmdshell[\\s]+", RegexOptions.IgnoreCase));
            _sqlFilteRegexDict.Add("exec ", new Regex("exec[\\s]+", RegexOptions.IgnoreCase));
            _sqlFilteRegexDict.Add("execute ", new Regex("execute[\\s]+", RegexOptions.IgnoreCase));
            _sqlFilteRegexDict.Add("truncate ", new Regex("truncate[\\s]+", RegexOptions.IgnoreCase));
            _sqlFilteRegexDict.Add("drop ", new Regex("drop[\\s]+", RegexOptions.IgnoreCase));
            _sqlFilteRegexDict.Add("restore ", new Regex("restore[\\s]+", RegexOptions.IgnoreCase));
            _sqlFilteRegexDict.Add("create ", new Regex("create[\\s]+", RegexOptions.IgnoreCase));
            _sqlFilteRegexDict.Add("alter ", new Regex("alter[\\s]+", RegexOptions.IgnoreCase));
            _sqlFilteRegexDict.Add("rename ", new Regex("rename[\\s]+", RegexOptions.IgnoreCase));
            _sqlFilteRegexDict.Add("insert ", new Regex("insert[\\s]+", RegexOptions.IgnoreCase));
            _sqlFilteRegexDict.Add("update ", new Regex("update[\\s]+", RegexOptions.IgnoreCase));
            _sqlFilteRegexDict.Add("delete ", new Regex("delete[\\s]+", RegexOptions.IgnoreCase));
            _sqlFilteRegexDict.Add("select ", new Regex("select[\\s]+", RegexOptions.IgnoreCase));
        }
        #endregion

        #region 构造函数
        /// <summary>
        /// 构造函数
        /// </summary>
        public DbSession(string connectionString, DBType dbType, SplitTableMapping splitTableMapping, DbConnectionFactory connFactory, bool autoIncrement = false)
        {
            _connectionString = connectionString;
            _provider = ProviderFactory.CreateProvider(dbType);
            _splitTableMapping = splitTableMapping;
            _connFactory = connFactory;
            _autoIncrement = autoIncrement;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public DbSession(string connectionString, Type providerType, SplitTableMapping splitTableMapping, DbConnectionFactory connFactory, bool autoIncrement = false)
        {
            _connectionString = connectionString;
            _provider = ProviderFactory.CreateProvider(providerType);
            _splitTableMapping = splitTableMapping;
            _connFactory = connFactory;
            _autoIncrement = autoIncrement;
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
        public ISqlQueryable<T> Sql<T>(string sql = null, params object[] args) where T : new()
        {
            return new SqlQueryable<T>(_provider, this, sql, args);
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
            SqlQueryable<T> sqlString = new SqlQueryable<T>(_provider, this, null);
            return sqlString.Queryable(alias);
        }

        /// <summary>
        /// 创建IQueryable
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="expression">返回匿名对象的表达式</param>
        public ISqlQueryable<T> Queryable<T>(Expression<Func<T, object>> expression) where T : new()
        {
            SqlQueryable<T> sqlString = new SqlQueryable<T>(_provider, this, null);
            return sqlString.Select(expression);
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

            using (_conn = _connFactory.GetConnection(_tran))
            {
                object obj = _conn.Conn.ExecuteScalar(sql);
                if (object.Equals(obj, null) || object.Equals(obj, DBNull.Value))
                {
                    return 1;
                }
                else
                {
                    return int.Parse(obj.ToString()) + 1;
                }
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

        #region 从连接池池获取连接
        /// <summary>
        /// 从连接池池获取连接
        /// </summary>
        public DbConnectionExt GetConnection(DbTransactionExt _tran = null)
        {
            return _connFactory.GetConnection(_tran);
        }

        /// <summary>
        /// 从连接池池获取连接
        /// </summary>
        public Task<DbConnectionExt> GetConnectionAsync(DbTransactionExt _tran = null)
        {
            return _connFactory.GetConnectionAsync(_tran);
        }
        #endregion

    }
}
