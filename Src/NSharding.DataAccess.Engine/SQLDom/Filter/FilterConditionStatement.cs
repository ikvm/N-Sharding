using System;
using System.Text;

namespace NSharding.DataAccess.Core
{
    /// <summary>
    /// 过滤条件语句类
    /// </summary>
    /// <remarks>过滤条件语句类</remarks>
    public class FilterConditionStatement : SqlElement
    {
        #region 构造函数

        /// <summary>
        /// 构造函数
        /// </summary>
        public FilterConditionStatement()
            : base()
        {
            base.CreateChildCollection();
        }

        #endregion

        #region 方法

        /// <summary>
        /// 转换成SQL语句
        /// </summary>
        /// <returns>SQL语句</returns>
        public override string ToSQL()
        {
            if (ChildCollection.Count == 0)
                return string.Empty;

            var condition = new StringBuilder(ChildCollection.Count * 56);
            for (int i = 0; i < ChildCollection.Count; i++)
            {
                if (ChildCollection[i] is ConditionStatement)
                {
                    var subCondition = ChildCollection[i] as ConditionStatement;
                    if (subCondition == null) continue;
                    if (i == 0)
                    {
                        condition.Append(subCondition.ToSQL());
                    }
                    else
                    {
                        if (subCondition.LogicalOperator == OperatorType.And)
                        {
                            condition.Append(And.Clause(subCondition.ToSQL()));
                        }
                        else
                        {
                            condition.Append(Or.Clause(subCondition.ToSQL()));
                        }
                    }
                }
                else  //SqlPrimaryKey
                {
                    condition.Append(ChildCollection[i].ToSQL());
                }
            }

            return condition.ToString();
        }

        #endregion

    }
}
