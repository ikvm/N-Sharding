// ===============================================================================
// 浪潮GSP平台
// 字段的值
// 请查看《GSP7-数据访问引擎子系统概要设计说明书》来了解关于此类的更多信息。
// ===============================================================================
// 变更历史纪录
// 时间			             版本	    修改人	        描述
// 2013/1/31 22:58:27        1.0        周国庆          初稿。
// ===============================================================================
// 开发者: 周国庆
// 2013/1/31 22:58:27 
// (C) 2013 Genersoft Corporation 版权所有
// 保留所有权利。
// ===============================================================================

using System;
using System.Xml;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace NSharding.DataAccess.Core
{
    /// <summary>
    /// 字段的值
    /// </summary>
    /// <remarks>字段的值</remarks>
    public class FieldValue : SqlElement
    {
        #region 常量

        /// <summary>
        /// FieldValue
        /// </summary>
        public const string FIELDVALUE = "FieldValue";

        /// <summary>
        /// Value
        /// </summary>
        public const string VALUE = "Value";

        /// <summary>
        /// Type
        /// </summary>
        public const string TYPE = "Type";

        #endregion

        #region 字段

        #endregion

        #region 构造函数

        /// <summary>
        /// 构造函数
        /// </summary>
        public FieldValue()
            : base()
        { }

        #endregion

        #region 属性

        /// <summary>
        /// 字段值
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// GSP数据类型
        /// </summary>
        public int DataType { get; set; }

        #endregion

        #region 方法

        /// <summary>
        /// 克隆
        /// </summary>
        /// <returns>字段的值</returns>
        public override object Clone()
        {
            var newValue = base.Clone() as FieldValue;
            if (Value != null)
                newValue.Value = Value;

            return newValue;
        }

        /// <summary>
        /// 转换成SQL
        /// </summary>
        /// <returns>SQL</returns>
        public override string ToSQL()
        {
            if (Value == null || Value == DBNull.Value)
            {
                return string.Empty.AddQuote();
            }

            var type = Value.GetType();
            string sql = string.Empty;
            if (type == typeof(String) || type == typeof(Boolean) || type == typeof(Char))
            {
                sql = Convert.ToString(Value).AddQuote();
            }
            if (type == typeof(Byte) || type == typeof(Double) || type == typeof(Single) || type == typeof(Int32) || type == typeof(Int64) || type == typeof(Int16) || type == typeof(Decimal))
            {
                sql = Convert.ToString(Value);
            }
            if (type == typeof(DateTime))
            {
                sql = Convert.ToDateTime(Value).ToStringByISO8601();
            }

            return sql;
        }

        #endregion
    }
}
