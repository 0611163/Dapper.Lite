using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Linq.Expressions;

namespace Dapper.Lite
{
    public partial class DbSession : IDbSession
    {
        #region 变量
        /// <summary>
        /// 新旧数据集合 key:新数据 value:旧数据
        /// </summary>
        private readonly ConcurrentDictionary<object, object> _oldObjs = new ConcurrentDictionary<object, object>();

        /// <summary>
        /// 只更新这些字段
        /// </summary>
        private readonly ConcurrentDictionary<Type, HashSet<string>> _updateColumns = new ConcurrentDictionary<Type, HashSet<string>>();

        /// <summary>
        /// 不更新这些字段
        /// </summary>
        private readonly ConcurrentDictionary<Type, HashSet<string>> _ignoreColumns = new ConcurrentDictionary<Type, HashSet<string>>();
        #endregion

        #region AttachOld 附加更新前的旧实体
        /// <summary>
        /// 附加更新前的旧实体，只更新数据发生变化的字段
        /// </summary>
        public void AttachOld<T>(T obj)
        {
            if (_oldObjs.ContainsKey(obj))
            {
                _oldObjs.TryRemove(obj, out object _);
            }

            if (!_oldObjs.ContainsKey(obj))
            {
                object cloneObj = ModelMapper<T>.Map(obj);
                _oldObjs.TryAdd(obj, cloneObj);
            }
        }

        /// <summary>
        /// 附加更新前的旧实体，只更新数据发生变化的字段
        /// </summary>
        public void AttachOld<T>(List<T> objList)
        {
            foreach (T obj in objList)
            {
                AttachOld(obj);
            }
        }
        #endregion

        #region UpdateColumns 设置只更新某些字段
        /// <summary>
        /// 设置只更新某些字段
        /// </summary>
        public IDbSession UpdateColumns<T>(Expression<Func<T, object>> expression)
        {
            Type type = expression.Body.Type;
            PropertyInfo[] props = type.GetProperties();
            HashSet<string> updateColumnsSet = new HashSet<string>();
            _updateColumns.TryAdd(typeof(T), updateColumnsSet);
            foreach (PropertyInfo propertyInfo in props)
            {
                updateColumnsSet.Add(propertyInfo.Name);
            }

            return this;
        }
        #endregion

        #region IgnoreColumns 设置不更新某些字段
        /// <summary>
        /// 设置不更新某些字段
        /// </summary>
        public IDbSession IgnoreColumns<T>(Expression<Func<T, object>> expression)
        {
            Type type = expression.Body.Type;
            PropertyInfo[] props = type.GetProperties();
            HashSet<string> ignoreColumnsSet = new HashSet<string>();
            _ignoreColumns.TryAdd(typeof(T), ignoreColumnsSet);
            foreach (PropertyInfo propertyInfo in props)
            {
                ignoreColumnsSet.Add(propertyInfo.Name);
            }

            return this;
        }
        #endregion


        #region Update 修改
        /// <summary>
        /// 修改
        /// </summary>
        public void Update(object obj)
        {
            //object oldObj = Find(obj);
            object oldObj;
            _oldObjs.TryGetValue(obj, out oldObj);

            StringBuilder strSql = new StringBuilder();
            int savedCount = 0;
            DbParameter[] parameters = null;
            PrepareUpdateSql(obj, oldObj, ref strSql, ref parameters, ref savedCount);

            if (savedCount > 0)
            {
                Execute(strSql.ToString(), parameters);
            }
        }
        #endregion

        #region UpdateAsync 修改
        /// <summary>
        /// 修改
        /// </summary>
        public async Task UpdateAsync(object obj)
        {
            //object oldObj = await FindAsync(obj);
            object oldObj;
            _oldObjs.TryGetValue(obj, out oldObj);

            StringBuilder strSql = new StringBuilder();
            int savedCount = 0;
            DbParameter[] parameters = null;
            PrepareUpdateSql(obj, oldObj, ref strSql, ref parameters, ref savedCount);

            if (savedCount > 0)
            {
                await ExecuteAsync(strSql.ToString(), parameters);
            }
        }
        #endregion

        #region Update<T> 批量修改
        /// <summary>
        /// 批量修改
        /// </summary>
        public void Update<T>(List<T> list)
        {
            Update<T>(list, 500);
        }

