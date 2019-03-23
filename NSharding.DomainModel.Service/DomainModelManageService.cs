using NSharding.DomainModel.Manager;
using NSharding.DomainModel.Spi;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Teld.Core.Metadata.Service
{
    /// <summary>
    /// 领域模型管理服务
    /// </summary>
    public class DomainModelManageService
    {
        private static ConcurrentDictionary<string, DomainModel> domainModelCache;

        private static object syncObject = new object();

        private static DomainModelManageService instance;

        private DomainModelManager manager;

        /// <summary>
        /// 构造函数
        /// </summary>
        private DomainModelManageService()
        {
            manager = new DomainModelManager();
            domainModelCache = new ConcurrentDictionary<string, DomainModel>();
        }

        /// <summary>
        /// 获取领域模型管理服务实例
        /// </summary>
        /// <returns>领域模型管理服务实例</returns>
        public static DomainModelManageService GetInstance()
        {
            if (instance == null)
            {
                lock (syncObject)
                {
                    if (instance == null)
                    {
                        instance = new DomainModelManageService();
                    }
                }
            }

            return instance;
        }

        /// <summary>
        /// 获取领域模型
        /// </summary>
        /// <param name="domainModelID">领域模型ID</param>
        /// <returns>领域模型</returns>
        public DomainModel GetDomainModel(string domainModelID)
        {
            if (string.IsNullOrWhiteSpace(domainModelID))
                throw new ArgumentNullException("GetDomainModel.domainModelID");

            if (!domainModelCache.ContainsKey(domainModelID))
            {
                lock (syncObject)
                {
                    if (!domainModelCache.ContainsKey(domainModelID))
                    {
                        var domainModel = GetDomainModelDetail(domainModelID);
                        domainModelCache.TryAdd(domainModelID, domainModel);

                        return domainModel;
                    }
                    else
                    {
                        return domainModelCache[domainModelID];
                    }
                }
            }

            return domainModelCache[domainModelID];
        }

        /// <summary>
        /// 循环构造领域模型
        /// </summary>
        /// <param name="domainModelID">领域模型ID</param>
        /// <returns>领域模型</returns>
        private DomainModel GetDomainModelDetail(string domainModelID)
        {
            var domainModel = new DomainModelManager().GetDomainModel(domainModelID);

            foreach (var modelObject in domainModel.DomainObjects)
            {
                //DataObject
                if (string.IsNullOrEmpty(modelObject.DataObjectID))
                {
                    throw new Exception("Domain Object cannot find associate DataObject: " + modelObject.ID);
                }

                modelObject.DataObject = DataObjectManageService.GetInstance().GetDataObject(modelObject.DataObjectID);

                //Association
                if (modelObject.Associations.Count > 0)
                {
                    foreach (var asso in modelObject.Associations)
                    {
                        if (asso.AssociateType == AssociateType.InnerJoin)
                        {
                            asso.AssoDomaiModel = domainModel;
                            asso.AssoDomainObject = domainModel.DomainObjects.FirstOrDefault(i => i.ID == asso.AssoDomainObjectID);
                        }
                        else if (asso.AssociateType == AssociateType.OuterLeftJoin)
                        {
                            asso.AssoDomaiModel = GetDomainModel(asso.AssoDomainModelID);
                            asso.AssoDomainObject = asso.AssoDomaiModel.DomainObjects.FirstOrDefault(i => i.ID == asso.AssoDomainObjectID);
                        }
                    }
                }
            }

            return domainModel;
        }

        /// <summary>
        /// 数据领域对象
        /// </summary>
        /// <param name="dataObject">领域对象</param>
        public void SaveDomainModel(DomainModel domainModel)
        {
            if (domainModel == null)
                throw new ArgumentNullException("DomainModelManageService.SaveDomainModel.domainModel");

            manager.SaveDomainModel(domainModel);
        }

        /// <summary>
        /// 删除领域对象
        /// </summary>
        /// <param name="id">领域对象ID</param>
        public void DeleteDomainModel(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentNullException("DomainModelManageService.DeleteDomainModel.id");

            manager.DeleteDomainModel(id);
        }
    }
}
