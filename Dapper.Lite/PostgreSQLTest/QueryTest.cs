using Dapper.Lite;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace PostgreSQLTest
{
    [TestClass]
    public partial class PostgreSQLTest
    {
        [TestMethod]
        public void Test4Query()
        {
            try
            {
                List<SysUser> list = new List<Models.SysUser>();
                var session = LiteSqlFactory.GetSession();

                session.OnExecuting = (s, p) => Console.WriteLine(s); //打印SQL

                list = session.QueryList<SysUser>("select * from sys_user");

                foreach (SysUser item in list)
                {
                    Console.WriteLine(ModelToStringUtil.ToString(item));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            }
        }

        [TestMethod]
        public void Test5PageQuery()
        {
            try
            {
                var session = LiteSqlFactory.GetSession();

                session.OnExecuting = (s, p) => Console.WriteLine(s); //打印SQL

                PageModel pageModel = new PageModel();
                pageModel.CurrentPage = 2;
                pageModel.PageSize = 5;

                ISqlString sql = session.Sql(@"
                    select t.*
                    from sys_user t
                    where 1=1");

                sql.Append(@" and t.""RealName"" like concat('%',@RealName,'%')", "测试");

                sql.Append(@" and t.""CreateTime"" < @startTime ", DateTime.Now);

                sql.Append(@" and t.""Id"" in @ids ", sql.ForList(new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }));

                string orderby = @" order by t.""Id"" ";
                pageModel.TotalRows = sql.QueryCount();
                List<SysUser> list = sql.QueryPage<SysUser>(orderby, pageModel.PageSize, pageModel.CurrentPage);
                foreach (SysUser item in list)
                {
                    Console.WriteLine(ModelToStringUtil.ToString(item));
                }
                Console.WriteLine("totalRows=" + pageModel.TotalRows);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            }
        }

        /// <summary>
        /// Lambda表达式 单表分页查询
        /// </summary>
        [TestMethod]
        public void Test6QueryByLambda()
        {
            try
            {
                var session = LiteSqlFactory.GetSession();

                session.OnExecuting = (s, p) => Console.WriteLine(s); //打印SQL

                ISqlQueryable<SysUser> sql = session.Queryable<SysUser>();

                string realName = "测试";
                DateTime now = DateTime.Now;
                int height = 160;

                sql.WhereIf(!string.IsNullOrWhiteSpace(realName),
                       t => t.Realname.Contains(realName)
                       && t.Createtime < now
                       && t.Createtime < DateTime.Now
                       && t.Createuserid == "1"
                       && t.Height >= height
                       && t.Height >= 160)
                   .OrderBy(t => t.Id);

                long total = sql.Count();
                List<SysUser> list = sql.ToPageList(1, 20);

                foreach (SysUser item in list)
                {
                    Console.WriteLine(ModelToStringUtil.ToString(item));
                }
                Console.WriteLine("total=" + total);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            }
        }

    }
}
