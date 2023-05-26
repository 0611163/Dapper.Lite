using System;
using Dapper.Lite;
using Models;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace DAL
{
    /// <summary>
    /// 用户
    /// </summary>
    public class SysUserDal
    {
        #region 根据ID查询单个记录
        /// <summary>
        /// 根据ID查询单个记录
        /// </summary>
        public SysUser Get(long id)
        {
            var session = LiteSqlFactory.GetSession();

            return session.QueryById<SysUser>(id);
        }
        #endregion

        #region 查询总数
        /// <summary>
        /// 根据ID查询单个记录
        /// </summary>
        public int GetTotalCount()
        {
            var session = LiteSqlFactory.GetSession();

            return session.QuerySingle<int>("select count(*) from sys_user");
        }
        #endregion

        #region 查询集合
        /// <summary>
        /// 查询集合
        /// </summary>
        public List<SysUser> GetList(string sql)
        {
            var session = LiteSqlFactory.GetSession();

            return session.QueryList<SysUser>(sql);
        }
        #endregion

        #region 添加
        /// <summary>
        /// 添加
        /// </summary>
        public long Insert(SysUser info)
        {
            var session = LiteSqlFactory.GetSession();

            session.OnExecuting = (s, p) => Console.WriteLine(s); //打印SQL

            info.CreateTime = DateTime.Now;
            long id = session.InsertReturnId(info, "select @@IDENTITY");
            return id;
        }
        #endregion

        #region 添加(异步)
        /// <summary>
        /// 添加
        /// </summary>
        public async Task<long> InsertAsync(SysUser info)
        {
            var session = LiteSqlFactory.GetSession();

            session.OnExecuting = (s, p) => Console.WriteLine(s); //打印SQL

            info.CreateTime = DateTime.Now;
            long id = await session.InsertReturnIdAsync(info, "select @@IDENTITY");
            return id;
        }
        #endregion

        #region 批量添加
        /// <summary>
        /// 批量添加
        /// </summary>
        public void Insert(List<SysUser> list)
        {
            list.ForEach(item => item.CreateTime = DateTime.Now);

            var session = LiteSqlFactory.GetSession();

            try
            {
                session.OnExecuting = (s, p) => Console.WriteLine(s); //打印SQL

                session.BeginTransaction();
                session.Insert(list);
                session.CommitTransaction();
            }
            catch
            {
                session.RollbackTransaction();
                throw;
            }
        }
        #endregion

        #region 批量添加(异步)
        /// <summary>
        /// 批量添加
        /// </summary>
        public async Task InsertAsync(List<SysUser> list)
        {
            list.ForEach(item => item.CreateTime = DateTime.Now);

            var session = LiteSqlFactory.GetSession();

            await session.InsertAsync(list);
        }
        #endregion

        #region 修改
        /// <summary>
        /// 修改
        /// </summary>
        public void Update(SysUser info)
        {
            var session = LiteSqlFactory.GetSession();

            info.UpdateTime = DateTime.Now;
            session.Update(info);
        }
        #endregion

        #region 修改(异步)
        /// <summary>
        /// 修改
        /// </summary>
        public async Task UpdateAsync(SysUser info)
        {
            var session = LiteSqlFactory.GetSession();

            session.OnExecuting = (s, p) => Console.WriteLine(s); //打印SQL

            info.UpdateTime = DateTime.Now;
            var task = session.UpdateAsync(info);
            await task;
        }
        #endregion

        #region 批量修改
        /// <summary>
        /// 批量修改
        /// </summary>
        public void Update(List<SysUser> list)
        {
            list.ForEach(item => item.UpdateTime = DateTime.Now);

            var session = LiteSqlFactory.GetSession();

            try
            {
                session.OnExecuting = (s, p) => Console.WriteLine(s); //打印SQL

                session.BeginTransaction();
                session.Update(list);
                session.CommitTransaction();
            }
            catch
            {
                session.RollbackTransaction();
                throw;
            }
        }
        #endregion

        #region 批量修改(异步)
        /// <summary>
        /// 批量修改
        /// </summary>
        public async Task UpdateAsync(List<SysUser> list)
        {
            list.ForEach(item => item.UpdateTime = DateTime.Now);

            var session = LiteSqlFactory.GetSession();

            var task = session.UpdateAsync(list);
            await task;
        }
        #endregion

        #region 删除
        /// <summary>
        /// 删除
        /// </summary>
        public void Delete(long id)
        {
            var session = LiteSqlFactory.GetSession();

            session.OnExecuting = (s, p) =>
            {
                Console.WriteLine(s); //打印SQL
            };

            session.DeleteById<SysUser>(id);
        }
        #endregion

    }
}
