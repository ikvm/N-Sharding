using NSharding.DomainModel.Manager;
using NSharding.Sharding.Database;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Teld.Core.Metadata.Service
{
    /// <summary>
    /// 数据源服务
    /// </summary>
    public class DataSourceService
    {
        private static ConcurrentDictionary<string, DataSource> dataSourceDic;

        private static object syncObj = new object();

        private static DataSourceService instance;

        private DataSourceManager manager;

        /// <summary>
        /// 构造函数
        /// </summary>
        private DataSourceService()
        {
            manager = new DataSourceManager();
            dataSourceDic = new ConcurrentDictionary<string, DataSource>();
            LoadAllDataSource();
        }

        private void LoadAllDataSource()
        {

        }

        public static DataSourceService GetInstance()
        {
            if (instance == null)
            {
                lock (syncObj)
                {
                    if (instance == null)
                    {
                        instance = new DataSourceService();
                    }
                }
            }

            return instance;
        }

        public List<DataSource> GetDataSource()
        {
            if (dataSourceDic.Count == 0)
            {
                var dataSources = manager.GetDataSources();
                if (dataSources != null)
                {
                    foreach (var dataSource in dataSources)
                    {
                        dataSourceDic.TryAdd(dataSource.Name, dataSource);
                    }
                }
            }
            return dataSourceDic.Values.ToList();
        }


        public DataSource GetDataSource(string dataSourceName)
        {
            if (string.IsNullOrWhiteSpace(dataSourceName))
                throw new ArgumentNullException("GetDataSource.dataSourceName");

            if (!dataSourceDic.ContainsKey(dataSourceName))
            {
                lock (syncObj)
                {
                    if (!dataSourceDic.ContainsKey(dataSourceName))
                    {
                        var dataSource = manager.GetDataSource(dataSourceName);
                        dataSourceDic.TryAdd(dataSourceName, dataSource);

                        return dataSource;
                    }
                    else
                    {
                        return dataSourceDic[dataSourceName];
                    }
                }
            }

            return dataSourceDic[dataSourceName];
        }

        /// <summary>
        /// 数据源保存
        /// </summary>
        /// <param name="dataSource">数据源</param>
        public void SaveDataSource(DataSource dataSource)
        {
            if (dataSource == null)
                throw new ArgumentNullException("DataSourceService.GetDataSource.dataSource");

            manager.SaveDataSource(dataSource);
        }

        /// <summary>
        /// 删除数据源
        /// </summary>
        /// <param name="name">数据源ID</param>
        public void DeleteDataSource(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException("DataSourceService.DeleteDataSource.name");

            manager.DeleteDataSource(name);
        }
    }
}
