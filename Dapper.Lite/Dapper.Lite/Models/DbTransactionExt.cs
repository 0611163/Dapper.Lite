using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dapper.Lite
{
    /// <summary>
    /// 数据库事物扩展
    /// </summary>
    public class DbTransactionExt
    {
        public DbTransaction Tran { get; set; }

        public DbConnectionExt ConnEx { get; set; }

        public DbConnection Conn
        {
            get
            {
                return ConnEx?.Conn;
            }
        }

        public DbTransactionExt(DbTransaction tran, DbConnectionExt connEx)
        {
            Tran = tran;
            ConnEx = connEx;
            ConnEx.Tran = this;
        }
    }
}
