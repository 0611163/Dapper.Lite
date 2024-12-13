using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Dapper.Lite;

namespace Models
{
    /// <summary>
    /// 订单明细表
    /// </summary>
    public partial class BsOrderDetail
    {
        /// <summary>
        /// 测试用的字段
        /// </summary>
        [NotMapped]
        public string TestTemp { get; set; }
    }
}
