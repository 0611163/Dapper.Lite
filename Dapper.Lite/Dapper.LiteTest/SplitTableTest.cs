using DAL;
using Dapper.Lite;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Utils;

namespace Dapper.LiteTest
{
    /// <summary>
    /// 手动分表测试
    /// </summary>
    [TestClass]
    public class SplitTableTest
    {
        #region 变量
        private BsOrderDal m_BsOrderDal = ServiceHelper.Get<BsOrderDal>();
        private Random _rnd = new Random();
        #endregion

        #region 构造函数
        public SplitTableTest()
        {
            m_BsOrderDal.Preheat();
        }
        #endregion

        #region 插入测试
        public long Test1InsertInternal(long start, long index)
        {
            SysUser user = new SysUser();
            user.UserName = "testUser";
            user.RealName = "测试插入分表数据";
            user.Remark = (start + index).ToString();
            user.Password = "123456";
            user.CreateUserid = "1";
            user.CreateTime = DateTime.Now;

            SplitTableMapping splitTableMapping = new SplitTableMapping(typeof(SysUser), "sys_user_202208");

            var session = LiteSqlFactory.GetSession(splitTableMapping);

            session.OnExecuting = (s, p) => Console.WriteLine(s); //打印SQL

            user.Id = session.InsertReturnId(user, "select @@IDENTITY");
            Console.WriteLine("插入成功, user.Id=" + user.Id);

            SysUser userInfo = session.Sql(
                "select * from sys_user_202208 where id = @Id", new { user.Id })
                .Query<SysUser>();
            Assert.IsTrue(userInfo != null);

            Assert.IsTrue(userInfo.Remark == (start + index).ToString());
            return user.Id;

        }

        [TestMethod]
        public void Test1Insert()
        {
            long start;
            var session = LiteSqlFactory.GetSession();

            start = session.QuerySingle<long>("select max(id) from sys_user_202208");

            Test1InsertInternal(start, 0);
        }

        [TestMethod]
        public void Test1InsertBatch()
        {
            ConcurrentDictionary<long, long> dict = new ConcurrentDictionary<long, long>();
            long start;
            var session = LiteSqlFactory.GetSession();

            start = session.QuerySingle<long>("select max(id)+1 from sys_user_202208");

            ThreadPool.SetMinThreads(100, 100);
            List<Task> taskList = new List<Task>();

            for (int i = 0; i < 100; i++)
            {
                Task task = Task.Factory.StartNew(obj =>
                {
                    int index = (int)obj;
                    long id = Test1InsertInternal(start, index);
                    dict.TryAdd(id, id);
                }, i);
                taskList.Add(task);
            }

            Task.WaitAll(taskList.ToArray());
            Assert.IsTrue(dict.Count == 100);
        }
        #endregion

        #region 修改测试
        [TestMethod]
        public void Test2Update()
        {
            long userId = 10;
            SysUser user = null;

            SplitTableMapping splitTableMapping = new SplitTableMapping(typeof(SysUser), "sys_user_202208");

            var session = LiteSqlFactory.GetSession(splitTableMapping);

            user = session.QueryById<SysUser>(userId);

            if (user != null)
            {
                session.OnExecuting = (s, p) => Console.WriteLine(s); //打印SQL

                session.AttachOld(user);

                user.UpdateUserid = "1";
                user.Remark = "测试修改分表数据" + _rnd.Next(1, 100);
                user.UpdateTime = DateTime.Now;

                session.Update(user);

                SysUser userInfo = session.Sql(
                    "select * from sys_user_202208 where Remark like @Remark", new { Remark = "测试修改分表数据%" })
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

        #region 删除测试
        [TestMethod]
        public void Test4Delete()
        {
            SplitTableMapping splitTableMapping = new SplitTableMapping(typeof(SysUser), "sys_user_202208");
            var session = LiteSqlFactory.GetSession(splitTableMapping);

            session.OnExecuting = (s, p) => Console.WriteLine(s); //打印SQL

            int deleteCount = session.DeleteByCondition<SysUser>(string.Format("id>20"));
            Console.WriteLine(deleteCount + "条数据已删除");
            int deleteCount2 = session.DeleteById<SysUser>(10000);
            Console.WriteLine(deleteCount2 + "条数据已删除");
        }
        #endregion

        #region 查询测试
        [TestMethod]
        public void Test3Query()
        {
            SplitTableMapping splitTableMapping = new SplitTableMapping(typeof(SysUser), "sys_user_202208");

            List<SysUser> list = new List<SysUser>();
            var session = LiteSqlFactory.GetSession(splitTableMapping);

            session.OnExecuting = (s, p) => Console.WriteLine(s); //打印SQL

            ISqlQueryable<SysUser> sql = session.Queryable<SysUser>();

            list = sql.Where(t => t.Id < 10)
                .OrderBy(t => t.Id)
                .ToList();

            foreach (SysUser item in list)
            {
                Console.WriteLine(ModelToStringUtil.ToString(item));
            }
            Assert.IsTrue(list.Count > 0);
        }
        #endregion

    }
}
