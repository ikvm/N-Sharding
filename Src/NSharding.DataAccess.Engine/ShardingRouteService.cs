using NSharding.DomainModel.Spi;
using NSharding.Sharding.Rule;
using NSharding.Sharding.RuleManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSharding.DataAccess.Core
{
    /// <summary>
    /// Sharding路由服务
    /// </summary>
    class ShardingRouteService
    {
        /// <summary>
        /// Sharding路由
        /// </summary>
        /// <param name="domainModel">领域模型</param>
        /// <param name="instance">数据</param>
        /// <param name="shardingKeyValue">ShardingValue</param>
        /// <returns>路由</returns>
        public Dictionary<string, ShardingTarget> Route(DomainModel.Spi.DomainModel domainModel, object instance, ShardingValue shardingKeyValue = null)
        {
            var targets = new Dictionary<string, ShardingTarget>();

            if (shardingKeyValue == null)
            {
                shardingKeyValue = CreateShardingValue(domainModel, domainModel.RootDomainObject, instance);
            }

            if (shardingKeyValue.ShardingValueType != ShardingValueType.SINGLE)
            {
                throw new Exception("Object Sharding route error: " + shardingKeyValue.ShardingValueType);
            }

            foreach (var domainObject in domainModel.DomainObjects)
            {
                var tars = ShardingRuleService.GetInstance().Parse(domainObject.DataObject, shardingKeyValue);
                targets.Add(domainObject.DataObject.ID, tars[0]);
            }

            return targets;
        }

        /// <summary>
        /// Sharding路由
        /// </summary>
        /// <param name="domainModel">领域模型</param>
        /// <param name="dataID">数据标识</param>
        /// <param name="shardingKeyValue">ShardingValue</param>
        /// <returns>路由</returns>
        public Dictionary<string, ShardingTarget> RouteByDataID(DomainModel.Spi.DomainModel domainModel, string dataID, ShardingValue shardingKeyValue = null)
        {
            var targets = new Dictionary<string, ShardingTarget>();

            if (shardingKeyValue == null)
            {
                shardingKeyValue = CreateShardingValueByDataID(domainModel, domainModel.RootDomainObject, dataID);
            }

            //不分库分表
            if (shardingKeyValue == null)
            {
                foreach (var domainObject in domainModel.DomainObjects)
                {
                    targets.Add(domainObject.DataObject.ID,
                        new ShardingTarget()
                        {
                            DataSource = domainObject.DataObject.DataSourceName,
                            TableName = domainObject.DataObject.ActualTableNames
                        });
                }
            }
            else
            {
                if (shardingKeyValue.ShardingValueType != ShardingValueType.SINGLE)
                {
                    throw new Exception("Object Sharding route error: " + shardingKeyValue.ShardingValueType);
                }

                foreach (var domainObject in domainModel.DomainObjects)
                {
                    var tars = ShardingRuleService.GetInstance().Parse(domainObject.DataObject, shardingKeyValue);
                    targets.Add(domainObject.DataObject.ID, tars[0]);
                }
            }

            return targets;
        }

        /// <summary>
        /// Sharding路由
        /// </summary>
        /// <param name="domainModel">领域模型</param>
        /// <param name="instance">数据</param>
        /// <param name="shardingKeyValue">ShardingValue</param>
        /// <returns>路由</returns>
        public Dictionary<string, ShardingTarget> Route(DomainModel.Spi.DomainModel domainModel, DomainModel.Spi.DomainObject domainObject, object instance, ShardingValue shardingKeyValue = null)
        {
            var targets = new Dictionary<string, ShardingTarget>();

            if (shardingKeyValue == null)
            {
                shardingKeyValue = CreateShardingValue(domainModel, domainObject, instance);
            }

            if (shardingKeyValue.ShardingValueType != ShardingValueType.SINGLE)
            {
                throw new Exception("Object Sharding route error: " + shardingKeyValue.ShardingValueType);
            }


            var tars = ShardingRuleService.GetInstance().Parse(domainObject.DataObject, shardingKeyValue);
            targets.Add(domainObject.DataObject.ID, tars[0]);


            return targets;
        }

        /// <summary>
        /// 构造ShardingValue
        /// </summary>
        /// <param name="domainModel">领域模型</param>
        /// <param name="instance">数据</param>
        /// <returns>ShardingValue</returns>
        private ShardingValue CreateShardingValue(DomainModel.Spi.DomainModel domainModel, DomainModel.Spi.DomainObject domainObject, object instance)
        {
            ShardingValue shardingValue = null;
            var shardingColumn = domainObject.DataObject.Columns.FirstOrDefault(i => i.IsShardingColumn);
            if (shardingColumn == null)
            {
                throw new Exception("DataObject unset sharding column:" + domainObject.DataObjectID);
            }

            var shardingElement = domainObject.Elements.FirstOrDefault(i => i.DataColumnID == shardingColumn.ID);

            var propValue = ObjectPropertyValueUtils.GetPropValue(shardingElement.PropertyName, instance);
            switch (shardingElement.DataType)
            {
                case ElementDataType.DateTime:
                case ElementDataType.Date:
                    shardingValue = new ShardingValue(domainModel.Name, shardingElement.ID, Convert.ToDateTime(propValue));
                    break;
                case ElementDataType.Integer:
                    shardingValue = new ShardingValue(domainModel.Name, shardingElement.ID, Convert.ToInt64(propValue));
                    break;
                case ElementDataType.String:
                default:
                    shardingValue = new ShardingValue(domainModel.Name, shardingElement.ID, Convert.ToString(propValue));
                    break;
            }

            return shardingValue;
        }

        /// <summary>
        /// 构造ShardingValue
        /// </summary>
        /// <param name="domainModel">领域模型</param>
        /// <param name="instance">数据</param>
        /// <returns>ShardingValue</returns>
        private ShardingValue CreateShardingValueByDataID(DomainModel.Spi.DomainModel domainModel, DomainObject domainObject, string dataID)
        {
            ShardingValue shardingValue = null;
            var shardingColumn = domainObject.DataObject.Columns.FirstOrDefault(i => i.IsShardingColumn);
            if (shardingColumn == null)
            {
                return shardingValue;
            }

            var shardingElement = domainObject.Elements.FirstOrDefault(i => i.DataColumnID == shardingColumn.ID);
            switch (shardingElement.DataType)
            {
                case ElementDataType.DateTime:
                case ElementDataType.Date:
                    shardingValue = new ShardingValue(domainModel.Name, shardingElement.ID, Convert.ToDateTime(dataID));
                    break;
                case ElementDataType.Integer:
                    shardingValue = new ShardingValue(domainModel.Name, shardingElement.ID, Convert.ToInt64(dataID));
                    break;
                case ElementDataType.String:
                default:
                    shardingValue = new ShardingValue(domainModel.Name, shardingElement.ID, Convert.ToString(dataID));
                    break;
            }

            return shardingValue;
        }
    }
}
