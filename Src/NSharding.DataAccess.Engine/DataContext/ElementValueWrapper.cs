using NSharding.DomainModel.Spi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSharding.DataAccess.Core
{
    /// <summary>
    /// 元素对应的字段值转换器
    /// </summary>
    /// <remarks>元素对应的字段值转换器</remarks>
    class ElementValueWrapper
    {
        #region 方法

        /// <summary>
        /// 转化元素对应字段的值。将日期的值转化成相应的格式字符串。
        /// </summary>
        /// <param name="element">元素。</param>
        /// <param name="elementValue">元素对应字段的值。</param>
        /// <returns>转化后的值。</returns>
        public static object ConvertElementValue(DomainObjectElement element, object elementValue)
        {
            if (element == null)
                throw new ArgumentNullException("ElementValueWrapper.ConvertElementValue.DomainObjectElement");

            switch (element.DataType)
            {
                case ElementDataType.Date:
                    // 系统的短日期格式。兼容以前的日期格式。
                    if (string.IsNullOrEmpty(Convert.ToString(elementValue)) ||
                        (Convert.ToDateTime(elementValue) == new DateTime()))
                    {
                        return System.DBNull.Value;
                    }
                    return DateTimeToStringByCHAR8(Convert.ToDateTime(elementValue));
                case ElementDataType.DateTime:
                    if (string.IsNullOrEmpty(Convert.ToString(elementValue)) ||
                        (Convert.ToDateTime(elementValue) == new DateTime()))
                        return System.DBNull.Value;
                    return elementValue;
                case ElementDataType.Binary:
                    byte[] byteAry = elementValue as Byte[];
                    if ((byteAry != null) && (byteAry.Length == 0))
                        return System.DBNull.Value;
                    return elementValue;
                default:
                    return elementValue;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static ElementDataType ConvertEleDataType(Type type)
        {
            switch (type.ToString())
            {
                case "int":
                case "long":
                    return ElementDataType.Integer;
                case "decimal":
                case "float":
                case "double":
                    return ElementDataType.Decimal;
                case "datetime":
                    return ElementDataType.DateTime;
                case "bool":
                    return ElementDataType.Boolean;
                case "string":
                    return ElementDataType.String;
                case "byte[]":
                    return ElementDataType.Binary;
                default:
                    return ElementDataType.String;
            }

        }

        /// <summary>
        /// 将日期数据转换为ISO8601格式字符串。
        /// </summary>
        /// <param name="dt">日期数据。</param>
        /// <returns>日期数据的ISO8601格式字符串。</returns>
        public static string DateTimeToStringByISO8601(DateTime dt)
        {
            string formatStringISO8601 = "yyyy'-'MM'-'dd'T'HH':'mm':'sszzz";
            return dt.ToString(formatStringISO8601, System.Globalization.DateTimeFormatInfo.InvariantInfo);
        }

        /// <summary>
        /// 将日期数据转换为CHAR8格式字符串。
        /// </summary>
        /// <param name="dt">日期数据。</param>
        /// <returns>日期数据的CHAR8格式字符串。</returns>
        public static string DateTimeToStringByCHAR8(DateTime dt)
        {
            string formatStringCHAR8 = "yyyyMMdd";
            return dt.ToString(formatStringCHAR8);
        }

        /// <summary>
        /// 获取ElementDataType对应的C#中的数据类型
        /// </summary>
        /// <param name="element">节点元素</param>
        /// <returns>ElementDataType对应的C#中的数据类型</returns>
        public static Type GetType(DomainObjectElement element)
        {
            if (element == null)
                throw new ArgumentNullException("ElementValueWrapper.GetType.DomainObjectElement");
            switch (element.DataType)
            {
                case ElementDataType.String:
                case ElementDataType.Text:
                    return typeof(string);
                case ElementDataType.Boolean:
                case ElementDataType.Integer:
                    return typeof(int);
                case ElementDataType.Decimal:
                    return typeof(decimal);
                case ElementDataType.Date:
                case ElementDataType.DateTime:
                    return typeof(DateTime);
                case ElementDataType.Binary:
                    return typeof(byte[]);
                default:
                    throw new NotSupportedException(element.DataType.ToString());
            }
        }

        /// <summary>
        /// 获取ElementDataType对应的C#中的数据类型
        /// </summary>
        /// <param name="element">节点元素</param>
        /// <returns>ElementDataType对应的C#中的数据类型</returns>
        public static Type GetType(ElementDataType dataType)
        {
            switch (dataType)
            {
                case ElementDataType.String:
                case ElementDataType.Text:
                    return typeof(string);
                case ElementDataType.Boolean:
                case ElementDataType.Integer:
                    return typeof(int);
                case ElementDataType.Decimal:
                    return typeof(decimal);
                case ElementDataType.Date:
                case ElementDataType.DateTime:
                    return typeof(DateTime);
                case ElementDataType.Binary:
                    return typeof(byte[]);
                default:
                    throw new NotSupportedException(dataType.ToString());
            }
        }

        #endregion
    }
}
