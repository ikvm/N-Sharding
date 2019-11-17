using NSharding.DomainModel.Annotation;
using NSharding.DomainModel.Spi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSharding.DomainModel.Service
{
    /// <summary>
    /// 领域模型解析器
    /// </summary>
    /// <remarks>
    /// 通过类的注解解析形成领域模型
    /// </remarks>
    public class DomainModelBuilder
    {
        public static DomainModel.Spi.DomainModel Parse(System.Type t)
        {
            var domainModel = new DomainModel.Spi.DomainModel();

            var attributes = t.GetCustomAttributes(false);

            var domainModelAttrObj = attributes.FirstOrDefault(i => i.GetType() == typeof(DomainModelAttribute));
            if (domainModelAttrObj == null)
                throw new Exception($"Type:{t.FullName} does not contain DomainModelAttribute");

            var domainModelAttr = domainModelAttrObj as DomainModelAttribute;
            domainModel.Name = domainModelAttr.Name;
            domainModel.ID = Guid.NewGuid().ToString();

            var domainModelCacheAttrObj = attributes.FirstOrDefault(i => i.GetType() == typeof(CacheAttribute));
            if (domainModelCacheAttrObj != null)
            {
                var domainModelCacheAttr = domainModelCacheAttrObj as CacheAttribute;
                domainModel.IsCache = true;
                domainModel.CacheStrategy = domainModelCacheAttr.Scope.ToString();                
            }

            var domainModelLogicDelAttrObj = attributes.FirstOrDefault(i => i.GetType() == typeof(LogicDeleteAttribute));
            if (domainModelLogicDelAttrObj != null)
            {
                domainModel.IsLogicDelete = true;
            }

            //解析子对象
            var rootDomainObjectAttr = attributes.FirstOrDefault(i => i.GetType() == typeof(DomainObjectAttribute)) as DomainObjectAttribute;
            if(rootDomainObjectAttr == null)
                throw new Exception($"Type:{t.FullName} does not contain DomainObjectAttribute");

            var rootDbTableAttr = attributes.FirstOrDefault(i => i.GetType() == typeof(DbTableAttribute)) as DbTableAttribute;
            if (rootDbTableAttr == null)
                throw new Exception($"Type:{t.FullName} does not contain DbTableAttribute");

            BuildDomainObjects(t, domainModel, rootDomainObjectAttr, rootDbTableAttr);

            return domainModel;
        }

        private static void BuildDomainObjects(System.Type t, DomainModel.Spi.DomainModel domainModel, 
            DomainObjectAttribute rootDomainObjectAttr, DbTableAttribute rootDbTableAttr)
        {                                 
            var rootDomainObject = new DomainObject
            {
                ID = Guid.NewGuid().ToString(),
                Name = rootDomainObjectAttr.Name,
                ClazzReflectType = t.FullName,
                DomainModel = domainModel,
                IsLazyLoad = rootDomainObjectAttr.IsLazyload,
            };

            var dataObject = new Sharding.Database.DataObject()
            {
                Name = rootDbTableAttr.Name,
                IsView = rootDbTableAttr.IsView,
                ID = Guid.NewGuid().ToString(),
                TableShardingStrategyID = rootDbTableAttr.TableShardingStrategyID,
                DatabaseShardingStrategyID = rootDbTableAttr.DBShardingStrategyID,
                LogicTableName = rootDbTableAttr.Name,
                DataSourceName = rootDbTableAttr.DataSourceName
            };

            if (string.IsNullOrEmpty(rootDbTableAttr.ID) != false)
            {
                dataObject = new Sharding.Database.DataObject() { ID = rootDbTableAttr.ID };
            }

            rootDomainObject.DataObject = dataObject;

            domainModel.RootDomainObject = rootDomainObject;
        }
    }
}
