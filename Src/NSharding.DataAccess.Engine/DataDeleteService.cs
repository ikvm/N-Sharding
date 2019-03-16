using NSharding.DataAccess.Spi;
using NSharding.Sharding.Rule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSharding.DataAccess.Core
{
    /// <summary>
    /// 数据删除服务
    /// </summary>
    class DataDeleteService : IDataDeleteService
    {
        /// <summary>
        /// 解析生成删除SQL
        /// </summary>
        /// <remarks>按主键数据作为查询依据</remarks>
        /// <param name="domainModel">领域模型</param>        
        /// <param name="dataID">主键数据</param>
        /// <param name="shardingKeyValue">分库分表键值对</param>
        /// <returns>删除SQL</returns>
        public void DeleteByID(DomainModel.Spi.DomainModel domainModel, string dataID, ShardingValue shardingKeyValue = null)
        {
            if (domainModel == null)
                throw new ArgumentNullException("DataDeleteService.DeleteByID.domainModel");
            if (string.IsNullOrWhiteSpace(dataID))
                throw new ArgumentNullException("DataDeleteService.DeleteByID.dataID");

            var sqls = SQLBuilderFactory.CreateSQLBuilder().ParseDeleteSqlByID(domainModel, dataID, shardingKeyValue);

            var db = DatabaseFactory.CreateDefaultDatabase();
            db.ExecuteSQLWithTransaction(sqls);
        }

        /// <summary>
        /// 批量删除
        /// </summary>
        /// <remarks>按主键数据作为查询依据</remarks>
        /// <param name="domainModel">领域模型</param>        
        /// <param name="dataID">主键数据</param>
        /// <param name="shardingKeyValue">分库分表键值对</param>
        /// <returns>删除SQL</returns>
        public void DeleteByIDs(DomainModel.Spi.DomainModel domainModel, IEnumerable<string> dataIDs, ShardingValue shardingKeyValue = null)
        {
            if (domainModel == null)
                throw new ArgumentNullException("DataDeleteService.DeleteByID.domainModel");
            if (dataIDs == null || dataIDs.Count() == 0)
                throw new ArgumentNullException("DataDeleteService.DeleteByID.dataIDs");

            var sqlList = new SqlStatementCollection();
            foreach (var dataID in dataIDs)
            {
                var sqls = SQLBuilderFactory.CreateSQLBuilder().ParseDeleteSqlByID(domainModel, dataID, shardingKeyValue);
                sqlList.AddRange(sqls);
            }

            var db = DatabaseFactory.CreateDefaultDatabase();
            db.ExecuteSQLWithTransaction(sqlList);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <remarks>按主键数据作为查询依据</remarks>
        /// <param name="domainModel">领域模型</param> 
        /// <param name="domainObject">领域对象</param>
        /// <param name="dataID">主键数据</param>
        /// <param name="shardingKeyValue">分库分表键值对</param>
        /// <returns>删除SQL</returns>
        public void DeleteByID(DomainModel.Spi.DomainModel domainModel, DomainModel.Spi.DomainObject domainObject, string dataID, ShardingValue shardingKeyValue = null)
        {
            if (domainModel == null)
                throw new ArgumentNullException("DataDeleteService.DeleteByID.domainModel");
            if (domainObject == null)
                throw new ArgumentNullException("DataDeleteService.DeleteByID.domainObject");
            if (string.IsNullOrWhiteSpace(dataID))
                throw new ArgumentNullException("DataDeleteService.DeleteByID.dataID");

            var sqls = SQLBuilderFactory.CreateSQLBuilder().ParseDeleteSqlByID(domainModel, domainObject, dataID, shardingKeyValue);

            var db = DatabaseFactory.CreateDefaultDatabase();
            db.ExecuteSQLWithTransaction(sqls);
        }

        /// <summary>
        /// 批量删除
        /// </summary>
        /// <remarks>按主键数据作为查询依据</remarks>
        /// <param name="domainModel">领域模型</param> 
        /// <param name="domainObject">领域对象</param>
        /// <param name="dataID">主键数据</param>
        /// <param name="shardingKeyValue">分库分表键值对</param>
        /// <returns>删除SQL</returns>
        public void DeleteByIDs(DomainModel.Spi.DomainModel domainModel, DomainModel.Spi.DomainObject domainObject, IEnumerable<string> dataIDs, ShardingValue shardingKeyValue = null)
        {
            if (domainModel == null)
                throw new ArgumentNullException("DataDeleteService.DeleteByID.domainModel");
            if (domainObject == null)
                throw new ArgumentNullException("DataDeleteService.DeleteByID.domainObject");
            if (dataIDs == null || dataIDs.Count() == 0)
                throw new ArgumentNullException("DataDeleteService.DeleteByID.dataIDs");

            var sqlList = new SqlStatementCollection();
            foreach (var dataID in dataIDs)
            {
                var sqls = SQLBuilderFactory.CreateSQLBuilder().ParseDeleteSqlByID(domainModel, domainObject, dataID, shardingKeyValue);
                sqlList.AddRange(sqls);
            }

            var db = DatabaseFactory.CreateDefaultDatabase();
            db.ExecuteSQLWithTransaction(sqlList);
        }
    }
}
