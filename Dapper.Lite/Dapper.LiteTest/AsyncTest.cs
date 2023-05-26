using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Models;
using DAL;
using System.Collections.Generic;
using Dapper.Lite;
using System.Threading;
using System.Threading.Tasks;
using Utils;

namespace Dapper.LiteTest
{
    [TestClass]
    public class AsyncTest
    {
        #region 变量
        private BsOrderDal m_BsOrderDal = ServiceHelper.Get<BsOrderDal>();
        #endregion

        #region 构造函数
        public AsyncTest()
        {
            m_BsOrderDal.Preheat();
        }
        #endregion

        #region 测试异步查询订单集合
        [TestMethod]
        public async Task TestQueryAsync()
        {
            var task = m_BsOrderDal.GetListAsync(0, "订单", DateTime.MinValue, DateTime.Now.AddDays(1));
            List<BsOrder> list = await task;
            Assert.IsTrue(list.Count > 0);

            foreach (BsOrder item in list)
            {
                Console.WriteLine(ModelToStringUtil.ToString(item));
            }
        }
        #endregion

    }
}
