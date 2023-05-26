using DAL;
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
    public class BatchInsertTest
    {
        #region 变量
        private BsOrderDal m_BsOrderDal = ServiceHelper.Get<BsOrderDal>();
        private SysUserDal m_SysUserDal = ServiceHelper.Get<SysUserDal>();
        #endregion

        #region 构造函数
        public BatchInsertTest()
        {
            m_BsOrderDal.Preheat();
        }
        #endregion

        #region 测试批量添加用户
        [TestMethod]
        public void TestInsertUserList()
        {
            List<SysUser> userList = new List<SysUser>();
            for (int i = 1; i <= 1000; i++)
            {
                SysUser user = new SysUser();
                user.UserName = "testUser";
                user.RealName = "测试插入用户";
                user.Password = "123456";
                user.CreateUserid = "1";
                userList.Add(user);
            }
            m_SysUserDal.Insert(userList);

            var session = LiteSqlFactory.GetSession();

            long count = session.QueryCount("select * from sys_user");
            Assert.IsTrue(count >= 1000);
        }
        #endregion

        #region 测试批量添加用户(异步)
        [TestMethod]
        public async Task TestInsertUserListAsync()
        {
            List<SysUser> userList = new List<SysUser>();
            for (int i = 1; i <= 1000; i++)
            {
                SysUser user = new SysUser();
                user.UserName = "testUser";
                user.RealName = "测试插入用户";
                user.Password = "123456";
                user.CreateUserid = "1";
                userList.Add(user);
            }
            await m_SysUserDal.InsertAsync(userList);

            var session = LiteSqlFactory.GetSession();

            long count = session.QueryCount("select * from sys_user");
            Assert.IsTrue(count >= 1000);
        }
        #endregion

    }
}
