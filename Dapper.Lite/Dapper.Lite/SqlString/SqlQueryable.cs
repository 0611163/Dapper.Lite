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

        private string _alias = "t";

        /// <summary>
        /// 参数化查询的SQL
        /// </summary>
        public string SQL => _sqlString?.SQL;

        /// <summary>
        /// 参数化查询的参数
        /// </summary>
        public DbParameter[] Params => _sqlString?.Params;

        /// <summary>
        /// 参数化查询的参数
        /// </summary>
        public DynamicParameters DynamicParameters => _sqlString.DynamicParameters;
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
        public ISqlQueryable<T> Queryable()
        {
            Type type = typeof(T);

            _sqlString.Sql.Append("select ");

            PropertyInfoEx[] propertyInfoExArray = DbSession.GetEntityProperties(type);
            foreach (PropertyInfoEx propertyInfoEx in propertyInfoExArray)
            {
                if (propertyInfoEx.IsDBField)
                {
                    _sqlString.Sql.AppendFormat(" {0}.{1}{2}{3},", _alias, _provider.OpenQuote, propertyInfoEx.FieldName, _provider.CloseQuote);
                }
            }

            _sqlString.Sql.Remove(_sqlString.Sql.Length - 1, 1);

            _sqlString.Sql.AppendFormat(" from {0} {1}", _dbSession.GetTableName(_provider, type), _alias);

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
                string result = condition.VisitLambda(expression, out dbParameters);

                if (dbParameters != null)
                {
                    result = _sqlString.ParamsAddRange(dbParameters, result);
                }

                if (_sqlString.Sql.ToString().Contains(" where "))
                {
                    _sqlString.Sql.Append(" and " + result);
                }
                else
                {
                    _sqlString.Sql.Append(" where " + result);
                }

                ReplaceAlias(condition.Alias);
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
                string result = condition.VisitLambda(expression, out dbParameters);

                if (dbParameters != null)
                {
                    result = _sqlString.ParamsAddRange(dbParameters, result);
                }

                if (_sqlString.Sql.ToString().Contains(" where "))
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

            ReplaceAlias(condition.Alias);

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

            ReplaceAlias(condition.Alias);

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

        #region 替换alias
        private void ReplaceAlias(string newAlias)
        {
            if (newAlias == null) return;
            if (_alias == newAlias) return;

            Regex regex1 = new Regex("[\\s]{1}" + _alias + "[\\s]{1}");
            Regex regex2 = new Regex("[\\s]{1}" + _alias + "$");

            string oldSql = _sqlString.SQL;
            if (regex1.IsMatch(oldSql))
            {
                _sqlString.Sql.Clear();
                _sqlString.Sql.Append(regex1.Replace(oldSql, $" {newAlias} "));
            }
            else if (regex2.IsMatch(oldSql))
            {
                _sqlString.Sql.Clear();
                _sqlString.Sql.Append(regex2.Replace(oldSql, $" {newAlias}"));
            }

            Regex regex3 = new Regex("[\\s]{1}" + _alias + "\\.");
            Regex regex4 = new Regex("[,]{1}" + _alias + "\\.");
            Regex regex5 = new Regex("[(]{1}" + _alias + "\\.");

            oldSql = _sqlString.SQL;
            if (regex3.IsMatch(oldSql))
            {
                _sqlString.Sql.Clear();
                _sqlString.Sql.Append(regex3.Replace(oldSql, $" {newAlias}."));
            }
            else if (regex4.IsMatch(oldSql))
            {
                _sqlString.Sql.Clear();
                _sqlString.Sql.Append(regex4.Replace(oldSql, $",{newAlias}."));
            }
            else if (regex5.IsMatch(oldSql))
            {
                _sqlString.Sql.Clear();
                _sqlString.Sql.Append(regex5.Replace(oldSql, $"({newAlias}."));
            }

            _alias = newAlias;
        }
        #endregion

    }
}
