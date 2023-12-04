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
        #region 根据Id查询实体
        /// <summary>
        /// 根据Id查询实体
        /// </summary>
        T QueryById<T>(object id);

        /// <summary>
        /// 根据Id查询实体
        /// </summary>
        Task<T> QueryByIdAsync<T>(object id);
        #endregion

        #region 根据sql查询实体
        /// <summary>
        /// 根据sql查询实体
        /// </summary>
        T Query<T>(string sql);

        /// <summary>
        /// 根据sql查询实体
        /// </summary>
        Task<T> QueryAsync<T>(string sql);
        #endregion

        #region 根据sql查询实体(参数化查询)
        /// <summary>
        /// 根据sql查询实体
        /// </summary>
        T Query<T>(string sql, DbParameter[] args);

        /// <summary>
        /// 根据sql查询实体
        /// </summary>
        Task<T> QueryAsync<T>(string sql, DbParameter[] args);
        #endregion

        #region 根据sql查询实体(传SqlString)
        /// <summary>
        /// 根据sql查询实体
        /// </summary>
        T Query<T>(ISqlString sql);

        /// <summary>
        /// 根据sql查询实体
        /// </summary>
        Task<T> QueryAsync<T>(ISqlString sql);
        #endregion

    }
}
