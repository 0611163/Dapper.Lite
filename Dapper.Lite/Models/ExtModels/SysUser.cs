using System;
using System.Collections.Generic;
using System.Linq;
using Dapper.Lite;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    /// <summary>
    /// 用户表
    /// </summary>
    [AutoIncrement]
    public partial class SysUser
    {
        /// <summary>
        /// 测试用的字段
        /// </summary>
        [NotMapped]
        public string TestTemp { get; set; }

        /// <summary>
        /// 统计数量
        /// </summary>
        [NotMapped]
        public int Count { get; set; }

        /// <summary>
        /// 用户的订单数量
        /// </summary>
        [NotMapped]
        public int OrderCount { get; set; }
    }
}
