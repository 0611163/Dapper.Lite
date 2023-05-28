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

                    ColumnAttribute dbFieldAttribute = propertyInfo.GetCustomAttribute<ColumnAttribute>();
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

                        propertyInfoEx.IsDBField = true;
                    }
                    else
                    {
                        propertyInfoEx.FieldName = propertyInfo.Name;
                    }

                    if (propertyInfo.GetCustomAttribute<KeyAttribute>() != null)
                    {
                        propertyInfoEx.IsDBKey = true;

                        AutoIncrementAttribute modelAutoIncrementAttribute = modelType.GetCustomAttribute<AutoIncrementAttribute>();
                        if (modelAutoIncrementAttribute != null)
                        {
                            propertyInfoEx.IsAutoIncrement = modelAutoIncrementAttribute.Value;
                        }
                        else
                        {
                            AutoIncrementAttribute propertyAutoIncrementAttribute = propertyInfo.GetCustomAttribute<AutoIncrementAttribute>();
                            if (propertyAutoIncrementAttribute != null)
                            {
                                propertyInfoEx.IsAutoIncrement = propertyAutoIncrementAttribute.Value;
                            }
                        }
                    }

                    propertyInfoEx.FieldNameUpper = propertyInfoEx.FieldName.ToUpper();
                    result.Add(propertyInfoEx);
                }
                return result.ToArray();
            });
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
        private static bool IsAutoIncrementPk(Type modelType, PropertyInfoEx propertyInfoEx, bool autoIncrement)
        {
            if (propertyInfoEx.IsDBKey)
            {
                if (propertyInfoEx.IsAutoIncrement != null)
                {
                    return propertyInfoEx.IsAutoIncrement.Value;
                }
                else
                {
                    return autoIncrement;
                }
            }
            return false;
        }
        #endregion

        #region 获取数据库表名
        /// <summary>
        /// 获取数据库表名
        /// </summary>
        public string GetTableName(IProvider provider, Type type)
        {
            if (_splitTableMapping == null)
            {
                TableAttribute dbTableAttribute = type.GetCustomAttribute<TableAttribute>();
                if (dbTableAttribute != null && !string.IsNullOrWhiteSpace(dbTableAttribute.Name))
                {
                    return provider.OpenQuote + dbTableAttribute.Name + provider.CloseQuote;
                }
                else
                {
                    return provider.OpenQuote + type.Name + provider.CloseQuote;
                }
            }
            else
            {
                string tableName = _splitTableMapping.GetTableName(type);
                if (tableName == null)
                {
                    throw new Exception("缺少分表映射");
                }
                return provider.OpenQuote + tableName + provider.CloseQuote;
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
            string upperSql = sql.ToUpper();
            foreach (string keyword in _sqlFilteRegexDict.Keys)
            {
                if (upperSql.IndexOf(keyword.ToUpper()) == 0)
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
            _dictForTypeMap.GetOrAdd(type, key =>
            {
                var map = new CustomPropertyTypeMap(key, (modelType, columnName) =>
                {
                    PropertyInfoEx[] props = DbSession.GetEntityProperties(modelType);

                    PropertyInfoEx propEx = props.FirstOrDefault(prop =>
                    {
                        if (prop.FieldNameUpper == columnName.ToUpper())
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
                    SqlMapper.SetTypeMap(key, map);
                }

                return true;
            });
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
