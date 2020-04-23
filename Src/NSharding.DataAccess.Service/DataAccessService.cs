using NSharding.DataAccess.Core;
using NSharding.DataAccess.Spi;
using NSharding.ORMapping.Service;
using NSharding.Sharding.Rule;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Teld.Core.Metadata.Service;

namespace NSharding.DataAccess.Service
{
    /// <summary>
    /// 数据访问服务
    /// </summary>
    public class DataAccessService
    {
        //线程同步对象
        private static object syncObj = new object();

        //数据访问服务统一入口
        private static DataAccessService instance;

        /// <summary>
        /// 构造函数
        /// </summary>
        private DataAccessService()
        {

        }

        /// <summary>
        /// 获取数据访问服务统一入口
        /// </summary>
        /// <returns>数据访问服务统一入口</returns>
        public static DataAccessService GetInstance()
        {
            if (instance == null)
            {
                lock (syncObj)
                {
                    if (instance == null)
                    {
                        instance = new DataAccessService();
                    }
                }
            }

            return instance;
        }

        #region Save

        /// <summary>
        /// 保存领域模型数据
        /// </summary>
        /// <param name="domainModelID">领域模型ID</param>
        /// <param name="instance">对象实例</param>
        public void Save(string domainModelID, object instance, ShardingValue shardingValue = null)
        {
            if (string.IsNullOrWhiteSpace(domainModelID))
                throw new ArgumentNullException("DataAccessService.Save.domainModelID");
            if (instance == null)
                throw new ArgumentNullException("DataAccessService.Save.instance");
            try
            {
                var domainModel = DomainModelManageService.GetInstance().GetDomainModel(domainModelID);
                if (domainModel == null)
                    throw new Exception("Dae-0001: Cannot find DomainModel: " + domainModelID);


                DataAccessEngine.GetInstance().GetDataSaveService().Save(domainModel, instance, shardingValue);
            }
            catch (Exception e)
            {
                throw new Exception("Dae-Save-001: 数据保存失败!", e);
            }
        }

        /// <summary>
        /// 保存领域模型数据
        /// </summary>
        /// <param name="domainModelID">领域模型ID</param>
        /// <param name="instance">对象实例</param>
        public void SaveBatch(string domainModelID, List<object> instances, ShardingValue shardingValue = null)
        {
            if (string.IsNullOrWhiteSpace(domainModelID))
                throw new ArgumentNullException("DataAccessService.Save.domainModelID");
            if (instances == null)
                throw new ArgumentNullException("DataAccessService.Save.instances");
            try
            {
                var domainModel = DomainModelManageService.GetInstance().GetDomainModel(domainModelID);
                if (domainModel == null)
                    throw new Exception("Dae-0001: Cannot find DomainModel: " + domainModelID);

                var shardings = new List<ShardingValue>();
                if (shardingValue != null)
                    instances.ForEach(i => shardings.Add(shardingValue));

                DataAccessEngine.GetInstance().GetDataSaveService().SaveBatch(domainModel, instances, shardings);
            }
            catch (Exception e)
            {
                throw new Exception("Dae-Save-001: 数据保存失败!", e);
            }
        }

        /// <summary>
        /// 保存领域模型数据
        /// </summary>
        /// <param name="domainModelID">领域模型ID</param>
        /// <param name="instance">对象实例</param>
        public void SaveBatch(string domainModelID, List<object> instances, List<ShardingValue> shardingValues)
        {
            if (string.IsNullOrWhiteSpace(domainModelID))
                throw new ArgumentNullException("DataAccessService.Save.domainModelID");
            if (instances == null)
                throw new ArgumentNullException("DataAccessService.Save.instances");
            if (shardingValues == null)
                throw new ArgumentNullException("DataAccessService.Save.shardingValues");

            try
            {
                var domainModel = DomainModelManageService.GetInstance().GetDomainModel(domainModelID);

                if (domainModel == null)
                    throw new Exception("Dae-0001: Cannot find DomainModel: " + domainModelID);

                DataAccessEngine.GetInstance().GetDataSaveService().SaveBatch(domainModel, instances, shardingValues);
            }
            catch (Exception e)
            {
                throw new Exception("Dae-Save-001: 数据保存失败!", e);
            }
        }

