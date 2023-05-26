using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dapper.Lite
{
    public partial interface IDbSession
    {
        /// <summary>
        /// 根据Id删除
        /// </summary>
        int DeleteById<T>(long id);

        /// <summary>
        /// 根据Id删除
        /// </summary>
        int DeleteById<T>(int id);

        /// <summary>
        /// 根据Id删除
        /// </summary>
        int DeleteById<T>(string id);

        /// <summary>
        /// 根据Id删除
        /// </summary>
        Task<int> DeleteByIdAsync<T>(long id);

        /// <summary>
        /// 根据Id删除
        /// </summary>
        Task<int> DeleteByIdAsync<T>(int id);

        /// <summary>
        /// 根据Id删除
        /// </summary>
        Task<int> DeleteByIdAsync<T>(string id);

        /// <summary>
        /// 根据Id集合删除
        /// </summary>
        int BatchDeleteByIds<T>(string ids);

        /// <summary>
        /// 根据Id集合删除
        /// </summary>
        Task<int> BatchDeleteByIdsAsync<T>(string ids);

        /// <summary>
        /// 根据条件删除
        /// </summary>
        int DeleteByCondition<T>(string conditions);

        /// <summary>
        /// 根据条件删除
        /// </summary>
        Task<int> DeleteByConditionAsync<T>(string condition);

        /// <summary>
        /// 根据条件删除
        /// </summary>
        int DeleteByCondition(Type type, string conditions);

        /// <summary>
        /// 根据条件删除
        /// </summary>
        Task<int> DeleteByConditionAsync(Type type, string condition);

        /// <summary>
        /// 根据条件删除
        /// </summary>
        int DeleteByCondition<T>(string conditions, DbParameter[] cmdParms);

        /// <summary>
        /// 根据条件删除
        /// </summary>
        Task<int> DeleteByConditionAsync<T>(string condition, DbParameter[] cmdParms);

        /// <summary>
        /// 根据条件删除
        /// </summary>
        int DeleteByCondition(Type type, string conditions, DbParameter[] cmdParms);

        /// <summary>
        /// 根据条件删除
        /// </summary>
        Task<int> DeleteByConditionAsync(Type type, string condition, DbParameter[] cmdParms);

        /// <summary>
        /// 根据条件删除
        /// </summary>
        int DeleteByCondition<T>(ISqlString sql);

        /// <summary>
        /// 根据条件删除
        /// </summary>
        Task<int> DeleteByConditionAsync<T>(ISqlString sql);

        /// <summary>
        /// 根据条件删除
        /// </summary>
        int DeleteByCondition(Type type, SqlString sql);

        /// <summary>
        /// 根据条件删除
        /// </summary>
        Task<int> DeleteByConditionAsync(Type type, SqlString sql);
    }
}
