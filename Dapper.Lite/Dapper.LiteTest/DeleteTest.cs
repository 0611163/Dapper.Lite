using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Models;
using DAL;
using System.Collections.Generic;
using Dapper.Lite;
using Utils;
using System.Threading.Tasks;
using System.Linq;

namespace Dapper.LiteTest
{
    [TestClass]
    public class DeleteTest
    {
        #region 变量
        private BsOrderDal m_BsOrderDal = ServiceHelper.Get<BsOrderDal>();
        private SysUserDal m_SysUserDal = ServiceHelper.Get<SysUserDal>();
        #endregion

        #region 构造函数
        public DeleteTest()
        {
            m_BsOrderDal.Preheat();
        }
        #endregion

        #region 测试删除用户
        [TestMethod]
        public void TestDeleteUser()
        {
            SysUser user = new SysUser();
            user.UserName = "testUser";
            user.RealName = "测试插入用户";
            user.Password = "123456";
            user.CreateUserid = "1";
            long id = m_SysUserDal.Insert(user);

            m_SysUserDal.Delete(id);

            var session = LiteSqlFactory.GetSession();

            SysUser userInfo = session.QueryById<SysUser>(id);
            Assert.IsTrue(userInfo == null);
        }
        #endregion

        #region 测试根据查询条件删除用户
        [TestMethod]
        public void TestDeleteUserByCondition()
        {
            var session = LiteSqlFactory.GetSession();

            session.OnExecuting = (s, p) => Console.WriteLine(s); //打印SQL

            session.Sql("id>@Id", 20).Delete<SysUser>();
            session.Queryable<SysUser>().Where(t => t.Id > 20).Delete();

            long count = session.QueryCount("select * from sys_user where id>20");
            Assert.IsTrue(count == 0);
        }
        #endregion

        #region 删除订单和订单明细
        [TestMethod]
        public void TestDeleteOrderByCondition()
        {
            var session = LiteSqlFactory.GetSession();

            session.OnExecuting = (sql, param) => Console.WriteLine(sql); //打印SQL

            int rows = session.Sql<BsOrder>("id not like @Id", new { Id = "10000_" }).Delete();
            Console.WriteLine("BsOrder表" + rows + "行已删除");
            rows = session.Sql("order_id not like @OrderId", new { OrderId = "10000_" }).Delete<BsOrderDetail>();
            Console.WriteLine("BsOrderDetail表" + rows + "行已删除");

            long count = session.Sql("select * from bs_order where id not like @Id", new { Id = "10000_" }).QueryCount();
            Assert.IsTrue(count == 0);
            count = session.Sql("select * from bs_order_detail where order_id not like @OrderId", new { OrderId = "10000_" }).QueryCount();
            Assert.IsTrue(count == 0);
        }
        #endregion

        #region TestLambdaDelete
        [TestMethod]
        public void TestLambdaDelete()
        {
            var session = LiteSqlFactory.GetSession();
            session.OnExecuting = (s, p) => Console.WriteLine(s);

            SysUser user = new SysUser();
            user.UserName = "testUser";
            user.RealName = "测试插入用户";
            user.Password = "123456";
            user.CreateUserid = "1";
            long id = m_SysUserDal.Insert(user);

            int rows1 = session.Queryable<SysUser>().Where(t => t.Id > 20 && !new long[] { 1, 2, 3 }.Contains(t.Id)).Delete();
            Assert.IsTrue(rows1 > 0);

            user = new SysUser();
            user.UserName = "testUser";
            user.RealName = "测试插入用户";
            user.Password = "123456";
            user.CreateUserid = "1";
            id = m_SysUserDal.Insert(user);

            Task.Run(async () =>
            {
                int rows2 = await session.Queryable<SysUser>().Where(t => t.Id > 20 || new long[] { 101, 102, 103 }.Contains(t.Id)).DeleteAsync();
                Assert.IsTrue(rows2 > 0);
            }).Wait();
        }
        #endregion

    }
}
