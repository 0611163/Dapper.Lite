using System;
using System.Collections.Generic;
using System.Linq;
using Dapper.Lite;

namespace Models
{
    /// <summary>
    /// 订单表
    /// </summary>
    public partial class BsOrder
    {
        /// <summary>
        /// 订单明细集合
        /// </summary>
        public List<BsOrderDetail> DetailList { get; set; }

        /// <summary>
        /// 下单用户姓名
        /// </summary>
        public string OrderUserRealName { get; set; }

        /// <summary>
        /// 下单用户名
        /// </summary>
        public string OrderUserName { get; set; }
    }
}
