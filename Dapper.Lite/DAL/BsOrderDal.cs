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
    /// 订单
    /// </summary>
    public class BsOrderDal
    {
        #region 预热
        /// <summary>
        /// 预热
        /// </summary>
        public void Preheat()
        {
            var session = LiteSqlFactory.GetSession();
            session.QuerySingle("select count(*) from bs_order");
        }
        #endregion

        #region 添加
        /// <summary>
        /// 添加
        /// </summary>
        public string Insert(BsOrder order, List<BsOrderDetail> detailList)
        {
            var session = LiteSqlFactory.GetSession();

            try
            {
                session.OnExecuting = (s, p) =>
                {
                    Console.WriteLine(s); //打印SQL
                };

                session.BeginTransaction();

                order.Id = Guid.NewGuid().ToString("N");
                order.CreateTime = DateTime.Now;

                decimal amount = 0;
                foreach (BsOrderDetail detail in detailList)
                {
                    detail.Id = Guid.NewGuid().ToString("N");
                    detail.OrderId = order.Id;
                    detail.CreateTime = DateTime.Now;
                    amount += detail.Price * detail.Quantity;
                    session.Insert(detail);
                }
                order.Amount = amount;

                session.Insert(order);

                session.CommitTransaction();

                return order.Id;
            }
            catch (Exception ex)
            {
                session.RollbackTransaction();
                Console.WriteLine(ex.ToString());
                throw;
            }
        }
        #endregion

        #region 添加(异步)
        /// <summary>
        /// 添加
        /// </summary>
        public async Task<string> InsertAsync(BsOrder order, List<BsOrderDetail> detailList)
        {
            var session = await LiteSqlFactory.GetSessionAsync();
            try
            {
                session.BeginTransaction();

                order.Id = Guid.NewGuid().ToString("N");
                order.CreateTime = DateTime.Now;

                decimal amount = 0;
                foreach (BsOrderDetail detail in detailList)
                {
                    detail.Id = Guid.NewGuid().ToString("N");
                    detail.OrderId = order.Id;
                    detail.CreateTime = DateTime.Now;
                    amount += detail.Price * detail.Quantity;
                    await session.InsertAsync(detail);
                }
                order.Amount = amount;

                await session.InsertAsync(order);

                session.CommitTransaction();

                return order.Id;
            }
            catch (Exception ex)
            {
                session.RollbackTransaction();
                Console.WriteLine(ex.ToString());
                throw;
            }
        }
        #endregion

        #region 修改
        /// <summary>
        /// 修改
        /// </summary>
        public string Update(BsOrder order, List<BsOrderDetail> detailList)
        {
            var session = LiteSqlFactory.GetSession();

            try
            {
                session.OnExecuting = (s, p) =>
                {
                    Console.WriteLine(s); //打印SQL
                };

                session.BeginTransaction();

                List<BsOrderDetail> oldDetailList = ServiceHelper.Get<BsOrderDetailDal>().GetListByOrderId(order.Id); //根据订单ID查询旧订单明细

                foreach (BsOrderDetail oldDetail in oldDetailList)
                {
                    if (!detailList.Exists(a => a.Id == oldDetail.Id)) //该旧订单明细已从列表中删除
                    {
                        session.DeleteById<BsOrderDetail>(oldDetail.Id); //删除旧订单明细
                    }
                }

                decimal amount = 0;
                foreach (BsOrderDetail detail in detailList)
                {
                    amount += detail.Price * detail.Quantity;

                    if (oldDetailList.Exists(a => a.Id == detail.Id)) //该订单明细存在
                    {
                        detail.UpdateTime = DateTime.Now;
                        session.Update(detail);
                    }
                    else //该订单明细不存在
                    {
                        detail.Id = Guid.NewGuid().ToString("N");
                        detail.OrderId = order.Id;
                        detail.CreateTime = DateTime.Now;
                        session.Insert(detail);
                    }
                }
                order.Amount = amount;

                order.UpdateTime = DateTime.Now;
                session.Update(order);

                session.CommitTransaction();

                return order.Id;
            }
            catch (Exception ex)
            {
                session.RollbackTransaction();
                Console.WriteLine(ex.ToString());
                throw;
            }
        }
        #endregion

        #region 修改(异步)
        /// <summary>
        /// 修改
        /// </summary>
        public async Task<string> UpdateAsync(BsOrder order, List<BsOrderDetail> detailList)
        {
            var session = LiteSqlFactory.GetSession();

            try
            {
                session.BeginTransaction();

                List<BsOrderDetail> oldDetailList = ServiceHelper.Get<BsOrderDetailDal>().GetListByOrderId(order.Id); //根据订单ID查询旧订单明细

                foreach (BsOrderDetail oldDetail in oldDetailList)
                {
                    if (!detailList.Exists(a => a.Id == oldDetail.Id)) //该旧订单明细已从列表中删除
                    {
                        session.DeleteById<BsOrderDetail>(oldDetail.Id); //删除旧订单明细
                    }
                }

                decimal amount = 0;
                foreach (BsOrderDetail detail in detailList)
                {
                    amount += detail.Price * detail.Quantity;

                    if (oldDetailList.Exists(a => a.Id == detail.Id)) //该订单明细存在
                    {
                        detail.UpdateTime = DateTime.Now;
                        await session.UpdateAsync(detail);
                    }
                    else //该订单明细不存在
                    {
                        detail.Id = Guid.NewGuid().ToString("N");
                        detail.OrderId = order.Id;
                        detail.CreateTime = DateTime.Now;
                        session.Insert(detail);
                    }
                }
                order.Amount = amount;

                order.UpdateTime = DateTime.Now;
                await session.UpdateAsync(order);

                session.CommitTransaction();

                return order.Id;
            }
            catch (Exception ex)
            {
                session.RollbackTransaction();
                Console.WriteLine(ex.ToString());
                throw;
            }
        }
        #endregion

        #region 根据ID查询单个记录
        /// <summary>
        /// 根据ID查询单个记录
        /// </summary>
        public BsOrder Get(string id)
        {
            var session = LiteSqlFactory.GetSession();

            session.OnExecuting = (s, p) =>
            {
                Console.WriteLine(s); //打印SQL
            };

            List<BsOrderDetail> detailList = ServiceHelper.Get<BsOrderDetailDal>().GetListByOrderId(id);

            BsOrder result = session.QueryById<BsOrder>(id);
            result.DetailList = detailList;

            return result;
        }
        #endregion

        #region 查询第一条记录
        /// <summary>
        /// 查询集合
        /// </summary>
        public BsOrder GetFirst()
        {
            var session = LiteSqlFactory.GetSession();

            BsOrder result = session.Query<BsOrder>("select * from bs_order");

            List<BsOrderDetail> detailList = ServiceHelper.Get<BsOrderDetailDal>().GetListByOrderId(result.Id);
            result.DetailList = detailList;

            return result;
        }
        #endregion

        #region 查询集合
        /// <summary>
        /// 查询集合
        /// </summary>
        public List<BsOrder> GetList(int? status, string remark, DateTime? startTime, DateTime? endTime, string ids)
        {
            var session = LiteSqlFactory.GetSession();

            ISqlString sql = session.Sql(@"
                select t.*, u.real_name as OrderUserRealName
                from bs_order t
                left join sys_user u on t.order_userid=u.id
                where 1=1");

            sql.AppendIf(status.HasValue, " and t.status=@status", status);

            sql.AppendIf(!string.IsNullOrWhiteSpace(remark), " and t.remark like @Remark", new { Remark = "%" + remark + "%" });

            sql.AppendIf(startTime.HasValue, " and t.order_time>=@startTime ", startTime);

            sql.AppendIf(endTime.HasValue, " and t.order_time<=@endTime ", endTime);

            int index = 0;
            string[] idArr = ids.Split(',');
            string args = string.Join(",", idArr.ToList().ConvertAll<string>(a => "@id" + index++));
            sql.Append(" and t.id in (" + args + ") ", idArr);

            sql.Append(" order by t.order_time desc, t.id asc ");

            List<BsOrder> list = session.QueryList<BsOrder>(sql);
            return list;
        }
        #endregion

        #region 查询集合(异步查询)
        /// <summary>
        /// 查询集合
        /// </summary>
        public async Task<List<BsOrder>> GetListAsync(int? status, string remark, DateTime? startTime, DateTime? endTime)
        {
            var session = LiteSqlFactory.GetSession();

            ISqlString sql = session.Sql(@"
                select t.*, u.real_name as OrderUserRealName
                from bs_order t
                left join sys_user u on t.order_userid=u.id
                where 1=1");

            sql.AppendIf(status.HasValue, " and t.status=@status", status);

            sql.AppendIf(!string.IsNullOrWhiteSpace(remark), " and t.remark like concat('%',@remark,'%')", remark);

            sql.AppendIf(startTime.HasValue, " and t.order_time>=STR_TO_DATE(@startTime, '%Y-%m-%d %H:%i:%s') ", () => startTime.Value.ToString("yyyy-MM-dd HH:mm:ss"));

            sql.AppendIf(endTime.HasValue, " and t.order_time<=STR_TO_DATE(@endTime, '%Y-%m-%d %H:%i:%s') ", () => endTime.Value.ToString("yyyy-MM-dd HH:mm:ss"));

            sql.Append(" order by t.order_time desc, t.id asc ");

            List<BsOrder> list = await session.QueryListAsync<BsOrder>(sql);
            return list;
        }
        #endregion

        #region 分页查询集合
        /// <summary>
        /// 分页查询集合
        /// </summary>
        public List<BsOrder> GetListPage(ref PageModel pageModel, int? status, string remark, DateTime? startTime, DateTime? endTime)
        {
            var session = LiteSqlFactory.GetSession();

            session.OnExecuting = (s, p) => Console.WriteLine(s);

            ISqlString sql = session.Sql(@"
                select t.*, u.real_name as OrderUserRealName
                from bs_order t
                left join sys_user u on t.order_userid=u.id
                where 1=1");

            sql.AppendIf(status.HasValue, " and t.status=@status", status);

            sql.AppendIf(!string.IsNullOrWhiteSpace(remark), " and t.remark like concat('%',@remark,'%')", remark);

            sql.AppendIf(startTime.HasValue, " and t.order_time>=STR_TO_DATE(@startTime, '%Y-%m-%d %H:%i:%s') ", () => startTime.Value.ToString("yyyy-MM-dd HH:mm:ss"));

            sql.AppendIf(endTime.HasValue, " and t.order_time<=STR_TO_DATE(@endTime, '%Y-%m-%d %H:%i:%s') ", () => endTime.Value.ToString("yyyy-MM-dd HH:mm:ss"));

            sql.Append(" order by t.order_time desc, t.id asc ");

            pageModel.TotalRows = sql.QueryCount();
            List<BsOrder> result = sql.QueryPage<BsOrder>(null, pageModel.PageSize, pageModel.CurrentPage);
            return result;
        }
        #endregion

        #region 分页查询集合(异步查询)
        /// <summary>
        /// 分页查询集合
        /// </summary>
        public async Task<List<BsOrder>> GetListPageAsync(PageModel pageModel, int? status, string remark, DateTime? startTime, DateTime? endTime)
        {
            var session = LiteSqlFactory.GetSession();

            ISqlString sql = session.Sql(@"
                select t.*, u.real_name as OrderUserRealName
                from bs_order t
                left join sys_user u on t.order_userid=u.id
                where 1=1");

            sql.AppendIf(status.HasValue, " and t.status=@status", status);

            sql.AppendIf(!string.IsNullOrWhiteSpace(remark), " and t.remark like concat('%',@remark,'%')", remark);

            sql.AppendIf(startTime.HasValue, " and t.order_time>=STR_TO_DATE(@startTime, '%Y-%m-%d %H:%i:%s') ", () => startTime.Value.ToString("yyyy-MM-dd HH:mm:ss"));

            sql.AppendIf(endTime.HasValue, " and t.order_time<=STR_TO_DATE(@endTime, '%Y-%m-%d %H:%i:%s') ", () => endTime.Value.ToString("yyyy-MM-dd HH:mm:ss"));

            string orderby = " order by t.order_time desc, t.id asc ";

            var countResult = await session.QueryCountAsync(sql, pageModel.PageSize);
            pageModel.TotalRows = countResult.Count;
            return await session.QueryPageAsync<BsOrder>(sql, orderby, pageModel.PageSize, pageModel.CurrentPage);
        }
        #endregion

        #region 查询集合(使用 ForList 辅助方法)
        /// <summary>
        /// 查询集合
        /// </summary>
        public List<BsOrder> GetListExt(int? status, string remark, DateTime? startTime, DateTime? endTime, string ids)
        {
            var session = LiteSqlFactory.GetSession();

            ISqlString sql = session.Sql(@"
                select t.*, u.real_name as OrderUserRealName
                from bs_order t
                left join sys_user u on t.order_userid=u.id
                where 1=1");

            sql.AppendIf(status.HasValue, " and t.status=@status", status);

            sql.AppendIf(!string.IsNullOrWhiteSpace(remark), " and t.remark like @remark", "%" + remark + "%");

            sql.AppendIf(startTime.HasValue, " and t.order_time >= @startTime ", () => startTime);

            sql.AppendIf(endTime.HasValue, " and t.order_time <= @endTime ", () => endTime);

            sql.Append(" and t.id in @ids ", sql.ForList(ids.Split(',').ToList()));

            sql.Append(" order by t.order_time desc, t.id asc ");

            List<BsOrder> list = session.QueryList<BsOrder>(sql);
            return list;
        }
        #endregion

    }
}
