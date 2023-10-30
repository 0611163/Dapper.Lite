using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;

namespace Dapper.Lite
{
    /// <summary>
    /// ClickHouse 数据库提供者基类
    /// </summary>
    public abstract class ClickHouseProviderBase : IProvider
    {
        #region OpenQuote 引号
        /// <summary>
        /// 引号
        /// </summary>
        public virtual string OpenQuote
        {
            get
            {
                return "\"";
            }
        }
        #endregion

        #region CloseQuote 引号
        /// <summary>
        /// 引号
        /// </summary>
        public virtual string CloseQuote
        {
            get
            {
                return "\"";
            }
        }
        #endregion

        #region 创建 DbConnection
        public virtual DbConnection CreateConnection(string connectionString)
        {
            return null;
        }
        #endregion

        #region 生成 DbParameter
        public virtual DbParameter GetDbParameter(string name, object value)
        {
            DbParameter parameter = null; // new ClickHouseDbParameter();
            parameter.ParameterName = name.Trim(new char[] { '{', '}' }).Split(':')[0];
            parameter.Value = value;
            DbType dbType = ColumnTypeUtil.GetDBType(value);
            parameter.DbType = dbType;
            return parameter;
        }
        #endregion

        #region GetParameterName
        public virtual string GetParameterName(string parameterName, Type parameterType)
        {
            return "{" + parameterName + ":" + ColumnTypeUtil.GetDBTypeName(parameterType) + "}";
        }
        #endregion

        #region 创建获取最大编号SQL
        public virtual string CreateGetMaxIdSql(string tableName, string key)
        {
            return string.Format("SELECT Max({0}) FROM {1}", key, tableName);
        }
        #endregion

        #region 创建分页SQL
        public virtual string CreatePageSql(string sql, string orderby, int pageSize, int currentPage)
        {
            StringBuilder sb = new StringBuilder();
            int startRow = 0;
            int endRow = 0;

            #region 分页查询语句
            startRow = pageSize * (currentPage - 1);

            sb.Append("select * from (");
            sb.Append(sql);
            if (!string.IsNullOrWhiteSpace(orderby))
            {
                sb.Append(" ");
                sb.Append(orderby);
            }
            sb.AppendFormat(" ) row_limit limit {0},{1}", startRow, pageSize);
            #endregion

            return sb.ToString();
        }
        #endregion

        #region 删除SQL语句模板
        /// <summary>
        /// 删除SQL语句模板 两个值分别对应 “delete from [表名] where [查询条件]”中的“delete from”和“where”
        /// </summary>
        public virtual Tuple<string, string> CreateDeleteSqlTempldate()
        {
            return new Tuple<string, string>("alter table", "delete where");
        }
        #endregion

        #region 更新SQL语句模板
        /// <summary>
        /// 更新SQL语句模板 三个值分别对应 “update [表名] set [赋值语句] where [查询条件]”中的“update”、“set”和“where”
        /// </summary>
        public virtual Tuple<string, string, string> CreateUpdateSqlTempldate()
        {
            return new Tuple<string, string, string>("alter table", "update", "where");
        }
        #endregion

        #region ForList

        public virtual SqlValue ForList(IList list)
        {
            string type = null;
            if (list.Count > 0)
            {
                object val = list[0];
                if (val != null)
                {
                    if (val.GetType() == typeof(string))
                    {
                        type = "String";
                    }
                    if (val.GetType() == typeof(int))
                    {
                        type = "Int32";
                    }
                    if (val.GetType() == typeof(int?))
                    {
                        type = "Int32";
                    }
                    if (val.GetType() == typeof(long))
                    {
                        type = "Int64";
                    }
                    if (val.GetType() == typeof(long?))
                    {
                        type = "Int64";
                    }
                }
                else
                {
                    throw new Exception("ForList集合项不能为null");
                }
            }
            else
            {
                throw new Exception("ForList集合不能为空");
            }

            List<string> argList = new List<string>();
            for (int i = 0; i < list.Count; i++)
            {
                argList.Add("{inParam" + i + ":" + type + "}");
            }
            string args = string.Join(",", argList);

            return new SqlValue("(" + args + ")", list);
        }

        #endregion

    }

    #region ColumnTypeUtil
    public class ColumnTypeUtil
    {
        public static DbType GetDBType(object value)
        {
            if (value != null)
            {
                Type type = value.GetType();
                if (type == typeof(DateTime) || type == typeof(DateTime?))
                {
                    return DbType.DateTime;
                }
                else if (type == typeof(string))
                {
                    return DbType.String;
                }
                else if (type == typeof(float) || type == typeof(float?))
                {
                    return DbType.Double;
                }
                else if (type == typeof(double) || type == typeof(double?))
                {
                    return DbType.Double;
                }
                else if (type == typeof(decimal) || type == typeof(decimal?))
                {
                    return DbType.Decimal;
                }
                else if (type == typeof(short) || type == typeof(short?))
                {
                    return DbType.Int16;
                }
                else if (type == typeof(int) || type == typeof(int?))
                {
                    return DbType.Int32;
                }
                else if (type == typeof(long) || type == typeof(long?))
                {
                    return DbType.Int64;
                }
            }
            return DbType.String;
        }

        public static string GetDBTypeName(Type parameterType)
        {
            if (parameterType == typeof(DateTime) || parameterType == typeof(DateTime?))
            {
                return "DateTime";
            }
            else if (parameterType == typeof(string))
            {
                return "String";
            }
            else if (parameterType == typeof(float) || parameterType == typeof(float?))
            {
                return "Float32";
            }
            else if (parameterType == typeof(double) || parameterType == typeof(double?))
            {
                return "Float64";
            }
            else if (parameterType == typeof(decimal) || parameterType == typeof(decimal?))
            {
                return "Float64";
            }
            else if (parameterType == typeof(short) || parameterType == typeof(short?))
            {
                return "Int16";
            }
            else if (parameterType == typeof(int) || parameterType == typeof(int?))
            {
                return "Int32";
            }
            else if (parameterType == typeof(long) || parameterType == typeof(long?))
            {
                return "Int64";
            }
            return "String";
        }

    }
    #endregion

}
