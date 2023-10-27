using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Models;
using DAL;
using System.Collections.Generic;
using System.Linq;
using Dapper.Lite;
using Utils;
using System.Data.Common;

namespace Dapper.LiteTest
{
    [TestClass]
    public class LambdaTest
    {
        #region 变量
        private BsOrderDal m_BsOrderDal = ServiceHelper.Get<BsOrderDal>();
        #endregion

        #region 构造函数
        public LambdaTest()
        {
            m_BsOrderDal.Preheat();
        }
        #endregion

        #region 测试查询订单集合(使用 Lambda 表达式)(传变量)
        [TestMethod]
        public void TestQueryByLambda1()
        {
            int? status = 0;
            string remark = "订单";
            DateTime? startTime = new DateTime(2010, 1, 1);
            DateTime? endTime = DateTime.Now.AddDays(1);

            var session = DapperLiteFactory.GetSession();

            session.OnExecuting = (s, p) => Console.WriteLine(s);

            List<BsOrder> list = session.Queryable<BsOrder>().Where(t => t.Status == status
                && t.Remark.Contains(remark.ToString())
                && startTime <= t.OrderTime
                && t.OrderTime <= endTime)
                .OrderByDescending(t => t.OrderTime).OrderBy(t => t.Id)
                .ToList();

            foreach (BsOrder item in list)
            {
                Console.WriteLine(ModelToStringUtil.ToString(item));
            }
            Assert.IsTrue(list.Count > 0);
        }
        #endregion

        #region 测试查询订单集合(使用 Lambda 表达式)(传值)
        [TestMethod]
        public void TestQueryByLambda2()
        {
            var session = DapperLiteFactory.GetSession();

            List<BsOrder> list = session.Queryable<BsOrder>().Where(t => t.Status == int.Parse("0")
                && t.Status == new BsOrder().Status
                && t.Remark.Contains("订单")
                && t.Remark != null
                && t.OrderTime >= new DateTime(2010, 1, 1)
                && t.OrderTime <= DateTime.Now.AddDays(1))
                .OrderByDescending(t => t.OrderTime).OrderBy(t => t.Id)
                .ToList();

            foreach (BsOrder item in list)
            {
                Console.WriteLine(ModelToStringUtil.ToString(item));
            }
            Assert.IsTrue(list.Count > 0);
        }
        #endregion

        #region 测试查询订单集合(使用 Lambda 表达式)(传对象的变量)
        [TestMethod]
        public void TestQueryByLambda3()
        {
            BsOrder order = new BsOrder();
            order.Status = 0;
            order.Remark = "订单";

            var time = new
            {
                startTime = new DateTime(2010, 1, 1),
                endTime = DateTime.Now.AddDays(1)
            };

            string[] idsNotIn = new string[] { "100007", "100008", "100009" };

            var session = DapperLiteFactory.GetSession();

            session.OnExecuting = (s, p) =>
                {
                    Console.WriteLine(s);
                    foreach (DbParameter item in p)
                    {
                        Console.Write(item.ParameterName + "：" + item.Value + ", ");
                    }
                    Console.WriteLine(string.Empty);
                };

            List<BsOrder> list = session.Queryable<BsOrder>().Where(t => new int[] { 0, 1 }.Contains(t.Status)
                && t.Remark.Contains(order.Remark.ToString())
                && t.OrderTime >= time.startTime
                && t.OrderTime <= time.endTime
                && !idsNotIn.Contains(t.Id)
                && !new string[] { "100007", "100008", "100009" }.Contains(t.Id))
                .OrderByDescending(t => t.OrderTime).OrderBy(t => t.Id)
                .ToList();

            foreach (BsOrder item in list)
            {
                Console.WriteLine(ModelToStringUtil.ToString(item));
            }
            Assert.IsTrue(list.Count > 0);
        }

