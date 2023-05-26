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
    public class BatchUpdateTest
    {
        #region 变量
        private BsOrderDal m_BsOrderDal = ServiceHelper.Get<BsOrderDal>();
        private SysUserDal m_SysUserDal = ServiceHelper.Get<SysUserDal>();
        private Random _rnd = new Random();
        #endregion

        #region 构造函数
        public BatchUpdateTest()
        {
            m_BsOrderDal.Preheat();
        }
        #endregion

        #region 测试批量修改用户
        [TestMethod]
        public void TestUpdateUserList()
        {
            List<SysUser> userList = m_SysUserDal.GetList("select t.* from sys_user t where t.id > 20");

            foreach (SysUser user in userList)
            {
                user.Remark = "测试修改用户" + _rnd.Next(1, 10000);
                user.UpdateUserid = "1";
                user.UpdateTime = DateTime.Now;
            }
            m_SysUserDal.Update(userList);

            var session = DapperLiteFactory.GetSession();

            session.OnExecuting = (s, p) => Console.WriteLine(s); //打印SQL

            long count = session.Sql(
                "select * from sys_user where Remark like @Remark", new { Remark = "测试修改用户%" }).QueryCount();
            Assert.IsTrue(count >= userList.Count);
        }
        #endregion

        #region 测试批量修改用户(异步)
        [TestMethod]
        public async Task TestUpdateUserListAsync()
        {
            List<SysUser> userList = m_SysUserDal.GetList("select t.* from sys_user t where t.id > 20");

            foreach (SysUser user in userList)
            {
                user.Remark = "测试修改用户Async" + _rnd.Next(1, 10000);
                user.UpdateUserid = "1";
                user.UpdateTime = DateTime.Now;
            }
            await m_SysUserDal.UpdateAsync(userList);

            var session = DapperLiteFactory.GetSession();

            session.OnExecuting = (s, p) => Console.WriteLine(s); //打印SQL

            long count = session.Sql(
                "select * from sys_user where Remark like @Remark", new { Remark = "测试修改用户Async%" }).QueryCount();
            Assert.IsTrue(count >= userList.Count);
        }
        #endregion

    }
}
