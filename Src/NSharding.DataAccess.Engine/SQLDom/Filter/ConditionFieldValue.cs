using NSharding.DataAccess.Spi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSharding.DataAccess.Core
{
    public class ConditionFieldValue : FieldValue
    {
        /// <summary>
        /// InsertValue
        /// </summary>
        public const string INSERTVALUE = "ConditionFieldValue";

        /// <summary>
        /// 构造函数
        /// </summary>
        public ConditionFieldValue()
            : base()
        { }

        public FieldValueType ValueType { get; set; }

        public string ConditionFieldName { get; set; }

        public bool IsNull { get; set; }

        /// <summary>
        /// 转换成SQL
        /// </summary>
        /// <returns>SQL</returns>
        public override string ToSQL()
        {
            if (IsNull)
                return "is null";

            switch (ValueType)
            {
                case FieldValueType.Consts:
                default:
                    return ConditionFieldName;
                case FieldValueType.Expression:
                    return Convert.ToString(this.Value);
            }
        }
    }
}
