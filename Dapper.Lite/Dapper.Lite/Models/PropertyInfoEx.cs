using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Dapper.Lite
{
    /// <summary>
    /// 类的属性信息扩展
    /// </summary>
    internal class PropertyInfoEx
    {
        /// <summary>
        /// 类的属性信息
        /// </summary>
        public PropertyInfo PropertyInfo { get; set; }

        /// <summary>
        /// 数据库字段名
        /// </summary>
        public string FieldName { get; set; }

        /// <summary>
        /// 是否数据库字段
        /// </summary>
        public bool IsDBField { get; set; }

        /// <summary>
        /// 数据库字段特性
        /// </summary>
        public ColumnAttribute DBFieldAttribute { get; set; }

        /// <summary>
        /// 是否主键
        /// </summary>
        public bool IsDBKey { get; set; }

        /// <summary>
        /// 是否自增(null表示未配置)
        /// </summary>
        public bool? IsAutoIncrement { get; set; }

        /// <summary>
        /// 类的属性信息扩展
        /// </summary>
        /// <param name="propertyInfo">类的属性信息</param>
        public PropertyInfoEx(PropertyInfo propertyInfo)
        {
            PropertyInfo = propertyInfo;
        }
    }
}
