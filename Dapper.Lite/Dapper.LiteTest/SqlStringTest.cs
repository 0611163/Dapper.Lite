using DAL;
using Dapper.Lite;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Utils;

namespace Dapper.LiteTest
{
    [TestClass]
    public class SqlStringTest
    {
        private IDapperLiteClient _db;

        private Regex _regSpace = new Regex(@"[\s]{2,}");

        #region 构造函数
        public SqlStringTest()
        {
            _db = DapperLiteFactory.Db;
        }
        #endregion

        #region TestUpdateSql1
        [TestMethod]
        public void TestUpdateSql1()
        {
            var sql = _db.Sql<SysUser>(@"update sys_user set user_name=@UserName where id=@Id", new { UserName = "abc", Id = 20 });
            Assert.IsTrue("update sys_user set user_name=@UserName where id=@Id" == sql.SQL.Trim());
            Assert.IsTrue(sql.Params.Length == 2);
            var parameters = sql.Params.ToList();
            Assert.IsTrue(parameters.Exists(a => a.ParameterName == "UserName"));
            Assert.IsTrue(parameters.Exists(a => a.ParameterName == "Id"));
        }
        #endregion

        #region TestQuerySql1
        [TestMethod]
        public void TestQuerySql1()
        {
            var sql = _db.Queryable<SysUser>().Where(t => t.Id < 20 && (t.UserName == "admin" || t.UserName == "admin2")).Where(t => t.Id > 10 && t.UserName == "admin3");
            Assert.IsTrue(_regSpace.Replace(sql.SQL.Trim(), " ") == "select t.`id`, t.`user_name`, t.`real_name`, t.`password`, t.`remark`, t.`create_userid`, t.`create_time`, t.`update_userid`, t.`update_time` from `sys_user` t where t.`id` < @Id AND ( t.`user_name` = @UserName OR t.`user_name` = @UserName1 ) and t.`id` > @Id1 AND t.`user_name` = @UserName2");
            Assert.IsTrue(sql.Params.Length == 5);
            var parameters = sql.Params.ToList();
            Assert.IsTrue(parameters.Exists(a => a.ParameterName == "Id"));
            Assert.IsTrue(parameters.Exists(a => a.ParameterName == "Id1"));
            Assert.IsTrue(parameters.Exists(a => a.ParameterName == "UserName"));
            Assert.IsTrue(parameters.Exists(a => a.ParameterName == "UserName1"));
            Assert.IsTrue(parameters.Exists(a => a.ParameterName == "UserName2"));
        }
        #endregion

        #region TestQuerySql2
        [TestMethod]
        public void TestQuerySql2()
        {
            var sql = _db.Queryable<SysUser>().Where(t => t.Remark.Contains("测试") || !t.Remark.Contains("修改")).Where(t => t.Id < 20).Where(t => t.UserName == "admin");
            Assert.IsTrue(_regSpace.Replace(sql.SQL.Trim(), " ") == "select t.`id`, t.`user_name`, t.`real_name`, t.`password`, t.`remark`, t.`create_userid`, t.`create_time`, t.`update_userid`, t.`update_time` from `sys_user` t where (t.`remark` like @Remark OR t.`remark` not like @Remark1) and t.`id` < @Id and t.`user_name` = @UserName");
            Assert.IsTrue(sql.Params.Length == 4);
            var parameters = sql.Params.ToList();
            Assert.IsTrue(parameters.Exists(a => a.ParameterName == "Id"));
            Assert.IsTrue(parameters.Exists(a => a.ParameterName == "UserName"));
            Assert.IsTrue(parameters.Exists(a => a.ParameterName == "Remark"));
            Assert.IsTrue(parameters.Exists(a => a.ParameterName == "Remark1"));
        }
        #endregion

    }
}
