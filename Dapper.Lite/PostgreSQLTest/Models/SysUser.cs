using System;
using System.Collections.Generic;
using System.Linq;
using Dapper.Lite;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Models
{
    /// <summary>
    /// 用户表
    /// </summary>
    [Serializable]
    [Table("sys_user")]
    public partial class SysUser
    {

        /// <summary>
        /// 主键
        /// </summary>
        [Key]
        [Column]
        public long Id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column]
        public string Password { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column]
        public string Remark { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column("CreateUserid")]
        public string Createuserid { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column("CreateTime")]
        public DateTime? Createtime { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column("UpdateUserid")]
        public string Updateuserid { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column("UpdateTime")]
        public DateTime? Updatetime { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        [Column("UserName")]
        public string Username { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        [Column("RealName")]
        public string Realname { get; set; }

        /// <summary>
        /// 身高
        /// </summary>
        [Column]
        public decimal? Height { get; set; }

    }
}
