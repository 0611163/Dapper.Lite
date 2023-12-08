using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Models;
using DAL;
using System.Collections.Generic;
using System.Linq;
using Dapper.Lite;
using Utils;
using System.Data;

namespace Dapper.LiteTest
{
    [TestClass]
    public class QueryTest
    {
        #region 变量
        private BsOrderDal m_BsOrderDal = ServiceHelper.Get<BsOrderDal>();
        #endregion

        #region 构造函数
        public QueryTest()
        {
            m_BsOrderDal.Preheat();
        }
        #endregion

        #region 测试查询订单集合
        [TestMethod]
        public void TestQuery()
        {
            List<BsOrder> list = m_BsOrderDal.GetList(0, "订单", DateTime.MinValue, DateTime.Now.AddDays(1), "100001,100002,100003");
            Assert.IsTrue(list.Count > 0);
            foreach (BsOrder item in list)
            {
                Console.WriteLine(ModelToStringUtil.ToString(item));
            }
        }
        #endregion

        #region 测试分页查询订单集合
        [TestMethod]
        public void TestQueryPage()
        {
            PageModel pageModel = new PageModel();
            pageModel.CurrentPage = 1;
            pageModel.PageSize = 10;

            List<BsOrder> list = m_BsOrderDal.GetListPage(ref pageModel, 0, null, DateTime.MinValue, DateTime.Now.AddDays(1));
            Assert.IsTrue(pageModel.TotalRows > 0);

            foreach (BsOrder item in list)
            {
                Console.WriteLine(ModelToStringUtil.ToString(item));
            }
            Console.WriteLine("totalRows=" + pageModel.TotalRows);
        }
        #endregion

        #region 测试AppendIf
        [TestMethod]
        public void TestAppendIf()
        {
            List<BsOrder> list = GetListForTestAppendIf(0, "订单", null, DateTime.Now.AddDays(1), "100001,100002,100003");

            foreach (BsOrder item in list)
            {
                Console.WriteLine(ModelToStringUtil.ToString(item));
            }
        }

