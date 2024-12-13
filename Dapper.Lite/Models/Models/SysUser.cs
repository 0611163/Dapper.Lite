using System;
using System.Collections.Generic;
using System.Linq;
using Dapper.Lite;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class BaseModel
    {
        public string PlatformName { get; set; }
    }

    /// <summary>
    /// 用户表
    /// </summary>
    [Serializable]
    [Table("sys_user")]
    public partial class SysUser: BaseModel
    {

        /// <summary>
        /// 主键
        /// </summary>
        [Key]
        public long Id { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        [Column("user_name")]
        public string UserName { get; set; }

        /// <summary>
        /// 用户姓名
        /// </summary>
        [Column("real_name")]
        public string RealName { get; set; }

        /// <summary>
        /// 用户密码
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
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