        /// <summary>
        /// 批量修改
        /// </summary>
        public void Update<T>(List<T> list, int pageSize)
        {
            for (int i = 0; i < list.Count; i += pageSize)
            {
                List<T> subList = list.Skip(i).Take(pageSize).ToList();

                List<T> newList = new List<T>();
                List<T> oldList = new List<T>();
                for (int k = 0; k < subList.Count; k++)
                {
                    T obj = subList[k];
                    //object oldObj = Find(obj);
                    object oldObj;
                    _oldObjs.TryGetValue(obj, out oldObj);

                    newList.Add(obj);
                    oldList.Add((T)oldObj);
                }

                StringBuilder strSql = new StringBuilder();
                int savedCount = 0;
                DbParameter[] parameters = null;

                PrepareUpdateSql<T>(newList, oldList, ref strSql, ref parameters, ref savedCount);

                if (savedCount > 0)
                {
                    Execute(strSql.ToString(), parameters);
                }
            }
        }
        #endregion

        #region UpdateAsync<T> 批量修改
        /// <summary>
        /// 批量修改
        /// </summary>
        public Task UpdateAsync<T>(List<T> list)
        {
            return UpdateAsync<T>(list, 500);
        }

        /// <summary>
        /// 批量修改
        /// </summary>
        public async Task UpdateAsync<T>(List<T> list, int pageSize)
        {
            for (int i = 0; i < list.Count; i += pageSize)
            {
                List<T> subList = list.Skip(i).Take(pageSize).ToList();

                List<T> newList = new List<T>();
                List<T> oldList = new List<T>();
                for (int k = 0; k < subList.Count; k++)
                {
                    T obj = subList[k];
                    //object oldObj = Find(obj);
                    object oldObj;
                    _oldObjs.TryGetValue(obj, out oldObj);

                    newList.Add(obj);
                    oldList.Add((T)oldObj);
                }

                StringBuilder strSql = new StringBuilder();
                int savedCount = 0;
                DbParameter[] parameters = null;

                PrepareUpdateSql<T>(newList, oldList, ref strSql, ref parameters, ref savedCount);

                if (savedCount > 0)
                {
                    await ExecuteAsync(strSql.ToString(), parameters);
                }
            }
        }
        #endregion

        #region PrepareUpdateSql 准备Update的SQL
        /// <summary>
        /// 准备Update的SQL
        /// </summary>
        private void PrepareUpdateSql(object obj, object oldObj, ref StringBuilder strSql, ref DbParameter[] parameters, ref int savedCount)
        {
            Type type = obj.GetType();
            SetTypeMap(type);

            List<DbParameter> paramList = new List<DbParameter>();
            PropertyInfoEx[] propertyInfoList = GetEntityProperties(type);
            StringBuilder sbPros = new StringBuilder();
            _updateColumns.TryGetValue(type, out HashSet<string> updateColumnsSet);
            _ignoreColumns.TryGetValue(type, out HashSet<string> ignoreColumnsSet);
            foreach (PropertyInfoEx propertyInfoEx in propertyInfoList)
            {
                PropertyInfo propertyInfo = propertyInfoEx.PropertyInfo;

                if (updateColumnsSet != null && updateColumnsSet.Count > 0 && !updateColumnsSet.Contains(propertyInfo.Name)) continue;
                if (ignoreColumnsSet != null && ignoreColumnsSet.Count > 0 && ignoreColumnsSet.Contains(propertyInfo.Name)) continue;

                if (propertyInfoEx.IsReadOnly) continue;

                if (propertyInfoEx.IsDBField && !propertyInfoEx.IsDBKey)
                {
                    object oldVal = oldObj == null ? null : propertyInfo.GetValue(oldObj, null);
                    object val = propertyInfo.GetValue(obj, null);
                    if (oldObj == null || !object.Equals(oldVal, val))
                    {
                        sbPros.Append(string.Format(" {0}={1},", string.Format("{0}{1}{2}", _provider.OpenQuote, propertyInfoEx.FieldName, _provider.CloseQuote), _provider.GetParameterName(propertyInfoEx.FieldName, propertyInfoEx.PropertyInfo.PropertyType)));
                        DbParameter param = _provider.GetDbParameter(_provider.GetParameterName(propertyInfoEx.FieldName, propertyInfoEx.PropertyInfo.PropertyType), val);
                        paramList.Add(param);
                        savedCount++;
                    }
                }
            }

            Tuple<string, string, string> updateTmpl = _provider.CreateUpdateSqlTempldate();
            strSql.Append(string.Format(updateTmpl.Item1 + " {0} ", GetTableName(_provider, type)));
            strSql.Append(string.Format(updateTmpl.Item2 + " "));
            if (sbPros.Length > 0)
            {
                strSql.Append(sbPros.ToString(0, sbPros.Length - 1));
            }
            strSql.Append(string.Format(" " + updateTmpl.Item3 + " {0}", CreatePkCondition(_provider, obj.GetType(), obj, 0, out DbParameter[] cmdParams)));
            paramList.AddRange(cmdParams);
            parameters = paramList.ToArray();
        }
        #endregion

