using Dapper.Lite;

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
        public string TestTemp { get; set; }

        /// <summary>
        /// 统计数量
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// 用户的订单数量
        /// </summary>
        public int OrderCount { get; set; }
    }
}
