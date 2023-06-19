using System.Data;
using System.Data.Common;

namespace Dapper.Lite
{
    public partial class DbSession : IDbSession
    {
        /// <summary>
        /// 事务关联的数据库连接
        /// </summary>
        private DbConnection _connForTran;

        #region 开始事务
        /// <summary>
        /// 开始事务
        /// </summary>
        public DbTransaction BeginTransaction()
        {
            var conn = GetConnection();
            if (conn.State == ConnectionState.Closed) conn.Open();
            try
            {
                _tran = conn.BeginTransaction();
                _connForTran = _tran.Connection;
            }
            catch
            {
                if (conn.State != ConnectionState.Closed) conn.Close();
                _tran = null;
                throw;
            }
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
                RollbackTransaction();
                throw;
            }
            finally
            {
                if (_tran != null)
                {
                    if (_connForTran != null)
                    {
                        if (_connForTran.State != ConnectionState.Closed) _connForTran.Close();
                        _connForTran = null;
                    }
                    _tran.Dispose();
                    _tran = null;
                }
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

            try
            {
                _tran.Rollback();
            }
            finally
            {
                if (_tran != null)
                {
                    if (_connForTran != null)
                    {
                        if (_connForTran.State != ConnectionState.Closed) _connForTran.Close();
                        _connForTran = null;
                    }
                    _tran.Dispose();
                    _tran = null;
                }
            }
        }
        #endregion

    }
}
