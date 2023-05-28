using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections;
using System.Data.SqlTypes;

namespace Dapper.Lite
{
    /// <summary>
    /// SQL字符串类
    /// </summary>
    public class SqlQueryable<T> : ISqlQueryable<T> where T : new()
    {
        #region 变量
        private SqlString _sqlString;

        private IProvider _provider;

        private IDbSession _session;

        private DbSession _dbSession;
        #endregion

        #region 构造函数
        /// <summary>
        /// SQL字符串类
        /// </summary>
        public SqlQueryable(IProvider provider, IDbSession session, string sql = null, params object[] args)
        {
            _sqlString = new SqlString(provider, session, sql, args);
            _provider = provider;
            _session = session;
            _dbSession = session as DbSession;
        }

        /// <summary>
        /// SQL字符串类
        /// </summary>
        public SqlQueryable(IProvider provider, IDbSession session, string sql, DbParameter[] args)
        {
            _sqlString = new SqlString(provider, session, sql, args);
            _provider = provider;
            _session = session;
            _dbSession = session as DbSession;
        }
        #endregion

        #region Queryable
        /// <summary>
        /// 创建单表查询SQL
        /// </summary>
        /// <param name="alias">别名，默认值t</param>
        public ISqlQueryable<T> Queryable(string alias = null)
        {
            Type type = typeof(T);
            alias = alias ?? "t";

            _sqlString.Sql.AppendFormat("select ", _dbSession.GetTableName(_provider, type));

            PropertyInfoEx[] propertyInfoExArray = DbSession.GetEntityProperties(type);
            foreach (PropertyInfoEx propertyInfoEx in propertyInfoExArray)
            {
                PropertyInfo propertyInfo = propertyInfoEx.PropertyInfo;
                if (propertyInfo.GetCustomAttribute<ColumnAttribute>() != null)
                {
                    _sqlString.Sql.AppendFormat("{0}.{1}{2}{3},", alias, _provider.OpenQuote, propertyInfoEx.FieldName, _provider.CloseQuote);
                }
            }

            _sqlString.Sql.Remove(_sqlString.Sql.Length - 1, 1);

            _sqlString.Sql.AppendFormat(" from {0} {1}", _dbSession.GetTableName(_provider, type), alias);

            return this;
        }
        #endregion

        #region WhereIf
        /// <summary>
        /// 追加参数化查询条件SQL
        /// </summary>
        /// <param name="condition">当condition等于true时追加SQL，等于false时不追加SQL</param>
        /// <param name="expression">Lambda 表达式</param>
        public ISqlQueryable<T> WhereIf(bool condition, Expression<Func<T, object>> expression)
        {
            if (condition)
            {
                Where(expression);
            }

            return this;
        }
        #endregion

        #region WhereIf
        /// <summary>
        /// 追加参数化查询条件SQL
        /// </summary>
        /// <param name="condition">当condition等于true时追加SQL，等于false时不追加SQL</param>
        /// <param name="expression">Lambda 表达式</param>
        public ISqlQueryable<T> WhereIf<U>(bool condition, Expression<Func<U, object>> expression)
        {
            if (condition)
            {
                Where<U>(expression);
            }

            return this;
        }
        #endregion

