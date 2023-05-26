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
        T QueryById<T>(object id) where T : new();

        /// <summary>
        /// 根据Id查询实体
        /// </summary>
        Task<T> QueryByIdAsync<T>(object id) where T : new();
        #endregion

        #region 根据sql查询实体
        /// <summary>
        /// 根据sql查询实体
        /// </summary>
        T Query<T>(string sql) where T : new();

        /// <summary>
        /// 根据sql查询实体
        /// </summary>
        Task<T> QueryAsync<T>(string sql) where T : new();
        #endregion

        #region 根据sql查询实体(参数化查询)
        /// <summary>
        /// 根据sql查询实体
        /// </summary>
        T Query<T>(string sql, DbParameter[] args) where T : new();

        /// <summary>
        /// 根据sql查询实体
        /// </summary>
        Task<T> QueryAsync<T>(string sql, DbParameter[] args) where T : new();
        #endregion

        #region 根据sql查询实体(传SqlString)
        /// <summary>
        /// 根据sql查询实体
        /// </summary>
        T Query<T>(ISqlString sql) where T : new();

        /// <summary>
        /// 根据sql查询实体
        /// </summary>
        Task<T> QueryAsync<T>(ISqlString sql) where T : new();
        #endregion

    }
}
