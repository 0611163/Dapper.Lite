using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Models;
using Dapper.Lite;

namespace PostgreSQLTest
{
    public partial class PostgreSQLTest
    {
        [TestMethod]
        public void Test3Update()
        {
            SysUser oldUser = null;
            var session = LiteSqlFactory.GetSession();
            oldUser = session.Query<SysUser>("select * from sys_user");

            session.OnExecuting = (s, p) => Console.WriteLine(s); //打印SQL

            session.AttachOld(oldUser);
            SysUser user = session.Query<SysUser>("select * from sys_user");
            user.Username = "testUser";
            user.Realname = "测试修改用户3";
            user.Password = "123456";
            user.Updateuserid = "1";
            user.Updatetime = DateTime.Now;
            session.Update(user);

            ISqlString sql = session.Sql("select * from sys_user where \"RealName\" like @RealName",
                new { RealName = "测试修改用户%" });
            long count = sql.QueryCount();
            Assert.IsTrue(count > 0);
        }
    }
}
