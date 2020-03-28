using NSharding.DomainModel.Spi;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Teld.Core.Metadata.Service;

namespace NSharding.ORMapping.Service
{
    /// <summary>
    /// 结果集映射工厂
    /// </summary>
    public class ResultMappingFactory
    {
        private static ConcurrentDictionary<string, ResultMapping> resultMappingDic;

        private static object syncObj = new object();

        //结果集映射实例
        private static ResultMappingFactory instance;

        /// <summary>
        /// 构造函数
        /// </summary>
        private ResultMappingFactory()
        {
            resultMappingDic = new ConcurrentDictionary<string, ResultMapping>();
        }

        /// <summary>
        /// 获取结果集映射实例
        /// </summary>
        /// <returns>结果集映射实例</returns>
        public static ResultMappingFactory GetInstance()
        {
            if (instance == null)
            {
                lock (syncObj)
                {
                    if (instance == null)
                    {
                        instance = new ResultMappingFactory();
                    }
                }
            }

            return instance;
        }

        /// <summary>
        /// 获取领域模型的结果集映射
        /// </summary>        
        /// <param name="model">领域模型</param>
        /// <returns>结果集映射</returns>
        public ResultMapping CreateOrGetResultMapping(NSharding.DomainModel.Spi.DomainModel model)
        {
            if (model == null)
                throw new ArgumentNullException("ResultMappingFactory.CreateOrGetResultMapping.model");

            var key = string.Format("{0}_{1}", model.ID, model.Version);
            ResultMapping mapping = null;
            if (!resultMappingDic.ContainsKey(key))
            {
                lock (syncObj)
                {
                    if (!resultMappingDic.ContainsKey(key))
                    {
                        mapping = CreateResultMapping(model);
                        if (mapping == null)
                            throw new Exception(key + " ResultMapping is null!");

                        resultMappingDic.TryAdd(key, mapping);
                    }
                    else
                    {
                        mapping = resultMappingDic[key];
                    }
                }
            }
            else
            {
                mapping = resultMappingDic[key];
            }

            if (mapping == null)
                throw new Exception(key + " ResultMapping is null!");

            return mapping;
        }

        /// <summary>
        /// 构造领域对象结果集映射
        /// </summary>
        /// <param name="model">领域对象</param>
        /// <returns>结果集映射</returns>
        private ResultMapping CreateResultMapping(NSharding.DomainModel.Spi.DomainModel model)
        {
            if (model == null)
                throw new ArgumentNullException("ResultMappingService.GetResultMapping.model");

            DomainModelValidate(model);

            var rootObject = model.RootDomainObject;
            var mapping = CreateDomainObjectMapping(model.Name, model, rootObject);

            foreach (var domainObject in rootObject.ChildDomainObjects)
            {
                LoopCreateResultMapping(model, rootObject, mapping, domainObject);
            }

            return mapping;
        }

        /// <summary>
        /// 循环构造结果集映射
        /// </summary>
        /// <param name="parentObject">父对象</param>
        /// <param name="mapping">已有结果集映射</param>
        /// <param name="currentObject">当前对象</param>
        private void LoopCreateResultMapping(NSharding.DomainModel.Spi.DomainModel model, DomainObject parentObject, ResultMapping mapping, DomainObject currentObject)
        {
            var subMapping = CreateDomainObjectMapping(currentObject.Name, model, currentObject);
            var subMappingItem = new ResultMappingItem()
            {
                //Property = currentObject.PropertyName,
                LazyLoad = currentObject.IsLazyLoad,
                ResultMapping = subMapping,
                ItemType = ResultMappingItemType.SubResultMapping
            };
            var innerAsso = parentObject.Associations.FirstOrDefault(
                i => i.AssociateType == AssociateType.InnerJoin && i.AssoDomainObjectID == currentObject.ID);
            if (innerAsso != null)
            {
                var parentElementID = innerAsso.Items.FirstOrDefault().SourceElementID;
                subMappingItem.Property = innerAsso.PropertyName;
                subMappingItem.GroupbyColumn = parentObject.DataObject.Columns.FirstOrDefault(c =>
                    c.ID == parentObject.Elements.FirstOrDefault(i => i.ID == parentElementID).DataColumnID).ColumnName;
            }
            mapping.MappingItems.Add(subMappingItem);

            //外键关联
            var leftAssociations = parentObject.Associations.Where(i => i.AssociateType == AssociateType.OuterLeftJoin);
            foreach (var asso in leftAssociations)
            {
                var assoMapping = CreateAssociationMapping(asso);
                mapping.MappingItems.Add(new ResultMappingItem()
                {
                    Property = asso.PropertyName,
                    LazyLoad = asso.IsLazyLoad,
                    ResultMapping = assoMapping,
                    ItemType = ResultMappingItemType.ForeignResultMapping
                });
            }

            if (currentObject.ChildDomainObjects.Count > 0)
            {
                foreach (var obj in currentObject.ChildDomainObjects)
                {
                    LoopCreateResultMapping(model, currentObject, subMapping, obj);
                }
            }
        }

        /// <summary>
        /// 通过关联构造结果集映射
        /// </summary>
        /// <param name="asso">关联</param>
        /// <returns>结果集映射</returns>
        private ResultMapping CreateAssociationMapping(Association asso)
        {
            var model = DomainModelManageService.GetInstance().GetDomainModel(asso.AssoDomainModelID);
            DomainModelValidate(model);

            return this.CreateResultMapping(model);
        }

