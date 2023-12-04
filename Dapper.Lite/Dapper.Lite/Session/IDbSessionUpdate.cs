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
        /// 修改
        /// </summary>
        void Update(object obj);

        /// <summary>
        /// 修改
        /// </summary>
        Task UpdateAsync(object obj);

        /// <summary>
        /// 批量修改
        /// </summary>
        void Update<T>(List<T> list);

        /// <summary>
        /// 批量修改
        /// </summary>
        Task UpdateAsync<T>(List<T> list);

        /// <summary>
        /// 批量修改
        /// </summary>
        void Update<T>(List<T> list, int pageSize);

        /// <summary>
        /// 批量修改
        /// </summary>
        Task UpdateAsync<T>(List<T> list, int pageSize);

        /// <summary>
        /// 附加更新前的旧数据，只更新数据发生变化的字段
        /// </summary>
        void AttachOld<T>(T obj);

        /// <summary>
        /// 附加更新前的旧数据，只更新数据发生变化的字段
        /// </summary>
        void AttachOld<T>(List<T> objList);
    }
}
