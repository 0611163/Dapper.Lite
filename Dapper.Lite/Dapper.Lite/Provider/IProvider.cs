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
    /// 数据库实现接口
    /// </summary>
    public interface IProvider
    {
        /// <summary>
        /// 引号
        /// </summary>
        string OpenQuote { get; }

        /// <summary>
        /// 引号
        /// </summary>
        string CloseQuote { get; }

        /// <summary>
        /// 创建 DbConnection
        /// </summary>
        DbConnection CreateConnection(string connectionString);

        /// <summary>
        /// 生成 DbParameter
        /// </summary>
        DbParameter GetDbParameter(string name, object value);

        /// <summary>
        /// 带参数的SQL插入和修改语句中，参数化的字段名称
        /// </summary>
        string GetParameterName(string parameterName, Type parameterType);

        /// <summary>
        /// 创建获取最大编号SQL
        /// </summary>
        string CreateGetMaxIdSql(string tableName, string key);

        /// <summary>
        /// 创建分页SQL
        /// </summary>
        string CreatePageSql(string sql, string orderby, int pageSize, int currentPage);

        /// <summary>
        /// 删除SQL语句模板 两个值分别对应 “delete from [表名] where [查询条件]”中的“delete from”和“where”
        /// </summary>
        Tuple<string, string> CreateDeleteSqlTempldate();

        /// <summary>
        /// 更新SQL语句模板 三个值分别对应 “update [表名] set [赋值语句] where [查询条件]”中的“update”、“set”和“where”
        /// </summary>
        Tuple<string, string, string> CreateUpdateSqlTempldate();

        /// <summary>
        /// 创建 in 或 not in SQL
        /// </summary>
        SqlValue ForList(IList list);

    }
}
