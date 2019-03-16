using NSharding.DataAccess.Spi;
using NSharding.DomainModel.Spi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace NSharding.DataAccess.Core
{
    /// <summary>
    /// 数据上下文构造类
    /// </summary>
    class DataContextBuilder
    {
        /// <summary>
        /// 构造数据上下文
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="model">领域模型</param>
        /// <param name="opType">数据访问类型</param>
        /// <param name="data">数据</param>
        /// <returns>数据上下文</returns>
        public static DataContext CreateDataContext<T>(DomainModel.Spi.DomainModel model, DomainModel.Spi.DomainObject domainObject, DataAccessOpType opType, T data)
        {
            DataContext dataContext = null;
            switch (opType)
            {
                case DataAccessOpType.I:
                    dataContext = CreateSaveContext(model, domainObject, data);
                    break;
                case DataAccessOpType.Q:
                    dataContext = CreateQueryContext(model, domainObject, data as IDictionary<string, object>);
                    break;
                case DataAccessOpType.U:
                    dataContext = CreateUpdateContext(model, domainObject, data);
                    break;
                case DataAccessOpType.D:
                    dataContext = CreateDeleteContext(model, domainObject, data as IDictionary<string, object>);
                    break;
                default:
                    break;
            }

            return dataContext;
        }

        protected static DataContext CreateQueryContext(DomainModel.Spi.DomainModel model, DomainModel.Spi.DomainObject domainObject, IDictionary<string, object> pkData)
        {
            var context = new DataContext();
            var dataContextItem = new DataContextItem()
            {
                OpType = DataAccessOpType.Q
            };

            dataContextItem.PrimaryKeyData = pkData;
            context.Add(domainObject.ID, new List<DataContextItem>() { dataContextItem });

            return context;
        }

        protected static DataContext CreateDeleteContext(DomainModel.Spi.DomainModel model, DomainModel.Spi.DomainObject domainObject, IDictionary<string, object> pkData)
        {
            var context = new DataContext();
            var dataContextItem = new DataContextItem()
            {
                OpType = DataAccessOpType.D
            };

            dataContextItem.PrimaryKeyData = pkData;
            context.Add(domainObject.ID, new List<DataContextItem>() { dataContextItem });

            return context;
        }

        protected static DataContext CreateUpdateContext(DomainModel.Spi.DomainModel model, DomainModel.Spi.DomainObject domainObject, object data)
        {
            var context = new DataContext();

            var dataContextItem = GetModelObjectPropValue(data, domainObject, DataAccessOpType.U, true);
            context.Add(domainObject.ID, new List<DataContextItem>() { dataContextItem });

            if (domainObject.ChildDomainObjects.Count > 0)
            {
                foreach (var childModelObject in domainObject.ChildDomainObjects)
                {
                    var objectList = ObjectPropertyValueUtils.GetCollectionPropValue(childModelObject.PropertyName, data);
                    var items = new List<DataContextItem>(objectList.Count());
                    foreach (var obj in objectList)
                    {
                        items.Add(GetModelObjectPropValue(obj, childModelObject, DataAccessOpType.U, true));
                        LoopGetModelObjectPropValue(childModelObject, context, obj, DataAccessOpType.U, true);
                    }

                    context.Add(childModelObject.ID, items);
                }
            }

            return context;
        }

        protected static DataContext CreateSaveContext(DomainModel.Spi.DomainModel model, DomainModel.Spi.DomainObject domainObject, object data)
        {
            var context = new DataContext();

            var rootDomainObject = domainObject;
            var rootDataContextItem = GetModelObjectPropValue(data, rootDomainObject, DataAccessOpType.I, false);
            context.Add(rootDomainObject.ID, new List<DataContextItem>() { rootDataContextItem });

            if (rootDomainObject.ChildDomainObjects.Count > 0)
            {
                foreach (var childModelObject in rootDomainObject.ChildDomainObjects)
                {
                    var objectList = ObjectPropertyValueUtils.GetCollectionPropValue(childModelObject.PropertyName, data);
                    var items = new List<DataContextItem>(objectList.Count());
                    foreach (var obj in objectList)
                    {
                        items.Add(GetModelObjectPropValue(obj, childModelObject, DataAccessOpType.I, false));

                        LoopGetModelObjectPropValue(childModelObject, context, obj, DataAccessOpType.I, false);
                    }

                    context.Add(childModelObject.ID, items);
                }
            }

            return context;
        }

        private static void LoopGetModelObjectPropValue(DomainModel.Spi.DomainObject domainObject, DataContext context, object data, DataAccessOpType opType, bool isUseDbNull)
        {
            if (domainObject.ChildDomainObjects.Count > 0)
            {
                foreach (var childModelObject in domainObject.ChildDomainObjects)
                {
                    var objectList = ObjectPropertyValueUtils.GetCollectionPropValue(childModelObject.PropertyName, data);
                    var items = new List<DataContextItem>(objectList.Count());
                    foreach (var obj in objectList)
                    {
                        items.Add(GetModelObjectPropValue(obj, childModelObject, opType, isUseDbNull));
                    }

                    context.Add(childModelObject.ID, items);
                }
            }
        }

        private static DataContextItem GetModelObjectPropValue(object data, DomainModel.Spi.DomainObject domainObject, DataAccessOpType opType, bool isUseDbNull)
        {
            var dataContextItem = new DataContextItem()
            {
                OpType = opType
            };

            foreach (var element in domainObject.Elements)
            {
                if (string.IsNullOrWhiteSpace(element.PropertyName)) continue;
                bool isPkElement = domainObject.DataObject.Columns.FirstOrDefault(i => i.ID == element.DataColumnID).IsPkColumn;
                var propValue = ObjectPropertyValueUtils.GetPropValue(element.PropertyName, data);
                if (propValue != null)
                {
                    dataContextItem.Data.Add(element.ID, propValue);
                    if (isPkElement)
                    {
                        dataContextItem.PrimaryKeyData.Add(element.ID, propValue);
                    }
                }
                else if (isUseDbNull)
                {
                    dataContextItem.Data.Add(element.ID, DBNull.Value);
                    if (isPkElement)
                    {
                        dataContextItem.PrimaryKeyData.Add(element.ID, DBNull.Value);
                    }
                }
            }

            foreach (var association in domainObject.Associations)
            {
                if (association.AssociateType == AssociateType.InnerJoin) continue;
                var associationObject = ObjectPropertyValueUtils.GetPropValue(association.PropertyName, data);

                foreach (var assoItem in association.Items)
                {
                    var element = association.AssoDomainObject.Elements.FirstOrDefault(i => i.ID == assoItem.TargetElementID);
                    if (element == null)
                    {
                        throw new Exception("Invalid Association:" + assoItem.TargetElementID);
                    }
                    var propValue = ObjectPropertyValueUtils.GetPropValue(element.PropertyName, associationObject);

                    if (propValue != null)
                        dataContextItem.Data.Add(assoItem.SourceElementID, propValue);
                    else if (isUseDbNull)
                    {
                        dataContextItem.Data.Add(assoItem.SourceElementID, DBNull.Value);
                    }
                }
            }

            return dataContextItem;
        }
    }
}
