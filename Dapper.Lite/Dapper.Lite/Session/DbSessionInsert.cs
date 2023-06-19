using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace Dapper.Lite
{
    public partial class DbSession : IDbSession
    {
        #region Insert 添加
        /// <summary>
        /// 添加
        /// </summary>
        public void Insert(object obj)
        {
            StringBuilder strSql = new StringBuilder();
            DbParameter[] parameters = null;

            PrepareInsertSql(obj, ref strSql, ref parameters);

            OnExecuting?.Invoke(strSql.ToString(), parameters);

            var conn = GetConnection(_tran);

            try
            {
                conn.Execute(strSql.ToString(), ToDynamicParameters(parameters), _tran);
            }
            finally
            {
                if (_tran == null)
                {
                    if (conn.State != ConnectionState.Closed) conn.Close();
                }
            }
        }

        /// <summary>
        /// 添加并返回ID
        /// </summary>
        public long InsertReturnId(object obj, string selectIdSql)
        {
            StringBuilder strSql = new StringBuilder();
            DbParameter[] parameters = null;

            PrepareInsertSql(obj, ref strSql, ref parameters);
            strSql.Append(";" + selectIdSql + ";");

            OnExecuting?.Invoke(strSql.ToString(), parameters);

            var conn = GetConnection(_tran);

            try
            {
                object id = conn.ExecuteScalar(strSql.ToString(), ToDynamicParameters(parameters), _tran);
                return Convert.ToInt64(id);
            }
            finally
            {
                if (_tran == null)
                {
                    if (conn.State != ConnectionState.Closed) conn.Close();
                }
            }
        }
        #endregion

        #region InsertAsync 添加
        /// <summary>
        /// 添加
        /// </summary>
        public async Task InsertAsync(object obj)
        {
            StringBuilder strSql = new StringBuilder();
            DbParameter[] parameters = null;

            PrepareInsertSql(obj, ref strSql, ref parameters);

            OnExecuting?.Invoke(strSql.ToString(), parameters);

            var conn = GetConnection(_tran);

            try
            {
                await conn.ExecuteAsync(strSql.ToString(), ToDynamicParameters(parameters), _tran);
            }
            finally
            {
                if (_tran == null)
                {
                    if (conn.State != ConnectionState.Closed) conn.Close();
                }
            }
        }

        /// <summary>
        /// 添加并返回ID
        /// </summary>
        public async Task<long> InsertReturnIdAsync(object obj, string selectIdSql)
        {
            StringBuilder strSql = new StringBuilder();
            DbParameter[] parameters = null;

            PrepareInsertSql(obj, ref strSql, ref parameters);
            strSql.Append(";" + selectIdSql + ";");

            OnExecuting?.Invoke(strSql.ToString(), parameters);

            var conn = GetConnection(_tran);

            try
            {
                object id = await conn.ExecuteScalarAsync(strSql.ToString(), ToDynamicParameters(parameters), _tran);
                return Convert.ToInt64(id);
            }
            finally
            {
                if (_tran == null)
                {
                    if (conn.State != ConnectionState.Closed) conn.Close();
                }
            }
        }
        #endregion

        #region Insert<T> 批量添加
        /// <summary>
        /// 批量添加
        /// </summary>
        public void Insert<T>(List<T> list)
        {
            Insert<T>(list, 500);
        }

        /// <summary>
        /// 批量添加
        /// </summary>
        public void Insert<T>(List<T> list, int pageSize)
        {
            for (int i = 0; i < list.Count; i += pageSize)
            {
                StringBuilder strSql = new StringBuilder();
                int savedCount = 0;
                DbParameter[] parameters = null;

                var listPage = list.Skip(i).Take(pageSize).ToList();

                PrepareInsertSql<T>(listPage, ref strSql, ref parameters, ref savedCount);

                OnExecuting?.Invoke(strSql.ToString(), parameters);

                var conn = GetConnection(_tran);

                try
                {
                    conn.Execute(strSql.ToString(), ToDynamicParameters(parameters), _tran);
                }
                finally
                {
                    if (_tran == null)
                    {
                        if (conn.State != ConnectionState.Closed) conn.Close();
                    }
                }
            }
        }
        #endregion

        #region InsertAsync<T> 批量添加
        /// <summary>
        /// 批量添加
        /// </summary>
        public Task InsertAsync<T>(List<T> list)
        {
            return InsertAsync<T>(list, 500);
        }

        /// <summary>
        /// 批量添加
        /// </summary>
        public async Task InsertAsync<T>(List<T> list, int pageSize)
        {
            for (int i = 0; i < list.Count; i += pageSize)
            {
                StringBuilder strSql = new StringBuilder();
                int savedCount = 0;
                DbParameter[] parameters = null;

                var listPage = list.Skip(i).Take(pageSize).ToList();

                PrepareInsertSql<T>(listPage, ref strSql, ref parameters, ref savedCount);

                OnExecuting?.Invoke(strSql.ToString(), parameters);

                var conn = GetConnection(_tran);

                try
                {
                    await conn.ExecuteAsync(strSql.ToString(), ToDynamicParameters(parameters), _tran);
                }
                finally
                {
                    if (_tran == null)
                    {
                        if (conn.State != ConnectionState.Closed) conn.Close();
                    }
                }
            }
        }
        #endregion

        #region PrepareInsertSql 准备Insert的SQL
        /// <summary>
        /// 准备Insert的SQL
        /// </summary>
        private void PrepareInsertSql(object obj, ref StringBuilder strSql, ref DbParameter[] parameters)
        {
            Type type = obj.GetType();
            SetTypeMap(type);
            strSql.Append(string.Format("insert into {0}(", GetTableName(_provider, type)));
            PropertyInfoEx[] propertyInfoList = GetEntityProperties(type);
            List<Tuple<string, Type>> propertyNameList = new List<Tuple<string, Type>>();
            List<DbParameter> parameterList = new List<DbParameter>();
            foreach (PropertyInfoEx propertyInfoEx in propertyInfoList)
            {
                PropertyInfo propertyInfo = propertyInfoEx.PropertyInfo;

                if (IsAutoIncrementPk(propertyInfoEx)) continue;

                if (propertyInfoEx.IsDBField)
                {
                    propertyNameList.Add(new Tuple<string, Type>(propertyInfoEx.FieldName, propertyInfoEx.PropertyInfo.PropertyType));

                    object val = propertyInfo.GetValue(obj, null);
                    Type parameterType = val == null ? typeof(object) : val.GetType();
                    DbParameter param = _provider.GetDbParameter(_provider.GetParameterName(propertyInfoEx.FieldName, parameterType), val);
                    parameterList.Add(param);
                }
            }
            parameters = parameterList.ToArray();

            strSql.Append(string.Format("{0})", string.Join(",", propertyNameList.Select(a => string.Format("{0}{1}{2}", _provider.OpenQuote, a.Item1, _provider.CloseQuote)))));
            strSql.Append(string.Format(" values ({0})", string.Join(",", propertyNameList.Select(a => _provider.GetParameterName(a.Item1, a.Item2)))));
        }
        #endregion

        #region PrepareInsertSql 准备批量Insert的SQL
        /// <summary>
        /// 准备批量Insert的SQL
        /// </summary>
        private void PrepareInsertSql<T>(List<T> list, ref StringBuilder strSql, ref DbParameter[] parameters, ref int savedCount)
        {
            Type type = typeof(T);
            SetTypeMap(type);
            strSql.Append(string.Format("insert into {0}(", GetTableName(_provider, type)));
            PropertyInfoEx[] propertyInfoList = GetEntityProperties(type);
            List<Tuple<string, Type>> propertyNameList = new List<Tuple<string, Type>>();
            foreach (PropertyInfoEx propertyInfoEx in propertyInfoList)
            {
                PropertyInfo propertyInfo = propertyInfoEx.PropertyInfo;

                if (IsAutoIncrementPk(propertyInfoEx)) continue;

                if (propertyInfoEx.IsDBField)
                {
                    propertyNameList.Add(new Tuple<string, Type>(propertyInfoEx.FieldName, propertyInfoEx.PropertyInfo.PropertyType));
                    savedCount++;
                }
            }

            strSql.Append(string.Format("{0}) values ", string.Join(",", propertyNameList.Select(a => _provider.OpenQuote + a.Item1 + _provider.CloseQuote))));
            for (int i = 0; i < list.Count; i++)
            {
                strSql.Append(string.Format(" ({0})", string.Join(",", propertyNameList.Select(a => _provider.GetParameterName(a.Item1 + i, a.Item2)))));
                if (i != list.Count - 1)
                {
                    strSql.Append(", ");
                }
            }

            parameters = new DbParameter[savedCount * list.Count];
            int k = 0;
            for (int n = 0; n < list.Count; n++)
            {
                T obj = list[n];
                for (int i = 0; i < propertyInfoList.Length && savedCount > 0; i++)
                {
                    PropertyInfoEx propertyInfoEx = propertyInfoList[i];
                    PropertyInfo propertyInfo = propertyInfoEx.PropertyInfo;

                    if (IsAutoIncrementPk(propertyInfoEx)) continue;

                    if (propertyInfoEx.IsDBField)
                    {
                        object val = propertyInfo.GetValue(obj, null);
                        Type parameterType = val == null ? typeof(object) : val.GetType();
                        DbParameter param = _provider.GetDbParameter(_provider.GetParameterName(propertyInfoEx.FieldName + n, parameterType), val);
                        parameters[k++] = param;
                    }
                }
            }
        }
        #endregion

    }
}
