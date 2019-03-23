using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSharding.DataAccess.Service
{
    /// <summary>
    /// 对象属性值工具类
    /// </summary>
    class ObjectPropertyValueUtils
    {
        public static object GetPropValue(string propName, object instance)
        {
            var prop = instance.GetType().GetProperty(propName);

            return prop.GetValue(instance);
        }

        public static IEnumerable<object> GetCollectionPropValue(string propName, object instance)
        {
            var prop = instance.GetType().GetProperty(propName);

            var list = prop.GetValue(instance) as IEnumerable;

            var valueList = new List<object>();
            foreach (var item in list)
            {
                valueList.Add(item);
            }

            return valueList;
        }

    }
}
