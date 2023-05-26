using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Models
{
    /// <summary>
    /// 分页
    /// </summary>
    [Serializable]
    public class PageModel
    {
        #region 字段
        /// <summary>
        /// 当前页数
        /// </summary>
        public int CurrentPage { get; set; }
        /// <summary>
        /// 每页记录数
        /// </summary>
        public int PageSize { get; set; }
        /// <summary>
        /// 总页数
        /// </summary>
        public long PageCount { get; set; }
        /// <summary>
        /// 排序字段
        /// </summary>
        public string Sort { get; set; }
        /// <summary>
        /// 排序的方式asc，desc
        /// </summary>
        public string Order { get; set; }
        /// <summary>
        /// 查询结果
        /// </summary>
        public object Result { get; set; }
        /// <summary>
        /// 记录总数
        /// </summary>
        public long TotalRows { get; set; }
        #endregion

        #region 构造函数
        public PageModel()
        {

        }

        /// <summary>
        /// 分页
        /// </summary>
        /// <param name="page">当前页数</param>
        /// <param name="rows">每页记录数</param>
        public PageModel(int page, int rows)
        {
            this.CurrentPage = page;
            this.PageSize = rows;
        }
        #endregion

        #region 扩展字段
        /// <summary>
        /// 上一页
        /// </summary>
        public long PrePage
        {
            get
            {
                if (CurrentPage - 1 > 0)
                {
                    return CurrentPage - 1;
                }
                return 1;
            }
        }
        /// <summary>
        /// 下一页
        /// </summary>
        public long NextPage
        {
            get
            {
                if (CurrentPage + 1 < PageCount)
                {
                    return CurrentPage + 1;
                }
                return PageCount;
            }
        }
        #endregion

        #region 方法
        /// <summary>
        /// 获取查询结果
        /// </summary>
        public List<T> GetResult<T>()
        {
            if (Result == null)
            {
                return new List<T>();
            }
            else
            {
                return Result as List<T>;
            }
        }
        #endregion

    }
}