        /// <summary>
        /// 保存指定对象的数据
        /// </summary>
        /// <param name="domainModelID">领域模型ID</param>
        /// <param name="domainObjectID">领域对象ID</param>
        /// <param name="instance">对象实例</param>
        public void Save(string domainModelID, string domainObjectID, object instance, ShardingValue shardingValue = null)
        {
            if (string.IsNullOrWhiteSpace(domainModelID))
                throw new ArgumentNullException("DataAccessService.Save.domainModelID");
            if (string.IsNullOrWhiteSpace(domainObjectID))
                throw new ArgumentNullException("DataAccessService.Save.domainObjectID");
            if (instance == null)
                throw new ArgumentNullException("DataAccessService.Save.instance");

            try
            {
                var domainModel = DomainModelManageService.GetInstance().GetDomainModel(domainModelID);
                if (domainModel == null)
                    throw new Exception("Dae-0001: Cannot find DomainModel: " + domainModelID);

                var domainObject = domainModel.DomainObjects.FirstOrDefault(i => i.ID == domainObjectID);
                if (domainObject == null)
                    throw new Exception("Dae-0001: Cannot find DomainObject: " + domainObjectID);

                DataAccessEngine.GetInstance().GetDataSaveService().Save(domainModel, domainObject, instance, shardingValue);
            }
            catch (Exception e)
            {
                throw new Exception("Dae-Save-001: 数据保存失败!", e);
            }
        }

        /// <summary>
        /// 保存指定对象的数据
        /// </summary>
        /// <param name="domainModelID">领域模型ID</param>
        /// <param name="domainObjectID">领域对象ID</param>
        /// <param name="instances">对象实例</param>
        public void Save(string domainModelID, string domainObjectID, List<object> instances, ShardingValue shardingValue = null)
        {
            if (string.IsNullOrWhiteSpace(domainModelID))
                throw new ArgumentNullException("DataAccessService.Save.domainModelID");
            if (string.IsNullOrWhiteSpace(domainObjectID))
                throw new ArgumentNullException("DataAccessService.Save.domainObjectID");
            if (instances == null)
                throw new ArgumentNullException("DataAccessService.Save.instances");

            try
            {
                var domainModel = DomainModelManageService.GetInstance().GetDomainModel(domainModelID);
                if (domainModel == null)
                    throw new Exception("Dae-0001: Cannot find DomainModel: " + domainModelID);

                var domainObject = domainModel.DomainObjects.FirstOrDefault(i => i.ID == domainObjectID);
                if (domainObject == null)
                    throw new Exception("Dae-0001: Cannot find DomainObject: " + domainObjectID);

                var shardings = new List<ShardingValue>();
                if (shardingValue != null)
                    instances.ForEach(i => shardings.Add(shardingValue));

                DataAccessEngine.GetInstance().GetDataSaveService().SaveBatch(domainModel, domainObject, instances, shardings);
            }
            catch (Exception e)
            {
                throw new Exception("Dae-Save-001: 数据保存失败!", e);
            }
        }

        /// <summary>
        /// 保存指定对象的数据
        /// </summary>
        /// <param name="domainModelID">领域模型ID</param>
        /// <param name="domainObjectID">领域对象ID</param>
        /// <param name="instances">对象实例</param>
        public void Save(string domainModelID, string domainObjectID, List<object> instances, List<ShardingValue> shardingValues)
        {
            if (string.IsNullOrWhiteSpace(domainModelID))
                throw new ArgumentNullException("DataAccessService.Save.domainModelID");
            if (string.IsNullOrWhiteSpace(domainObjectID))
                throw new ArgumentNullException("DataAccessService.Save.domainObjectID");
            if (instances == null)
                throw new ArgumentNullException("DataAccessService.Save.instances");

            try
            {
                var domainModel = DomainModelManageService.GetInstance().GetDomainModel(domainModelID);
                if (domainModel == null)
                    throw new Exception("Dae-0001: Cannot find DomainModel: " + domainModelID);

                var domainObject = domainModel.DomainObjects.FirstOrDefault(i => i.ID == domainObjectID);
                if (domainObject == null)
                    throw new Exception("Dae-0001: Cannot find DomainObject: " + domainObjectID);

                DataAccessEngine.GetInstance().GetDataSaveService().SaveBatch(domainModel, domainObject, instances, shardingValues);
            }
            catch (Exception e)
            {
                throw new Exception("Dae-Save-001: 数据保存失败!", e);
            }
        }

        #endregion

        #region Update

