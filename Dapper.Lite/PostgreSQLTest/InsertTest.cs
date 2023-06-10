using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Models;
using Dapper.Lite;
using System.Collections.Generic;

namespace PostgreSQLTest
{
    public partial class PostgreSQLTest
    {
        public PostgreSQLTest()
        {
            LiteSqlFactory.GetSession(); //预热
        }

        [TestMethod]
        public void Test1Insert()
        {
            try
            {
                int id;
                var session = LiteSqlFactory.GetSession();
                id = session.QueryNextId<SysUser>();

                SysUser user = new SysUser();
                user.Id = id;
                user.Username = "testUser";
                user.Realname = "测试插入用户";
                user.Password = "123456";
                user.Createuserid = "1";
                user.Height = 175;

                session.OnExecuting = (s, p) => Console.WriteLine(s); //打印SQL

                user.Createtime = DateTime.Now;
                session.Insert(user);

                long count = session.Sql(
                    "select * from sys_user where \"Id\" = @Id", new { Id = user.Id })
                    .QueryCount();
                Assert.IsTrue(count > 0);

                Console.WriteLine("user.Id=" + user.Id);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            }
        }

        #region 测试批量添加用户
        [TestMethod]
        public void Test2InsertList()
        {
            int id;
            var session = LiteSqlFactory.GetSession();
            session.OnExecuting = (s, p) => Console.WriteLine(s); //打印SQL

            id = session.QueryNextId<SysUser>();

            List<SysUser> userList = new List<SysUser>();
            for (int i = 1; i <= 1000; i++)
            {
                SysUser user = new SysUser();
                user.Id = id++;
                user.Username = "testUser" + i;
                user.Realname = "测试插入用户" + i;
                user.Password = "123456";
                user.Createuserid = "1";
                user.Createtime = DateTime.Now;
                user.Height = 175;
                userList.Add(user);
            }

            try
            {
                session.OnExecuting = (s, p) => Console.WriteLine(s); //打印SQL

                session.BeginTransaction();
                session.Insert(userList);
                session.CommitTransaction();

                int count = LiteSqlFactory.GetSession().QuerySingle<int>("select count(*) from sys_user");
                Assert.IsTrue(count >= 1000);
            }
            catch (Exception ex)
            {
                session.RollbackTransaction();
                throw;
            }
        }
        #endregion

    }
}
