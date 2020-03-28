//using NSharding.DomainModel.Spi;
//using System;
//using System.Collections.Generic;
//using System.Data;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//namespace NSharding.DataAccess.Service
//{
//    /// <summary>
//    /// 对象组装器
//    /// </summary>
//    public class ObjectAssemblier
//    {
//        private ResultMappingService resultMappingService;

//        /// <summary>
//        /// 构造函数
//        /// </summary>
//        public ObjectAssemblier()
//        {
//            resultMappingService = new ResultMappingService();
//        }

//        public object MaptoObject(IDataReader reader, DomainModel.Spi.DomainModel model)
//        {
//            var obj = ORMAssemblyContainer.GetInstance().CreateInstance(model.RootDomainObject.ClazzReflectType);
//            var type = ORMAssemblyContainer.GetInstance().GetObjectType(model.RootDomainObject.ClazzReflectType);
//            var props = type.GetProperties();
//            var resultMapping = resultMappingService.GetResultMapping(model);

//            while (reader.Read())
//            {
//                foreach (var item in resultMapping.MappingItems)
//                {
//                    switch (item.ItemType)
//                    {
//                        case ResultMappingItemType.Normal:
//                            MappingCommonProperty(reader, obj, props, item);
//                            break;
//                        case ResultMappingItemType.Enum:
//                            var element = model.RootDomainObject.Elements.FirstOrDefault(i => i.PropertyName == item.Property);
//                            MappingEnumProperty(reader, obj, props, item, element.PropertyType);
//                            break;
//                        case ResultMappingItemType.Virtual:
//                            var virtualElement = model.RootDomainObject.Elements.FirstOrDefault(i => i.PropertyName == item.Property);
//                            MappingVirtualProperty(virtualElement, obj, props, item);
//                            break;
//                        case ResultMappingItemType.ResultMapping:
//                            var asso = model.RootDomainObject.Associations.FirstOrDefault(i => i.PropertyName == item.Property);
//                            MappingComplexProperty(reader, obj, props, item, asso);
//                            break;
//                        default:
//                            break;
//                    }

//                }
//            }

//            return obj;
//        }

//        private void MappingComplexProperty(IDataReader reader, object obj, System.Reflection.PropertyInfo[] props, ResultMappingItem item, Association asso)
//        {
//            if (asso == null)
//                throw new Exception("Cannot find Association :" + item.Property);

//            try
//            {
//                var prop = props.FirstOrDefault(i => string.Equals(i.Name, item.Property, StringComparison.OrdinalIgnoreCase));

//            }
//            catch (Exception e)
//            {
//                e.Data.Add("PropertyName", item.Property);
//                e.Data.Add("ColumnName", item.Column);
//                throw e;
//            }
//        }

//        private void MappingVirtualProperty(DomainObjectElement element, object obj, System.Reflection.PropertyInfo[] props, ResultMappingItem item)
//        {
//            try
//            {
//                var prop = props.FirstOrDefault(i => string.Equals(i.Name, item.Property, StringComparison.OrdinalIgnoreCase));
//                prop.SetValue(obj, element.DefaultValue);
//            }
//            catch (Exception e)
//            {
//                e.Data.Add("PropertyName", item.Property);
//                e.Data.Add("ColumnName", item.Column);
//                throw e;
//            }
//        }

//        private void MappingEnumProperty(IDataReader reader, object obj, System.Reflection.PropertyInfo[] props, ResultMappingItem item, string enumTypeInfo)
//        {
//            try
//            {
//                var enumType = ORMAssemblyContainer.GetInstance().GetObjectType(enumTypeInfo);
//                if (enumType == null)
//                    throw new Exception("Cannot find type: " + enumTypeInfo);

//                var prop = props.FirstOrDefault(i => string.Equals(i.Name, item.Property, StringComparison.OrdinalIgnoreCase));
//                var enumValue = Enum.Parse(enumType, Convert.ToString(reader[item.Column]));
//                prop.SetValue(obj, enumValue);
//            }
//            catch (Exception e)
//            {
//                e.Data.Add("PropertyName", item.Property);
//                e.Data.Add("ColumnName", item.Column);
//                e.Data.Add("EnumTypeInfo", enumTypeInfo);
//                throw e;
//            }
//        }

//        private void MappingCommonProperty(IDataReader reader, object obj, System.Reflection.PropertyInfo[] props, ResultMappingItem item)
//        {
//            try
//            {
//                var prop = props.FirstOrDefault(i => string.Equals(i.Name, item.Property, StringComparison.OrdinalIgnoreCase));
//                prop.SetValue(obj, reader[item.Column]);
//            }
//            catch (Exception e)
//            {
//                e.Data.Add("PropertyName", item.Property);
//                e.Data.Add("ColumnName", item.Column);
//                throw e;
//            }
//        }
//    }
//}
