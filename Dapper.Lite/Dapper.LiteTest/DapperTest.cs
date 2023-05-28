using DAL;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL;
using Models;
using Utils;
using Dapper;
using static Dapper.SqlMapper;
using System.Data.Common;
using System.Threading;

namespace Dapper.LiteTest
{
    [TestClass]
    public class DapperTest
    {
        #region 构造函数
        public DapperTest()
        {
            DapperLiteFactory.GetSession();
        }
        #endregion

        #region 测试直接使用Dapper
        [TestMethod]
        public void TestUseDapper()
        {
            var session = DapperLiteFactory.GetSession();

            session.SetTypeMap<SysUser>(); //设置数据库字段名与实体类属性名映射

            using (var conn = session.GetConnection()) //此处从连接池获取连接，用完一定要释放，也可以不使用连接池，直接new MySqlConnection
            {
                DynamicParameters dynamicParameters = new DynamicParameters();
                dynamicParameters.Add("id", 20);

                List<SysUser> list = conn.Conn.Query<SysUser>(@"
                    select *
                    from sys_user 
                    where id < @id", dynamicParameters).ToList();

                foreach (SysUser item in list)
                {
                    Console.WriteLine(ModelToStringUtil.ToString(item));

                    Assert.IsTrue(!string.IsNullOrWhiteSpace(item.UserName));
                }
            }

        }
        #endregion

        #region 测试混合并发使用Dapper和LiteSql
        [TestMethod]
        public void TestUseDapper2()
        {
            ThreadPool.SetMinThreads(50, 50);

            Console.WriteLine("开始");
            List<Task> tasks = new List<Task>();
            for (int i = 0; i < 200; i++)
            {
                var task = Task.Run(() =>
                {
                    try
                    {
                        var session = DapperLiteFactory.GetSession();
                        var list = session.Queryable<SysUser>().Where(t => t.Id < 20).ToList();
                        Console.WriteLine("Dapper.Lite查询成功, count=" + list.Count);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }
                });
                tasks.Add(task);

                var task2 = Task.Run(() =>
                {
                    try
                    {
                        var session = DapperLiteFactory.GetSession();

                        session.SetTypeMap<SysUser>(); //设置数据库字段名与实体类属性名映射

                        using (var conn = session.GetConnection())
                        {
                            DynamicParameters dynamicParameters = new DynamicParameters();
                            dynamicParameters.Add("id", 20);

                            List<SysUser> list = conn.Conn.Query<SysUser>(@"
                                select *
                                from sys_user 
                                where id < @id", dynamicParameters).ToList();

                            Console.WriteLine("Dapper查询成功, count=" + list.Count);

                            foreach (SysUser item in list)
                            {
                                Assert.IsTrue(!string.IsNullOrWhiteSpace(item.UserName));
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }
                });
                tasks.Add(task2);
            }
            Task.WaitAll(tasks.ToArray());
            Console.WriteLine("完成");
        }
        #endregion

        #region 测试直接使用Dapper3
        [TestMethod]
        public void TestUseDapper3()
        {
            var db = DapperLiteFactory.Client;

            db.SetTypeMap<SysUser>(); //设置数据库字段名与实体类属性名映射

            using (var conn = db.GetConnection()) //此处从连接池获取连接，用完一定要释放，也可以不使用连接池，直接new MySqlConnection
            {
                DynamicParameters dynamicParameters = new DynamicParameters();
                dynamicParameters.Add("id", 20);

                List<SysUser> list = conn.Conn.Query<SysUser>(@"
                    select *
                    from sys_user 
                    where id < @id", dynamicParameters).ToList();

                foreach (SysUser item in list)
                {
                    Console.WriteLine(ModelToStringUtil.ToString(item));

                    Assert.IsTrue(!string.IsNullOrWhiteSpace(item.UserName));
                }
            }

        }
        #endregion

        #region 测试直接使用Dapper4
        [TestMethod]
        public void TestUseDapper4()
        {
            var db = DapperLiteFactory.Client;

            db.SetTypeMap<SysUser>(); //设置数据库字段名与实体类属性名映射

            using (var conn = db.GetConnection()) //此处从连接池获取连接，用完一定要释放，也可以不使用连接池，直接new MySqlConnection
            {
                var sql = db.Sql<SysUser>(@"
                    select *
                    from sys_user 
                    where id < @id", 20);

                var list = conn.Conn.Query<SysUser>(sql.SQL, sql.DynamicParameters).ToList();

                foreach (var item in list)
                {
                    Console.WriteLine(ModelToStringUtil.ToString(item));

                    Assert.IsTrue(!string.IsNullOrWhiteSpace(item.UserName));
                }
            }

        }
        #endregion

        #region 测试直接使用Dapper5
        [TestMethod]
        public void TestUseDapper5()
        {
            Random rnd = new Random();
            var remark1 = $"测试修改用户{rnd.Next(1, 10000)}";
            var remark2 = $"测试修改用户{rnd.Next(1, 10000)}";

            var session = DapperLiteFactory.GetSession();

            session.SetTypeMap<SysUser>(); //设置数据库字段名与实体类属性名映射

            using (var conn = session.GetConnection()) //此处从连接池获取连接，用完一定要释放，也可以不使用连接池，直接new MySqlConnection
            {
                try
                {
                    var trans = session.BeginTransaction();

                    var sql1 = session.Sql<SysUser>(@"update sys_user set remark=@Remark where id=@Id", new { Remark = remark1, Id = 1 });
                    var sql2 = session.Sql<SysUser>(@"update sys_user set remark=@Remark where id=@Id", new { Remark = remark2, Id = 2 });
                    trans.Conn.Execute(sql1.SQL, sql1.DynamicParameters);
                    trans.Conn.Execute(sql2.SQL, sql2.DynamicParameters);

                    session.CommitTransaction();
                }
                catch
                {
                    session.RollbackTransaction();
                    throw;
                }

                var sql3 = session.Queryable<SysUser>().Where(t => t.Id == 1);
                var user1 = conn.Conn.QuerySingleOrDefault<SysUser>(sql3.SQL, sql3.DynamicParameters);
                var sql4 = session.Queryable<SysUser>().Where(t => t.Id == 2);
                var user2 = conn.Conn.QuerySingleOrDefault<SysUser>(sql4.SQL, sql4.DynamicParameters);

                Assert.IsTrue(user1.Remark == remark1);
                Assert.IsTrue(user2.Remark == remark2);
            }

        }
        #endregion

    }
}
