using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dapper.Lite
{
    /// <summary>
    /// 标识该属性是自增字段 或 标识该类的主键是自增字段
    /// </summary>
    [Serializable, AttributeUsage(AttributeTargets.Property | AttributeTargets.Class)]
    public class AutoIncrementAttribute : Attribute
    {
        private bool _value = true;

        /// <summary>
        /// true:自增 false:不自增
        /// </summary>
        public bool Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
            }
        }

        /// <summary>
        /// 标识该属性是自增字段 或 标识该类的主键是自增字段
        /// </summary>
        /// <param name="value">true:自增 false:不自增</param>
        public AutoIncrementAttribute(bool value = true)
        {
            _value = value;
        }
    }
}
