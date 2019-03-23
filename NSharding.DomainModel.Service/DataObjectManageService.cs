using NSharding.DomainModel.Manager;
using NSharding.Sharding.Database;
using NSharding.Sharding.RuleManager;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Teld.Core.Metadata.Service
{
    /// <summary>
    /// 数据对象管理服务
    /// </summary>
    public class DataObjectManageService
    {
        private static ConcurrentDictionary<string, DataObject> dataObjectCache;

        private static object syncObject = new object();

        private static DataObjectManageService instance;

        private DataObjectManager manager;

        /// <summary>
        /// 构造函数
        /// </summary>
        private DataObjectManageService()
        {
            manager = new DataObjectManager();
            dataObjectCache = new ConcurrentDictionary<string, DataObject>();
        }

        /// <summary>
        /// 获取数据对象管理服务实例
        /// </summary>
        /// <returns>数据对象管理服务实例</returns>
        public static DataObjectManageService GetInstance()
        {
            if (instance == null)
            {
                lock (syncObject)
                {
                    if (instance == null)
                    {
                        instance = new DataObjectManageService();
                    }
                }
            }

            return instance;
        }

        /// <summary>
        /// 数据对象保存
        /// </summary>
        /// <param name="dataObject">数据对象</param>
        public void SaveDataObject(DataObject dataObject)
        {
            if (dataObject == null)
                throw new ArgumentNullException("DataObjectManageService.SaveDataObject.dataObject");

            manager.SaveDataObject(dataObject);
        }

        /// <summary>
        /// 删除数据对象
        /// </summary>
        /// <param name="id">数据对象ID</param>
        public void DeleteDataObject(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentNullException("DataObjectManageService.DeleteDataObject.id");

            manager.DeleteDataObject(id);
        }

        /// <summary>
        /// 获取数据对象
        /// </summary>
        /// <param name="dataObjectID">数据对象ID</param>
        /// <returns>数据对象</returns>
        public DataObject GetDataObject(string dataObjectID)
        {
            if (string.IsNullOrWhiteSpace(dataObjectID))
                throw new ArgumentNullException("GetDataObject.DataObjectID");

            if (!dataObjectCache.ContainsKey(dataObjectID))
            {
                lock (syncObject)
                {
                    if (!dataObjectCache.ContainsKey(dataObjectID))
                    {
                        var dataobject = GetDataObjectDetail(dataObjectID);
                        dataObjectCache.TryAdd(dataObjectID, dataobject);

                        return dataobject;
                    }
                    else
                    {
                        return dataObjectCache[dataObjectID];
                    }
                }
            }

            return dataObjectCache[dataObjectID];
        }

        private DataObject GetDataObjectDetail(string dataObjectID)
        {
            var dataobject = new DataObjectManager().GetDataObject(dataObjectID);
            if (dataobject.DataSource == null)
            {
                var dataSource = DataSourceService.GetInstance().GetDataSource(dataobject.DataSourceName);
                if (dataSource != null)
                {
                    dataobject.DataSource = dataSource;
                }
            }
            if (!string.IsNullOrWhiteSpace(dataobject.TableShardingStrategyID))
            {
                var strategy = ShardingStrategyService.GetInstance().GetTableShardingStrategy(dataobject.TableShardingStrategyID);
                if (strategy != null)
                {
                    dataobject.TableShardingStrategy = strategy;
                    if (string.IsNullOrWhiteSpace(dataobject.ActualTableNames))
                    {
                        var tableNames = ShardingStrategyService.GetInstance().GetShardingTableNames(strategy, dataobject.LogicTableName);
                        if (tableNames != null && tableNames.Count > 0)
                        {
                            dataobject.ActualTableNames = string.Join(",", tableNames);
                        }
                    }
                }
            }

            return dataobject;
        }
    }
}
