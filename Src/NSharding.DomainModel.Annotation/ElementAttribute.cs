using NSharding.DomainModel.Spi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSharding.DomainModel.Annotation
{
    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = true)]
    public class ElementAttribute : Attribute
    {
        public string DisplayName { get; set; }

        /// <summary>
        /// 数据类型
        /// </summary>
        public ElementDataType DataType { get; set; }

        /// <summary>
        /// 长度
        /// </summary>       
        public int Length { get; set; }

        /// <summary>
        /// 精度
        /// </summary>        
        public int Precision { get; set; }

        /// <summary>
        /// 默认值
        /// </summary>        
        public object DefaultValue { get; set; }

        public ElementAttribute(ElementDataType dataType, int length, int precision = 0, object defaultValue = null, string displayName = "")
        {
            this.DataType = dataType;
            this.Length = length;
            this.Precision = precision;
            this.DefaultValue = defaultValue;
            this.DisplayName = displayName;
        }
    }
}
