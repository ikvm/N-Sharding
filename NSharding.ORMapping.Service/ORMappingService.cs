using NSharding.DataAccess.Spi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSharding.ORMapping.Service
{
    /// <summary>
    /// 数据访问服务ORMapping服务
    /// </summary>
    public class ORMappingService
    {
        public static T MapToObject<T>(QueryResultSet resultSet, DomainModel.Spi.DomainModel model, DomainModel.Spi.DomainObject domainObject = null)
            where T : class
        {
            if (resultSet == null)
                throw new ArgumentNullException("ORMappingService.MapToObject.resultSet");

            if (model == null)
                throw new ArgumentNullException("ORMappingService.MapToObject.model");

            if (domainObject == null)
                domainObject = model.RootDomainObject;

            var plugin = ORMPluginFactory.GetInstance().GetOrCreatePlugin(model);
            var obj = plugin.MapToObject(resultSet, model, domainObject);

            return obj as T;
        }

        public static List<T> MapToObjects<T>(QueryResultSet resultSet, DomainModel.Spi.DomainModel model, DomainModel.Spi.DomainObject domainObject = null)
            where T : class
        {
            if (resultSet == null)
                throw new ArgumentNullException("ORMappingService.MapToObject.resultSet");

            if (model == null)
                throw new ArgumentNullException("ORMappingService.MapToObject.model");

            if (domainObject == null)
                domainObject = model.RootDomainObject;

            var plugin = ORMPluginFactory.GetInstance().GetOrCreatePlugin(model);
            var objs = plugin.MapToObjects(resultSet, model, domainObject);

            return new List<T>(objs.Select(i => i as T));
        }
    }
}
