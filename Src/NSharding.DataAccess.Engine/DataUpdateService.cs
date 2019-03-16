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
    /// 数据更新服务
    /// </summary>
    class DataUpdateService : IDataUpdateService
    {
        /// <summary>
        /// 更新领域模型数据
        /// </summary>
        /// <param name="domainModel">领域模型</param>
        /// <param name="instance">对象实例</param>
        /// <param name="shardingValue">分区分表键值对</param>
        public void Update(DomainModel.Spi.DomainModel domainModel, object instance, ShardingValue shardingValue = null)
        {
            if (domainModel == null)
                throw new ArgumentNullException("DataUpdateService.Update.domainModel");
            if (instance == null)
                throw new ArgumentNullException("DataUpdateService.Update.instance");

            var sqls = SQLBuilderFactory.CreateSQLBuilder().ParseUpdateSql(domainModel, instance, shardingValue);

            var db = DatabaseFactory.CreateDefaultDatabase();
            db.ExecuteSQLWithTransaction(sqls);
        }

        /// <summary>
        /// 更新领域模型数据
        /// </summary>
        /// <param name="domainModel">领域模型</param>
        /// <param name="domainObject">领域对象</param>
        /// <param name="instance">对象实例</param>
        /// <param name="shardingValue">分区分表键值对</param>
        public void Update(DomainModel.Spi.DomainModel domainModel, DomainModel.Spi.DomainObject domainObject, object instance, ShardingValue shardingValue = null)
        {
            if (domainModel == null)
                throw new ArgumentNullException("DataUpdateService.Update.domainModel");
            if (domainObject == null)
                throw new ArgumentNullException("DataUpdateService.Update.domainObject");
            if (instance == null)
                throw new ArgumentNullException("DataUpdateService.Update.instance");

            var sqls = SQLBuilderFactory.CreateSQLBuilder().ParseUpdateSql(domainModel, domainObject, instance, shardingValue);

            var db = DatabaseFactory.CreateDefaultDatabase();
            db.ExecuteSQLWithTransaction(sqls);
        }
    }
}
