using DAL;
using Dapper.Lite;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace Dapper.LiteTest
{
    /// <summary>
    /// 测试 byte[] Guid char 等字段类型
    /// </summary>
    [TestClass]
    public class ValuesTest
    {
        #region 变量
        private Random _rnd = new Random();
        private IDbSession session = DapperLiteFactory.GetSession();
        #endregion

        #region 构造函数
        public ValuesTest()
        {
            session.Queryable<ValuesInfo>().Count();

            session.OnExecuting = (s, p) =>
            {
                Console.WriteLine(s);
            };
        }
        #endregion

        #region 测试添加
        [TestMethod]
        public void Test1Insert()
        {
            ValuesInfo info = new ValuesInfo();
            info.BytesValue = ASCIIEncoding.UTF8.GetBytes("字段类型测试");
            info.ByteValue = (byte)123;
            info.GuidValue = Guid.NewGuid();
            info.CharValue = 'A';
            info.CharsValue = "ABC";
            info.BoolValue = true;
            long id = session.InsertReturnId(info, "select @@IDENTITY");

            bool bl = session.Queryable<ValuesInfo>().Where(t => t.Id == id).Exists();
            Assert.IsTrue(bl);
        }
        #endregion

        #region 测试修改
        [TestMethod]
        public void Test1Update()
        {
            ValuesInfo info = session.Queryable<ValuesInfo>().Where(t => t.Id == 10).First();
            session.AttachOld(info);
            info.BytesValue = ASCIIEncoding.UTF8.GetBytes("字段类型测试修改");
            info.ByteValue = (byte)123;
            info.GuidValue = Guid.NewGuid();
            info.CharValue = 'B';
            info.CharsValue = "DEF";
            info.BoolValue = true;
            session.Update(info);

            ValuesInfo newInfo = session.Queryable<ValuesInfo>().Where(t => t.Id == info.Id).First();
            Assert.IsTrue(newInfo.CharsValue == "DEF");
        }
        #endregion

        #region 测试删除
        [TestMethod]
        public void Test9Delete()
        {
            session.Queryable<ValuesInfo>().Where(t => t.Id > 10).Delete();
            long count = session.Queryable<ValuesInfo>().Count();
            Assert.IsTrue(count <= 10);
        }
        #endregion

        #region 测试查询
        [TestMethod]
        public void Test2QuerySingle()
        {
            var guidValue = new Guid("181d4961-2b35-4a18-96a7-9334740160ba");
            ValuesInfo2 info = session.Queryable<ValuesInfo2>().Where(t => t.GuidValue == new Guid("181d4961-2b35-4a18-96a7-9334740160ba")).First();
            Assert.IsTrue(info != null);

            Console.WriteLine(ModelToStringUtil.ToString(info));
        }
        #endregion

        #region 测试查询
        [TestMethod]
        public void Test3Query()
        {
            List<ValuesInfo2> list = session.Queryable<ValuesInfo2>().ToList();
            Assert.IsTrue(list.Count > 0);

            foreach (ValuesInfo2 item in list)
            {
                Console.WriteLine(ModelToStringUtil.ToString(item));
            }
        }
        #endregion

    }
}
