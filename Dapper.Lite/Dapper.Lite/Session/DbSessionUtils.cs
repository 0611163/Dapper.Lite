using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Dapper;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Dapper.Lite
{
    public partial class DbSession : IDbSession
    {
        private static object _lockGetEntityPropertiesDict = new object();

        private static ConcurrentDictionary<Type, object> _dictEntityPropertiesDict = new ConcurrentDictionary<Type, object>();

        #region 获取主键名称
        /// <summary>
        /// 获取主键名称
        /// </summary>
        internal static string GetIdName(Type type, out Type idType)
        {
            PropertyInfoEx[] propertyInfoList = GetEntityProperties(type);
            foreach (PropertyInfoEx propertyInfoEx in propertyInfoList)
            {
                if (propertyInfoEx.IsDBKey)
                {
                    idType = propertyInfoEx.PropertyInfo.PropertyType;
                    return propertyInfoEx.FieldName;
                }
            }
            idType = typeof(object);
            return "Id";
        }
        #endregion

        #region 获取实体类属性
        /// <summary>
        /// 获取实体类属性
        /// </summary>
        internal static PropertyInfoEx[] GetEntityProperties(Type type)
        {
            return PropertiesCache.TryGet<PropertyInfoEx[]>(type, modelType =>
            {
                List<PropertyInfoEx> result = new List<PropertyInfoEx>();
                PropertyInfo[] propertyInfoList = type.GetProperties();
                foreach (PropertyInfo propertyInfo in propertyInfoList)
                {
                    PropertyInfoEx propertyInfoEx = new PropertyInfoEx(propertyInfo);

                    NotMappedAttribute notMappedAttribute = propertyInfo.GetCustomAttribute<NotMappedAttribute>(false);
                    if (notMappedAttribute == null)
                    {
                        propertyInfoEx.IsDBField = true;
                    }

                    ColumnAttribute dbFieldAttribute = propertyInfo.GetCustomAttribute<ColumnAttribute>(false);
                    if (dbFieldAttribute != null)
                    {
                        if (!string.IsNullOrWhiteSpace(dbFieldAttribute.Name))
                        {
                            propertyInfoEx.FieldName = dbFieldAttribute.Name;
                        }
                        else
                        {
                            propertyInfoEx.FieldName = propertyInfo.Name;
                        }

                        propertyInfoEx.DBFieldAttribute = dbFieldAttribute;
                    }
                    else
                    {
                        propertyInfoEx.FieldName = propertyInfo.Name;
                    }

                    if (propertyInfo.GetCustomAttribute<KeyAttribute>() != null)
                    {
                        propertyInfoEx.IsDBKey = true;

                        AutoIncrementAttribute modelAutoIncrementAttribute = modelType.GetCustomAttribute<AutoIncrementAttribute>(false);
                        if (modelAutoIncrementAttribute != null)
                        {
                            propertyInfoEx.IsAutoIncrement = modelAutoIncrementAttribute.Value;
                        }
                    }

                    AutoIncrementAttribute propertyAutoIncrementAttribute = propertyInfo.GetCustomAttribute<AutoIncrementAttribute>(false);
                    if (propertyAutoIncrementAttribute != null)
                    {
                        propertyInfoEx.IsAutoIncrement = propertyAutoIncrementAttribute.Value;
                    }

                    ReadOnlyAttribute readOnlyAttribute = propertyInfo.GetCustomAttribute<ReadOnlyAttribute>(false);
                    propertyInfoEx.IsReadOnly = readOnlyAttribute != null;

                    result.Add(propertyInfoEx);
                }
                return result.ToArray();
            });
        }

        /// <summary>
        /// 获取实体类属性
        /// </summary>
        internal static Dictionary<string, PropertyInfoEx> GetEntityPropertiesDict(Type type)
        {
            lock (_lockGetEntityPropertiesDict)
            {
                if (_dictEntityPropertiesDict.TryGetValue(type, out object obj))
                {
                    return (Dictionary<string, PropertyInfoEx>)obj;
                }
                else
                {
                    var properties = GetEntityProperties(type);
                    var dict = properties.ToDictionary(t => t.PropertyInfo.Name);
                    _dictEntityPropertiesDict.TryAdd(type, dict);
                    return dict;
                }
            }
        }
        #endregion

        #region 创建主键查询条件
        /// <summary>
        /// 创建主键查询条件
        /// </summary>
        private static string CreatePkCondition(IProvider provider, Type type, object val, int index, out DbParameter[] cmdParams)
        {
            StringBuilder sql = new StringBuilder();

            List<DbParameter> paramList = new List<DbParameter>();
            PropertyInfoEx[] propertyInfoList = GetEntityProperties(type);
            int i = 0;
            foreach (PropertyInfoEx propertyInfoEx in propertyInfoList)
            {
                PropertyInfo propertyInfo = propertyInfoEx.PropertyInfo;

                if (propertyInfoEx.IsDBKey)
                {
                    if (i != 0) sql.Append(" and ");
                    object fieldValue = propertyInfo.GetValue(val, null);
                    DbParameter parameter = provider.GetDbParameter(provider.GetParameterName("Param" + index + propertyInfoEx.FieldName, propertyInfoEx.PropertyInfo.PropertyType), fieldValue);
                    paramList.Add(parameter);
                    sql.AppendFormat(" {0}={1}", provider.OpenQuote + propertyInfoEx.FieldName + provider.CloseQuote, provider.GetParameterName("Param" + index + propertyInfoEx.FieldName, propertyInfoEx.PropertyInfo.PropertyType));
                    i++;
                }
            }

            cmdParams = paramList.ToArray();
            return sql.ToString();
        }
        #endregion

        #region 判断是否是自增的主键
        /// <summary>
        /// 判断是否是自增的主键
        /// </summary>
        private static bool IsAutoIncrementPk(PropertyInfoEx propertyInfoEx)
        {
            if (propertyInfoEx.IsAutoIncrement != null)
            {
                return propertyInfoEx.IsAutoIncrement.Value;
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region 获取数据库表名
        /// <summary>
        /// 获取数据库表名
        /// </summary>
        public string GetTableName(IProvider provider, Type type)
        {
            if (_splitTableMapping != null && _splitTableMapping.Exists(type))
            {
                string tableName = _splitTableMapping.GetTableName(type);
                string schema = _splitTableMapping.GetSchema(type);
                if (schema == null)
                {
                    return $"{provider.OpenQuote}{tableName}{provider.CloseQuote}";
                }
                else
                {
                    return $"{provider.OpenQuote}{schema}{provider.CloseQuote}.{provider.OpenQuote}{tableName}{provider.CloseQuote}";
                }
            }
            else
            {
                TableAttribute dbTableAttribute = type.GetCustomAttribute<TableAttribute>();
                if (dbTableAttribute != null && !string.IsNullOrWhiteSpace(dbTableAttribute.Name))
                {
                    if (dbTableAttribute.Schema == null)
                    {
                        return $"{provider.OpenQuote}{dbTableAttribute.Name}{provider.CloseQuote}";
                    }
                    else
                    {
                        return $"{provider.OpenQuote}{dbTableAttribute.Schema}{provider.CloseQuote}.{provider.OpenQuote}{dbTableAttribute.Name}{provider.CloseQuote}";
                    }
                }
                else
                {
                    if (dbTableAttribute.Schema == null)
                    {
                        return $"{provider.OpenQuote}{type.Name}{provider.CloseQuote}";
                    }
                    else
                    {
                        return $"{provider.OpenQuote}{dbTableAttribute.Schema}{provider.CloseQuote}.{provider.OpenQuote}{type.Name}{provider.CloseQuote}";
                    }
                }
            }
        }
        #endregion

        #region SqlFilter 过滤SQL防注入
        /// <summary>
        /// 过滤SQL防注入
        /// </summary>
        private static void SqlFilter(ref string sql)
        {
            sql = sql.Trim();
            string ignore = string.Empty;
            foreach (string keyword in _sqlFilteRegexDict.Keys)
            {
                if (sql.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    ignore = keyword;
                }
            }
            foreach (string keyword in _sqlFilteRegexDict.Keys)
            {
                if (ignore == "select " && ignore == keyword) continue;
                Regex regex = _sqlFilteRegexDict[keyword];
                sql = sql.Substring(0, ignore.Length) + regex.Replace(sql.Substring(ignore.Length), string.Empty);
            }
        }
        #endregion

        #region DbParameter集合转DynamicParameters
        /// <summary>
        /// DbParameter集合转DynamicParameters
        /// </summary>
        internal DynamicParameters ToDynamicParameters(DbParameter[] parameters)
        {
            if (parameters != null)
            {
                DynamicParameters dynamicParameters = new DynamicParameters();
                foreach (DbParameter param in parameters)
                {
                    dynamicParameters.Add(param.ParameterName, param.Value);
                }
                return dynamicParameters;
            }
            else
            {
                return null;
            }
        }
        #endregion

        #region 设置 数据库字段名与实体类属性名映射
        /// <summary>
        /// 设置 数据库字段名与实体类属性名映射
        /// </summary>
        public void SetTypeMap(Type type)
        {
            lock (_lockSetTypeMap)
            {
                if (!_dictForTypeMap.ContainsKey(type))
                {
                    var map = new CustomPropertyTypeMap(type, (modelType, columnName) =>
                    {
                        PropertyInfoEx[] props = DbSession.GetEntityProperties(modelType);

                        PropertyInfoEx propEx = props.FirstOrDefault(prop =>
                        {
                            if (string.Compare(prop.FieldName, columnName, true) == 0)
                            {
                                return true;
                            }

                            return false;
                        });

                        if (propEx != null)
                        {
                            return propEx.PropertyInfo;
                        }
                        else
                        {
                            return null;
                        }
                    });

                    if (map != null)
                    {
                        SqlMapper.SetTypeMap(type, map);
                    }

                    _dictForTypeMap.TryAdd(type, true);
                }
            }
        }

        /// <summary>
        /// 设置 数据库字段名与实体类属性名映射
        /// </summary>
        public void SetTypeMap<T>()
        {
            Type type = typeof(T);
            SetTypeMap(type);
        }
        #endregion

    }
}