        /// <summary>
        /// 创建领域对象结果集映射
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="domainObject">领域对象</param>
        /// <returns>结果集映射</returns>
        private ResultMapping CreateDomainObjectMapping(string name, NSharding.DomainModel.Spi.DomainModel model, DomainObject domainObject)
        {
            var mapping = new ResultMapping()
            {
                DomainObject = name,
                ClassType = domainObject.ClazzReflectType
            };

            //遍历对象自身的元素
            foreach (var element in domainObject.Elements)
            {
                switch (element.ElementType)
                {
                    case ElementType.Normal:
                        var item = CreateCommonMappingItem(domainObject, element);
                        mapping.MappingItems.Add(item);
                        break;
                    case ElementType.Virtual:
                        var virtualItem = CreateVirtualMappingItem(domainObject, element);
                        mapping.MappingItems.Add(virtualItem);
                        break;
                    case ElementType.Enum:
                        var enumItem = CreateEnumMappingItem(domainObject, element);
                        mapping.MappingItems.Add(enumItem);
                        break;
                    default:
                        break;
                }
            }

            //主子关联
            var childObjects = model.DomainObjects.Where(i => i.ParentObjectID == domainObject.ID);
            if (childObjects.Count() > 0)
            {
                foreach (var child in childObjects)
                {
                    var asso = child.Associations.FirstOrDefault(i => i.AssociateType == AssociateType.InnerJoin && i.AssoDomainObjectID == domainObject.ID);
                    var assoItem = CreateInnerAssoMapping(model, child, asso);
                    mapping.MappingItems.Add(assoItem);
                }
            }

            //外键关联 TODO
            var leftAssociations = domainObject.Associations.Where(i => i.AssociateType == AssociateType.OuterLeftJoin && i.AssoDomainObjectID == domainObject.ID);
            foreach (var asso in leftAssociations)
            {
                var assoMapping = CreateAssociationMapping(asso);
                mapping.MappingItems.Add(new ResultMappingItem()
                {
                    Property = asso.PropertyName,
                    LazyLoad = asso.IsLazyLoad,
                    ResultMapping = assoMapping,
                    ItemType = ResultMappingItemType.ForeignResultMapping
                });
            }

            return mapping;
        }

        private ResultMappingItem CreateInnerAssoMapping(NSharding.DomainModel.Spi.DomainModel model, DomainObject currentDomainObject, Association association)
        {
            var item = new ResultMappingItem()
            {
                Property = association.PropertyName,
                TypeHandler = association.PropertyType,
                ItemType = ResultMappingItemType.SubResultMapping,
                ResultMapping = CreateDomainObjectMapping(currentDomainObject.Name, model, currentDomainObject),
                ParentDomainObjectId = currentDomainObject.ParentObjectID,
                CurrentDomainObjectId = currentDomainObject.ID,
                AssociationId = association.ID
            };

            return item;
        }

        private ResultMappingItem CreateEnumMappingItem(NSharding.DomainModel.Spi.DomainObject domainObject, DomainObjectElement element)
        {
            var item = new ResultMappingItem()
            {
                NullValue = element.DefaultValue,
                Property = element.PropertyName,
                ItemType = ResultMappingItemType.Enum,
                Column = element.Alias,
                //Column = domainObject.DataObject.Columns.FirstOrDefault(i => i.ID == element.DataColumnID).ColumnName,
                TypeHandler = element.PropertyType,
                ParentDomainObjectId = domainObject.ID
            };

            return item;
        }

        private ResultMappingItem CreateVirtualMappingItem(NSharding.DomainModel.Spi.DomainObject domainObject, DomainObjectElement element)
        {
            var item = new ResultMappingItem()
            {
                NullValue = element.DefaultValue,
                Property = element.PropertyName,
                ItemType = ResultMappingItemType.Virtual,
                Column = element.Alias,
                ParentDomainObjectId = domainObject.ID
            };

            return item;
        }

        private ResultMappingItem CreateCommonMappingItem(NSharding.DomainModel.Spi.DomainObject domainObject, DomainObjectElement element)
        {
            var item = new ResultMappingItem()
            {
                NullValue = element.DefaultValue,
                Property = element.PropertyName,
                ItemType = ResultMappingItemType.Normal,
                Column = element.Alias,
                ParentDomainObjectId = domainObject.ID
                //Column = domainObject.DataObject.Columns.FirstOrDefault(i => i.ID == element.DataColumnID).ColumnName
            };

            return item;
        }

        /// <summary>
        /// 领域对象校验
        /// </summary>
        /// <param name="model">领域对象</param>
        private void DomainModelValidate(NSharding.DomainModel.Spi.DomainModel model)
        {
            foreach (var domainObject in model.DomainObjects)
            {
                if (string.IsNullOrWhiteSpace(domainObject.DataObjectID))
                    throw new Exception(string.Format("DomainObject:{0}, can not find DataObject.", model.Name));

                if (domainObject.DataObject == null)
                {
                    domainObject.DataObject = DataObjectManageService.GetInstance().GetDataObject(domainObject.DataObjectID);
                }
            }
        }
    }
}
