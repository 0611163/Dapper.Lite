using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dapper.Lite
{
    /// <summary>
    /// 表达式树解析，返回值类型
    /// </summary>
    public enum ExpValueType
    {
        OnlyValue,

        MemberValue,

        SqlAndDbParameter
    }
}
