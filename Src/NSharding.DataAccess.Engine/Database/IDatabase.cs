using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSharding.DataAccess.Core
{
    /// <summary>
    /// 数据访问接口
    /// </summary>
    public interface IDatabase
    {
        /// <summary>
        /// 获取数据库服务
        /// </summary>
        /// <param name="sqlStatement">SQL语句</param>
        /// <returns>数据库服务</returns>
        ITeldDatabase GetDatabase(SqlStatement sqlStatement);

        /// <summary>
        /// 获取数据库服务
        /// </summary>
        /// <param name="dataSourceName">数据源</param>
        /// <returns>数据库服务</returns>
        ITeldDatabase GetDatabase(string dataSourceName);

        /// <summary>
        /// 在数据库事务中执行SQL语句返回影响行数
        /// </summary>
        /// <param name="sqls">SQL语句</param>        
        void ExecuteSQLWithTransaction(SqlStatementCollection sqls);

        /// <summary>
        /// 执行SQL获取数据
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <returns>数据</returns>
        DataSet GetData(SqlStatement sql);

        /// <summary>
        /// 执行SQL获取数据
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <returns>数据</returns>
        List<DataTable> GetDataCollection(SqlStatementCollection sql);

        /// <summary>
        /// 执行SQL获取数据的第一行第一列
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <returns>数据的第一行第一列</returns>
        object ExecuteScalar(SqlStatement sql);

        /// <summary>
        /// 执行SQL返回IDataReader
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <returns>IDataReader</returns>
        IDataReader GetDataReader(SqlStatement sql);
    }
}
