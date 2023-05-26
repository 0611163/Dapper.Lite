using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dapper.Lite
{
    /// <summary>
    /// 查询结果的数量信息
    /// </summary>
    public class CountResult
    {
        /// <summary>
        /// 查询结果的数量
        /// </summary>
        public long Count { get; set; }

        /// <summary>
        /// 总页数
        /// </summary>
        public long PageCount { get; set; }

        /// <summary>
        /// 查询结果的数量信息
        /// </summary>
        public CountResult(long count, long pageCount)
        {
            Count = count;
            PageCount = pageCount;
        }
    }
}
