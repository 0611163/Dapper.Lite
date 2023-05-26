using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Models;
using DAL;
using System.Collections.Generic;
using System.Linq;
using Dapper.Lite;
using Utils;
using System.Data.Common;
using System.Threading.Tasks;
using System.Threading;

namespace Dapper.LiteTest
{
    /// <summary>
    /// 多线程并发测试
    /// 一个IDBSession实例对应一个数据库连接，一个IDBSession实例只有一个数据库连接
    /// IDBSession不是线程安全的，不能跨线程使用
    /// 多线程并发的情况，通过LiteSqlFactory在每个线程中创建一个IDBSession实例
    /// </summary>
    [TestClass]
    public class MultiThreadTest
    {
        /// <summary>
        /// 任务数量
        /// </summary>
        private int _count = 1000;

        /// <summary>
        /// 独立线程池
        /// 当任务数量较大，且每个任务都开启一个线程的情况下，如果不限制使用线程数量，线程池被占满后性能非常差乃至报错。
        /// 线程不是越少越好，也不是越多越好。
        /// </summary>
        private TaskSchedulerEx _taskEx = new TaskSchedulerEx(0, 30);

        /// <summary>
        /// true:使用独立线程池,false:使用.NET默认线程池
        /// </summary>
        private bool _useTaskEx = false;

        #region 构造函数
        public MultiThreadTest()
        {
            ThreadPool.SetMaxThreads(1000, 1000);
            ThreadPool.SetMinThreads(200, 200);
            LiteSqlFactory.GetSession(); //预热
        }
        #endregion

        #region RunTask
        private Task RunTask<T>(Action<T> action, T t)
        {
            if (_useTaskEx)
            {
                return _taskEx.Run(obj =>
                {
                    try
                    {
                        action((T)obj);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                        throw;
                    }
                }, t);
            }
            else
            {
                return Task.Factory.StartNew(obj =>
                {
                    try
                    {
                        action((T)obj);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                        throw;
                    }
                }, t);
            }
        }
        #endregion

        #region 多线程并发插入
        [TestMethod]
        public void Test11Insert()
        {
            List<SysUser> userList = new List<SysUser>();
            for (int i = 1; i <= _count; i++)
            {
                SysUser user = new SysUser();
                user.UserName = "testUser";
                user.RealName = "测试插入用户";
                user.Password = "123456";
                user.CreateUserid = "1";
                user.CreateTime = DateTime.Now;
                userList.Add(user);
            }

            List<Task> tasks = new List<Task>();
            foreach (SysUser item in userList)
            {
                var task = RunTask(user =>
                {
                    LiteSqlFactory.GetSession().Insert(user);
                }, item);
                tasks.Add(task);
            }
            Task.WaitAll(tasks.ToArray());

            List<SysUser> list = LiteSqlFactory.GetSession()
                .Queryable<SysUser>().Where(t => t.Id > 20).ToList();
            Assert.IsTrue(list.Count >= _count);
        }
        #endregion

        #region 批量插入(开启事务)
        [TestMethod]
        public void Test21Insert_Tran()
        {
            List<SysUser> userList = new List<SysUser>();
            for (int i = 1; i <= _count; i++)
            {
                SysUser user = new SysUser();
                user.UserName = "testUser";
                user.RealName = "测试插入用户";
                user.Password = "123456";
                user.CreateUserid = "1";
                user.CreateTime = DateTime.Now;
                userList.Add(user);
            }

            var session = LiteSqlFactory.GetSession();

            try
            {
                session.BeginTransaction();
                foreach (SysUser user in userList)
                {
                    session.Insert(user);
                }
                session.CommitTransaction();

                List<SysUser> list = session.Queryable<SysUser>().Where(t => t.Id > 20).ToList();
                Assert.IsTrue(list.Count >= _count);
            }
            catch
            {
                session.RollbackTransaction();
                throw;
            }
        }
        #endregion

