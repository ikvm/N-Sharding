using NSharding.DataAccess.Spi;
using NSharding.DomainModel.Spi;
using NSharding.Sharding.Rule;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSharding.DataAccess.Core
{
    /// <summary>
    /// 数据查询服务
    /// </summary>
    class DataQueryService : IDataQueryService
    {
        /// <summary>
        /// 获取对象数据
        /// </summary>        
        /// <param name="domainModel">领域模型</param>
        /// <param name="dataID">数据唯一标识</param>
        /// <param name="shardingValue">分库分表键值对</param>
        /// <returns>对象数据</returns>
        public QueryResultSet GetData(NSharding.DomainModel.Spi.DomainModel domainModel, string dataID, ShardingValue shardingValue = null)
        {
            if (domainModel == null)
                throw new ArgumentNullException("DataQueryService.GetData.domainModel");
            if (string.IsNullOrWhiteSpace(dataID))
                throw new ArgumentNullException("DataQueryService.GetData.dataID");

            var sqls = SQLBuilderFactory.CreateSQLBuilder(domainModel).ParseQuerySqlByID(domainModel, dataID, shardingValue);

            var db = DatabaseFactory.CreateDatabase(domainModel);
            var dts = db.GetDataCollection(sqls);

            return new QueryResultSet { ShardingInfo = sqls.ShardingInfo, DataTables = dts };
        }

        /// <summary>
        /// 获取对象数据
        /// </summary>        
        /// <param name="domainModel">领域模型</param>
        /// <param name="domainObject">领域对象</param>
        /// <param name="dataID">数据唯一标识</param>
        /// <param name="shardingValue">分库分表键值对</param>
        /// <returns>对象数据</returns>
        public QueryResultSet GetData(NSharding.DomainModel.Spi.DomainModel domainModel, DomainObject domainObject, string dataID, ShardingValue shardingValue = null)
        {
            if (domainModel == null)
                throw new ArgumentNullException("DataQueryService.GetData.domainModel");
            if (domainObject == null)
                throw new ArgumentNullException("DataQueryService.GetData.domainObject");
            if (string.IsNullOrWhiteSpace(dataID))
                throw new ArgumentNullException("DataQueryService.GetData.dataID");

            var sqls = SQLBuilderFactory.CreateSQLBuilder(domainModel).ParseQuerySqlByID(domainModel, domainObject, dataID, shardingValue);

            var db = DatabaseFactory.CreateDatabase(domainModel);
            var dts = db.GetDataCollection(sqls);

            return new QueryResultSet { ShardingInfo = sqls.ShardingInfo, DataTables = dts };
        }

        /// <summary>
        /// 获取对象数据
        /// </summary>        
        /// <param name="domainModel">领域模型</param>
        /// <param name="queryFilter">查询条件</param>        
        /// <returns>查询结果</returns>
        public QueryResultSet GetData(NSharding.DomainModel.Spi.DomainModel domainModel, QueryFilter queryFilter)
        {
            if (domainModel == null)
                throw new ArgumentNullException("DataQueryService.GetData.domainModel");
            if (queryFilter == null)
                throw new ArgumentNullException("DataQueryService.GetData.queryFilter");

            var sqls = SQLBuilderFactory.CreateSQLBuilder(domainModel).ParseQuerySqlByFilter(domainModel, queryFilter);

            var db = DatabaseFactory.CreateDatabase(domainModel);
            var dts = db.GetDataCollection(sqls);

            return new QueryResultSet { ShardingInfo = sqls.ShardingInfo, DataTables = dts };
        }

        /// <summary>
        /// 获取对象数据
        /// </summary>        
        /// <param name="domainModel">领域模型</param>
        /// <param name="domainObject">领域对象</param>
        /// <param name="queryFilter">查询条件</param>        
        /// <returns>查询结果</returns>
        public QueryResultSet GetData(NSharding.DomainModel.Spi.DomainModel domainModel, DomainObject domainObject, QueryFilter queryFilter)
        {
            if (domainModel == null)
                throw new ArgumentNullException("DataQueryService.GetData.domainModel");
            if (queryFilter == null)
                throw new ArgumentNullException("DataQueryService.GetData.queryFilter");

            var sqls = SQLBuilderFactory.CreateSQLBuilder(domainModel).ParseQuerySqlByFilter(domainModel, domainObject, queryFilter);

            var db = DatabaseFactory.CreateDatabase(domainModel);
            var dts = db.GetDataCollection(sqls);

            return new QueryResultSet { ShardingInfo = sqls.ShardingInfo, DataTables = dts };
        }
    }
}
