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

            return domainModel;
        }
    }
}
