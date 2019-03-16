using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace NSharding.DataAccess.Core
{
    /// <summary>
    /// 多表之间的连接定义类
    /// </summary>
    /// <remarks>多表之间的连接</remarks>
    public class JoinConditionStatement : SqlElement
    {
        #region 常量

        /// <summary> 
        /// JoinConditionStatement 
        /// </summary>
        public const string JOINCONDITIONSTATEMENT = "JoinConditionStatement";

        #endregion

        #region 构造函数

        /// <summary>
        /// 构造函数
        /// </summary>
        public JoinConditionStatement()
            : base()
        {
            base.CreateChildCollection();
        }

        #endregion

        #region 方法

        /// <summary>
        /// 转换成SQL
        /// </summary>
        /// <returns>SQL</returns>
        public override string ToSQL()
        {
            StringBuilder result = new StringBuilder(" ");

            var joinConditionList = new List<string>();
            for (int index = 0; index < this.ChildCollection.Count; index++)
            {
                var join = this.ChildCollection[index].ToSQL();
                if (!string.IsNullOrEmpty(join))
                    joinConditionList.Add(join);
            }

            result.Append(string.Join(" AND ", joinConditionList));
            result.Append(" ");

            return result.ToString();
        }

        #endregion
    }
}