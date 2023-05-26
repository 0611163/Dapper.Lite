using Microsoft.VisualStudio.TestTools.UnitTesting;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PostgreSQLTest
{
    public partial class PostgreSQLTest
    {
        [TestMethod]
        public void Test9Delete()
        {
            try
            {
                var session = LiteSqlFactory.GetSession();

                session.OnExecuting = (s, p) => Console.WriteLine(s); //打印SQL

                int count1 = session.QuerySingle<int>("select count(*) from sys_user");
                Assert.IsTrue(count1 > 0);

                session.DeleteByCondition<SysUser>("1=1");

                int count2 = session.QuerySingle<int>("select count(*) from sys_user");
                Assert.AreEqual(0, count2);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            }
        }
    }
}