        /// <summary>
        /// 更新领域模型数据
        /// </summary>
        /// <param name="domainModelID">领域模型ID</param>
        /// <param name="instance">对象实例</param>
        public void Update(string domainModelID, object instance, ShardingValue shardingValue = null)
        {
            if (string.IsNullOrWhiteSpace(domainModelID))
                throw new ArgumentNullException("DataAccessService.Update.domainModelID");
            if (instance == null)
                throw new ArgumentNullException("DataAccessService.Update.instance");
            try
            {
                var domainModel = DomainModelManageService.GetInstance().GetDomainModel(domainModelID);
                if (domainModel == null)
                    throw new Exception("Dae-0001: Cannot find DomainModel: " + domainModelID);


                DataAccessEngine.GetInstance().GetDataUpdateService().Update(domainModel, instance, shardingValue);
            }
            catch (Exception e)
            {
                throw new Exception("Dae-Update-001: 数据保存失败!", e);
            }
        }

        /// <summary>
        /// 更新指定对象的数据
        /// </summary>
        /// <param name="domainModelID">领域模型ID</param>
        /// <param name="domainObjectID">领域对象ID</param>
        /// <param name="instance">对象实例</param>
        public void Update(string domainModelID, string domainObjectID, object instance, ShardingValue shardingValue = null)
        {
            if (string.IsNullOrWhiteSpace(domainModelID))
                throw new ArgumentNullException("DataAccessService.Update.domainModelID");
            if (string.IsNullOrWhiteSpace(domainObjectID))
                throw new ArgumentNullException("DataAccessService.Update.domainObjectID");
            if (instance == null)
                throw new ArgumentNullException("DataAccessService.Update.instance");

            try
            {
                var domainModel = DomainModelManageService.GetInstance().GetDomainModel(domainModelID);
                if (domainModel == null)
                    throw new Exception("Dae-0001: Cannot find DomainModel: " + domainModelID);

                var domainObject = domainModel.DomainObjects.FirstOrDefault(i => i.ID == domainObjectID);
                if (domainObject == null)
                    throw new Exception("Dae-0001: Cannot find DomainObject: " + domainObjectID);

                DataAccessEngine.GetInstance().GetDataUpdateService().Update(domainModel, domainObject, instance, shardingValue);
            }
            catch (Exception e)
            {
                throw new Exception("Dae-Update-001: 数据保存失败!", e);
            }
        }

        #endregion

        #region Delete

        /// <summary>
        ///  删除数据
        /// </summary>
        /// <param name="domainModelID">领域模型ID</param>
        /// <param name="dataId">数据唯一标识</param>
        /// <param name="shardingValue">分库分表键值对</param>
        public void Delete(string domainModelID, string dataId, ShardingValue shardingValue = null)
        {
            if (string.IsNullOrWhiteSpace(domainModelID))
                throw new ArgumentNullException("DataAccessService.Delete.domainModelID");
            if (string.IsNullOrWhiteSpace(dataId))
                throw new ArgumentNullException("DataAccessService.Delete.dataId");

            try
            {
                var domainModel = DomainModelManageService.GetInstance().GetDomainModel(domainModelID);

                if (domainModel == null)
                    throw new Exception("Dae-0001: Cannot find DomainModel: " + domainModelID);


                DataAccessEngine.GetInstance().GetDataDeleteService().DeleteByID(domainModel, dataId, shardingValue);
            }
            catch (Exception e)
            {
                throw new Exception("Dae-Delete-001: 数据删除失败!", e);
            }
        }

        /// <summary>
        ///  删除数据
        /// </summary>
        /// <param name="domainModelID">领域模型ID</param>
        /// <param name="dataIds">数据唯一标识集合</param>
        /// <param name="shardingKeyValue">分库分表键值对</param>
        public void Delete(string domainModelID, IList<string> dataIds, ShardingValue shardingValue = null)
        {
            if (string.IsNullOrWhiteSpace(domainModelID))
                throw new ArgumentNullException("DataAccessService.Delete.domainModelID");
            if (dataIds == null || dataIds.Count == 0)
                throw new ArgumentNullException("DataAccessService.Delete.dataIds");

            try
            {
                var domainModel = DomainModelManageService.GetInstance().GetDomainModel(domainModelID);

                if (domainModel == null)
                    throw new Exception("Dae-0001: Cannot find DomainModel: " + domainModelID);


                DataAccessEngine.GetInstance().GetDataDeleteService().DeleteByIDs(domainModel, dataIds, shardingValue);
            }
            catch (Exception e)
            {
                throw new Exception("Dae-Delete-002: 数据批量删除失败!", e);
            }
        }

