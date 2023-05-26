using System;
using System.Collections.Generic;
using System.Linq;
using Dapper.Lite;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Models
{
    /// <summary>
    /// 订单明细表
    /// </summary>
    [Serializable]
    [Table("bs_order_detail")]
    public partial class BsOrderDetail
    {

        /// <summary>
        /// 主键
        /// </summary>
        [Key]
        [Column("id")]
        public string Id { get; set; }

        /// <summary>
        /// 订单ID
        /// </summary>
        [Column("order_id")]
        public string OrderId { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        [Column("goods_name")]
        public string GoodsName { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        [Column("quantity")]
        public int Quantity { get; set; }

        /// <summary>
        /// 价格
        /// </summary>
        [Column("price")]
        public decimal Price { get; set; }

        /// <summary>
        /// 物品规格
        /// </summary>
        [Column("spec")]
        public string Spec { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        [Column("order_num")]
        public int? OrderNum { get; set; }

        /// <summary>
        /// 创建者ID
        /// </summary>
        [Column("create_userid")]
        public string CreateUserid { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [Column("create_time")]
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 更新者ID
        /// </summary>
        [Column("update_userid")]
        public string UpdateUserid { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        [Column("update_time")]
        public DateTime? UpdateTime { get; set; }

    }
}
