using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Models
{
    /// <summary>
    /// 订单表
    /// </summary>
    [Serializable]
    [Table("bs_order")]
    public partial class BsOrder
    {

        /// <summary>
        /// 主键
        /// </summary>
        [Key]
        [Column("id")]
        public string Id { get; set; }

        /// <summary>
        /// 订单时间
        /// </summary>
        [Column("order_time")]
        public DateTime OrderTime { get; set; }

        /// <summary>
        /// 订单金额
        /// </summary>
        [Column("amount")]
        public decimal? Amount { get; set; }

        /// <summary>
        /// 下单用户
        /// </summary>
        [Column("order_userid")]
        public long OrderUserid { get; set; }

        /// <summary>
        /// 订单状态(0草稿 1已下单 2已付款 3已发货 4完成)
        /// </summary>
        [Column("status")]
        public int Status { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [Column("remark")]
        public string Remark { get; set; }

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
