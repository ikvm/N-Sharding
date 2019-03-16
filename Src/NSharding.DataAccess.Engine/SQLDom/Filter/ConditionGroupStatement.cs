using System;
using System.Text;

namespace NSharding.DataAccess.Core
{
    /// <summary>
    /// 条件组
    /// </summary>
    /// <remarks>条件组，描述一组条件</remarks>
    internal class ConditionGroupStatement : ConditionStatement
    {
        #region 构造函数

        /// <summary>
        /// 构造函数
        /// </summary>
        public ConditionGroupStatement()
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
                var subCondition = ChildCollection[i] as ConditionStatement;
                if (subCondition == null) continue;
                if (i == 0)
                {
                    condition.Append(subCondition.ToSQL());
                }
                else
                {
                    if (subCondition.RelationOperator == OperatorType.And)
                    {
                        condition.Append(And.Clause(subCondition.ToSQL()));
                    }
                    else
                    {
                        condition.Append(Or.Clause(subCondition.ToSQL()));
                    }
                }
            }

            this.ConditionString = condition.ToString();

            return base.ToSQL();
        }

        #endregion
    }
}
