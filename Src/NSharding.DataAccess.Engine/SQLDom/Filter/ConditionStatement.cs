using NSharding.DataAccess.Spi;
using System;

namespace NSharding.DataAccess.Core
{
    /// <summary>
    /// 动态条件SQL语句类
    /// </summary>
    /// <remarks>动态条件</remarks>
    [Serializable]
    public class ConditionStatement : SqlElement
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public ConditionStatement()
            : base()
        {
            base.CreateChildCollection();
        }

        #region 常量

        /// <summary> 
        /// ConditionStatement 
        /// </summary>
        public const string CONDITIONSTATEMENT = "ConditionStatement";

        /// <summary> 
        /// ConditionString 
        /// </summary>
        public const string CONDITIONSTRING = "ConditionString";

        #endregion

        #region 属性

        private OperatorType logicalOperator = OperatorType.And;

        /// <summary>
        /// 关系运算符
        /// </summary>
        public OperatorType LogicalOperator
        {
            get
            {
                return logicalOperator;
            }
            set
            {
                logicalOperator = value;
            }
        }

        public void SetRelationOperator(LogicalOperator loperator)
        {
            switch (loperator)
            {
                case Spi.LogicalOperator.OR:
                    LogicalOperator = OperatorType.Or;
                    break;
                case Spi.LogicalOperator.AND:
                default:
                    LogicalOperator = OperatorType.And;
                    break;
            }
        }

        /// <summary>
        /// 动态条件SQL字符串
        /// </summary>
        public string ConditionString { get; set; }

        #endregion

        #region 方法

        /// <summary>
        /// 转换成SQL语句
        /// </summary>
        /// <returns>SQL语句</returns>
        public override string ToSQL()
        {
            return ConditionString;
        }

        #endregion
    }
}
