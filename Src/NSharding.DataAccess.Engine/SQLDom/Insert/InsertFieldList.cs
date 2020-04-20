using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace NSharding.DataAccess.Core
{
    /// <summary>
    /// Insert语句中的插入字段列表类
    /// </summary>
    /// <remarks>Insert语句中插入字段列表</remarks>
    public class InsertFieldList : SqlElement
    {
        #region 常量

        /// <summary>
        /// Insert语句中的插入字段列表常量
        /// </summary>
        public const string INSERTFIELDLIST = "InsertFieldList";
        #endregion

        #region 字段

        #endregion

        #region 构造函数

        /// <summary>
        /// 构造函数
        /// </summary>
        public InsertFieldList()
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
        /// <returns>SQL</returns>
        public override string ToSQL()
        {
            if (ChildCollection == null || ChildCollection.Count == 0)
                return string.Empty;

            var result = new StringBuilder();
            var values = new StringBuilder();
            result.Append("(");
            values.Append("(");
            for (int index = 0; index < this.ChildCollection.Count; index++)
            {
                result.Append(ChildCollection[index].ToSQL());
                //result.Append("\"").Append(ChildCollection[index].ToSQL()).Append("\"");
                if (index < this.ChildCollection.Count - 1)
                    result.Append(",");

                values.Append(":").Append(ChildCollection[index].ToSQL());
                if (index < this.ChildCollection.Count - 1)
                    values.Append(",");
            }

            result.Append(")").ToString();
            values.Append(")").ToString();

            return string.Format("{0}VALUES{1}", result.ToString(), values.ToString());
        }

        #endregion
    }
}
