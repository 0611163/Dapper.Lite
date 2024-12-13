using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
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
        [NotMapped]
        public List<BsOrderDetail> DetailList { get; set; }

        /// <summary>
        /// 下单用户姓名
        /// </summary>
        [NotMapped]
        public string OrderUserRealName { get; set; }

        /// <summary>
        /// 下单用户名
        /// </summary>
        [NotMapped]
        public string OrderUserName { get; set; }
    }
}
