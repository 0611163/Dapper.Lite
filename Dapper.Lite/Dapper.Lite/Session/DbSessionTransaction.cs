﻿using System.Data;
using System.Data.Common;

namespace Dapper.Lite
{
    public partial class DbSession : IDbSession
    {
        #region 开始事务
        /// <summary>
        /// 开始事务
        /// </summary>
        public DbTransaction BeginTransaction()
        {
            var conn = GetConnection();
            if (conn.State == ConnectionState.Closed) conn.Open();
            _tran = conn.BeginTransaction();
            return _tran;
        }
        #endregion

        #region 提交事务
        /// <summary>
        /// 提交事务
        /// </summary>
        public void CommitTransaction()
        {
            if (_tran == null) return; //防止重复提交

            try
            {
                _tran.Commit();
            }
            catch
            {
                _tran.Rollback();
                throw;
            }
            finally
            {
                _tran.Connection.Close();
                _tran.Dispose();
            }
        }
        #endregion

        #region 回滚事务(出错时调用该方法回滚)
        /// <summary>
        /// 回滚事务(出错时调用该方法回滚)
        /// </summary>
        public void RollbackTransaction()
        {
            if (_tran == null) return; //防止重复回滚

            _tran.Rollback();
        }
        #endregion

    }
}
