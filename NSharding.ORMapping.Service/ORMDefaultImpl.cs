using NSharding.DataAccess.Service;
using NSharding.DataAccess.Spi;
using NSharding.DomainModel.Spi;
using NSharding.ORMapping.Spi;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSharding.ORMapping.Service
{
    /// <summary>
    /// 对象组装器
    /// </summary>
    public class ORMDefaultImpl : IORMPlugin
    {
        private ResultMappingService resultMappingService;

        /// <summary>
        /// 构造函数
        /// </summary>
        public ORMDefaultImpl()
        {
            resultMappingService = new ResultMappingService();
        }

        //public object MaptoObject(IDataReader reader, DomainModel model)
        //{
        //    var obj = ORMAssemblyContainer.GetInstance().CreateInstance(model.RootDomainObject.ClazzReflectType);
        //    var type = ORMAssemblyContainer.GetInstance().GetObjectType(model.RootDomainObject.ClazzReflectType);
        //    var props = type.GetProperties();
        //    var resultMapping = resultMappingService.GetResultMapping(model);

        //    while (reader.Read())
        //    {
        //        foreach (var item in resultMapping.MappingItems)
        //        {
        //            switch (item.ItemType)
        //            {
        //                case ResultMappingItemType.Normal:
        //                    MappingCommonProperty(reader, obj, props, item);
        //                    break;
        //                case ResultMappingItemType.Enum:
        //                    var element = model.RootDomainObject.Elements.FirstOrDefault(i => i.PropertyName == item.Property);
        //                    MappingEnumProperty(reader, obj, props, item, element.PropertyType);
        //                    break;
        //                case ResultMappingItemType.Virtual:
        //                    var virtualElement = model.RootDomainObject.Elements.FirstOrDefault(i => i.PropertyName == item.Property);
        //                    MappingVirtualProperty(virtualElement, obj, props, item);
        //                    break;
        //                case ResultMappingItemType.SubResultMapping:
        //                    var asso = model.RootDomainObject.Associations.FirstOrDefault(i => i.PropertyName == item.Property);
        //                    MappingComplexProperty(reader, obj, props, item, asso);
        //                    break;
        //                default:
        //                    break;
        //            }

        //        }
        //    }

        //    return obj;
        //}      

        public object MapToObject(QueryResultSet resultSet, DomainModel.Spi.DomainModel model, DomainObject domainObject)
        {
            if (domainObject == null)
                domainObject = model.RootDomainObject;

            var obj = ORMAssemblyContainer.GetInstance().CreateInstance(domainObject.ClazzReflectType);
            var type = ORMAssemblyContainer.GetInstance().GetObjectType(domainObject.ClazzReflectType);
            var props = type.GetProperties();
            var resultMapping = resultMappingService.GetResultMapping(model);

            var dataTable = resultSet.GetDataTable(domainObject.DataObjectID);
            if (dataTable.Rows.Count == 0) return null;

            var firstRow = dataTable.Rows[0];
            var mappingItems = new List<ResultMappingItem>();
            if (domainObject.IsRootObject)
            {
                mappingItems = resultMapping.MappingItems.Where(i => i.ParentDomainObjectId == domainObject.ID).ToList();
            }
            else
            {
                var items = resultMapping.MappingItems.Where(i => i.CurrentDomainObjectId == domainObject.ID);
                foreach (var item in items)
                {
                    if (item.ItemType == ResultMappingItemType.SubResultMapping)
                        mappingItems.AddRange(item.ResultMapping.MappingItems);
                    else
                    {
                        mappingItems.Add(item);
                    }
                }
            }

            foreach (var item in mappingItems)
            {
                switch (item.ItemType)
                {
                    case ResultMappingItemType.Normal:
                        MappingCommonProperty(firstRow, obj, props, item, domainObject);
                        break;
                    case ResultMappingItemType.Enum:
                        var element = domainObject.Elements.FirstOrDefault(i => i.PropertyName == item.Property);
                        MappingEnumProperty(firstRow, obj, props, item, element.PropertyType);
                        break;
                    case ResultMappingItemType.Virtual:
                        var virtualElement = domainObject.Elements.FirstOrDefault(i => i.PropertyName == item.Property);
                        MappingVirtualProperty(virtualElement, obj, props, item);
                        break;
                    case ResultMappingItemType.SubResultMapping:
                        var currentObject = model.DomainObjects.FirstOrDefault(i => i.ID == item.CurrentDomainObjectId);
                        var subObjects = GetSubObjects(resultSet, model, currentObject, item);
                        var prop = props.FirstOrDefault(i => i.Name == item.Property);
                        var itemType = Type.GetType(currentObject.ClazzReflectType);
                        var listType = typeof(List<>).MakeGenericType(itemType);
                        var objectList = Activator.CreateInstance(listType);
                        var method = listType.GetMethod("Add", new Type[] { itemType });
                        foreach (var o in subObjects)
                        {
                            method.Invoke(objectList, new object[] { o });
                        }

                        prop.SetValue(obj, objectList);
                        break;
                    case ResultMappingItemType.ForeignResultMapping:
                        var forasso = domainObject.Associations.FirstOrDefault(i => i.PropertyName == item.Property);
                        var forObject = GetForeignObjects(firstRow, model, item, model.RootDomainObject, forasso);
                        props.FirstOrDefault(i => i.Name == item.Property).SetValue(obj, forObject);
                        break;
                    default:
                        break;
                }
            }

            return obj;
        }

        public List<object> MapToObjects(QueryResultSet resultSet, DomainModel.Spi.DomainModel model, DomainObject domainObject)
        {
            if (domainObject == null)
                domainObject = model.RootDomainObject;

            var result = new List<object>();
            var dataTable = resultSet.GetDataTable(domainObject.DataObjectID);

            var type = ORMAssemblyContainer.GetInstance().GetObjectType(domainObject.ClazzReflectType);
            var props = type.GetProperties();
            var resultMapping = resultMappingService.GetResultMapping(model);

            var mappingItems = new List<ResultMappingItem>();
            if (domainObject.IsRootObject)
            {
                mappingItems = resultMapping.MappingItems.Where(i => i.ParentDomainObjectId == domainObject.ID).ToList();
            }
            else
            {
                var items = resultMapping.MappingItems.Where(i => i.CurrentDomainObjectId == domainObject.ID);
                foreach (var item in items)
                {
                    if (item.ItemType == ResultMappingItemType.SubResultMapping)
                        mappingItems.AddRange(item.ResultMapping.MappingItems);
                    else
                    {
                        mappingItems.Add(item);
                    }
                }
            }

            for (int r = 0; r < dataTable.Rows.Count; r++)
            {
                var firstRow = dataTable.Rows[r];
                var obj = ORMAssemblyContainer.GetInstance().CreateInstance(domainObject.ClazzReflectType);

                foreach (var item in mappingItems)
                {
                    switch (item.ItemType)
                    {
                        case ResultMappingItemType.Normal:
                            MappingCommonProperty(firstRow, obj, props, item, domainObject);
                            break;
                        case ResultMappingItemType.Enum:
                            var element = domainObject.Elements.FirstOrDefault(i => i.PropertyName == item.Property);
                            MappingEnumProperty(firstRow, obj, props, item, element.PropertyType);
                            break;
                        case ResultMappingItemType.Virtual:
                            var virtualElement = domainObject.Elements.FirstOrDefault(i => i.PropertyName == item.Property);
                            MappingVirtualProperty(virtualElement, obj, props, item);
                            break;
                        case ResultMappingItemType.SubResultMapping:
                            var currentObject = model.DomainObjects.FirstOrDefault(i => i.ID == item.CurrentDomainObjectId);
                            var subObjects = GetSubObjects(resultSet, model, currentObject, item);
                            var prop = props.FirstOrDefault(i => i.Name == item.Property);
                            var itemType = Type.GetType(currentObject.ClazzReflectType);
                            var listType = typeof(List<>).MakeGenericType(itemType);
                            var objectList = Activator.CreateInstance(listType);
                            var method = listType.GetMethod("Add", new Type[] { itemType });
                            foreach (var o in subObjects)
                            {
                                method.Invoke(objectList, new object[] { o });
                            }

                            prop.SetValue(obj, objectList);
                            break;
                        case ResultMappingItemType.ForeignResultMapping:
                            var forasso = domainObject.Associations.FirstOrDefault(a => a.PropertyName == item.Property);
                            var forObject = GetForeignObjects(firstRow, model, item, model.RootDomainObject, forasso);
                            props.FirstOrDefault(p => p.Name == item.Property).SetValue(obj, forObject);
                            break;
                        default:
                            break;
                    }
                }

                result.Add(obj);
            }

            return result;
        }

        public object GetForeignObjects(DataRow row, DomainModel.Spi.DomainModel model, ResultMappingItem mappingItem, DomainObject domainObject, Association forAssociation)
        {
            var type = ORMAssemblyContainer.GetInstance().GetObjectType(forAssociation.AssoDomainObject.ClazzReflectType);
            var obj = ORMAssemblyContainer.GetInstance().CreateInstance(forAssociation.AssoDomainObject.ClazzReflectType);
            var props = type.GetProperties();

            foreach (var item in forAssociation.Items)
            {
                var targetElement = domainObject.Elements.FirstOrDefault(i => i.ID == item.TargetElementID);
                MappingCommonProperty(row, obj, props, mappingItem.ResultMapping.MappingItems.FirstOrDefault(i => i.Column == targetElement.Alias), domainObject);
            }

            foreach (var refElement in forAssociation.RefElements)
            {
                var targetElement = domainObject.Elements.FirstOrDefault(i => i.ID == refElement.ElementID);
                MappingCommonProperty(row, obj, props, mappingItem.ResultMapping.MappingItems.FirstOrDefault(i => i.Column == targetElement.Alias), domainObject);
            }

            return obj;
        }

        public List<object> GetSubObjects(QueryResultSet resultSet, DomainModel.Spi.DomainModel model, DomainObject domainObject, ResultMappingItem mappingItem)
        {
            var result = new List<object>();
            var type = ORMAssemblyContainer.GetInstance().GetObjectType(domainObject.ClazzReflectType);
            var props = type.GetProperties();

            var resultMapping = mappingItem.ResultMapping;
            var dataTable = resultSet.GetDataTable(domainObject.DataObjectID);
            for (int r = 0; r < dataTable.Rows.Count; r++)
            {
                var row = dataTable.Rows[r];
                var obj = ORMAssemblyContainer.GetInstance().CreateInstance(domainObject.ClazzReflectType);
                foreach (var item in resultMapping.MappingItems)
                {
                    switch (item.ItemType)
                    {
                        case ResultMappingItemType.Normal:
                            MappingCommonProperty(row, obj, props, item, domainObject);
                            break;
                        case ResultMappingItemType.Enum:
                            var element = domainObject.Elements.FirstOrDefault(i => i.PropertyName == item.Property);
                            MappingEnumProperty(row, obj, props, item, element.PropertyType);
                            break;
                        case ResultMappingItemType.Virtual:
                            var virtualElement = domainObject.Elements.FirstOrDefault(i => i.PropertyName == item.Property);
                            MappingVirtualProperty(virtualElement, obj, props, item);
                            break;
                    }

                    result.Add(obj);
                }
            }

            return result;
        }

        private void MappingComplexProperty(IDataReader reader, object obj, System.Reflection.PropertyInfo[] props, ResultMappingItem item, Association asso)
        {
            if (asso == null)
                throw new Exception("Cannot find Association :" + item.Property);

            try
            {
                var prop = props.FirstOrDefault(i => string.Equals(i.Name, item.Property, StringComparison.OrdinalIgnoreCase));

            }
            catch (Exception e)
            {
                e.Data.Add("PropertyName", item.Property);
                e.Data.Add("ColumnName", item.Column);
                throw e;
            }
        }

        private void MappingVirtualProperty(DomainObjectElement element, object obj, System.Reflection.PropertyInfo[] props, ResultMappingItem item)
        {
            try
            {
                var prop = props.FirstOrDefault(i => string.Equals(i.Name, item.Property, StringComparison.OrdinalIgnoreCase));
                prop.SetValue(obj, element.DefaultValue);
            }
            catch (Exception e)
            {
                e.Data.Add("PropertyName", item.Property);
                e.Data.Add("ColumnName", item.Column);
                throw e;
            }
        }

        private void MappingEnumProperty(IDataReader reader, object obj, System.Reflection.PropertyInfo[] props, ResultMappingItem item, string enumTypeInfo)
        {
            try
            {
                var enumType = ORMAssemblyContainer.GetInstance().GetObjectType(enumTypeInfo);
                if (enumType == null)
                    throw new Exception("Cannot find type: " + enumTypeInfo);

                var prop = props.FirstOrDefault(i => string.Equals(i.Name, item.Property, StringComparison.OrdinalIgnoreCase));
                var enumValue = Enum.Parse(enumType, Convert.ToString(reader[item.Column]));
                prop.SetValue(obj, enumValue);
            }
            catch (Exception e)
            {
                e.Data.Add("PropertyName", item.Property);
                e.Data.Add("ColumnName", item.Column);
                e.Data.Add("EnumTypeInfo", enumTypeInfo);
                throw e;
            }
        }

        private void MappingEnumProperty(DataRow row, object obj, System.Reflection.PropertyInfo[] props, ResultMappingItem item, string enumTypeInfo)
        {
            try
            {
                var enumType = ORMAssemblyContainer.GetInstance().GetObjectType(enumTypeInfo);
                if (enumType == null)
                    throw new Exception("Cannot find type: " + enumTypeInfo);

                var prop = props.FirstOrDefault(i => string.Equals(i.Name, item.Property, StringComparison.OrdinalIgnoreCase));
                if (row[item.Column] != DBNull.Value)
                {
                    var enumValue = Enum.Parse(enumType, Convert.ToString(row[item.Column]));
                    prop.SetValue(obj, enumValue);
                }
            }
            catch (Exception e)
            {
                e.Data.Add("PropertyName", item.Property);
                e.Data.Add("ColumnName", item.Column);
                e.Data.Add("EnumTypeInfo", enumTypeInfo);
                throw e;
            }
        }

        private void MappingCommonProperty(IDataReader reader, object obj, System.Reflection.PropertyInfo[] props, ResultMappingItem item)
        {
            try
            {
                var prop = props.FirstOrDefault(i => string.Equals(i.Name, item.Property, StringComparison.OrdinalIgnoreCase));
                if (reader[item.Column] != DBNull.Value)
                    prop.SetValue(obj, reader[item.Column]);
            }
            catch (Exception e)
            {
                e.Data.Add("PropertyName", item.Property);
                e.Data.Add("ColumnName", item.Column);
                throw e;
            }
        }

        private void MappingCommonProperty(DataRow row, object obj, System.Reflection.PropertyInfo[] props, ResultMappingItem item, DomainObject domainObject)
        {
            try
            {
                var prop = props.FirstOrDefault(i => string.Equals(i.Name, item.Property, StringComparison.OrdinalIgnoreCase));
                if (prop == null)
                    throw new ArgumentNullException("Prop:" + item.Property);
                if (row[item.Column] != DBNull.Value)
                {
                    var realValue = row[item.Column];
                    if (prop.PropertyType == typeof(Boolean))
                    {
                        if (row[item.Column].GetType() == typeof(int))
                        {
                            realValue = Convert.ToBoolean(Convert.ToInt32(realValue));
                        }
                    }

                    prop.SetValue(obj, realValue);
                }
            }
            catch (Exception e)
            {
                e.Data.Add("PropertyName", item.Property);
                e.Data.Add("ColumnName", item.Column);
                throw e;
            }
        }
    }
}
