using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlTypes;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Dapper.Lite
{
    /// <summary>
    /// 参数化查询SQL字符串
    /// </summary>
    public class SqlString<T> : SqlString, ISqlString<T> where T : new()
    {
        #region 构造函数
        public SqlString(IProvider provider, IDbSession session, string sql = null, params object[] args) : base(provider, session, sql, args)
        {

        }

        public SqlString(IProvider provider, IDbSession session, string sql, DbParameter[] args) : base(provider, session, sql, args)
        {

        }
        #endregion

        #region Append
        /// <summary>
        /// 追加参数化SQL
        /// </summary>
        /// <param name="sql">SQL</param>
        /// <param name="args">参数(支持多个参数或者把多个参数放在一个匿名对象中)</param>
        public new ISqlString<T> Append(string sql, params object[] args)
        {
            base.Append(sql, args);
            return this;
        }

        /// <summary>
        /// 追加参数化SQL
        /// </summary>
        /// <param name="condition">当condition等于true时追加SQL，等于false时不追加SQL</param>
        /// <param name="sql">SQL</param>
        /// <param name="args">参数</param>
        public new ISqlString<T> AppendIf(bool condition, string sql, params object[] args)
        {
            base.AppendIf(condition, sql, args);
            return this;
        }
        #endregion

        #region Where
        /// <summary>
        /// 追加参数化查询条件SQL
        /// </summary>
        /// <param name="expression">Lambda 表达式</param>
        public ISqlString<T> Where(Expression<Func<T, object>> expression)
        {
            try
            {
                ExpressionHelper<T> condition = new ExpressionHelper<T>(_provider, DbParameterNames, SqlStringMethod.Where);

                DbParameter[] dbParameters;
                string result = condition.VisitLambda(expression, out dbParameters);

                if (dbParameters != null)
                {
                    result = ParamsAddRange(dbParameters, result);
                }

                if (Sql.ToString().Contains(" where "))
                {
                    Sql.Append(" and " + result);
                }
                else
                {
                    Sql.Append(" where " + result);
                }
            }
            catch
            {
                throw;
            }

            return this;
        }
        #endregion

        #region Where
        /// <summary>
        /// 追加参数化查询条件SQL
        /// </summary>
        /// <param name="expression">Lambda 表达式</param>
        public ISqlString<T> Where<U>(Expression<Func<U, object>> expression)
        {
            try
            {
                ExpressionHelper<U> condition = new ExpressionHelper<U>(_provider, DbParameterNames, SqlStringMethod.Where);

                DbParameter[] dbParameters;
                string result = condition.VisitLambda(expression, out dbParameters);

                if (dbParameters != null)
                {
                    result = ParamsAddRange(dbParameters, result);
                }

                if (Sql.ToString().Contains(" where "))
                {
                    Sql.Append(" and " + result);
                }
                else
                {
                    Sql.Append(" where " + result);
                }
            }
            catch
            {
                throw;
            }

            return this;
        }
        #endregion

        #region 实现增删改查接口

        #region ToList
        /// <summary>
        /// 执行查询
        /// </summary>
        public List<T> ToList()
        {
            return _session.QueryList<T>(this.SQL, this.Params);
        }

        /// <summary>
        /// 执行查询
        /// </summary>
        public async Task<List<T>> ToListAsync()
        {
            return await _session.QueryListAsync<T>(this.SQL, this.Params);
        }
        #endregion

        #region ToPageList
        /// <summary>
        /// 执行查询
        /// </summary>
        public List<T> ToPageList(int page, int pageSize)
        {
            string ORDER_BY = " order by ";
            string[] strArr = this.SQL.Split(new string[] { ORDER_BY }, StringSplitOptions.None);
            string orderBy = strArr.Length > 1 ? ORDER_BY + strArr[1] : string.Empty;

            return _session.QueryPage<T>(strArr[0], orderBy, pageSize, page, this.Params);
        }

        /// <summary>
        /// 执行查询
        /// </summary>
        public async Task<List<T>> ToPageListAsync(int page, int pageSize)
        {
            string ORDER_BY = " order by ";
            string[] strArr = this.SQL.Split(new string[] { ORDER_BY }, StringSplitOptions.None);
            string orderBy = strArr.Length > 1 ? ORDER_BY + strArr[1] : string.Empty;

            return await _session.QueryPageAsync<T>(strArr[0], orderBy, pageSize, page, this.Params);
        }
        #endregion

        #region Count
        /// <summary>
        /// 返回数量
        /// </summary>
        public long Count()
        {
            return _session.QueryCount(this.SQL, this.Params);
        }

        /// <summary>
        /// 返回数量
        /// </summary>
        public async Task<long> CountAsync()
        {
            return await _session.QueryCountAsync(this.SQL, this.Params);
        }
        #endregion

        #region First
        /// <summary>
        /// 返回数量
        /// </summary>
        public T First()
        {
            return _session.Query<T>(this.SQL, this.Params);
        }

        /// <summary>
        /// 返回数量
        /// </summary>
        public async Task<T> FirstAsync()
        {
            return await _session.QueryAsync<T>(this.SQL, this.Params);
        }
        #endregion

        #region Exists
        /// <summary>
        /// 是否存在
        /// </summary>
        public new bool Exists()
        {
            return _session.Exists(this.SQL, this.Params);
        }

        /// <summary>
        /// 返回数量
        /// </summary>
        public new async Task<bool> ExistsAsync()
        {
            return await _session.ExistsAsync(this.SQL, this.Params);
        }
        #endregion

        #region Delete
        /// <summary>
        /// 删除
        /// </summary>
        public int Delete()
        {
            string[] sqlParts = this.SQL.Split(new string[] { " where " }, StringSplitOptions.None);
            string right;
            if (sqlParts.Length > 1)
            {
                right = sqlParts[1];
            }
            else
            {
                right = sqlParts[0];
            }

            Regex regex = new Regex("[\\(]?[\\s]*([\\w]+\\.)", RegexOptions.IgnoreCase);
            Match match = regex.Match(right);
            if (match.Success)
            {
                right = right.Replace(match.Groups[1].Value, " ");
            }

            return _session.DeleteByCondition<T>(right, this.Params);
        }

        /// <summary>
        /// 删除
        /// </summary>
        public Task<int> DeleteAsync()
        {
            string[] sqlParts = this.SQL.Split(new string[] { " where " }, StringSplitOptions.None);
            string right;
            if (sqlParts.Length > 1)
            {
                right = sqlParts[1];
            }
            else
            {
                right = sqlParts[0];
            }

            Regex regex = new Regex("[\\(]?[\\s]*([\\w]+\\.)", RegexOptions.IgnoreCase);
            Match match = regex.Match(right);
            if (match.Success)
            {
                right = right.Replace(match.Groups[1].Value, " ");
            }

            return _session.DeleteByConditionAsync<T>(right, this.Params);
        }
        #endregion

        #endregion

    }

    /// <summary>
    /// 参数化查询SQL字符串
    /// </summary>
    public class SqlString : ISqlString
    {
        #region 变量属性

        protected IProvider _provider;

        protected StringBuilder _sql = new StringBuilder();

        protected Dictionary<string, DbParameter> _params = new Dictionary<string, DbParameter>();

        protected Regex _regex = new Regex(@"[@|:]([a-zA-Z_]{1}[a-zA-Z0-9_]+)", RegexOptions.IgnoreCase);

        /// <summary>
        /// 参数化查询的参数
        /// </summary>
        public DbParameter[] Params { get { return _params.Values.ToArray(); } }

        /// <summary>
        /// 参数化查询的参数
        /// </summary>
        public DynamicParameters DynamicParameters
        {
            get
            {
                return _dbSession.ToDynamicParameters(Params);
            }
        }

        /// <summary>
        /// 参数化查询的SQL
        /// </summary>
        public string SQL { get { return _sql.ToString(); } }

        /// <summary>
        /// 参数化查询的SQL
        /// </summary>
        internal StringBuilder Sql
        {
            get
            {
                return _sql;
            }
            set
            {
                _sql = value;
            }
        }

        /// <summary>
        /// SQL参数的参数名称(防止参数名称重名)
        /// </summary>
        protected HashSet<string> _dbParameterNames = new HashSet<string>();

        /// <summary>
        /// SQL参数的参数名称(防止参数名称重名)
        /// </summary>
        internal HashSet<string> DbParameterNames
        {
            get
            {
                return _dbParameterNames;
            }
        }

        protected IDbSession _session;

        protected DbSession _dbSession;

        #endregion

        #region 构造函数
        public SqlString(IProvider provider, IDbSession session, string sql = null, params object[] args)
        {
            _provider = provider;
            _session = session;
            _dbSession = session as DbSession;

            if (sql != null)
            {
                Append(sql, args);
            }
        }

        public SqlString(IProvider provider, IDbSession session, string sql, DbParameter[] args)
        {
            _provider = provider;
            _session = session;
            _dbSession = session as DbSession;

            if (sql != null)
            {
                Append(sql, args);
            }
        }
        #endregion

        #region Append
        /// <summary>
        /// 追加参数化SQL
        /// </summary>
        /// <param name="sql">SQL</param>
        /// <param name="args">参数</param>
        public ISqlString Append(string sql, DbParameter[] args)
        {
            if (args != null)
            {
                foreach (var param in args)
                {
                    _params.Add(param.ParameterName, param);
                }
            }

            _sql.Append(string.Format(" {0} ", sql.Trim()));

            return this;
        }

        /// <summary>
        /// 追加参数化SQL
        /// </summary>
        /// <param name="sql">SQL</param>
        /// <param name="args">参数(支持多个参数或者把多个参数放在一个匿名对象中)</param>
        public ISqlString Append(string sql, params object[] args)
        {
            if (args == null) throw new Exception("参数args不能为null");

            //从匿名对象获取参数(参数名称、参数值)
            Dictionary<string, object> anonymousValues = GetAnonymousParameters(out bool isAnonymous, args);

            //获取SQL中的参数(参数名称、参数值)
            Dictionary<string, object> dict = GetParametersFromSql(ref sql, isAnonymous, anonymousValues, args);

            if (!isAnonymous && args.Length < dict.Count) throw new Exception("SqlString.AppendFormat参数不够");

            foreach (string name in dict.Keys)
            {
                object value = dict[name];
                Type valueType = value?.GetType();

                if (valueType == typeof(SqlValue))
                {
                    SqlValue sqlValue = value as SqlValue;
                    Type parameterType = sqlValue.Value.GetType();
                    if (sqlValue.Value.GetType().Name != typeof(List<>).Name)
                    {
                        string markKey = _provider.GetParameterName(name, parameterType);
                        sql = sql.Replace(markKey, sqlValue.Sql.Replace("{0}", markKey));
                        DbParameter param = _provider.GetDbParameter(name, sqlValue.Value);
                        _params.Add(param.ParameterName, param);
                    }
                    else
                    {
                        string markKey = _provider.GetParameterName(name, parameterType);
                        sql = sql.Replace(markKey, sqlValue.Sql.Replace("{0}", markKey));
                        string[] keyArr = sqlValue.Sql.Replace("(", string.Empty).Replace(")", string.Empty).Replace("@", string.Empty).Split(',');
                        IList valueList = (IList)sqlValue.Value;
                        for (int k = 0; k < valueList.Count; k++)
                        {
                            object item = valueList[k];
                            DbParameter param = _provider.GetDbParameter(keyArr[k], item);
                            _params.Add(param.ParameterName, param);
                        }
                    }
                }
                else
                {
                    DbParameter param = _provider.GetDbParameter(name, value);
                    _params.Add(param.ParameterName, param);
                }
            }

            _sql.Append(string.Format(" {0} ", sql.Trim()));

            return this;
        }

        /// <summary>
        /// 从匿名对象中获取参数
        /// 返回参数名称、参数值字典
        /// </summary>
        private Dictionary<string, object> GetAnonymousParameters(out bool isAnonymous, params object[] args)
        {
            isAnonymous = false;
            Dictionary<string, object> dict = new Dictionary<string, object>();
            if (args?.Length == 1)
            {
                Type type = args[0].GetType();
                if (type.Name.Contains("<>f__AnonymousType"))
                {
                    isAnonymous = true;
                    PropertyInfo[] props = type.GetProperties();
                    foreach (PropertyInfo propInfo in props)
                    {
                        dict.Add(propInfo.Name, propInfo.GetValue(args[0]));
                    }
                }
            }
            return dict;
        }

        /// <summary>
        /// 获取SQL中的参数
        /// 返回参数名称、参数值字典
        /// </summary>
        private Dictionary<string, object> GetParametersFromSql(ref string sql, bool isAnonymous, Dictionary<string, object> anonymousValues, params object[] args)
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            MatchCollection mc = _regex.Matches(sql);
            int argIndex = 0;
            foreach (Match m in mc)
            {
                var oldSql = m.Value;
                string name = m.Groups[1].Value;

                if (!dict.ContainsKey(name))
                {
                    Type parameterType = typeof(object);
                    if (isAnonymous)
                    {
                        if (anonymousValues.ContainsKey(name))
                        {
                            object obj = anonymousValues[name];
                            parameterType = obj?.GetType();
                            dict.Add(name, obj);
                        }
                    }
                    else
                    {
                        if (argIndex < args?.Length)
                        {
                            object obj = args[argIndex++];
                            parameterType = obj?.GetType();
                            dict.Add(name, obj);
                        }
                    }
                    sql = ReplaceSql(sql, oldSql, name, parameterType);
                }
            }
            return dict;
        }
        #endregion

        #region AppendIf
        /// <summary>
        /// 追加参数化SQL
        /// </summary>
        /// <param name="condition">当condition等于true时追加SQL，等于false时不追加SQL</param>
        /// <param name="sql">SQL</param>
        /// <param name="args">参数</param>
        public ISqlString AppendIf(bool condition, string sql, params object[] args)
        {
            if (condition)
            {
                Append(sql, args);
            }

            return this;
        }
        #endregion

        #region ToSql
        public string ToSql()
        {
            return _sql.ToString();
        }
        #endregion

        #region ReplaceSql
        /// <summary>
        /// 调用该方法的原因：参数化查询，SQL语句中统一使用@，而有的数据库不是@
        /// </summary>
        private string ReplaceSql(string sql, string oldStr, string name, Type parameterType)
        {
            string newStr = _provider.GetParameterName(name, parameterType);
            if (newStr == oldStr) return sql;
            return sql.Replace(oldStr, newStr);
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

        #region ParamsAddRange
        /// <summary>
        /// 批量添加参数
        /// </summary>
        internal string ParamsAddRange(DbParameter[] cmdParams, string sql)
        {
            foreach (DbParameter param in cmdParams)
            {
                if (!_params.ContainsKey(param.ParameterName))
                {
                    _params.Add(param.ParameterName, param);
                }
                else
                {
                    int index = 0;
                    while (_params.ContainsKey(param.ParameterName + (index == 0 ? "" : index.ToString())))
                    {
                        index++;
                    }
                    string newName = param.ParameterName + (index == 0 ? "" : index.ToString());
                    DbParameter newParam = _provider.GetDbParameter(newName, param.Value);
                    _params.Add(newParam.ParameterName, newParam);
                    string oldParamName = _provider.GetParameterName(param.ParameterName, param.Value.GetType());
                    string newParamName = _provider.GetParameterName(newParam.ParameterName, param.Value.GetType());
                    int pos = sql.IndexOf(oldParamName);
                    Regex regex = new Regex(oldParamName + "[)]{1}", RegexOptions.None);
                    Regex regex2 = new Regex(oldParamName + "[\\s]{1}", RegexOptions.None);
                    Regex regex3 = new Regex(oldParamName + "[,]{1}", RegexOptions.None);
                    if (regex.IsMatch(sql))
                    {
                        sql = regex.Replace(sql, newParamName + ")", 1);
                    }
                    else if (regex2.IsMatch(sql))
                    {
                        sql = regex2.Replace(sql, newParamName + " ", 1);
                    }
                    else if (regex3.IsMatch(sql))
                    {
                        sql = regex3.Replace(sql, newParamName + ",", 1);
                    }
                    else
                    {

                    }
                }
            }
            return sql;
        }
        #endregion

        #region 实现ISqlString增删改查接口

        /// <summary>
        /// 查询实体
        /// </summary>
        public T Query<T>() where T : new()
        {
            return _session.Query<T>(this);
        }

        /// <summary>
        /// 查询实体
        /// </summary>
        public Task<T> QueryAsync<T>() where T : new()
        {
            return _session.QueryAsync<T>(this);
        }

        /// <summary>
        /// 查询列表
        /// </summary>
        public List<T> QueryList<T>() where T : new()
        {
            return _session.QueryList<T>(this);
        }

        /// <summary>
        /// 查询列表
        /// </summary>
        public Task<List<T>> QueryListAsync<T>() where T : new()
        {
            return _session.QueryListAsync<T>(this);
        }

        /// <summary>
        /// 分页查询
        /// </summary>
        public List<T> QueryPage<T>(string orderby, int pageSize, int currentPage) where T : new()
        {
            return _session.QueryPage<T>(this, orderby, pageSize, currentPage);
        }

        /// <summary>
        /// 分页查询
        /// </summary>
        public Task<List<T>> QueryPageAsync<T>(string orderby, int pageSize, int currentPage) where T : new()
        {
            return _session.QueryPageAsync<T>(this, orderby, pageSize, currentPage);
        }

        /// <summary>
        /// 条件删除
        /// </summary>
        [Obsolete]
        public int DeleteByCondition<T>()
        {
            return _session.DeleteByCondition<T>(this);
        }

        /// <summary>
        /// 条件删除
        /// </summary>
        [Obsolete]
        public Task<int> DeleteByConditionAsync<T>()
        {
            return _session.DeleteByConditionAsync<T>(this);
        }

        /// <summary>
        /// 条件删除
        /// </summary>
        [Obsolete]
        public int DeleteByCondition(Type type)
        {
            return _session.DeleteByCondition(type, this);
        }

        /// <summary>
        /// 条件删除
        /// </summary>
        [Obsolete]
        public Task<int> DeleteByConditionAsync(Type type)
        {
            return _session.DeleteByConditionAsync(type, this);
        }

        /// <summary>
        /// 条件删除
        /// </summary>
        public int Delete<T>()
        {
            return _session.DeleteByCondition<T>(this);
        }

        /// <summary>
        /// 条件删除
        /// </summary>
        public Task<int> DeleteAsync<T>()
        {
            return _session.DeleteByConditionAsync<T>(this);
        }

        /// <summary>
        /// 条件删除
        /// </summary>
        public int Delete(Type type)
        {
            return _session.DeleteByCondition(type, this);
        }

        /// <summary>
        /// 条件删除
        /// </summary>
        public Task<int> DeleteAsync(Type type)
        {
            return _session.DeleteByConditionAsync(type, this);
        }

        /// <summary>
        /// 执行SQL语句，返回影响的记录数
        /// </summary>
        public int Execute()
        {
            return _session.Execute(this);
        }

        /// <summary>
        /// 执行SQL语句，返回影响的记录数
        /// </summary>
        public Task<int> ExecuteAsync()
        {
            return _session.ExecuteAsync(this);
        }

        /// <summary>
        /// 是否存在
        /// </summary>
        public bool Exists()
        {
            return _session.Exists(this);
        }

        /// <summary>
        /// 是否存在
        /// </summary>
        public Task<bool> ExistsAsync()
        {
            return _session.ExistsAsync(this);
        }

        /// <summary>
        /// 查询单个值
        /// </summary>
        public object QuerySingle()
        {
            return _session.QuerySingle(this);
        }

        /// <summary>
        /// 查询单个值
        /// </summary>
        public T QuerySingle<T>()
        {
            return _session.QuerySingle<T>(this);
        }

        /// <summary>
        /// 查询单个值
        /// </summary>
        public Task<object> QuerySingleAsync()
        {
            return _session.QuerySingleAsync(this);
        }

        /// <summary>
        /// 查询单个值
        /// </summary>
        public Task<T> QuerySingleAsync<T>()
        {
            return _session.QuerySingleAsync<T>(this);
        }

        /// <summary>
        /// 给定一条查询SQL，返回其查询结果的数量
        /// </summary>
        public long QueryCount()
        {
            return _session.QueryCount(this);
        }

        /// <summary>
        /// 给定一条查询SQL，返回其查询结果的数量
        /// </summary>
        public Task<long> QueryCountAsync()
        {
            return _session.QueryCountAsync(this);
        }

        /// <summary>
        /// 给定一条查询SQL，返回其查询结果的数量
        /// </summary>
        public long QueryCount(int pageSize, out long pageCount)
        {
            return _session.QueryCount(this, pageSize, out pageCount);
        }

        /// <summary>
        /// 给定一条查询SQL，返回其查询结果的数量
        /// </summary>
        public Task<CountResult> QueryCountAsync(int pageSize)
        {
            return _session.QueryCountAsync(this, pageSize);
        }

        #endregion

    }
}