        private List<BsOrder> GetListForTestAppendIf(int? status, string remark, DateTime? startTime, DateTime? endTime, string ids)
        {
            var session = DapperLiteFactory.GetSession();
            session.OnExecuting = (s, p) => Console.WriteLine(s); //打印SQL

            ISqlString sql = session.Sql(@"
                select t.*, u.real_name as OrderUserRealName
                from bs_order t
                left join sys_user u on t.order_userid=u.id");

            sql.AppendIf(status.HasValue, " and t.status=@status", status);

            sql.AppendIf(!string.IsNullOrWhiteSpace(remark), " and t.remark like @remark", "%" + remark + "%");

            sql.AppendIf(startTime.HasValue, " and t.order_time >= @startTime ", startTime);

            sql.AppendIf(endTime.HasValue, " and t.order_time <= @endTime ", endTime);

            sql.Append(" and t.id in @ids ", sql.ForList(ids.Split(',').ToList()));

            sql.Append(" order by t.order_time desc, t.id asc ");

            Assert.IsFalse(sql.SQL.Contains("and t.order_time >="));
            Assert.IsTrue(sql.SQL.Contains("and t.order_time <="));

            List<BsOrder> list = session.QueryList<BsOrder>(sql);
            return list;
        }
        #endregion

        #region 测试查询订单集合(使用 ForContains、ForStartsWith、ForEndsWith、ForDateTime、ForList 等辅助方法)
        [TestMethod]
        public void TestQueryExt()
        {
            List<BsOrder> list = m_BsOrderDal.GetListExt(0, "订单", DateTime.MinValue, DateTime.Now.AddDays(1), "100001,100002,100003");

            Assert.IsTrue(list.Count > 0);
            foreach (BsOrder item in list)
            {
                Console.WriteLine(ModelToStringUtil.ToString(item));
            }
        }
        #endregion

        #region 测试Append方法传匿名对象
        [TestMethod]
        public void TestAppendWithAnonymous()
        {
            var session = DapperLiteFactory.GetSession();
            session.OnExecuting = (s, p) => Console.WriteLine(s); //打印SQL

            ISqlString sql = session.Sql(@"
                select * from sys_user t where t.id <= @Id", new { Id = 20 });

            sql.Append(@" and t.create_userid = @userId 
                and t.password = @password", new { userId = "1", password = "123456" });

            long id = session.Sql("select id from sys_user where id=@Id", new { Id = 1 }).QuerySingle<long>();
            Assert.IsTrue(id == 1);

            List<SysUser> list = session.QueryList<SysUser>(sql);
            foreach (SysUser item in list)
            {
                Console.WriteLine(ModelToStringUtil.ToString(item));
            }
            Assert.IsTrue(list.Count > 0);
        }
        #endregion

        #region 测试最佳实践
        [TestMethod]
        public void TestBestCode()
        {
            DateTime? startTime = null;

            var session = DapperLiteFactory.GetSession();
            session.OnExecuting = (s, p) => Console.WriteLine(s); //打印SQL

            List<SysUser> list = session.Sql(@"
                 select * from sys_user t where t.id <= @Id", new { Id = 20 })

                .Append(@" and t.create_userid = @CreateUserId 
                    and t.password like @Password
                    and t.id in @Ids",
                    new
                    {
                        CreateUserId = "1",
                        Password = "%345%",
                        Ids = session.ForList(new List<int> { 1, 2, 9, 10, 11 })
                    })

                .AppendIf(startTime.HasValue, " and t.create_time >= @StartTime ", new { StartTime = startTime })

                .Append(" and t.create_time <= @EndTime ", new { EndTime = new DateTime(2022, 8, 1) })

                .QueryList<SysUser>();

            long id = session.Sql("select id from sys_user where id=@Id", new { Id = 1 })
                .QuerySingle<long>();
            Assert.IsTrue(id == 1);

            foreach (SysUser item in list)
            {
                Console.WriteLine(ModelToStringUtil.ToString(item));
            }
            Assert.IsTrue(list.Count > 0);
        }
        #endregion

        #region 测试相同参数名称问题
        [TestMethod]
        public void TestSameParam()
        {

        }
        #endregion

        #region 测试IDapperLiteClient扩展接口
        [TestMethod]
        public void TestIDapperLiteClient()
        {
            var db = DapperLiteFactory.Db;

            db.OnExecuting = (s, p) =>
            {
                Console.WriteLine(s); //打印SQL
            };

            string remark = "测试";

            List<BsOrder> list = db.Queryable<BsOrder>()

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

        #region 测试IDapperLiteClient扩展接口2
        [TestMethod]
        public void TestIDapperLiteClient2()
        {
            var db = DapperLiteFactory.Db;
            db.OnExecuting = (s, p) => Console.WriteLine(s); //打印SQL

            ISqlString sql = db.Sql(@"
                select * from sys_user t where t.id <= @Id", new { Id = 20 });

            sql.Append(@" and t.create_userid = @userId 
                and t.password = @password", new { userId = "1", password = "123456" });

            long id = db.Sql("select id from sys_user where id=@Id", new { Id = 1 }).QuerySingle<long>();
            Assert.IsTrue(id == 1);

            List<SysUser> list = sql.QueryList<SysUser>();
            foreach (SysUser item in list)
            {
                Console.WriteLine(ModelToStringUtil.ToString(item));
            }
            Assert.IsTrue(list.Count > 0);
        }
        #endregion

        #region 测试SQL查询拼接Lambda
        [TestMethod]
        public void TestSqlQueryAppendLambda()
        {
            var session = DapperLiteFactory.GetSession();

            session.OnExecuting = (s, p) =>
            {
                Console.WriteLine(s);
            };

            List<BsOrder> list = session
                .Sql<BsOrder>(@"
                    select o.*, u.user_name as OrderUserName, u.real_name as OrderUserRealName 
                    from bs_order o
                    left join sys_user u on u.id=o.order_userid")
                .Where(o => o.Status == int.Parse("0")
                    && o.Status == new BsOrder().Status
                    && o.Remark.Contains("订单")
                    && o.Remark != null
                    && o.OrderTime >= new DateTime(2010, 1, 1)
                    && o.OrderTime <= DateTime.Now.AddDays(1))
                .Where<SysUser>(u => u.Id == 10)
                .Append(" order by o.order_time desc, o.id asc")
                .ToList();

            foreach (BsOrder item in list)
            {
                Console.WriteLine(ModelToStringUtil.ToString(item));
            }
            Assert.IsTrue(list.Count > 0);
        }
        #endregion

        #region 测试SQL查询拼接Lambda
        [TestMethod]
        public void TestSqlQueryAppendLambda2()
        {
            var session = DapperLiteFactory.GetSession();

            session.OnExecuting = (s, p) =>
            {
                Console.WriteLine(s);
            };

            List<BsOrder> list = session
                .Sql<BsOrder>(@"
                    select o.*, u.user_name as OrderUserName, u.real_name as OrderUserRealName 
                    from bs_order o
                    left join sys_user u on u.id=o.order_userid")
                .Where(o => o.Status == int.Parse("0")
                    && o.Status == new BsOrder().Status
                    && o.Remark.Contains("订单")
                    && o.Remark != null
                    && o.OrderTime >= new DateTime(2010, 1, 1)
                    && o.OrderTime <= DateTime.Now.AddDays(1))
                .Where<SysUser>(u => u.Id == 10)
                .ToPageList("order by o.order_time desc, o.id asc", 1, 10);

            foreach (BsOrder item in list)
            {
                Console.WriteLine(ModelToStringUtil.ToString(item));
            }
            Assert.IsTrue(list.Count > 0);
        }
        #endregion

        #region 测试SQL查询拼接Lambda
        [TestMethod]
        public void TestSqlQueryAppendLambda3()
        {
            var session = DapperLiteFactory.GetSession();

            session.OnExecuting = (s, p) =>
            {
                Console.WriteLine(s);
            };

            List<BsOrder> list = session
                .Sql<BsOrder>(@"
                    select o.*, u.user_name as OrderUserName, u.real_name as OrderUserRealName 
                    from bs_order o
                    left join sys_user u on u.id=o.order_userid")
                .Where(o => o.Status == 0
                    && o.Remark.Contains("订单")
                    && o.OrderTime >= new DateTime(2010, 1, 1)
                    && o.OrderTime < DateTime.Now.Date.AddDays(1))
                .Where<SysUser>(u => new long[] { 8, 9, 10 }.Contains(u.Id))
                .Append("order by o.order_time desc, o.id asc")
                .ToList();

            foreach (BsOrder item in list)
            {
                Console.WriteLine(ModelToStringUtil.ToString(item));
            }
            Assert.IsTrue(list.Count > 0);
        }
        #endregion

        #region 测试List<string>类型
        [TestMethod]
        public void TestListString()
        {
            var session = DapperLiteFactory.GetSession();

            session.OnExecuting = (s, p) =>
            {
                Console.WriteLine(s);
            };

            var sql = session.Sql("select user_name from sys_user where id<=@Id", 20);
            List<string> list = session.QueryList<string>(sql.SQL, sql.Params);

            foreach (string item in list)
            {
                Console.WriteLine($"{item}");
            }
            Assert.IsTrue(list.Count > 0);
        }
        #endregion

        #region 测试List<string>类型
        [TestMethod]
        public void TestListString2()
        {
            var session = DapperLiteFactory.GetSession();

            session.OnExecuting = (s, p) =>
            {
                Console.WriteLine(s);
            };

            List<string> list = session.Sql("select user_name from sys_user where id<=@Id", 20).ToList<string>();

            foreach (string item in list)
            {
                Console.WriteLine($"{item}");
            }
            Assert.IsTrue(list.Count > 0);
        }
        #endregion

        #region 测试Dynamic类型
        [TestMethod]
        public void TestDynamic()
        {
            var session = DapperLiteFactory.GetSession();

            session.OnExecuting = (s, p) =>
            {
                Console.WriteLine(s);
            };

            var sql = session.Sql("select user_name from sys_user where id<=@Id", 20);
            List<dynamic> list = session.QueryList<dynamic>(sql.SQL, sql.Params);

            foreach (var item in list)
            {
                Console.WriteLine($"{item.user_name}");
            }
            Assert.IsTrue(list.Count > 0);
        }
        #endregion

        #region 测试Dynamic类型
        [TestMethod]
        public void TestDynamic2()
        {
            var session = DapperLiteFactory.GetSession();

            session.OnExecuting = (s, p) =>
            {
                Console.WriteLine(s);
            };

            var sql = session.Sql("select user_name, real_name, remark from sys_user where id<=@Id", 20);
            List<dynamic> list = session.QueryList<dynamic>(sql.SQL, sql.Params);

            foreach (var item in list)
            {
                Console.WriteLine($"{item.user_name}, {item.real_name}, {item.remark}");
            }
            Assert.IsTrue(list.Count > 0);
        }
        #endregion

        #region 测试Dynamic类型
        [TestMethod]
        public void TestDynamic3()
        {
            var session = DapperLiteFactory.GetSession();

            session.OnExecuting = (s, p) =>
            {
                Console.WriteLine(s);
            };

            List<dynamic> list = session.Sql("select user_name, real_name, remark from sys_user where id<=@Id", 20).ToList<dynamic>();

            foreach (var item in list)
            {
                Console.WriteLine($"{item.user_name}, {item.real_name}, {item.remark}");
            }
            Assert.IsTrue(list.Count > 0);
        }
        #endregion

        #region 测试调用存储过程
        [TestMethod]
        public void TestStoredProcedure()
        {
            var session = DapperLiteFactory.GetSession();

            session.OnExecuting = (s, p) =>
            {
                Console.WriteLine(s);
            };

            List<dynamic> list = session.SetCommandType(CommandType.StoredProcedure).Sql("query_user", new { maxId = 20 }).ToList<dynamic>();

            foreach (var item in list)
            {
                Console.WriteLine($"{item.user_name}, {item.real_name}, {item.remark}");
            }
            Assert.IsTrue(list.Count > 0);

            list = session.SetCommandType(CommandType.Text).Sql("select user_name, real_name, remark from sys_user where id<=@Id", 20).ToList<dynamic>();
            Assert.IsTrue(list.Count > 0);
        }
        #endregion

        #region 测试查询并返回DataTable
        [TestMethod]
        public void TestReturnDataTable()
        {
            var session = DapperLiteFactory.GetSession();

            session.OnExecuting = (s, p) =>
            {
                Console.WriteLine(s);
            };

            var sql = session.Sql("select * from sys_user where id<=@Id", 20);
            DataTable dt = session.QueryDataTable(sql.SQL, sql.Params);

            foreach (DataRow dr in dt.Rows)
            {
                Console.WriteLine($"{dr["user_name"]}, {dr["real_name"]}, {dr["remark"]}");
            }
            Assert.IsTrue(dt.Rows.Count > 0);

            var list = sql.ToList<dynamic>();
            Assert.IsTrue(list.Count > 0);
        }
        #endregion

    }
}
