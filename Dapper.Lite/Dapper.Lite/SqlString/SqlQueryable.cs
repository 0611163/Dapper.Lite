using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Dapper.Lite
{
    /// <summary>
    /// SQL字符串类
    /// </summary>
    public class SqlQueryable<T> : ISqlQueryable<T>
    {
        #region 变量
        private SqlString _sqlString;

        private IProvider _provider;

        private IDbSession _session;

        private DbSession _dbSession;

        private string _alias = "t";

        /// <summary>
        /// 参数化查询的SQL
        /// </summary>
        public string SQL
        {
            get
            {
                Append();
                return _sqlString.SQL;
            }
        }

        /// <summary>
        /// 参数化查询的参数
        /// </summary>
        public DbParameter[] Params
        {
            get
            {
                return _sqlString?.Params;
            }
        }

        /// <summary>
        /// 参数化查询的参数
        /// </summary>
        public DynamicParameters DynamicParameters => _sqlString.DynamicParameters;

        /// <summary>
        /// Queryable 信息
        /// </summary>
        private QueryableInfo queryableInfo = new QueryableInfo();

        /// <summary>
        /// where 信息集合
        /// </summary>
        private List<WhereInfo> whereList = new List<WhereInfo>();

        /// <summary>
        /// order by 信息集合
        /// </summary>
        private List<OrderByInfo> orderByList = new List<OrderByInfo>();
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

        #region Append 拼接
        private void Append()
        {
            if (_sqlString.Sql.Length == 0)
            {
                AppendQueryable();
                AppendWhere();
                AppendOrderBy();
            }
        }

        private void AppendQueryable()
        {
            _sqlString.Sql.AppendFormat(queryableInfo.Sql.ToString(), _alias);
        }

        private void AppendWhere()
        {
            foreach (WhereInfo whereInfo in whereList)
            {
                if (whereInfo.DbParameters != null && whereInfo.DbParameters.Length > 0)
                {
                    whereInfo.Sql = _sqlString.ParamsAddRange(whereInfo.DbParameters, whereInfo.Sql);
                }

                if (_sqlString.Sql.ToString().Contains(" where "))
                {
                    _sqlString.Sql.Append(" and " + whereInfo.Sql);
                }
                else
                {
                    _sqlString.Sql.Append(" where " + whereInfo.Sql);
                }
            }
        }

        private void AppendOrderBy()
        {
            foreach (OrderByInfo orderByInfo in orderByList)
            {
                if (!_sqlString.Sql.ToString().Contains(" order by "))
                {
                    _sqlString.Sql.AppendFormat(" order by {0} {1} ", orderByInfo.Sql, orderByInfo.sortType);
                }
                else
                {
                    _sqlString.Sql.AppendFormat(", {0} {1} ", orderByInfo.Sql, orderByInfo.sortType);
                }
            }
        }
        #endregion

        #region Queryable
        /// <summary>
        /// 创建单表查询SQL
        /// </summary>
        public ISqlQueryable<T> Queryable()
        {
            Type type = typeof(T);

            queryableInfo.Sql.Append("select ");

            PropertyInfoEx[] propertyInfoExArray = DbSession.GetEntityProperties(type);
            foreach (PropertyInfoEx propertyInfoEx in propertyInfoExArray)
            {
                if (propertyInfoEx.IsDBField)
                {
                    queryableInfo.Sql.AppendFormat(" {0}.{1}{2}{3},", "{0}", _provider.OpenQuote, propertyInfoEx.FieldName, _provider.CloseQuote);
                }
            }

            queryableInfo.Sql.Remove(queryableInfo.Sql.Length - 1, 1);

            queryableInfo.Sql.AppendFormat(" from {0} {1}", _dbSession.GetTableName(_provider, type), "{0}");

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
                string sql = condition.VisitLambda(expression, out dbParameters);

                _alias = condition.Alias;
                WhereInfo whereInfo = new WhereInfo() { DbParameters = dbParameters, Sql = sql };
                whereList.Add(whereInfo);
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
                ExpressionHelper<U> condition = new ExpressionHelper<U>(_provider, _sqlString.DbParameterNames, SqlStringMethod.Where);

                DbParameter[] dbParameters;
                string sql = condition.VisitLambda(expression, out dbParameters);

                _alias = condition.Alias;
                WhereInfo whereInfo = new WhereInfo() { DbParameters = dbParameters, Sql = sql };
                whereList.Add(whereInfo);
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

            _alias = condition.Alias;
            OrderByInfo orderByInfo = new OrderByInfo() { sortType = "asc", Sql = sql };
            orderByList.Add(orderByInfo);

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

            _alias = condition.Alias;
            OrderByInfo orderByInfo = new OrderByInfo() { sortType = "desc", Sql = sql };
            orderByList.Add(orderByInfo);

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
        /// 返回第一行的值，不存在则返回null
        /// </summary>
        public T First()
        {
            return _session.Query<T>(this.SQL, this.Params);
        }

        /// <summary>
        /// 返回第一行的值，不存在则返回null
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
        public bool Exists()
        {
            return _session.Exists(this.SQL, this.Params);
        }

        /// <summary>
        /// 返回数量
        /// </summary>
        public async Task<bool> ExistsAsync()
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

    #region QueryableInfo
    /// <summary>
    /// Queryable 信息
    /// </summary>
    internal class QueryableInfo
    {
        /// <summary>
        /// SQL
        /// </summary>
        public StringBuilder Sql { get; set; } = new StringBuilder();
    }
    #endregion

    #region WhereInfo
    /// <summary>
    /// Where 信息
    /// </summary>
    internal class WhereInfo
    {
        /// <summary>
        /// 参数
        /// </summary>
        public DbParameter[] DbParameters { get; set; }

        /// <summary>
        /// SQL
        /// </summary>
        public string Sql { get; set; }
    }
    #endregion

    #region OrderByInfo
    /// <summary>
    /// order by 信息
    /// </summary>
    internal class OrderByInfo
    {
        /// <summary>
        /// asc desc
        /// </summary>
        public string sortType { get; set; }

        /// <summary>
        /// SQL
        /// </summary>
        public string Sql { get; set; }
    }
    #endregion

}
