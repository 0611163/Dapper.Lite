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
    /// <summary>
    /// 异步同步对比测试
    /// </summary>
    [TestClass]
    public class AsyncSyncTest
    {
        #region 变量
        private BsOrderDal m_BsOrderDal = ServiceHelper.Get<BsOrderDal>();
        private int _n = 100;
        private int _pageSize = 20;
        #endregion

        #region 构造函数
        public AsyncSyncTest()
        {
            ThreadPool.SetMaxThreads(1000, 1000);
            ThreadPool.SetMinThreads(200, 200);

            m_BsOrderDal.Preheat();
        }
        #endregion

        #region 测试同步查询订单集合
        [TestMethod]
        public void TestQuery()
        {
            List<Task> taskList = new List<Task>();
            for (int i = 0; i < _n; i++)
            {
                var tsk = Task.Factory.StartNew((obj) =>
                {
                    try
                    {
                        PageModel pageModel = new PageModel();
                        pageModel.CurrentPage = 1;
                        pageModel.PageSize = _pageSize;

                        List<BsOrder> list = m_BsOrderDal.GetListPage(ref pageModel, 0, null, DateTime.MinValue, DateTime.Now.AddDays(1));
                        Assert.IsTrue(pageModel.TotalRows > 0);

                        if ((int)obj == 0)
                        {
                            foreach (BsOrder item in list)
                            {
                                Console.WriteLine(ModelToStringUtil.ToString(item));
                            }
                            Console.WriteLine("totalRows=" + pageModel.TotalRows);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                        throw;
                    }
                }, i);
                taskList.Add(tsk);
            }
            Task.WaitAll(taskList.ToArray());
        }
        #endregion

        #region 测试异步查询订单集合
        [TestMethod]
        public async Task TestQueryAsync()
        {
            for (int i = 0; i < _n; i++)
            {
                try
                {
                    PageModel pageModel = new PageModel();
                    pageModel.CurrentPage = 1;
                    pageModel.PageSize = _pageSize;

                    List<BsOrder> list = await m_BsOrderDal.GetListPageAsync(pageModel, 0, null, DateTime.MinValue, DateTime.Now.AddDays(1));
                    Assert.IsTrue(pageModel.TotalRows > 0);

                    if (i == 0)
                    {
                        foreach (BsOrder item in list)
                        {
                            Console.WriteLine(ModelToStringUtil.ToString(item));
                        }
                        Console.WriteLine("totalRows=" + pageModel.TotalRows);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    throw;
                }
            }
        }
        #endregion

    }
}