        #region PrepareUpdateSql 准备批量Update的SQL
        /// <summary>
        /// 准备批量Update的SQL
        /// </summary>
        private void PrepareUpdateSql<T>(List<T> list, List<T> oldList, ref StringBuilder strSql, ref DbParameter[] parameters, ref int savedCount)
        {
            Type type = typeof(T);
            SetTypeMap(type);

            Tuple<string, string, string> updateTmpl = _provider.CreateUpdateSqlTempldate();
            List<DbParameter> paramList = new List<DbParameter>();
            _updateColumns.TryGetValue(type, out HashSet<string> updateColumnsSet);
            _ignoreColumns.TryGetValue(type, out HashSet<string> ignoreColumnsSet);
            PropertyInfoEx[] propertyInfoList = GetEntityProperties(type);
            for (int n = 0; n < list.Count; n++)
            {
                T obj = list[n];
                T oldObj = oldList[n];

                int subSavedCount = 0;

                StringBuilder sbPros = new StringBuilder();
                foreach (PropertyInfoEx propertyInfoEx in propertyInfoList)
                {
                    PropertyInfo propertyInfo = propertyInfoEx.PropertyInfo;

                    if (updateColumnsSet != null && updateColumnsSet.Count > 0 && !updateColumnsSet.Contains(propertyInfo.Name)) continue;
                    if (ignoreColumnsSet != null && ignoreColumnsSet.Count > 0 && ignoreColumnsSet.Contains(propertyInfo.Name)) continue;

                    if (propertyInfoEx.IsReadOnly) continue;

                    if (propertyInfoEx.IsDBField && !propertyInfoEx.IsDBKey)
                    {
                        object oldVal = oldObj == null ? null : propertyInfo.GetValue(oldObj, null);
                        object val = propertyInfo.GetValue(obj, null);
                        if (oldObj == null || !object.Equals(oldVal, val))
                        {
                            sbPros.Append(string.Format(" {0}={1},", string.Format("{0}{1}{2}", _provider.OpenQuote, propertyInfoEx.FieldName, _provider.CloseQuote), _provider.GetParameterName(propertyInfoEx.FieldName + n, propertyInfoEx.PropertyInfo.PropertyType)));
                            DbParameter param = _provider.GetDbParameter(_provider.GetParameterName(propertyInfoEx.FieldName + n, propertyInfoEx.PropertyInfo.PropertyType), val);
                            paramList.Add(param);
                            subSavedCount++;
                        }
                    }
                }

                if (subSavedCount > 0)
                {
                    savedCount++;
                    strSql.Append(string.Format(updateTmpl.Item1 + " {0} ", GetTableName(_provider, type)));
                    strSql.Append(string.Format(updateTmpl.Item2 + " "));
                    if (sbPros.Length > 0)
                    {
                        strSql.Append(sbPros.ToString(0, sbPros.Length - 1));
                    }
                    strSql.Append(string.Format(" " + updateTmpl.Item3 + " {0}; ", CreatePkCondition(_provider, obj.GetType(), obj, n, out DbParameter[] cmdParams)));
                    paramList.AddRange(cmdParams);
                    parameters = paramList.ToArray();
                }
            }
        }
        #endregion

    }
}