        [TestMethod]
        public void TestQueryByLambda3_2()
        {
            BsOrder order = new BsOrder();
            order.Status = 0;
            order.Remark = "订单";

            var time = new
            {
                startTime = new DateTime(2010, 1, 1),
                endTime = DateTime.Now.AddDays(1)
            };

            List<string> idsNotIn = new List<string>() { "100007", "100008", "100009" };

            var session = DapperLiteFactory.GetSession();

            session.OnExecuting = (s, p) =>
            {
                Console.WriteLine(s);
                foreach (DbParameter item in p)
                {
                    Console.Write(item.ParameterName + "：" + item.Value + ", ");
                }
                Console.WriteLine(string.Empty);
            };

            List<BsOrder> list = session.Queryable<BsOrder>().Where(t => t.Status == order.Status
                && t.Remark.Contains(order.Remark.ToString())
                && !idsNotIn.Contains(t.Id)
                && !new List<string>() { "100021", "100022", "100023" }.Contains(t.Id))
                .Where(t => !(new List<string>() { "100015", "100016", "100017" }).Contains(t.Id))
                .OrderByDescending(t => t.OrderTime).OrderBy(t => t.Id)
                .ToList();

            foreach (BsOrder item in list)
            {
                Console.WriteLine(ModelToStringUtil.ToString(item));
            }
            Assert.IsTrue(list.Count > 0);
        }
        #endregion

        #region 测试查询订单集合(使用 Lambda 表达式)(单个条件)
        [TestMethod]
        public void TestQueryByLambda4()
        {
            var time = new
            {
                startTime = new DateTime(2010, 1, 1),
                endTime = DateTime.Now.AddDays(1)
            };

            DateTime dt = new DateTime(2010, 1, 1);

            List<string> ids = new List<string>() { "100001", "100002", "100003" };

            List<string> idsNotIn = new List<string>() { "100007", "100008", "100009" };

            var session = DapperLiteFactory.GetSession();

            ISqlQueryable<BsOrder> sql = session.Queryable<BsOrder>();

            sql.Where(t => t.Status >= 0);

            sql.Where(t => t.Remark.StartsWith("订单"));

            sql.Where(t => t.Remark.StartsWith(GetStr(9)));

            sql.Where(t => time.startTime <= t.OrderTime);

            sql.Where(t => t.OrderTime >= dt);

            sql.Where(t => t.OrderTime >= DateTime.Parse(new DateTime(2010, 1, 1).ToString("yyyy-MM-dd HH:mm:ss")));

            // sql.Where<BsOrder>(t => ids.Contains(t.Id)); //同一个字段不能同时 in 和 not in

            sql.Where(t => !idsNotIn.Contains(t.Id));

            sql.OrderByDescending(t => t.OrderTime).OrderBy(t => t.Id);

            List<BsOrder> list = sql.ToList();
            foreach (BsOrder item in list)
            {
                Console.WriteLine(ModelToStringUtil.ToString(item));
            }
        }

        private string GetStr(int n)
        {
            return "订单";
        }
        #endregion

        #region 测试查询订单集合(使用 Lambda 表达式)(单表查询)
        [TestMethod]
        public void TestQueryByLambda6()
        {
            var session = DapperLiteFactory.GetSession();

            session.OnExecuting = (s, p) =>
            {
                Console.WriteLine(s); //打印SQL
            };

            string remark = "测试";

            List<BsOrder> list = session.Queryable<BsOrder>()

                .Where(t => t.Remark.Contains(remark)
                    && t.CreateTime < DateTime.Now
                    && t.CreateUserid == "10")

                .OrderByDescending(t => t.OrderTime).OrderBy(t => t.Id)
                .ToList();

            foreach (BsOrder item in list)
            {
                Console.WriteLine(ModelToStringUtil.ToString(item));
            }
            Assert.IsTrue(list.Count > 0);
        }
        #endregion

