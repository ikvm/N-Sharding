using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace NSharding.DataAccess.Core
{
    /// <summary>
    /// Insert语句中的插入值集合类
    /// </summary>
    /// <remarks>Insert语句中的插入值集合</remarks>
    public class InsertValueList : SqlElement
    {
        #region 常量

        /// <summary>
        /// InsertValueList
        /// </summary>
        public const string INSERTVALUELIST = "InsertValueList";

        #endregion

        #region 字段

        #endregion

        #region 构造函数

        /// <summary>
        /// 构造函数
        /// </summary>
        public InsertValueList()
            : base()
        {
            base.CreateChildCollection();
        }

        #endregion

        #region 属性

        #endregion

        #region 方法

        /// <summary>
        /// 转换成SQL
        /// </summary>
        /// <remarks>InsertSQL中的参数以:p1这种形式进行拼装</remarks>
        /// <returns>SQL</returns>
        public override string ToSQL()
        {
            var result = new StringBuilder();

            result.Append(" VALUES(");
            for (int index = 0; index < this.ChildCollection.Count; index++)
            {
                result.Append(":P").Append(Convert.ToString(index));
                if (index < this.ChildCollection.Count - 1)
                    result.Append(", ");
            }

            return result.Append(") ").ToString();
        }

        #endregion
    }
}
