using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dapper.Lite
{
    internal enum SqlStringMethod
    {
        Query,

        Select,

        LeftJoin,

        WhereIf,

        Where,

        OrderBy,

        OrderByDescending
    }
}
