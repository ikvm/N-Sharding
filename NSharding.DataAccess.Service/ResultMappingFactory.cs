using NSharding.DomainModel.Spi;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Teld.Core.Metadata.Service;

namespace NSharding.DataAccess.Service
{
    /// <summary>
    /// 结果集映射工厂
    /// </summary>
    class ResultMappingFactory
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
        public ResultMapping CreateOrGetResultMapping(DomainModel.Spi.DomainModel model)
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
        private ResultMapping CreateResultMapping(DomainModel.Spi.DomainModel model)
        {
            if (model == null)
                throw new ArgumentNullException("ResultMappingService.GetResultMapping.model");

            DomainModelValidate(model);

            var rootObject = model.RootDomainObject;
            var mapping = CreateDomainObjectMapping(model.Name, rootObject);

            foreach (var domainObject in rootObject.ChildDomainObjects)
            {
                LoopCreateResultMapping(rootObject, mapping, domainObject);
            }

            return mapping;
        }

        /// <summary>
        /// 循环构造结果集映射
        /// </summary>
        /// <param name="parentObject">父对象</param>
        /// <param name="mapping">已有结果集映射</param>
        /// <param name="currentObject">当前对象</param>
        private void LoopCreateResultMapping(DomainObject parentObject, ResultMapping mapping, DomainObject currentObject)
        {
            var subMapping = CreateDomainObjectMapping(currentObject.Name, currentObject);
            var subMappingItem = new ResultMappingItem()
            {
                Property = currentObject.PropertyName,
                LazyLoad = currentObject.IsLazyLoad,
                ResultMapping = subMapping,
                ItemType = ResultMappingItemType.ResultMapping
            };
            var innerAsso = parentObject.Associations.FirstOrDefault(
                i => i.AssociateType == AssociateType.InnerJoin && i.AssoDomainObjectID == currentObject.ID);
            if (innerAsso != null)
            {
                var parentElementID = innerAsso.Items.FirstOrDefault().SourceElementID;
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
                    ItemType = ResultMappingItemType.ResultMapping
                });
            }

            if (currentObject.ChildDomainObjects.Count > 0)
            {
                foreach (var obj in currentObject.ChildDomainObjects)
                {
                    LoopCreateResultMapping(currentObject, subMapping, obj);
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
        private ResultMapping CreateDomainObjectMapping(string name, DomainObject domainObject)
        {
            var mapping = new ResultMapping()
            {
                DomainObject = name,
                ClassType = domainObject.ClazzReflectType
            };

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
                    //case ElementType.Association:
                    //    var assoItem = CreateAssoMappingItem(domainObject, element);
                    //    mapping.MappingItems.Add(assoItem);
                    //    break;
                    default:
                        break;
                }
            }

            return mapping;
        }

        private ResultMappingItem CreateAssoMappingItem(DomainObject domainObject, DomainObjectElement element)
        {
            var item = new ResultMappingItem()
            {
                Property = element.PropertyName,
                ItemType = ResultMappingItemType.ResultMapping,
            };

            return item;
        }

        private ResultMappingItem CreateEnumMappingItem(DomainObject domainObject, DomainObjectElement element)
        {
            var item = new ResultMappingItem()
            {
                NullValue = element.DefaultValue,
                Property = element.PropertyName,
                ItemType = ResultMappingItemType.Enum,
                Column = domainObject.DataObject.Columns.FirstOrDefault(i => i.ID == element.DataColumnID).ColumnName,
                TypeHandler = element.PropertyType
            };

            return item;
        }

        private ResultMappingItem CreateVirtualMappingItem(DomainObject domainObject, DomainObjectElement element)
        {
            var item = new ResultMappingItem()
            {
                NullValue = element.DefaultValue,
                Property = element.PropertyName,
                ItemType = ResultMappingItemType.Virtual,
                Column = null
            };

            return item;
        }

        private ResultMappingItem CreateCommonMappingItem(DomainObject domainObject, DomainObjectElement element)
        {
            var item = new ResultMappingItem()
            {
                NullValue = element.DefaultValue,
                Property = element.PropertyName,
                ItemType = ResultMappingItemType.Normal,
                Column = domainObject.DataObject.Columns.FirstOrDefault(i => i.ID == element.DataColumnID).ColumnName
            };

            return item;
        }

        /// <summary>
        /// 领域对象校验
        /// </summary>
        /// <param name="model">领域对象</param>
        private void DomainModelValidate(DomainModel.Spi.DomainModel model)
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
