using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dapper.Lite
{
    /// <summary>
    /// Guid类型转换
    /// </summary>
    public class GuidTypeHanlder : SqlMapper.TypeHandler<Guid>
    {
        public override void SetValue(IDbDataParameter parameter, Guid value)
        {
            parameter.Size = 50;
            parameter.DbType = DbType.Object;
            //parameter.Value = value.ToString();
            parameter.Value = value;
        }

        public override Guid Parse(object value)
        {
            //return Guid.Parse((string)value);
            return Convert(value);
        }

        internal static Guid Convert(object value)
        {
            if (value.GetType().Name.Contains("Guid"))
            {
                return (Guid)value;
            }
            else
            {
                return Guid.Parse(value.ToString());
            }
        }
    }
}
