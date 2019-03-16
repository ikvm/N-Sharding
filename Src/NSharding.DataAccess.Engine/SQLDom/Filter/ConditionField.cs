using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace NSharding.DataAccess.Core
{
    /// <summary>
    /// SQL条件中的字段类
    /// </summary>
    /// <remarks>SQL条件中的字段</remarks>
    public class ConditionField : Field
    {
        #region 常量

        /// <summary>
        /// ConditionField
        /// </summary>
        public const string CONDITIONFIELD = "ConditionField";

        #endregion

        #region 构造函数

        /// <summary>
        /// 构造函数。
        /// </summary>
        public ConditionField()
            : base()
        { }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="table">表</param>
        /// <param name="fieldName">字段</param>
        public ConditionField(SqlTable table, string fieldName)
            : base(table, fieldName)
        {
        }

        #endregion

        #region 属性

        #endregion

        #region 方法

        #endregion
    }
}
