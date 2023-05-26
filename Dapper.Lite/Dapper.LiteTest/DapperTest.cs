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

    }
}
