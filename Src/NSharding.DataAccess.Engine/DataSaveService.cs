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
    /// 数据保存服务
    /// </summary>
    class DataSaveService : IDataSaveService
    {
        /// <summary>
        /// 保存领域模型数据
        /// </summary>
        /// <param name="domainModel">领域模型</param>
        /// <param name="instance">对象实例</param>
        /// <param name="shardingValue">分区分表键值对</param>
        public void Save(DomainModel.Spi.DomainModel domainModel, object instance, ShardingValue shardingValue = null)
        {
            if (domainModel == null)
                throw new ArgumentNullException("SaveService.Save.domainModel");
            if (instance == null)
                throw new ArgumentNullException("SaveService.Save.instance");

            var sqls = SQLBuilderFactory.CreateSQLBuilder().ParseInsertSql(domainModel, instance, shardingValue);

            var db = DatabaseFactory.CreateDefaultDatabase();
            db.ExecuteSQLWithTransaction(sqls);
        }

        /// <summary>
        /// 保存领域模型数据
        /// </summary>
        /// <param name="domainModel">领域模型</param>
        /// <param name="instanceList">对象实例集合</param>
        /// <param name="shardingValueList">分区分表键值对集合</param>
        public void SaveBatch(DomainModel.Spi.DomainModel domainModel, List<object> instanceList, List<ShardingValue> shardingValueList = null)
        {
            if (domainModel == null)
                throw new ArgumentNullException("SaveService.Save.domainModel");
            if (instanceList == null)
                throw new ArgumentNullException("SaveService.Save.instanceList");

            var sqls = new SqlStatementCollection();
            for (int i = 0; i < instanceList.Count; i++)
            {
                var currentObj = instanceList[i];
                ShardingValue shardingValue = null;
                if (shardingValueList != null)
                    shardingValue = shardingValueList[i];

                var sqlstatements = SQLBuilderFactory.CreateSQLBuilder().ParseInsertSql(domainModel, currentObj, shardingValue);

                sqls.AddRange(sqlstatements);
            }

            var db = DatabaseFactory.CreateDefaultDatabase();
            db.ExecuteSQLWithTransaction(sqls);
        }

        /// <summary>
        /// 保存领域模型数据
        /// </summary>
        /// <param name="domainModel">领域模型</param>
        /// <param name="domainObject">领域对象</param>
        /// <param name="instance">对象实例</param>
        /// <param name="shardingValue">分区分表键值对</param>
        public void Save(DomainModel.Spi.DomainModel domainModel, DomainModel.Spi.DomainObject domainObject, object instance, ShardingValue shardingValue = null)
        {
            if (domainModel == null)
                throw new ArgumentNullException("SaveService.Save.domainModel");
            if (domainObject == null)
                throw new ArgumentNullException("SaveService.Save.domainObject");
            if (instance == null)
                throw new ArgumentNullException("SaveService.Save.instance");

            var sqls = SQLBuilderFactory.CreateSQLBuilder().ParseInsertSql(domainModel, domainObject, instance, shardingValue);

            var db = DatabaseFactory.CreateDefaultDatabase();
            db.ExecuteSQLWithTransaction(sqls);
        }

        /// <summary>
        /// 保存领域模型数据
        /// </summary>
        /// <param name="domainModel">领域模型</param>
        /// <param name="domainObject">领域对象</param>
        /// <param name="instance">对象实例</param>
        /// <param name="shardingValueList">分区分表键值对集合</param>
        public void SaveBatch(DomainModel.Spi.DomainModel domainModel, DomainModel.Spi.DomainObject domainObject, IList<object> instanceList, List<ShardingValue> shardingValueList = null)
        {
            if (domainModel == null)
                throw new ArgumentNullException("SaveService.Save.domainModel");
            if (domainObject == null)
                throw new ArgumentNullException("SaveService.Save.domainObject");
            if (instanceList == null)
                throw new ArgumentNullException("SaveService.Save.instanceList");

            var sqls = new SqlStatementCollection();
            for (int i = 0; i < instanceList.Count; i++)
            {
                var currentObj = instanceList[i];
                ShardingValue shardingValue = null;
                if (shardingValueList != null)
                    shardingValue = shardingValueList[i];

                var sqlstatements = SQLBuilderFactory.CreateSQLBuilder().ParseInsertSql(domainModel, domainObject, currentObj, shardingValue);

                sqls.AddRange(sqlstatements);
            }


            var db = DatabaseFactory.CreateDefaultDatabase();
            db.ExecuteSQLWithTransaction(sqls);
        }
    }
}
