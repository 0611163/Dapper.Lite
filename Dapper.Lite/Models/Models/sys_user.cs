using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public partial class Sys_user
    {
        /// <summary>
        /// 主键
        /// </summary>
        [Key]
        public long Id { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string User_name { get; set; }

        /// <summary>
        /// 用户姓名
        /// </summary>
        public string Real_name { get; set; }

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
        public string create_userid { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime create_time { get; set; }

        /// <summary>
        /// 更新者ID
        /// </summary>
        public string update_userid { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime? update_time { get; set; }
    }
}
