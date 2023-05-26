using Dapper.Lite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Models
{
    /// <summary>
    /// 测试 byte[] Guid char 等字段类型
    /// </summary>
    [Serializable]
    [Table("values_test")]
    public class ValuesInfo
    {
        /// <summary>
        /// 主键
        /// </summary>
        [Key]
        [Column("id")]
        public long Id { get; set; }

        [Column("bytes_value")]
        public byte[] BytesValue { get; set; }

        [Column("byte_value")]
        public byte? ByteValue { get; set; }

        //[Column("guid_value")]
        //public Guid? GuidValue { get; set; }

        [Column("char_value")]
        public char? CharValue { get; set; }

        [Column("chars_valiue")]
        public string CharsValue { get; set; }

        [Column("bool_value")]
        public bool? BoolValue { get; set; }
    }
}
