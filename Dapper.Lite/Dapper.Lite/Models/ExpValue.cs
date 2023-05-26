using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dapper.Lite
{
    /// <summary>
    /// 表达式树解析，返回值
    /// </summary>
    public class ExpValue
    {
        public ExpValueType Type { get; set; }

        /// <summary>
        /// Type = SqlAndDbParameter
        /// </summary>
        public List<DbParameter> DbParameters { get; set; }

        /// <summary>
        /// Type = SqlAndDbParameter
        /// </summary>
        public String Sql { get; set; }

        /// <summary>
        /// Type = SqlValue
        /// </summary>
        public SqlValue SqlValue { get; set; }

        /// <summary>
        /// Type = OnlyValue
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// Type = MemberValue
        /// </summary>
        public string MemberName { get; set; }

        /// <summary>
        /// Type = MemberValue
        /// </summary>
        public string MemberAliasName { get; set; }

        /// <summary>
        /// Type = MemberValue
        /// </summary>
        public string MemberDBField { get; set; }

        /// <summary>
        /// Type = MemberValue
        /// </summary>
        public string MemberParentName { get; set; }

        /// <summary>
        /// 表达式树解析，返回值
        /// </summary>
        public ExpValue()
        {
            DbParameters = new List<DbParameter>();
        }
    }
}