        #region 多线程并发更新
        [TestMethod]
        public void Test12Update()
        {
            var session = LiteSqlFactory.GetSession();
            List<SysUser> list = session.Queryable<SysUser>().Where(t => t.Id > 20).ToList();
            Random rnd = new Random();

            session.AttachOld(list);
            foreach (SysUser user in list)
            {
                user.Remark = "1测试修改用户" + rnd.Next(1, 10000);
                user.UpdateUserid = "1";
                user.UpdateTime = DateTime.Now;
            }

            List<Task> tasks = new List<Task>();
            foreach (SysUser item in list)
            {
                var task = RunTask(user =>
                {
                    LiteSqlFactory.GetSession().Update(user);
                }, item);
                tasks.Add(task);
            }
            Task.WaitAll(tasks.ToArray());

            list = session.Queryable<SysUser>().Where(t => t.Id > 20 && t.Remark.Contains("1测试修改用户")).ToList();
            Assert.IsTrue(list.Count >= _count);
        }
        #endregion

        #region 批量更新(开启事务)
        [TestMethod]
        public void Test12Update_Tran()
        {
            var session = LiteSqlFactory.GetSession();
            List<SysUser> list = session.Queryable<SysUser>().Where(t => t.Id > 20).ToList();
            Random rnd = new Random();

            try
            {
                session.AttachOld(list);
                foreach (SysUser user in list)
                {
                    user.Remark = "2测试修改用户" + rnd.Next(1, 10000);
                    user.UpdateUserid = "1";
                    user.UpdateTime = DateTime.Now;
                }

                session.BeginTransaction();
                foreach (SysUser user in list)
                {
                    session.Update(user);
                }
                session.CommitTransaction();

                list = session.Queryable<SysUser>().Where(t => t.Id > 20 && t.Remark.Contains("2测试修改用户")).ToList();
                Assert.IsTrue(list.Count >= _count);
            }
            catch
            {
                session.RollbackTransaction();
                throw;
            }
        }
        #endregion

        #region 多线程并发删除(不带事务)
        [TestMethod]
        public void Test19Delete()
        {
            var session = LiteSqlFactory.GetSession();
            List<SysUser> list = session.Queryable<SysUser>().Where(t => t.Id > 20).ToList();
            Random rnd = new Random();

            List<Task> tasks = new List<Task>();
            foreach (SysUser item in list)
            {
                var task = RunTask(user =>
                {
                    LiteSqlFactory.GetSession().DeleteById<SysUser>(user.Id);
                }, item);
                tasks.Add(task);
            }
            Task.WaitAll(tasks.ToArray());

            long count = session.Queryable<SysUser>().Where(t => t.Id > 20).Count();
            Assert.IsTrue(count == 0);
        }
        #endregion

        #region 逐个删除(开启事务)
        [TestMethod]
        public void Test29Delete_Tran()
        {
            var session = LiteSqlFactory.GetSession();
            List<SysUser> list = session.Queryable<SysUser>().Where(t => t.Id > 20).ToList();
            Random rnd = new Random();

            try
            {
                session.BeginTransaction();
                foreach (SysUser user in list)
                {
                    session.DeleteById<SysUser>(user.Id);
                }
                session.CommitTransaction();
            }
            catch
            {
                session.RollbackTransaction();
                throw;
            }

            long count = session.Queryable<SysUser>().Where(t => t.Id > 20).Count();
            Assert.IsTrue(count == 0);
        }
        #endregion

        #region 多线程并发查询
        [TestMethod]
        public void Test13Query()
        {
            List<Task> tasks = new List<Task>();
            for (int i = 0; i < 100; i++)
            {
                var task = RunTask(obj =>
                {
                    var session = LiteSqlFactory.GetSession();
                    List<SysUser> list = session.Queryable<SysUser>().Where(t => t.Id <= 20).ToList();
                    Assert.IsTrue(list.Count > 0);
                    if (obj == 0)
                    {
                        foreach (SysUser item in list)
                        {
                            Console.WriteLine(ModelToStringUtil.ToString(item));
                        }
                    }
                }, i);
                tasks.Add(task);
            }
            Task.WaitAll(tasks.ToArray());

            List<SysUser> list2 = LiteSqlFactory.GetSession().Queryable<SysUser>().Where(t => t.Id > 20).ToList();
            Assert.IsTrue(list2.Count >= _count);
        }
        #endregion

    }
}
