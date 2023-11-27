using Dapper.Lite;
using Models;
using Provider;

IDapperLite db = new DapperLite(
    "Data Source=localhost;Port=3306;User ID=root;Password=123456;Initial Catalog=litesql_test;Charset=utf8mb4;SslMode=none;Allow User Variables=True;",
    new MySQLProvider());

IDbSession session = db.GetSession();

List<SysUser> list = session.Queryable<SysUser>().Where(t => t.Id <= 20 && t.Remark.Contains("测试")).ToList(); //单表查询支持Lambda

list = session.Sql("select * from sys_user where id <= @Id and remark like @Remark", 20, "%测试%").ToList<SysUser>(); //参数按顺序来，一两个也不容易眼花

list = session.Sql("select * from sys_user where id <= @Id and remark like @Remark", new { Id = 20, Remark = "%测试%" }).ToList<SysUser>(); //参数多的话就这么写吧

List<BsOrder> orders = session
    .Sql<BsOrder>(@"
        select t.*, u.user_name as OrderUserName 
        from bs_order t
        left join sys_user u on u.id = t.order_userid")
    .Where(t => t.Amount > 100 && t.OrderTime > new DateTime(2022, 1, 1))
    .Where<SysUser>(u => u.RealName == "张三")
    .ToList();

Console.WriteLine("完成");
Console.ReadLine();
