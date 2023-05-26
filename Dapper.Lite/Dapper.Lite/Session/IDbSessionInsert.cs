using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dapper.Lite
{
    public partial interface IDbSession
    {
        /// <summary>
        /// 添加
        /// </summary>
        void Insert(object obj);

        /// <summary>
        /// 添加并返回ID
        /// </summary>
        long InsertReturnId(object obj, string selectIdSql);

        /// <summary>
        /// 添加
        /// </summary>
        Task InsertAsync(object obj);

        /// <summary>
        /// 添加并返回ID
        /// </summary>
        Task<long> InsertReturnIdAsync(object obj, string selectIdSql);

        /// <summary>
        /// 批量添加
        /// </summary>
        void Insert<T>(List<T> list);

        /// <summary>
        /// 批量添加
        /// </summary>
        Task InsertAsync<T>(List<T> list);

        /// <summary>
        /// 批量添加
        /// </summary>
        void Insert<T>(List<T> list, int pageSize);

        /// <summary>
        /// 批量添加
        /// </summary>
        Task InsertAsync<T>(List<T> list, int pageSize);

    }
}
