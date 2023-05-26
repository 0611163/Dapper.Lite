using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Models;
using DAL;
using System.Collections.Generic;
using Dapper.Lite;
using Utils;
using System.Threading.Tasks;

namespace Dapper.LiteTest
{
    [TestClass]
    public class UpdateTest
    {
        #region 变量
        private BsOrderDal m_BsOrderDal = ServiceHelper.Get<BsOrderDal>();
        private SysUserDal m_SysUserDal = ServiceHelper.Get<SysUserDal>();
        private Random _rnd = new Random();
        #endregion

        #region 构造函数
        public UpdateTest()
        {
            m_BsOrderDal.Preheat();
        }
        #endregion

        #region 测试修改订单
        [TestMethod]
        public void TestUpdateOrder()
        {
            string userId = "10";

            BsOrder order = m_BsOrderDal.Get("100001");
            if (order == null)
            {
                throw new Exception("测试数据被删除");
            }

            order.Remark = "订单已修改" + _rnd.Next(0, 100);
            order.UpdateUserid = userId;

            order.DetailList.Clear(); //删除全部明细

            //删除某条明细
            /*
            for (int i = order.DetailList.Count - 1; i >= 0; i--)
            {
                if (order.DetailList[i].GOODS_NAME == "鼠标")
                {
                    order.DetailList.RemoveAt(i);
                }
            } 
            */

            foreach (BsOrderDetail oldDetail in order.DetailList)
            {
                oldDetail.UpdateUserid = userId;
            }

            BsOrderDetail detail = new BsOrderDetail();
            detail.GoodsName = "桌子" + _rnd.Next(1, 100);
            detail.Quantity = 10;
            detail.Price = (decimal)78.89;
            detail.Spec = "张";
            detail.CreateUserid = userId;
            detail.OrderNum = 4;
            order.DetailList.Add(detail);

            detail = new BsOrderDetail();
            detail.GoodsName = "椅子" + _rnd.Next(1, 100);
            detail.Quantity = 20;
            detail.Price = (decimal)30.23;
            detail.Spec = "把";
            detail.CreateUserid = userId;
            detail.OrderNum = 5;
            order.DetailList.Add(detail);

            m_BsOrderDal.Update(order, order.DetailList);

            var session = DapperLiteFactory.GetSession();
            BsOrder orderInfo = session.Sql(
                "select * from bs_order where Id like @Id", new { Id = order.Id })
                .Query<BsOrder>();
            Assert.IsTrue(orderInfo.Remark == order.Remark);
        }
        #endregion

        #region 测试修改用户
        [TestMethod]
        public void TestUpdateUser()
        {
            long userId = 10;
            SysUser user = m_SysUserDal.Get(userId);
            if (user != null)
            {
                var session = DapperLiteFactory.GetSession();
                session.OnExecuting = (s, p) =>
                {
                    Console.WriteLine(s); //打印SQL
                };

                session.AttachOld(user);

                user.UpdateUserid = "1";
                user.Remark = "测试修改用户" + _rnd.Next(1, 100);

                user.UpdateTime = DateTime.Now;
                session.Update(user);

                SysUser userInfo = session.Sql(
                    "select * from sys_user where Id = @Id", new { Id = userId })
                    .Query<SysUser>();
                Assert.IsTrue(userInfo.Remark == user.Remark);
                Console.WriteLine("用户 ID=" + user.Id + " 已修改");
            }
            else
            {
                throw new Exception("测试数据被删除");
            }
        }
        #endregion

        #region 测试修改用户(异步)
        [TestMethod]
        public async Task TestUpdateUserAsync()
        {
            long userId = 10;
            SysUser user = m_SysUserDal.Get(userId);
            if (user != null)
            {
                user.UpdateUserid = "1";
                user.Remark = "测试修改用户" + _rnd.Next(1, 100);
                await m_SysUserDal.UpdateAsync(user);
                Console.WriteLine("用户 ID=" + user.Id + " 已修改");

                var session = DapperLiteFactory.GetSession();
                SysUser userInfo = session.Sql(
                    "select * from sys_user where Id like @Id", new { Id = userId })
                    .Query<SysUser>();
                Assert.IsTrue(userInfo.Remark == user.Remark);
            }
            else
            {
                throw new Exception("测试数据被删除");
            }
        }
        #endregion

    }
}
