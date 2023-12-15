using Dapper.Lite;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Dapper.LiteTest
{
    /// <summary>
    /// 测试SqlFilter
    /// </summary>
    [TestClass]
    public class SqlFilterTest
    {
        #region 测试SqlFilter
        [TestMethod]
        public void TestSqlFilter()
        {
            string sql = "select id, user_name, _Delete, FDelete from sys_user;delete * from sys_user;delete * from sys_user;";
            var parameters = new object[] { sql };
            TestSqlFilterInternal(ref parameters);

            Console.WriteLine(parameters[0].ToString());
            Assert.AreEqual("select id, user_name, _Delete, FDelete from sys_user;* from sys_user;* from sys_user;", parameters[0].ToString());
        }

        [TestMethod]
        public void TestSqlFilter2()
        {
            string sql = "select id, user_name, _Delete, FDelete from sys_user; delete * from sys_user;";
            var parameters = new object[] { sql };
            TestSqlFilterInternal(ref parameters);

            Console.WriteLine(parameters[0].ToString());
            Assert.AreEqual("select id, user_name, _Delete, FDelete from sys_user; * from sys_user;", parameters[0].ToString());
        }

        [TestMethod]
        public void TestSqlFilter3()
        {
            string sql = "delete * from sys_user;select * from sys_user;";
            var parameters = new object[] { sql };
            TestSqlFilterInternal(ref parameters);

            Console.WriteLine(parameters[0].ToString());
            Assert.AreEqual("delete * from sys_user;* from sys_user;", parameters[0].ToString());
        }

        private void TestSqlFilterInternal(ref object[] parameters)
        {
            var method = typeof(DbSession).GetMethod("SqlFilter", BindingFlags.NonPublic | BindingFlags.Static);
            method.Invoke(null, parameters);
        }
        #endregion

    }
}