        /// <summary>
        ///  删除数据
        /// </summary>
        /// <param name="domainModelID">领域模型ID</param>
        /// <param name="domainObjectID">领域对象ID</param>
        /// <param name="dataId">数据唯一标识</param>
        /// <param name="shardingValue">分库分表键值对</param>
        public void Delete(string domainModelID, string domainObjectID, string dataId, ShardingValue shardingValue = null)
        {
            if (string.IsNullOrWhiteSpace(domainModelID))
                throw new ArgumentNullException("DataAccessService.Delete.domainModelID");
            if (string.IsNullOrWhiteSpace(domainObjectID))
                throw new ArgumentNullException("DataAccessService.Delete.domainObjectID");
            if (string.IsNullOrWhiteSpace(dataId))
                throw new ArgumentNullException("DataAccessService.Delete.dataId");

            try
            {
                var domainModel = DomainModelManageService.GetInstance().GetDomainModel(domainModelID);
                if (domainModel == null)
                    throw new Exception("Dae-0001: Cannot find DomainModel: " + domainModelID);

                var domainObject = domainModel.DomainObjects.FirstOrDefault(i => i.ID == domainObjectID);
                if (domainObject == null)
                    throw new Exception("Dae-0001: Cannot find DomainObject: " + domainObjectID);

                DataAccessEngine.GetInstance().GetDataDeleteService().DeleteByID(domainModel, domainObject, dataId, shardingValue);
            }
            catch (Exception e)
            {
                throw new Exception("Dae-Delete-002: 数据批量删除失败!", e);
            }
        }

        /// <summary>
        ///  删除数据
        /// </summary>
        /// <param name="domainModelID">领域模型ID</param>
        /// <param name="domainObjectID">领域对象ID</param>
        /// <param name="dataIds">数据唯一标识集合</param>
        /// <param name="shardingValue">分库分表键值对</param>
        public void Delete(string domainModelID, string domainObjectID, IList<string> dataIds, ShardingValue shardingValue = null)
        {
            if (string.IsNullOrWhiteSpace(domainModelID))
                throw new ArgumentNullException("DataAccessService.Delete.domainModelID");
            if (string.IsNullOrWhiteSpace(domainObjectID))
                throw new ArgumentNullException("DataAccessService.Delete.domainObjectID");
            if (dataIds == null || dataIds.Count == 0)
                throw new ArgumentNullException("DataAccessService.Delete.dataIds");

            try
            {
                var domainModel = DomainModelManageService.GetInstance().GetDomainModel(domainModelID);
                if (domainModel == null)
                    throw new Exception("Dae-0001: Cannot find DomainModel: " + domainModelID);

                var domainObject = domainModel.DomainObjects.FirstOrDefault(i => i.ID == domainObjectID);
                if (domainObject == null)
                    throw new Exception("Dae-0001: Cannot find DomainObject: " + domainObjectID);

                DataAccessEngine.GetInstance().GetDataDeleteService().DeleteByIDs(domainModel, domainObject, dataIds, shardingValue);
            }
            catch (Exception e)
            {
                throw new Exception("Dae-Delete-002: 数据批量删除失败!", e);
            }
        }

        /// <summary>
        /// 获取对象数据
        /// </summary>        
        /// <param name="domainModel">领域模型</param>
        /// <param name="dataId">数据唯一标识</param>
        /// <param name="shardingValue">分库分表键值对</param>
        /// <returns>对象数据</returns>
        public List<DataTable> GetData(string domainModelID, string dataId, ShardingValue shardingValue = null)
        {
            if (string.IsNullOrWhiteSpace(domainModelID))
                throw new ArgumentNullException("DataAccessService.GetData.domainModelID");
            if (string.IsNullOrWhiteSpace(dataId))
                throw new ArgumentNullException("DataAccessService.GetData.dataId");

            try
            {
                var domainModel = DomainModelManageService.GetInstance().GetDomainModel(domainModelID);
                if (domainModel == null)
                    throw new Exception("Dae-0001: Cannot find DomainModel: " + domainModelID);

                return DataAccessEngine.GetInstance().GetDataQueryService().GetData(domainModel, dataId, shardingValue);
            }
            catch (Exception e)
            {
                throw new Exception("Dae-Query-001: 数据查询失败!", e);
            }
        }

