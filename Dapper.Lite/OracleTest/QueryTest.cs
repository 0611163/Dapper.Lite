using Dapper.Lite;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Models;
using System;
using System.Collections.Generic;
using System.Threading;
using Utils;

namespace OracleTest
{
    [TestClass]
    public class QueryTest
    {
        #region 构造函数
        public QueryTest()
        {
            ThreadPool.SetMaxThreads(1000, 1000);
            ThreadPool.SetMinThreads(200, 200);

            //预热
            LiteSqlFactoryMySQL.GetSession().QuerySingle("select count(*) from bs_order");

            LiteSqlFactory.GetSession().QuerySingle("select count(*) from CARINFO_MERGE");
        }
        #endregion

        [TestMethod]
        public void Test1Query()
        {
            List<CarinfoMerge> list = new List<CarinfoMerge>();

            var session = LiteSqlFactory.GetSession();

            list = session.Sql(@"
                select * 
                from CARINFO_MERGE 
                where rownum<20000
                and modify_time < @Time", new { Time = new DateTime(2022, 1, 1) }).QueryList<CarinfoMerge>();

            Assert.IsTrue(list.Count > 0);

            int outputCount = 0;
            foreach (CarinfoMerge item in list)
            {
                if (outputCount++ < 20)
                {
                    Console.WriteLine(ModelToStringUtil.ToString(item));
                }
            }
        }

        [TestMethod]
        public void Test2Query()
        {
            List<CarinfoMerge> list = new List<CarinfoMerge>();

            var session = LiteSqlFactory.GetSession();

            list = session.Sql(@"
                select * 
                from CARINFO_MERGE 
                where rownum<200
                and modify_time < @Time
                and license_no like @NO", new { Time = new DateTime(2022, 1, 1), NO = "%A81063%" }).QueryList<CarinfoMerge>();

            Assert.IsTrue(list.Count > 0);

            int outputCount = 0;
            foreach (CarinfoMerge item in list)
            {
                if (outputCount++ < 20)
                {
                    Console.WriteLine(ModelToStringUtil.ToString(item));
                }
            }
        }

        [TestMethod]
        public void Test2QueryMySqlAndOracle()
        {
            BsOrder order = LiteSqlFactoryMySQL.GetSession().QueryById<BsOrder>("100001");
            Assert.IsTrue(order != null);
            Console.WriteLine("订单：");
            Console.WriteLine(ModelToStringUtil.ToString(order));

            var session = LiteSqlFactory.GetSession();
            ISqlString sql = session.Sql("select * from CARINFO_MERGE where rownum<1000");

            //sql.Append(" and id in @ids", sql.ForList(new List<long> { 715299 }));

            List<CarinfoMerge> list = sql.QueryList<CarinfoMerge>();
            Assert.IsTrue(list.Count > 0);
            Console.WriteLine("CARINFO_MERGE：");
            for (int i = 0; i < 20; i++)
            {
                Console.WriteLine(ModelToStringUtil.ToString(list[i]));
            }
        }
    }
}
