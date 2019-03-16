using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace NSharding.DataAccess.Core
{
    /// <summary>
    /// Insert语句中的插入值类
    /// </summary>
    /// <remarks>Insert语句中的插入项</remarks>
    public class InsertValue : FieldValue
    {
        #region 常量

        /// <summary>
        /// InsertValue
        /// </summary>
        public const string INSERTVALUE = "InsertValue";

        #endregion

        #region 字段

        #endregion

        #region 构造函数

        /// <summary>
        /// 构造函数
        /// </summary>
        public InsertValue()
            : base()
        { }

        #endregion

        #region 属性

        #endregion

        #region 方法

        /// <summary>
        /// 转换成SQL
        /// </summary>
        /// <returns>SQL</returns>
        public override string ToSQL()
        {
            return string.Empty;
        }

        #endregion
    }
}