        /// <summary>
        /// 获取对象数据
        /// </summary>        
        /// <param name="domainModel">领域模型</param>
        /// <param name="domainObject">领域对象</param>
        /// <param name="dataId">数据唯一标识</param>
        /// <param name="shardingValue">分库分表键值对</param>
        /// <returns>对象数据</returns>
        public List<DataTable> GetData(string domainModelID, string domainObjectID, string dataId, ShardingValue shardingValue = null)
        {
            if (string.IsNullOrWhiteSpace(domainModelID))
                throw new ArgumentNullException("DataAccessService.GetData.domainModelID");
            if (string.IsNullOrWhiteSpace(domainObjectID))
                throw new ArgumentNullException("DataAccessService.GetData.domainObjectID");
            if (string.IsNullOrWhiteSpace(dataId))
                throw new ArgumentNullException("DataAccessService.GetData.dataId");

            try
            {
                var domainModel = DomainModelManageService.GetInstance().GetDomainModel(domainModelID);
                if (domainModel == null)
                    throw new Exception("Dae-0001: Cannot find DomainModel: " + domainModelID);

                var domainObject = domainModel.DomainObjects.FirstOrDefault(i => i.ID == domainObjectID);
                if (domainObject == null)
                    throw new Exception("Dae-0001: Cannot find DomainObject: " + domainObjectID);

                return DataAccessEngine.GetInstance().GetDataQueryService().GetData(domainModel, domainObject, dataId, shardingValue);
            }
            catch (Exception e)
            {
                throw new Exception("Dae-Query-001: 数据查询失败!", e);
            }
        }

        #endregion

        /// <summary>
        /// 获取对象数据
        /// </summary>        
        /// <param name="domainModel">领域模型</param>
        /// <param name="domainObject">领域对象</param>
        /// <param name="queryFilter">查询条件</param>        
        /// <returns>对象数据</returns>
        public List<DataTable> GetData(string domainModelID, string domainObjectID, QueryFilter queryFilter)
        {
            if (string.IsNullOrWhiteSpace(domainModelID))
                throw new ArgumentNullException("DataAccessService.GetData.domainModelID");
            if (string.IsNullOrWhiteSpace(domainObjectID))
                throw new ArgumentNullException("DataAccessService.GetData.domainObjectID");
            if (queryFilter == null)
                throw new ArgumentNullException("DataAccessService.GetData.queryFilter");

            try
            {
                var domainModel = DomainModelManageService.GetInstance().GetDomainModel(domainModelID);
                if (domainModel == null)
                    throw new Exception("Dae-0001: Cannot find DomainModel: " + domainModelID);

                var domainObject = domainModel.DomainObjects.FirstOrDefault(i => i.ID == domainObjectID);
                if (domainObject == null)
                    throw new Exception("Dae-0001: Cannot find DomainObject: " + domainObjectID);

                return DataAccessEngine.GetInstance().GetDataQueryService().GetData(domainModel, domainObject, queryFilter).DataTables;
            }
            catch (Exception e)
            {
                throw new Exception("Dae-Query-001: 数据查询失败!", e);
            }
        }

        /// <summary>
        /// 获取对象数据
        /// </summary>        
        /// <param name="domainModel">领域模型</param>
        /// <param name="dataID">数据唯一标识</param>
        /// <param name="shardingValue">分库分表键值对</param>
        /// <returns>对象数据</returns>
        public T GetObject<T>(string domainModelID, string dataId, ShardingValue shardingValue = null,
            Func<QueryResultSet, DomainModel.Spi.DomainModel, DomainModel.Spi.DomainObject, T> ormappingFunc = null)
            where T : class
        {
            if (string.IsNullOrWhiteSpace(domainModelID))
                throw new ArgumentNullException("DataAccessService.GetObject.domainModelID");
            if (string.IsNullOrWhiteSpace(dataId))
                throw new ArgumentNullException("DataAccessService.GetObject.dataId");

            try
            {
                var domainModel = DomainModelManageService.GetInstance().GetDomainModel(domainModelID);

                if (domainModel == null)
                    throw new Exception("Dae-0001: Cannot find DomainModel: " + domainModelID);


                var resultSet = DataAccessEngine.GetInstance().GetDataQueryService().GetData(domainModel, dataId, shardingValue);

                if (ormappingFunc != null)
                    return ormappingFunc(resultSet, domainModel, domainModel.RootDomainObject);

                return ORMappingService.MapToObject<T>(resultSet, domainModel);
            }
            catch (Exception e)
            {
                throw new Exception("Dae-Query-001: 数据查询失败!", e);
            }
        }
    }
}