        #region 测试查询订单集合(使用 Lambda 表达式)(单表分页查询)
        [TestMethod]
        public void TestQueryByLambda8()
        {
            var session = DapperLiteFactory.GetSession();

            session.OnExecuting = (s, p) =>
            {
                Console.WriteLine(s);
            };

            ISqlQueryable<BsOrder> sql = session.Queryable<BsOrder>();

            string remark = "测试";

            sql.Where(t => t.Remark.Contains(remark)
                && t.CreateTime < DateTime.Now
                && t.CreateUserid == "10")

                .OrderByDescending(t => t.OrderTime).OrderBy(t => t.Id);


            long total = sql.Count();
            List<BsOrder> list = sql.ToPageList(1, 20);

            foreach (BsOrder item in list)
            {
                Console.WriteLine(ModelToStringUtil.ToString(item));
            }
            Console.WriteLine("total=" + total);
            Assert.IsTrue(list.Count > 0);
        }
        #endregion

        #region 测试查询订单集合(使用 Lambda 表达式)(临时测试)
        [TestMethod]
        public void TestQueryByLambda10()
        {
            var session = DapperLiteFactory.GetSession();

            session.OnExecuting = (s, p) => Console.WriteLine(s);

            ISqlQueryable<BsOrder> sql = session.Queryable<BsOrder>();

            string remark = "测试";

            List<BsOrder> list = session.Queryable<BsOrder>()
                .Where(t => t.Remark.Contains(remark)
                    && t.CreateTime < DateTime.Now
                    && !t.CreateUserid.Contains(string.Format("12{0}", 3)))

                .OrderByDescending(t => t.OrderTime).OrderBy(t => t.Id)
                .ToList();

            foreach (BsOrder item in list)
            {
                Console.WriteLine(ModelToStringUtil.ToString(item));
            }
            Assert.IsTrue(list.Count > 0);
        }
        #endregion

        #region 测试查询订单集合(使用 Lambda 表达式)(查询单条记录)
        [TestMethod]
        public void TestQueryByLambda11()
        {
            TestQueryByLambda11Internal();
        }

        private async void TestQueryByLambda11Internal()
        {
            var session = DapperLiteFactory.GetSession();

            session.OnExecuting = (s, p) => Console.WriteLine(s); //打印SQL

            ISqlQueryable<BsOrder> sql = session.Queryable<BsOrder>();

            BsOrder order = await sql.Where(o => o.Id == "100001").FirstAsync();

            sql = session.Queryable<BsOrder>();
            bool bl = await sql.Where(o => o.Id == "100001").ExistsAsync();
            Assert.IsTrue(bl);

            if (order != null)
            {
                Console.WriteLine(ModelToStringUtil.ToString(order));
            }
        }
        #endregion

        #region 测试查询订单集合(使用 Lambda 表达式)(测试DateTime)
        [TestMethod]
        public void TestQueryByLambda16()
        {
            DateTime? endTime = new DateTime(2023, 1, 1);

            var session = DapperLiteFactory.GetSession();

            session.OnExecuting = (s, p) => Console.WriteLine(s);

            List<SysUser> list = session.Queryable<SysUser>()
                .Where(t => t.CreateTime < endTime.Value.Date.AddDays(1).AddSeconds(-1)).ToList();

            foreach (SysUser item in list)
            {
                Console.WriteLine(ModelToStringUtil.ToString(item));
            }
            Assert.IsTrue(list.Count > 0);
        }
        #endregion

        #region 测试查询订单集合(使用 Lambda 表达式)(Lambda 表达式不使用t)
        [TestMethod]
        public void TestQueryByLambda18()
        {
            DateTime? endTime = new DateTime(2023, 1, 1);

            var session = DapperLiteFactory.GetSession();

            session.OnExecuting = (s, p) =>
            {
                Console.WriteLine(s);
            };

            List<SysUser> list = session.Queryable<SysUser>()
                .Where(user => user.CreateTime < endTime.Value.Date.AddDays(1).AddSeconds(-1) && user.Id > 0)
                .Where(user => user.Id <= 20).ToList();

            foreach (SysUser item in list)
            {
                Console.WriteLine(ModelToStringUtil.ToString(item));
            }
            Assert.IsTrue(list.Count > 0);

            list = session.Queryable<SysUser>().ToList();
            long count = session.Queryable<SysUser>().Count();
            Assert.IsTrue(count > 0);
            Assert.IsTrue(list.Count > 0);
        }
        #endregion

    }
}
