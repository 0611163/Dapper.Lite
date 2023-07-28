using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dapper.Lite
{
    /// <summary>
    /// MSSQL 数据库提供者基类
    /// </summary>
    public abstract class SQLiteProviderBase : IProvider
    {
        #region OpenQuote 引号
        /// <summary>
        /// 引号
        /// </summary>
        public virtual string OpenQuote
        {
            get
            {
                return "`";
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
                return "`";
            }
        }
        #endregion

        #region 创建 DbConnection
        public virtual DbConnection CreateConnection(string connectionString) { return null; }
        #endregion

        #region 生成 DbParameter
        public virtual DbParameter GetDbParameter(string name, object value) { return null; }
        #endregion

        #region GetParameterName
        public virtual string GetParameterName(string parameterName, Type parameterType)
        {
            return ":" + parameterName;
        }
        #endregion

        #region 创建获取最大编号SQL
        public virtual string CreateGetMaxIdSql(string tableName, string key)
        {
            return string.Format("SELECT Max(cast({0} as int)) FROM {1}", key, tableName);
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

            sb.Append(sql);
            if (!string.IsNullOrWhiteSpace(orderby))
            {
                sb.Append(" ");
                sb.Append(orderby);
            }
            sb.AppendFormat(" limit {0} offset {1}", pageSize, startRow);
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
            return new Tuple<string, string>("delete from", "where");
        }
        #endregion

        #region 更新SQL语句模板
        /// <summary>
        /// 更新SQL语句模板 三个值分别对应 “update [表名] set [赋值语句] where [查询条件]”中的“update”、“set”和“where”
        /// </summary>
        public virtual Tuple<string, string, string> CreateUpdateSqlTempldate()
        {
            return new Tuple<string, string, string>("update", "set", "where");
        }
        #endregion

        #region ForList
        public virtual SqlValue ForList(IList list)
        {
            List<string> argList = new List<string>();
            for (int i = 0; i < list.Count; i++)
            {
                argList.Add(":inParam" + i);
            }
            string args = string.Join(",", argList);

            return new SqlValue("(" + args + ")", list);
        }
        #endregion

    }
}
