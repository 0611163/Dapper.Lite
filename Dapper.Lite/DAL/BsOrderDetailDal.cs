using Dapper.Lite;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace DAL
{
    /// <summary>
    /// 订单明细
    /// </summary>
    public class BsOrderDetailDal
    {
        #region 根据订单ID查询订单明细集合
        /// <summary>
        /// 根据订单ID查询订单明细集合
        /// </summary>
        public List<BsOrderDetail> GetListByOrderId(string orderId)
        {
            BsOrderDal m_BsOrderDetailDal = ServiceHelper.Get<BsOrderDal>(); //该行代码用于测试DAL相互引用，运行不报错即为通过测试

            var session = DapperLiteFactory.GetSession();

            ISqlString sql = session.Sql("select * from bs_order_detail where order_id=@orderId order by order_num", orderId);

            return session.QueryList<BsOrderDetail>(sql);
        }
        #endregion

    }
}
