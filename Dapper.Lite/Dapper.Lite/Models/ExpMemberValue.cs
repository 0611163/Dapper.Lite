using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dapper.Lite
{
    /// <summary>
    /// 表达式树解析，返回值
    /// </summary>
    internal class ExpMemberValue
    {
        public string MemberName { get; set; }

        public string MemberAliasName { get; set; }

        public string MemberDBField { get; set; }

        public string MemberParentName { get; set; }
    }
}
