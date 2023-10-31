using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dapper.Lite
{
    /// <summary>
    /// 标识该属性是只读的
    /// </summary>
    [Serializable, AttributeUsage(AttributeTargets.Property)]
    public class ReadOnlyAttribute : Attribute
    {
    }
}
