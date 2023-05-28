using DAL;
using Dapper.Lite;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace Dapper.LiteTest
{
    [TestClass]
    public class SqlStringTest
    {
        private IDapperLiteClient _db;

        #region 构造函数
        public SqlStringTest()
        {
            _db = DapperLiteFactory.Client;
        }
        #endregion

        #region TestUpdateSql1
        [TestMethod]
        public void TestUpdateSql1()
        {
            var sql = _db.Sql<SysUser>(@"update sys_user set user_name=@UserName where id=@Id", new { UserName = "abc", Id = 20 });
            Assert.IsTrue("update sys_user set user_name=@UserName where id=@Id" == sql.SQL.Trim());
            Assert.IsTrue(sql.Params.Length == 2);
            var parameters = sql.Params.ToList();
            Assert.IsTrue(parameters.Exists(a => a.ParameterName == "UserName"));
            Assert.IsTrue(parameters.Exists(a => a.ParameterName == "Id"));
        }
        #endregion

    }
}