        #region Where
        /// <summary>
        /// 追加参数化查询条件SQL
        /// </summary>
        /// <param name="expression">Lambda 表达式</param>
        public ISqlQueryable<T> Where(Expression<Func<T, object>> expression)
        {
            try
            {
                ExpressionHelper<T> condition = new ExpressionHelper<T>(_provider, _sqlString.DbParameterNames, SqlStringMethod.Where);

                DbParameter[] dbParameters;
                string result = " (" + condition.VisitLambda(expression, out dbParameters) + ")";

                if (dbParameters != null)
                {
                    result = _sqlString.ParamsAddRange(dbParameters, result);
                }

                if (_sqlString.RemoveSubSqls(_sqlString.Sql.ToString()).Contains(" where "))
                {
                    _sqlString.Sql.Append(" and " + result);
                }
                else
                {
                    _sqlString.Sql.Append(" where " + result);
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
        public ISqlQueryable<T> Where<U>(Expression<Func<U, object>> expression)
        {
            try
            {
                ExpressionHelper<T> condition = new ExpressionHelper<T>(_provider, _sqlString.DbParameterNames, SqlStringMethod.Where);

                DbParameter[] dbParameters;
                string result = " (" + condition.VisitLambda(expression, out dbParameters) + ")";

                if (dbParameters != null)
                {
                    result = _sqlString.ParamsAddRange(dbParameters, result);
                }

                if (_sqlString.RemoveSubSqls(_sqlString.Sql.ToString()).Contains(" where "))
                {
                    _sqlString.Sql.Append(" and " + result);
                }
                else
                {
                    _sqlString.Sql.Append(" where " + result);
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
        public ISqlQueryable<T> Where<U>(Expression<Func<T, U, object>> expression)
        {
            try
            {
                ExpressionHelper<T> condition = new ExpressionHelper<T>(_provider, _sqlString.DbParameterNames, SqlStringMethod.Where);

                DbParameter[] dbParameters;
                string result = " (" + condition.VisitLambda(expression, out dbParameters) + ")";

                if (dbParameters != null)
                {
                    result = _sqlString.ParamsAddRange(dbParameters, result);
                }

                if (_sqlString.RemoveSubSqls(_sqlString.Sql.ToString()).Contains(" where "))
                {
                    _sqlString.Sql.Append(" and " + result);
                }
                else
                {
                    _sqlString.Sql.Append(" where " + result);
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
        public ISqlQueryable<T> Where<U, D>(Expression<Func<T, U, D, object>> expression)
        {
            try
            {
                ExpressionHelper<T> condition = new ExpressionHelper<T>(_provider, _sqlString.DbParameterNames, SqlStringMethod.Where);

                DbParameter[] dbParameters;
                string result = " (" + condition.VisitLambda(expression, out dbParameters) + ")";

                if (dbParameters != null)
                {
                    result = _sqlString.ParamsAddRange(dbParameters, result);
                }

                if (_sqlString.RemoveSubSqls(_sqlString.Sql.ToString()).Contains(" where "))
                {
                    _sqlString.Sql.Append(" and " + result);
                }
                else
                {
                    _sqlString.Sql.Append(" where " + result);
                }
            }
            catch
            {
                throw;
            }

            return this;
        }
        #endregion

        #region OrderBy
        /// <summary>
        /// 追加 order by SQL
        /// </summary>
        public ISqlQueryable<T> OrderBy(Expression<Func<T, object>> expression)
        {
            ExpressionHelper<T> condition = new ExpressionHelper<T>(_provider, _sqlString.DbParameterNames, SqlStringMethod.OrderBy);
            DbParameter[] dbParameters;
            string sql = condition.VisitLambda(expression, out dbParameters);

            if (!_sqlString.Sql.ToString().Contains(" order by "))
            {
                _sqlString.Sql.AppendFormat(" order by {0} asc ", sql);
            }
            else
            {
                _sqlString.Sql.AppendFormat(", {0} asc ", sql);
            }

            return this;
        }
        #endregion

        #region OrderByDescending
        /// <summary>
        /// 追加 order by SQL
        /// </summary>
        public ISqlQueryable<T> OrderByDescending(Expression<Func<T, object>> expression)
        {
            ExpressionHelper<T> condition = new ExpressionHelper<T>(_provider, _sqlString.DbParameterNames, SqlStringMethod.OrderByDescending);
            DbParameter[] dbParameters;
            string sql = condition.VisitLambda(expression, out dbParameters);

            if (!_sqlString.Sql.ToString().Contains(" order by "))
            {
                _sqlString.Sql.AppendFormat(" order by {0} desc ", sql);
            }
            else
            {
                _sqlString.Sql.AppendFormat(", {0} desc ", sql);
            }

            return this;
        }
        #endregion

        #region LeftJoin
        /// <summary>
        /// 追加 left join SQL
        /// </summary>
        public ISqlQueryable<T> LeftJoin<U>(Expression<Func<T, U, object>> expression)
        {
            ExpressionHelper<T> condition = new ExpressionHelper<T>(_provider, _sqlString.DbParameterNames, SqlStringMethod.LeftJoin);
            DbParameter[] dbParameters;
            string sql = condition.VisitLambda(expression, out dbParameters);

            string tableName = _dbSession.GetTableName(_provider, typeof(U));

            string alias = sql.Split('=')[1].Split('.')[0].Trim();

            _sqlString.Sql.AppendFormat(" left join {0} {1} on {2} ", tableName, alias, sql);

            return this;
        }
        #endregion

        #region InnerJoin
        /// <summary>
        /// 追加 inner join SQL
        /// </summary>
        public ISqlQueryable<T> InnerJoin<U>(Expression<Func<T, U, object>> expression)
        {
            ExpressionHelper<T> condition = new ExpressionHelper<T>(_provider, _sqlString.DbParameterNames, SqlStringMethod.LeftJoin);
            DbParameter[] dbParameters;
            string sql = condition.VisitLambda(expression, out dbParameters);

            string tableName = _dbSession.GetTableName(_provider, typeof(U));

            string alias = sql.Split('=')[1].Split('.')[0].Trim();

            _sqlString.Sql.AppendFormat(" inner join {0} {1} on {2} ", tableName, alias, sql);

            return this;
        }
        #endregion

        #region RightJoin
        /// <summary>
        /// 追加 right join SQL
        /// </summary>
        public ISqlQueryable<T> RightJoin<U>(Expression<Func<T, U, object>> expression)
        {
            ExpressionHelper<T> condition = new ExpressionHelper<T>(_provider, _sqlString.DbParameterNames, SqlStringMethod.LeftJoin);
            DbParameter[] dbParameters;
            string sql = condition.VisitLambda(expression, out dbParameters);

            string tableName = _dbSession.GetTableName(_provider, typeof(U));

            string alias = sql.Split('=')[1].Split('.')[0].Trim();

            _sqlString.Sql.AppendFormat(" right join {0} {1} on {2} ", tableName, alias, sql);

            return this;
        }
        #endregion

        #region WhereJoin
        /// <summary>
        /// Where 连表
        /// </summary>
        public ISqlQueryable<T> WhereJoin<U>(Expression<Func<T, U, object>> expression)
        {
            ExpressionHelper<T> condition = new ExpressionHelper<T>(_provider, _sqlString.DbParameterNames, SqlStringMethod.LeftJoin);
            DbParameter[] dbParameters;
            string sql = condition.VisitLambda(expression, out dbParameters);

            _sqlString.Sql.AppendFormat(" where {0} ", sql);

            return this;
        }
        #endregion

        #region Select
        /// <summary>
        /// 追加 select SQL
        /// </summary>
        /// <param name="subSql">子SQL</param>
        /// <param name="alias">别名，默认值t</param>
        public ISqlQueryable<T> Select(ISqlQueryable<T> subSql = null, string alias = null)
        {
            return Select<T>(null, subSql, alias);
        }

        /// <summary>
        /// 追加 select SQL
        /// </summary>
        /// <param name="sql">SQL，插入到子SQL的前面，或者插入到{0}的位置</param>
        /// <param name="subSql">子SQL</param>
        /// <param name="alias">别名，默认值t</param>
        public ISqlQueryable<T> Select(string sql, ISqlQueryable<T> subSql = null, string alias = null)
        {
            return Select<T>(sql, subSql, alias);
        }

        /// <summary>
        /// 追加 select SQL
        /// </summary>
        /// <param name="subSql">子SQL</param>
        /// <param name="alias">别名，默认值t</param>
        public ISqlQueryable<T> Select<U>(ISqlQueryable<U> subSql = null, string alias = null) where U : new()
        {
            return Select<U>(null, subSql, alias);
        }

        /// <summary>
        /// 追加 select SQL
        /// </summary>
        /// <param name="sql">SQL，插入到子SQL的前面，或者插入到{0}的位置</param>
        /// <param name="subSql">子SQL</param>
        /// <param name="alias">别名，默认值t</param>
        public ISqlQueryable<T> Select<U>(string sql, ISqlQueryable<U> subSql = null, string alias = null) where U : new()
        {
            alias = alias ?? "t";
            if (sql == null) sql = string.Empty;
            if (subSql == null) subSql = _session.Sql<U>();
            if (sql.Contains("{0}"))
            {
                sql = sql.Replace("{0}", subSql.SQL);
            }
            else
            {
                sql = sql + subSql.SQL;
            }
            if (_sqlString.Sql.ToString().Contains(" from "))
            {
                string[] leftRigth = _sqlString.Sql.ToString().Split(new string[] { " from " }, StringSplitOptions.None);
                string left = leftRigth[0];
                string right = leftRigth[1];

                if (left.Trim().EndsWith("select"))
                {
                    _sqlString.Sql = new StringBuilder(string.Format("{0} {1} from {2}", left, sql, right));
                }
                else
                {
                    _sqlString.Sql = new StringBuilder(string.Format("{0}, {1} from {2}", left, sql, right));
                }
            }
            else
            {
                _sqlString.Sql = new StringBuilder(string.Format("select {0} from {1} {2}", sql, _dbSession.GetTableName(_provider, typeof(T)), alias));
            }

            string newSubSql = _sqlString.ParamsAddRange(subSql.Params, subSql.SQL);

            if (!string.IsNullOrWhiteSpace(newSubSql)
                && newSubSql.Contains("select ")
                && newSubSql.Contains(" from "))
            {
                _sqlString.SubSqls.Add(newSubSql);
            }

            return this;
        }

        /// <summary>
        /// 追加 select SQL
        /// </summary>
        /// <param name="expression">返回匿名对象的表达式</param>
        public ISqlQueryable<T> Select(Expression<Func<T, object>> expression)
        {
            Type type = expression.Body.Type;
            PropertyInfo[] props = type.GetProperties();
            Dictionary<string, string> dict = DbSession.GetEntityProperties(typeof(T)).ToLookup(a => a.PropertyInfo.Name).ToDictionary(a => a.Key, a => a.First().FieldName);
            int i = 0;
            StringBuilder fields = new StringBuilder();
            if (type != typeof(string))
            {
                foreach (PropertyInfo propInfo in props)
                {
                    i++;
                    fields.AppendFormat("{0}.{1}", expression.Parameters[0].Name, _provider.OpenQuote + dict[propInfo.Name] + _provider.CloseQuote);
                    if (i < props.Length) fields.Append(", ");
                }
            }
            else
            {
                if (expression.Body is ConstantExpression)
                {
                    fields.Append((expression.Body as ConstantExpression).Value.ToString());
                }
                else
                {
                    throw new Exception("不支持");
                }
            }

            if (_sqlString.Sql.ToString().Contains(" from "))
            {
                string[] leftRigth = _sqlString.Sql.ToString().Split(new string[] { " from " }, StringSplitOptions.None);
                string left = leftRigth[0];
                string right = leftRigth[1];

                if (left.Trim().EndsWith("select"))
                {
                    _sqlString.Sql = new StringBuilder(string.Format("{0} {1} from {2}", left, fields.ToString(), right));
                }
                else
                {
                    _sqlString.Sql = new StringBuilder(string.Format("{0}, {1} from {2}", left, fields.ToString(), right));
                }
            }
            else
            {
                _sqlString.Sql.AppendFormat("select {0} from {1} {2}", fields.ToString(), _dbSession.GetTableName(_provider, typeof(T)), expression.Parameters[0].Name);
            }

            return this;
        }

        /// <summary>
        /// 追加 select SQL
        /// </summary>
        /// <typeparam name="U">实体类型</typeparam>
        /// <param name="expression">属性名表达式</param>
        /// <param name="expression2">别名表达式</param>
        public ISqlQueryable<T> Select<U>(Expression<Func<U, object>> expression, Expression<Func<T, object>> expression2)
        {
            DbParameter[] dbParameters;

            ExpressionHelper<U> condition = new ExpressionHelper<U>(_provider, _sqlString.DbParameterNames, SqlStringMethod.Select);
            string sql = condition.VisitLambda(expression, out dbParameters);

            ExpressionHelper<U> condition2 = new ExpressionHelper<U>(_provider, _sqlString.DbParameterNames, SqlStringMethod.Select);
            string sql2 = condition.VisitLambda(expression2, out dbParameters);

            if (_sqlString.Sql.ToString().Contains(" from "))
            {
                string[] leftRigth = _sqlString.Sql.ToString().Split(new string[] { " from " }, StringSplitOptions.None);
                string left = leftRigth[0];
                string right = leftRigth[1];

                _sqlString.Sql = new StringBuilder(string.Format("{0}, {1} as {2} from {3}", left, sql, sql2.Split('.')[1].Trim(), right));
            }
            else
            {
                _sqlString.Sql = new StringBuilder(string.Format("select {0} as {1} from {2} {3}", sql, sql2.Split('.')[1].Trim(), _dbSession.GetTableName(_provider, typeof(T)), expression2.Parameters[0].Name));
            }

            return this;
        }
        #endregion

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
            return _session.QueryList<T>(this.SQL, this.Params).FirstOrDefault();
        }

        /// <summary>
        /// 返回数量
        /// </summary>
        public async Task<T> FirstAsync()
        {
            return (await _session.QueryListAsync<T>(this.SQL, this.Params)).FirstOrDefault();
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

        #region 部分ISqlString接口实现
        /// <summary>
        /// 转换为ISqlString
        /// </summary>
        public ISqlString AsSqlString()
        {
            return _sqlString;
        }

        /// <summary>
        /// 参数化查询的SQL
        /// </summary>
        public string SQL => _sqlString.SQL;

        /// <summary>
        /// 参数化查询的参数
        /// </summary>
        public DbParameter[] Params => _sqlString.Params;

        /// <summary>
        /// 参数化查询的参数
        /// </summary>
        public DynamicParameters DynamicParameters => _sqlString.DynamicParameters;

        /// <summary>
        /// 追加参数化SQL
        /// </summary>
        /// <param name="sql">SQL</param>
        /// <param name="args">参数(支持多个参数或者把多个参数放在一个匿名对象中)</param>
        public ISqlQueryable<T> Append(string sql, params object[] args)
        {
            return _sqlString.Append<T>(sql, args);
        }

        /// <summary>
        /// 追加参数化SQL
        /// </summary>
        /// <param name="condition">当condition等于true时追加SQL，等于false时不追加SQL</param>
        /// <param name="sql">SQL</param>
        /// <param name="args">参数(支持多个参数或者把多个参数放在一个匿名对象中)</param>
        public ISqlQueryable<T> AppendIf(bool condition, string sql, params object[] args)
        {
            return _sqlString.AppendIf<T>(condition, sql, args);
        }

        /// <summary>
        /// 追加参数化SQL
        /// </summary>
        /// <param name="condition">当condition等于true时追加SQL，等于false时不追加SQL</param>
        /// <param name="sql">SQL</param>
        /// <param name="argsFunc">参数</param>
        public ISqlQueryable<T> AppendIf(bool condition, string sql, params Func<object>[] argsFunc)
        {
            return _sqlString.AppendIf<T>(condition, sql, argsFunc);
        }

        /// <summary>
        /// 追加参数化SQL
        /// </summary>
        /// <param name="sql">SQL，插入到子SQL的前面，或者插入到{0}的位置</param>
        /// <param name="subSql">子SQL</param>
        public ISqlQueryable<T> AppendSubSql(string sql, ISqlString subSql)
        {
            return _sqlString.AppendSubSql<T>(sql, subSql);
        }

        /// <summary>
        /// 追加参数化SQL
        /// </summary>
        /// <param name="sql">SQL，插入到子SQL的前面，或者插入到{0}的位置</param>
        /// <param name="subSql">子SQL</param>
        public ISqlQueryable<T> AppendSubSql(string sql, ISqlQueryable<T> subSql)
        {
            return _sqlString.AppendSubSql<T>(sql, subSql.AsSqlString());
        }

        /// <summary>
        /// 追加参数化SQL
        /// </summary>
        /// <param name="sql">SQL</param>
        /// <param name="args">参数(支持多个参数或者把多个参数放在一个匿名对象中)</param>
        public ISqlQueryable<T> LeftJoin(string sql, params object[] args)
        {
            return _sqlString.LeftJoin<T>(sql, args);
        }

        /// <summary>
        /// 追加参数化SQL
        /// </summary>
        /// <param name="sql">SQL</param>
        /// <param name="args">参数(支持多个参数或者把多个参数放在一个匿名对象中)</param>
        public ISqlQueryable<T> InnerJoin(string sql, params object[] args)
        {
            return _sqlString.InnerJoin<T>(sql, args);
        }

        /// <summary>
        /// 追加参数化SQL
        /// </summary>
        /// <param name="sql">SQL</param>
        /// <param name="args">参数(支持多个参数或者把多个参数放在一个匿名对象中)</param>
        public ISqlQueryable<T> RightJoin(string sql, params object[] args)
        {
            return _sqlString.RightJoin<T>(sql, args);
        }

        /// <summary>
        /// 追加参数化SQL
        /// </summary>
        /// <param name="sql">SQL</param>
        /// <param name="args">参数(支持多个参数或者把多个参数放在一个匿名对象中)</param>
        public ISqlQueryable<T> Where(string sql, params object[] args)
        {
            return _sqlString.Where<T>(sql, args);
        }

        /// <summary>
        /// 追加参数化SQL
        /// </summary>
        /// <param name="condition">当condition等于true时追加SQL，等于false时不追加SQL</param>
        /// <param name="sql">SQL</param>
        /// <param name="args">参数(支持多个参数或者把多个参数放在一个匿名对象中)</param>
        public ISqlQueryable<T> WhereIf(bool condition, string sql, params object[] args)
        {
            return _sqlString.WhereIf<T>(condition, sql, args);
        }

        /// <summary>
        /// 追加参数化SQL
        /// </summary>
        /// <param name="sql">SQL</param>
        /// <param name="args">参数(支持多个参数或者把多个参数放在一个匿名对象中)</param>
        public ISqlQueryable<T> Having(string sql, params object[] args)
        {
            return _sqlString.Having<T>(sql, args);
        }

        /// <summary>
        /// 追加参数化SQL
        /// </summary>
        /// <param name="sql">SQL</param>
        /// <param name="args">参数(支持多个参数或者把多个参数放在一个匿名对象中)</param>
        public ISqlQueryable<T> GroupBy(string sql, params object[] args)
        {
            return _sqlString.GroupBy<T>(sql, args);
        }

        /// <summary>
        /// 追加参数化SQL
        /// </summary>
        /// <param name="sql">SQL</param>
        /// <param name="args">参数(支持多个参数或者把多个参数放在一个匿名对象中)</param>
        public ISqlQueryable<T> OrderBy(string sql, params object[] args)
        {
            return _sqlString.OrderBy<T>(sql, args);
        }

        /// <summary>
        /// 封装 StringBuilder AppendFormat 追加非参数化SQL
        /// </summary>
        /// <param name="sql">SQL</param>
        /// <param name="args">参数</param>
        public ISqlQueryable<T> AppendFormat(string sql, params object[] args)
        {
            return _sqlString.AppendFormat<T>(sql, args);
        }

        public string ToSql()
        {
            return _sqlString.ToSql();
        }

        /// <summary>
        /// 创建 in 或 not in SQL
        /// </summary>
        public SqlValue ForList(IList list)
        {
            return _sqlString.ForList(list);
        }
        #endregion

    }
}
